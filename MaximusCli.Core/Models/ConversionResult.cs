namespace MaximusCli.Core.Models;

/// <summary>
/// Represents the result of a configuration conversion operation.
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the conversion was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the converted MCP configuration (null if conversion failed).
    /// </summary>
    public McpConfig? Config { get; set; }

    /// <summary>
    /// Gets or sets the list of error messages encountered during conversion.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of warning messages (non-fatal issues).
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a successful conversion result.
    /// </summary>
    public static ConversionResult SuccessResult(McpConfig config, List<string>? warnings = null)
    {
        return new ConversionResult
        {
            Success = true,
            Config = config,
            Warnings = warnings ?? new List<string>()
        };
    }

    /// <summary>
    /// Creates a failed conversion result.
    /// </summary>
    public static ConversionResult FailureResult(params string[] errors)
    {
        return new ConversionResult
        {
            Success = false,
            Errors = errors.ToList()
        };
    }

    /// <summary>
    /// Creates a failed conversion result with a list of errors.
    /// </summary>
    public static ConversionResult FailureResult(List<string> errors)
    {
        return new ConversionResult
        {
            Success = false,
            Errors = errors
        };
    }
}
