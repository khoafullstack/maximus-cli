using System.CommandLine;
using MaximusCli.Core.Interfaces;

namespace MaximusCli.Commands;

/// <summary>
/// CLI command for converting MCP configurations between different agent formats.
/// </summary>
public class ConvertCommand : Command
{
    public ConvertCommand() : base("convert", "Convert MCP configuration from one agent format to another")
    {
        // Define options
        var fromOption = new Option<string>(
            aliases: new[] { "--from", "-f" },
            description: "Source agent format (e.g., cursor)")
        {
            IsRequired = true
        };

        var toOption = new Option<string>(
            aliases: new[] { "--to", "-t" },
            description: "Target agent format (e.g., roocode)")
        {
            IsRequired = true
        };

        var inputOption = new Option<string>(
            aliases: new[] { "--input", "-i" },
            description: "Path to the source configuration file")
        {
            IsRequired = true
        };

        var outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            description: "Path where the converted configuration should be written")
        {
            IsRequired = true
        };

        var validateOption = new Option<bool>(
            name: "--validate",
            description: "Validate both input and output configurations",
            getDefaultValue: () => true);

        var strictOption = new Option<bool>(
            name: "--strict",
            description: "Fail if any features cannot be converted (treats warnings as errors)",
            getDefaultValue: () => false);

        var forceOption = new Option<bool>(
            name: "--force",
            description: "Overwrite output file without prompting",
            getDefaultValue: () => false);

        // Add options to command
        AddOption(fromOption);
        AddOption(toOption);
        AddOption(inputOption);
        AddOption(outputOption);
        AddOption(validateOption);
        AddOption(strictOption);
        AddOption(forceOption);
    }

    public static async Task<int> HandleAsync(
        string from,
        string to,
        string input,
        string output,
        bool validate,
        bool strict,
        bool force,
        IConversionEngine conversionEngine)
    {
        try
        {
            // Check if input file exists
            if (!File.Exists(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Input file not found: {input}");
                Console.ResetColor();
                return 1;
            }

            // Check if output file exists and prompt if needed
            if (File.Exists(output) && !force)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Output file '{output}' already exists. Overwrite? (y/n): ");
                Console.ResetColor();

                var response = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (response != "y" && response != "yes")
                {
                    Console.WriteLine("Operation cancelled.");
                    return 0;
                }
            }

            // Check if output directory needs to be created
            var outputDir = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir) && !force)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Output directory '{outputDir}' does not exist. Create it? (y/n): ");
                Console.ResetColor();

                var response = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (response != "y" && response != "yes")
                {
                    Console.WriteLine("Operation cancelled.");
                    return 0;
                }
            }

            // Show progress
            Console.WriteLine($"Reading config from {input}...");
            Console.WriteLine($"Parsing {from} config...");
            Console.WriteLine($"Converting to {to} format...");

            if (validate)
            {
                Console.WriteLine("Validating output...");
            }

            // Perform conversion
            var result = await conversionEngine.ConvertAsync(from, to, input, output, strict);

            if (result.Success)
            {
                Console.WriteLine($"Writing config to {output}...");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Successfully converted config from {from} to {to}");
                Console.ResetColor();

                // Display warnings if any
                if (result.Warnings.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nWARNINGS ({result.Warnings.Count}):");
                    foreach (var warning in result.Warnings)
                    {
                        Console.WriteLine($"  - {warning}");
                    }
                    Console.ResetColor();
                }

                return 0;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Conversion failed");
                Console.WriteLine($"\nERRORS ({result.Errors.Count}):");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
                Console.ResetColor();

                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nERROR: Unexpected error during conversion");
            Console.WriteLine($"  {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }
}
