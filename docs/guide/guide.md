# SarifMark Usage Guide

This guide provides comprehensive documentation for using SarifMark to generate markdown reports from SARIF (Static
Analysis Results Interchange Format) files.

## Introduction

SarifMark is a .NET command-line tool that reads SARIF files produced by various static analysis tools and generates
comprehensive markdown reports. It's designed to integrate seamlessly into CI/CD pipelines for automated quality
reporting.

### Key Features

- **SARIF Processing**: Read and parse SARIF files from any compatible static analysis tool
- **Markdown Reports**: Generate human-readable markdown reports from SARIF data
- **Self-Validation**: Built-in validation with test result output
- **Configurable Output**: Customizable report depth and headings
- **Enforcement Mode**: Support for failing builds based on analysis results
- **Multi-Platform**: Works on Windows, Linux, and macOS with .NET 8, 9, or 10

## Installation

### Prerequisites

- [.NET SDK][dotnet-download] 8.0, 9.0, or 10.0

[dotnet-download]: https://dotnet.microsoft.com/download

### Global Installation

Install SarifMark as a global .NET tool for system-wide access:

```bash
dotnet tool install --global DemaConsulting.SarifMark
```

Verify the installation:

```bash
sarifmark --version
```

### Local Installation

For team projects, install SarifMark as a local tool to ensure version consistency:

```bash
# Create tool manifest if it doesn't exist
dotnet new tool-manifest

# Install the tool
dotnet tool install DemaConsulting.SarifMark
```

Run the locally installed tool:

```bash
dotnet sarifmark --version
```

### Update

To update to the latest version:

```bash
# Global installation
dotnet tool update --global DemaConsulting.SarifMark

# Local installation
dotnet tool update DemaConsulting.SarifMark
```

## Getting Started

### Basic Usage

The simplest way to use SarifMark is to process a SARIF file and generate a markdown report:

```bash
sarifmark --sarif analysis.sarif --report report.md
```

This command:

1. Reads the SARIF file `analysis.sarif`
2. Extracts tool information and results
3. Generates a markdown report in `report.md`

### Display Version

To display the version of SarifMark:

```bash
sarifmark --version
```

### Display Help

To display help information:

```bash
sarifmark --help
```

## Command-Line Options

SarifMark supports the following command-line options:

- `-v, --version`: Display version information
- `-?, -h, --help`: Display help message
- `--silent`: Suppress console output
- `--validate`: Run self-validation tests
- `--results <file>`: Write validation results to file (.trx or .xml format)
- `--enforce`: Return non-zero exit code if issues are found in the SARIF file
- `--log <file>`: Write console output to log file
- `--sarif <file>`: SARIF file to process (required for analysis)
- `--report <file>`: Export analysis results to markdown file
- `--report-depth <depth>`: Markdown header depth for report (default: 1)
- `--heading <text>`: Custom heading for report (default: [ToolName] Analysis)

## Common Usage Patterns

### Generate Basic Report

Process a SARIF file and generate a markdown report:

```bash
sarifmark --sarif codeql-results.sarif --report quality-report.md
```

### Custom Report Heading

Use a custom heading in the generated report:

```bash
sarifmark --sarif analysis.sarif --report report.md --heading "Security Analysis Results"
```

### Adjust Report Depth

Control the markdown header depth in the report:

```bash
sarifmark --sarif analysis.sarif --report report.md --report-depth 2
```

This is useful when including the report in a larger document where you want the sections to be at a deeper level.

### Enforce Quality Gates

Fail the build if any issues are found:

```bash
sarifmark --sarif analysis.sarif --report report.md --enforce
```

The command will exit with a non-zero exit code if the SARIF file contains any results.

### Run Self-Validation

Run the built-in validation tests:

```bash
sarifmark --validate
```

Generate a test results file:

```bash
sarifmark --validate --results validation-results.trx
```

### Silent Mode with Log File

Suppress console output while saving to a log file:

```bash
sarifmark --sarif analysis.sarif --report report.md --silent --log sarifmark.log
```

## Integration Examples

### CI/CD Pipeline Integration

SarifMark can be easily integrated into CI/CD pipelines. Here's an example GitHub Actions workflow:

```yaml
- name: Run static analysis
  run: |
    # Run your static analysis tool that generates SARIF
    codeql database analyze --format=sarif-latest --output=results.sarif

- name: Generate report with SarifMark
  run: |
    dotnet tool install --global DemaConsulting.SarifMark
    sarifmark --sarif results.sarif --report quality-report.md --enforce
```

### Multiple SARIF Files

Process multiple SARIF files from different tools:

```bash
# Process CodeQL results
sarifmark --sarif codeql.sarif --report codeql-report.md --heading "CodeQL Analysis"

# Process other tool results
sarifmark --sarif other-tool.sarif --report other-report.md --heading "Other Tool Analysis"
```

## Report Format

The generated markdown reports include:

- **Tool Information**: Name and version of the analysis tool
- **Summary**: Count of results found
- **Results Details**: Detailed information about each finding, including:
  - File location and line number
  - Severity level
  - Rule ID
  - Issue message

## Exit Codes

SarifMark uses the following exit codes:

- `0`: Success
- `1`: Error (invalid arguments, file not found, processing error)
- Non-zero (with `--enforce`): Issues found in SARIF file

## Troubleshooting

### SARIF File Not Found

**Error**: `Error: SARIF file not found: analysis.sarif`

**Solution**: Verify the path to the SARIF file is correct and the file exists.

### Invalid SARIF Format

**Error**: `Error: Failed to read SARIF file`

**Solution**: Ensure the SARIF file is valid and conforms to the SARIF specification. You can validate SARIF files
using the SARIF validator.

### Missing --sarif Parameter

**Error**: `Error: --sarif parameter is required`

**Solution**: Provide the `--sarif` parameter with the path to your SARIF file.

## Support

For issues, questions, or contributions:

- **GitHub Issues**: <https://github.com/demaconsulting/SarifMark/issues>
- **Documentation**: <https://github.com/demaconsulting/SarifMark>

## License

SarifMark is released under the MIT License. See the LICENSE file in the repository for details.
