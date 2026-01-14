namespace MaximusCli.Core.Models;

/// <summary>
/// Represents an agent-agnostic MCP configuration.
/// This domain model serves as an intermediary between different agent-specific formats.
/// </summary>
public class McpConfig
{
    /// <summary>
    /// Gets or sets the configuration version.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the list of MCP servers.
    /// </summary>
    public List<McpServer> Servers { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata that may be agent-specific.
    /// This allows preserving properties that don't map to the common model.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
