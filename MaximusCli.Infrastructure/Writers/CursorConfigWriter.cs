using System.Text.Json;
using MaximusCli.Core.Interfaces;
using MaximusCli.Core.Models;

namespace MaximusCli.Infrastructure.Writers;

/// <summary>
/// Writes MCP configuration to Cursor format.
/// </summary>
public class CursorConfigWriter : IConfigWriter
{
    public string AgentName => "cursor";

    public async Task<ConversionResult> WriteAsync(McpConfig config, string outputPath)
    {
        try
        {
            // Validate first
            var validation = await ValidateAsync(config);
            if (!validation.IsValid)
            {
                return ConversionResult.FailureResult(validation.Errors);
            }

            // Ensure output directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Build Cursor JSON structure
            var cursorConfig = new Dictionary<string, object>
            {
                ["mcpServers"] = BuildServersObject(config.Servers)
                // Cursor might have other fields, but for now we only support mcpServers
            };

            // Serialize to JSON with indentation
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string jsonContent = JsonSerializer.Serialize(cursorConfig, options);

            // Write to file
            await File.WriteAllTextAsync(outputPath, jsonContent);

            return ConversionResult.SuccessResult(config, validation.Warnings);
        }
        catch (Exception ex)
        {
            return ConversionResult.FailureResult($"Error writing config: {ex.Message}");
        }
    }

    public Task<ValidationResult> ValidateAsync(McpConfig config)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Validate servers exist
        if (config.Servers == null || config.Servers.Count == 0)
        {
            errors.Add("Configuration must contain at least one server");
        }
        else
        {
            // Validate each server
            foreach (var server in config.Servers)
            {
                if (string.IsNullOrWhiteSpace(server.Name))
                {
                    errors.Add("Server name cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(server.Command))
                {
                    errors.Add($"Server '{server.Name}' must have a command");
                }
            }
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(ValidationResult.Invalid(errors));
        }

        return Task.FromResult(ValidationResult.Valid(warnings));
    }

    private static Dictionary<string, object> BuildServersObject(List<McpServer> servers)
    {
        var serversDict = new Dictionary<string, object>();

        foreach (var server in servers)
        {
            var serverConfig = new Dictionary<string, object>
            {
                ["command"] = server.Command
            };

            if (server.Args != null && server.Args.Count > 0)
            {
                serverConfig["args"] = server.Args;
            }

            if (server.Env != null && server.Env.Count > 0)
            {
                serverConfig["env"] = server.Env;
            }

            serversDict[server.Name] = serverConfig;
        }

        return serversDict;
    }
}
