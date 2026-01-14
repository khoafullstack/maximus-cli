using MaximusCli.Core.Models;

namespace MaximusCli.Core.Interfaces;

/// <summary>
/// Defines a contract for writing the domain model to agent-specific MCP configuration files.
/// </summary>
public interface IConfigWriter
{
    /// <summary>
    /// Gets the name of the agent this writer supports (e.g., "cursor", "roocode").
    /// </summary>
    string AgentName { get; }

    /// <summary>
    /// Writes an MCP configuration to a file in the agent-specific format.
    /// </summary>
    /// <param name="config">The MCP configuration to write.</param>
    /// <param name="outputPath">The path where the configuration file should be written.</param>
    /// <returns>A conversion result indicating success or failure.</returns>
    Task<ConversionResult> WriteAsync(McpConfig config, string outputPath);

    /// <summary>
    /// Validates that the given configuration can be written in this agent's format.
    /// </summary>
    /// <param name="config">The configuration to validate.</param>
    /// <returns>A validation result indicating whether the config is compatible.</returns>
    Task<ValidationResult> ValidateAsync(McpConfig config);
}
