namespace MaximusCli.Core.Models;

/// <summary>
/// Represents an MCP server configuration.
/// </summary>
public class McpServer
{
    /// <summary>
    /// Gets or sets the unique name of the MCP server.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command to execute for this server.
    /// </summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command-line arguments for the server.
    /// </summary>
    public List<string> Args { get; set; } = new();

    /// <summary>
    /// Gets or sets the environment variables for the server.
    /// </summary>
    public Dictionary<string, string> Env { get; set; } = new();
}
