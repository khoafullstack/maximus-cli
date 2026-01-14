using MaximusCli.Core.Interfaces;
using MaximusCli.Core.Models;

namespace MaximusCli.Core.Validators;

/// <summary>
/// Validates MCP configuration domain models.
/// </summary>
public class McpConfigValidator : IConfigValidator
{
    public Task<ValidationResult> ValidateAsync(McpConfig config)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Validate config is not null
        if (config == null)
        {
            errors.Add("Configuration cannot be null");
            return Task.FromResult(ValidationResult.Invalid(errors));
        }

        // Validate version
        if (string.IsNullOrWhiteSpace(config.Version))
        {
            warnings.Add("Configuration version is not specified");
        }

        // Validate servers
        if (config.Servers == null)
        {
            errors.Add("Servers list cannot be null");
        }
        else if (config.Servers.Count == 0)
        {
            errors.Add("Configuration must contain at least one server");
        }
        else
        {
            // Validate each server
            for (int i = 0; i < config.Servers.Count; i++)
            {
                var server = config.Servers[i];

                if (string.IsNullOrWhiteSpace(server.Name))
                {
                    errors.Add($"Server at index {i} has no name");
                }

                if (string.IsNullOrWhiteSpace(server.Command))
                {
                    errors.Add($"Server '{server.Name}' (index {i}) has no command");
                }

                // Validate Args is not null
                if (server.Args == null)
                {
                    errors.Add($"Server '{server.Name}' has null Args list");
                }

                // Validate Env is not null
                if (server.Env == null)
                {
                    errors.Add($"Server '{server.Name}' has null Env dictionary");
                }
            }

            // Check for duplicate server names
            var duplicateNames = config.Servers
                .GroupBy(s => s.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Count > 0)
            {
                errors.Add($"Duplicate server names found: {string.Join(", ", duplicateNames)}");
            }
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(ValidationResult.Invalid(errors));
        }

        return Task.FromResult(ValidationResult.Valid(warnings));
    }
}
