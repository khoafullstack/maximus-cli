using MaximusCli.Core.Models;

namespace MaximusCli.Core.Interfaces;

/// <summary>
/// Defines a contract for the MCP configuration conversion engine.
/// </summary>
public interface IConversionEngine
{
    /// <summary>
    /// Converts an MCP configuration from one agent format to another.
    /// </summary>
    /// <param name="sourceAgent">The source agent name (e.g., "cursor").</param>
    /// <param name="targetAgent">The target agent name (e.g., "roocode").</param>
    /// <param name="inputPath">The path to the source configuration file.</param>
    /// <param name="outputPath">The path where the converted configuration should be written.</param>
    /// <param name="strict">If true, fail on any warnings or incompatibilities.</param>
    /// <returns>A conversion result indicating success or failure.</returns>
    Task<ConversionResult> ConvertAsync(
        string sourceAgent,
        string targetAgent,
        string inputPath,
        string outputPath,
        bool strict = false);

    /// <summary>
    /// Gets the list of supported source agents.
    /// </summary>
    IEnumerable<string> GetSupportedSourceAgents();

    /// <summary>
    /// Gets the list of supported target agents.
    /// </summary>
    IEnumerable<string> GetSupportedTargetAgents();
}
