# Introduction

This guide provides comprehensive documentation for using SarifMark to generate markdown reports from SARIF (Static
Analysis Results Interchange Format) files.

## Purpose

SarifMark is a .NET command-line tool that reads SARIF files produced by various static analysis tools and generates
comprehensive markdown reports. It's designed to integrate seamlessly into CI/CD pipelines for automated quality
reporting.

## Scope

This guide covers:

- Installation and setup of SarifMark
- Command-line options and usage patterns
- CI/CD pipeline integration examples
- Report format and customization
- Troubleshooting common issues

## Key Features

- **SARIF Processing**: Read and parse SARIF files from any compatible static analysis tool,
  including files with multiple runs from different tools
- **Markdown Reports**: Generate human-readable markdown reports from SARIF data
- **Self-Validation**: Built-in validation with test result output
- **Configurable Output**: Customizable report depth and headings
- **Enforcement Mode**: Support for failing builds based on analysis results
- **Multi-Platform**: Works on Windows, Linux, and macOS with .NET 8, 9, or 10

# Continuous Compliance

This tool follows the [Continuous Compliance][continuous-compliance] methodology, which ensures
compliance evidence is generated automatically on every CI run.

## Key Practices

- **Requirements Traceability**: Every requirement is linked to passing tests, and a trace matrix is
  auto-generated on each release
- **Linting Enforcement**: markdownlint, cspell, and yamllint are enforced before any build proceeds
- **Automated Audit Documentation**: Each release ships with generated requirements, justifications,
  trace matrix, and quality reports
- **CodeQL and SonarCloud**: Security and quality analysis runs on every build

# Installation

## Prerequisites

- [.NET SDK][dotnet-download] 8.0, 9.0, or 10.0

[dotnet-download]: https://dotnet.microsoft.com/download

## Global Installation

Install SarifMark as a global .NET tool for system-wide access:

```bash
dotnet tool install --global DemaConsulting.SarifMark
```

Verify the installation:

```bash
sarifmark --version
```

## Local Installation

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

## Update

To update to the latest version:

```bash
# Global installation
dotnet tool update --global DemaConsulting.SarifMark

# Local installation
dotnet tool update DemaConsulting.SarifMark
```

# Getting Started

## Basic Usage

The simplest way to use SarifMark is to process a SARIF file and generate a markdown report:

```bash
sarifmark --sarif analysis.sarif --report report.md
```

This command:

1. Reads the SARIF file `analysis.sarif`
2. Extracts tool information and results
3. Generates a markdown report in `report.md`

## Display Version

To display the version of SarifMark:

```bash
sarifmark --version
```

## Display Help

To display help information:

```bash
sarifmark --help
```

# Command-Line Options

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
- `--depth <depth>`: Markdown header depth for report (default: 1)
- `--heading <text>`: Custom heading for report (default: [ToolName] Analysis)

# Common Usage Patterns

## Generate Basic Report

Process a SARIF file and generate a markdown report:

```bash
sarifmark --sarif codeql-results.sarif --report quality-report.md
```

## Custom Report Heading

Use a custom heading in the generated report:

```bash
sarifmark --sarif analysis.sarif --report report.md --heading "Security Analysis Results"
```

## Adjust Report Depth

Control the markdown header depth in the report:

```bash
sarifmark --sarif analysis.sarif --report report.md --depth 2
```

This is useful when including the report in a larger document where you want the sections to be at a deeper level.

## Enforce Quality Gates

Fail the build if any issues are found:

```bash
sarifmark --sarif analysis.sarif --report report.md --enforce
```

The command will exit with a non-zero exit code if the SARIF file contains any issues.

## Self-Validation

Self-validation produces a report demonstrating that SarifMark is functioning correctly. This is useful in
regulated industries where tool validation evidence is required.

### Running Validation

To perform self-validation:

```bash
sarifmark --validate
```

To save validation results to a file:

```bash
sarifmark --validate --results validation-results.trx
```

The results file format is determined by the file extension: `.trx` for TRX (MSTest) format,
or `.xml` for JUnit format.

### Validation Report

The validation report contains the tool version, machine name, operating system version,
.NET runtime version, timestamp, and test results.

Example validation report:

```text
# DEMA Consulting SarifMark

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| SarifMark Version   | 1.0.0                                              |
| Machine Name        | BUILD-SERVER                                       |
| OS Version          | Ubuntu 22.04.3 LTS                                 |
| DotNet Runtime      | .NET 10.0.0                                        |
| Time Stamp          | 2024-01-15 10:30:00 UTC                            |

✓ SarifMark_SarifReading - Passed
✓ SarifMark_MarkdownReportGeneration - Passed
✓ SarifMark_Enforcement - Passed

Total Tests: 3
Passed: 3
Failed: 0
```

### Validation Tests

Each test proves specific functionality works correctly:

- **`SarifMark_SarifReading`** - SARIF file reading and parsing works correctly.
- **`SarifMark_MarkdownReportGeneration`** - Markdown report generation from SARIF data works correctly.
- **`SarifMark_Enforcement`** - Enforcement mode returns a non-zero exit code when issues are found.

