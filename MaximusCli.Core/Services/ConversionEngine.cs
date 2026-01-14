using MaximusCli.Core.Interfaces;
using MaximusCli.Core.Models;

namespace MaximusCli.Core.Services;

/// <summary>
/// Coordinates the conversion of MCP configurations between different agent formats.
/// </summary>
public class ConversionEngine : IConversionEngine
{
    private readonly Dictionary<string, IConfigParser> _parsers = new();
    private readonly Dictionary<string, IConfigWriter> _writers = new();
    private readonly IConfigValidator _validator;

    public ConversionEngine(
        IEnumerable<IConfigParser> parsers,
        IEnumerable<IConfigWriter> writers,
        IConfigValidator validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        // Register parsers
        foreach (var parser in parsers)
        {
            _parsers[parser.AgentName.ToLowerInvariant()] = parser;
        }

        // Register writers
        foreach (var writer in writers)
        {
            _writers[writer.AgentName.ToLowerInvariant()] = writer;
        }
    }

    public async Task<ConversionResult> ConvertAsync(
        string sourceAgent,
        string targetAgent,
        string inputPath,
        string outputPath,
        bool strict = false)
    {
        var sourceKey = sourceAgent.ToLowerInvariant();
        var targetKey = targetAgent.ToLowerInvariant();

        // Validate source parser exists
        if (!_parsers.ContainsKey(sourceKey))
        {
            var availableAgents = string.Join(", ", GetSupportedSourceAgents());
            return ConversionResult.FailureResult(
                $"No parser found for agent '{sourceAgent}'",
                $"Available source agents: {availableAgents}");
        }

        // Validate target writer exists
        if (!_writers.ContainsKey(targetKey))
        {
            var availableAgents = string.Join(", ", GetSupportedTargetAgents());
            return ConversionResult.FailureResult(
                $"No writer found for agent '{targetAgent}'",
                $"Available target agents: {availableAgents}");
        }

        var parser = _parsers[sourceKey];
        var writer = _writers[targetKey];

        // Step 1: Parse source config
        var parseResult = await parser.ParseAsync(inputPath);
        if (!parseResult.Success)
        {
            return parseResult;
        }

        var config = parseResult.Config!;
        var allWarnings = new List<string>(parseResult.Warnings);

        // Step 2: Validate parsed config
        var validationResult = await _validator.ValidateAsync(config);
        if (!validationResult.IsValid)
        {
            return ConversionResult.FailureResult(validationResult.Errors);
        }

        allWarnings.AddRange(validationResult.Warnings);

        // Step 3: Validate target compatibility
        var targetValidation = await writer.ValidateAsync(config);
        if (!targetValidation.IsValid)
        {
            var errors = new List<string>
            {
                $"Configuration is not compatible with {targetAgent} format"
            };
            errors.AddRange(targetValidation.Errors);
            return ConversionResult.FailureResult(errors);
        }

        allWarnings.AddRange(targetValidation.Warnings);

        // Step 4: In strict mode, fail if there are any warnings
        if (strict && allWarnings.Count > 0)
        {
            var errors = new List<string>
            {
                $"Conversion failed in strict mode due to {allWarnings.Count} warning(s)"
            };
            errors.AddRange(allWarnings);
            return ConversionResult.FailureResult(errors);
        }

        // Step 5: Write output
        var writeResult = await writer.WriteAsync(config, outputPath);
        if (!writeResult.Success)
        {
            return writeResult;
        }

        // Merge all warnings
        allWarnings.AddRange(writeResult.Warnings);

        return ConversionResult.SuccessResult(config, allWarnings);
    }

    public IEnumerable<string> GetSupportedSourceAgents()
    {
        return _parsers.Keys.OrderBy(k => k);
    }

    public IEnumerable<string> GetSupportedTargetAgents()
    {
        return _writers.Keys.OrderBy(k => k);
    }
}
