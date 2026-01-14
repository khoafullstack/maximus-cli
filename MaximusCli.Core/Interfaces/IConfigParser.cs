using MaximusCli.Core.Models;

namespace MaximusCli.Core.Interfaces;

/// <summary>
/// Defines a contract for parsing agent-specific MCP configuration files into the domain model.
/// </summary>
public interface IConfigParser
{
    /// <summary>
    /// Gets the name of the agent this parser supports (e.g., "cursor", "roocode").
    /// </summary>
    string AgentName { get; }

    /// <summary>
    /// Gets the list of configuration versions this parser supports.
    /// </summary>
    string[] SupportedVersions { get; }

    /// <summary>
    /// Parses an MCP configuration file and converts it to the domain model.
    /// </summary>
    /// <param name="filePath">The path to the configuration file.</param>
    /// <returns>A conversion result containing the parsed configuration or errors.</returns>
    Task<ConversionResult> ParseAsync(string filePath);
}