## Silent Mode with Log File

Suppress console output while saving to a log file:

```bash
sarifmark --sarif analysis.sarif --report report.md --silent --log sarifmark.log
```

# Integration Examples

## CI/CD Pipeline Integration

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

## Multiple SARIF Files

Process multiple SARIF files from different tools:

```bash
# Process CodeQL results
sarifmark --sarif codeql.sarif --report codeql-report.md --heading "CodeQL Analysis"

# Process other tool results
sarifmark --sarif other-tool.sarif --report other-report.md --heading "Other Tool Analysis"
```

SarifMark also supports SARIF files that contain multiple runs from different tools within a
single file. When processing a multi-run SARIF file, SarifMark generates a combined report that
includes a section for each run, with headings indexed as `(#1)`, `(#2)`, etc.

# Report Format

The generated markdown reports include:

- **Tool Information**: Name and version of the analysis tool
- **File Count**: Total number of files analyzed (sum of artifacts across all runs)
- **Summary**: Count of issues found
- **Issues Details**: Detailed information about each finding, including:
  - File location and line number
  - Severity level
  - Rule ID
  - Issue message

# Exit Codes

SarifMark uses the following exit codes:

- `0`: Success
- `1`: Error (invalid arguments, file not found, processing error)
- Non-zero (with `--enforce`): Issues found in SARIF file

# Frequently Asked Questions

## What is SARIF?

SARIF (Static Analysis Results Interchange Format) is a standard format for the output of static analysis tools. It's
designed to be easily integrated into development workflows and provides a consistent structure for representing
analysis results from different tools.

For more information, visit the [SARIF website][sarif-web].

## Which static analysis tools produce SARIF output?

Many popular static analysis tools support SARIF output, including:

- **CodeQL** - Security and quality analysis
- **SonarQube** - Code quality and security analysis
- **ESLint** - JavaScript linting (with SARIF formatter)
- **Pylint** - Python linting (with SARIF converter)
- **Semgrep** - Pattern-based code analysis
- **Checkmarx** - Security scanning
- **Trivy** - Container vulnerability scanning

Check your tool's documentation for SARIF export options.

## Can I process multiple SARIF files at once?

Currently, SarifMark processes one SARIF file at a time. To process multiple files, run the tool multiple times with
different input and output files. You can combine the reports manually or use a script to merge them.

## How do I use SarifMark in a CI/CD pipeline?

See the Integration Examples section for CI/CD integration examples. The key steps are:

1. Run your static analysis tool to generate a SARIF file
2. Install SarifMark in your pipeline
3. Run SarifMark to generate a report
4. Optionally use `--enforce` to fail the build if issues are found

## What happens when I use the --enforce flag?

When you use the `--enforce` flag, SarifMark will:

- Process the SARIF file normally
- Generate any requested reports
- Return a non-zero exit code if the SARIF file contains any issues

This is useful in CI/CD pipelines to fail builds when quality issues are detected.

## Can I customize the report format?

Yes, you can customize:

- **Heading**: Use `--heading "Custom Title"` to set a custom report heading
- **Header Depth**: Use `--depth 2` to adjust the markdown header level (useful when including the report in
  a larger document)

The report content format is standardized but these options allow you to integrate reports into different documentation
structures.

## What .NET versions are supported?

SarifMark supports .NET 8.0, 9.0, and 10.0. You need at least one of these SDK versions installed to use the tool.
The tool is built as a multi-targeted package, so it will automatically use the appropriate version based on your
installed .NET runtime.

## How do I update SarifMark?

To update to the latest version:

```bash
# Global installation
dotnet tool update --global DemaConsulting.SarifMark

# Local installation
dotnet tool update DemaConsulting.SarifMark
```

## Where can I find more help?

- **GitHub Issues**: [GitHub Issues][issues]
- **GitHub Discussions**: [GitHub Discussions][discussions]
- **Documentation**: [Documentation][docs]

# Troubleshooting

## SARIF File Not Found

**Error**: `Error: SARIF file not found: analysis.sarif`

**Solution**: Verify the path to the SARIF file is correct and the file exists.

## Invalid SARIF Format

**Error**: `Error: Failed to read SARIF file`

**Solution**: Ensure the SARIF file is valid and conforms to the SARIF specification. You can validate SARIF files
using the SARIF validator.

## Missing --sarif Parameter

**Error**: `Error: --sarif parameter is required`

**Solution**: Provide the `--sarif` parameter with the path to your SARIF file.

# Support

For issues, questions, or contributions:

- **GitHub Issues**: [GitHub Issues][issues]
- **Documentation**: [Documentation][docs]

# License

SarifMark is released under the MIT License. See the LICENSE file in the repository for details.

[sarif-web]: https://sarifweb.azurewebsites.net/
[issues]: https://github.com/demaconsulting/SarifMark/issues
[discussions]: https://github.com/demaconsulting/SarifMark/discussions
[docs]: https://github.com/demaconsulting/SarifMark
[continuous-compliance]: https://github.com/demaconsulting/ContinuousCompliance
