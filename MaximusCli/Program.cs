using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using MaximusCli.Commands;
using MaximusCli.Core.Interfaces;
using MaximusCli.Core.Services;
using MaximusCli.Core.Validators;
using MaximusCli.Infrastructure.Parsers;
using MaximusCli.Infrastructure.Writers;

// Configure services
var services = new ServiceCollection();

// Register parsers
services.AddSingleton<IConfigParser, CursorConfigParser>();

// Register writers
services.AddSingleton<IConfigWriter, RooCodeConfigWriter>();

// Register validator
services.AddSingleton<IConfigValidator, McpConfigValidator>();

// Register conversion engine
services.AddSingleton<IConversionEngine, ConversionEngine>();

var serviceProvider = services.BuildServiceProvider();

// Create root command
var rootCommand = new RootCommand("MaximusCli - MCP Configuration Converter")
{
    Description = "Convert MCP configuration files between different AI coding agent formats"
};

// Create convert command
var convertCommand = new ConvertCommand();

// Get options by name for binding
var fromOption = convertCommand.Options.OfType<Option<string>>().First(o => o.Name == "from");
var toOption = convertCommand.Options.OfType<Option<string>>().First(o => o.Name == "to");
var inputOption = convertCommand.Options.OfType<Option<string>>().First(o => o.Name == "input");
var outputOption = convertCommand.Options.OfType<Option<string>>().First(o => o.Name == "output");
var validateOption = convertCommand.Options.OfType<Option<bool>>().First(o => o.Name == "validate");
var strictOption = convertCommand.Options.OfType<Option<bool>>().First(o => o.Name == "strict");
var forceOption = convertCommand.Options.OfType<Option<bool>>().First(o => o.Name == "force");

// Set handler with proper parameter binding
convertCommand.SetHandler(
    async (from, to, input, output, validate, strict, force) =>
    {
        var engine = serviceProvider.GetRequiredService<IConversionEngine>();
        var exitCode = await ConvertCommand.HandleAsync(from, to, input, output, validate, strict, force, engine);
        Environment.ExitCode = exitCode;
    },
    fromOption,
    toOption,
    inputOption,
    outputOption,
    validateOption,
    strictOption,
    forceOption
);

rootCommand.AddCommand(convertCommand);

// Execute
return await rootCommand.InvokeAsync(args);
