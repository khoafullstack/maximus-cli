using System.Text.Json;
using MaximusCli.Core.Interfaces;
using MaximusCli.Core.Models;

namespace MaximusCli.Infrastructure.Parsers;

/// <summary>
/// Parses Roo Code MCP configuration files into the domain model.
/// </summary>
public class RooCodeConfigParser : IConfigParser
{
    public string AgentName => "roocode";

    public string[] SupportedVersions => new[] { "1.0" };

    public async Task<ConversionResult> ParseAsync(string filePath)
    {
        try
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                return ConversionResult.FailureResult($"Input file not found: {filePath}");
            }

            // Read file content
            string jsonContent = await File.ReadAllTextAsync(filePath);

            // Parse JSON
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                return ConversionResult.FailureResult($"Invalid JSON format: {ex.Message}");
            }

            var root = document.RootElement;

            // Create domain model
            var config = new McpConfig();
            var warnings = new List<string>();

            // Roo Code structure is simply:
            // {
            //   "mcpServers": { ... }
            // }

            // Parse mcpServers
            if (!root.TryGetProperty("mcpServers", out var serversElement))
            {
                return ConversionResult.FailureResult("Missing required property 'mcpServers'");
            }

            if (serversElement.ValueKind != JsonValueKind.Object)
            {
                return ConversionResult.FailureResult("Property 'mcpServers' must be an object");
            }

            // Parse each server
            foreach (var serverProperty in serversElement.EnumerateObject())
            {
                var serverName = serverProperty.Name;
                var serverValue = serverProperty.Value;

                var server = new McpServer
                {
                    Name = serverName
                };

                // Parse command
                if (serverValue.TryGetProperty("command", out var commandElement))
                {
                    server.Command = commandElement.GetString() ?? string.Empty;
                }
                else
                {
                    return ConversionResult.FailureResult($"Server '{serverName}' is missing required field 'command'");
                }

                // Parse args (optional)
                if (serverValue.TryGetProperty("args", out var argsElement) && argsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var arg in argsElement.EnumerateArray())
                    {
                        var argValue = arg.GetString();
                        if (argValue != null)
                        {
                            server.Args.Add(argValue);
                        }
                    }
                }

                // Parse env (optional)
                if (serverValue.TryGetProperty("env", out var envElement) && envElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var envProp in envElement.EnumerateObject())
                    {
                        var envValue = envProp.Value.GetString();
                        if (envValue != null)
                        {
                            server.Env[envProp.Name] = envValue;
                        }
                    }
                }

                config.Servers.Add(server);
            }

            // Validate at least one server exists
            if (config.Servers.Count == 0)
            {
                return ConversionResult.FailureResult("Configuration must contain at least one server");
            }

            return ConversionResult.SuccessResult(config, warnings);
        }
        catch (Exception ex)
        {
            return ConversionResult.FailureResult($"Unexpected error parsing config: {ex.Message}");
        }
    }
}
