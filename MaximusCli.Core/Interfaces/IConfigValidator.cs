using MaximusCli.Core.Models;

namespace MaximusCli.Core.Interfaces;

/// <summary>
/// Defines a contract for validating MCP configurations.
/// </summary>
public interface IConfigValidator
{
    /// <summary>
    /// Validates an MCP configuration.
    /// </summary>
    /// <param name="config">The configuration to validate.</param>
    /// <returns>A validation result indicating whether the config is valid.</returns>
    Task<ValidationResult> ValidateAsync(McpConfig config);
}
