# MaximusCli - MCP Configuration Converter

A command-line tool for converting Model Context Protocol (MCP) configuration files between different AI coding agent formats.

## Overview

AI coding agents like Cursor, Roo Code, Cline, and Claude Code CLI use MCP (Model Context Protocol) for configuration, but each agent has its own unique file format, syntax, and location conventions. MaximusCli automates the conversion between these formats, eliminating the need for manual translation.

## Features

- ✅ Convert MCP configs from Cursor to Roo Code format
- ✅ Extensible architecture for adding new agent formats
- ✅ Input and output validation
- ✅ Clear error messages and warnings
- ✅ Strict mode for ensuring full compatibility
- ✅ Safe file handling with overwrite prompts

## Installation

### Prerequisites

- .NET 10.0 SDK or later

### Build from Source

```bash
git clone <repository-url>
cd maximus
dotnet build
```

## Usage

### Basic Conversion

```bash
dotnet run --project MaximusCli -- convert \
  --from cursor \
  --to roocode \
  --input ./cursor-config.json \
  --output ./roocode-config.json
```

### Command Options

- `-f, --from <agent>` (REQUIRED): Source agent format (e.g., "cursor")
- `-t, --to <agent>` (REQUIRED): Target agent format (e.g., "roocode")
- `-i, --input <path>` (REQUIRED): Path to source configuration file
- `-o, --output <path>` (REQUIRED): Path for converted configuration file
- `--validate` (default: true): Validate both input and output configurations
- `--strict` (default: false): Fail if any features cannot be converted
- `--force` (default: false): Overwrite output file without prompting

### Examples

**Basic conversion with prompts:**
```bash
dotnet run --project MaximusCli -- convert -f cursor -t roocode -i config.json -o output.json
```

**Force overwrite without prompts:**
```bash
dotnet run --project MaximusCli -- convert -f cursor -t roocode -i config.json -o output.json --force
```

**Strict mode (fail on warnings):**
```bash
dotnet run --project MaximusCli -- convert -f cursor -t roocode -i config.json -o output.json --strict
```

**Get help:**
```bash
dotnet run --project MaximusCli -- convert --help
```

## Supported Agents

### Current Support

| Agent | Read (Parse) | Write | Status |
|-------|-------------|-------|--------|
| Cursor | ✅ | ❌ | Supported |
| Roo Code | ❌ | ✅ | Supported |

### Planned Support

- Cline
- Claude Code CLI
- Amp Code
- Other AI coding agents

## Configuration Format

### Cursor MCP Config Format

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-example"],
      "env": {
        "API_KEY": "your-key"
      }
    }
  }
}
```

### Roo Code MCP Config Format

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-example"],
      "env": {
        "API_KEY": "your-key"
      }
    }
  }
}
```

## Feature Mapping

| Feature | Cursor | Roo Code | Notes |
|---------|--------|----------|-------|
| Server name | ✅ | ✅ | Preserved |
| Command | ✅ | ✅ | Preserved |
| Arguments | ✅ | ✅ | Preserved as array |
| Environment vars | ✅ | ✅ | Preserved as key-value pairs |

## Architecture

MaximusCli uses a provider pattern for extensibility:

```
MaximusCli.Core/
├── Models/           # Domain models (McpConfig, McpServer)
├── Interfaces/       # Abstractions (IConfigParser, IConfigWriter)
├── Services/         # Conversion engine
└── Validators/       # Configuration validation

MaximusCli.Infrastructure/
├── Parsers/          # Agent-specific parsers (CursorConfigParser)
├── Writers/          # Agent-specific writers (RooCodeConfigWriter)
└── Validators/       # Schema validators

MaximusCli/
└── Commands/         # CLI commands
```

### Adding New Agent Support

To add support for a new agent:

1. **Create Parser** (for reading):
   - Implement `IConfigParser` interface
   - Parse agent-specific JSON to `McpConfig` domain model
   - Add to DI container in `Program.cs`

2. **Create Writer** (for writing):
   - Implement `IConfigWriter` interface
   - Convert `McpConfig` domain model to agent-specific JSON
   - Add to DI container in `Program.cs`

Example:
```csharp
public class NewAgentParser : IConfigParser
{
    public string AgentName => "newagent";
    public string[] SupportedVersions => new[] { "1.0" };

    public async Task<ConversionResult> ParseAsync(string filePath)
    {
        // Implementation
    }
}
```

## Known Limitations

- Currently only supports Cursor → Roo Code conversion
- Comments in JSON configs are not preserved
- Agent-specific features not in the common model may be lost

## Error Handling

The tool provides clear error messages for common issues:

- **File not found**: Input file path is invalid
- **Invalid JSON**: Source config is malformed
- **Missing required fields**: Config lacks required properties
- **Unsupported agent**: No parser/writer registered for agent
- **Validation failure**: Config doesn't meet format requirements

## Development

### Project Structure

- `MaximusCli.Core`: Domain logic and interfaces
- `MaximusCli.Infrastructure`: Implementation of parsers/writers
- `MaximusCli`: CLI application

### Running Tests

```bash
dotnet test
```

### Building

```bash
dotnet build
```

### Publishing

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Contributing

Contributions are welcome! To add support for a new agent:

1. Fork the repository
2. Create a feature branch
3. Implement parser and/or writer
4. Add tests
5. Submit pull request

## License

[Specify your license here]

## Support

For issues and feature requests, please open an issue on GitHub.
