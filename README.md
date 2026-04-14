# SarifMark

[![GitHub forks](https://img.shields.io/github/forks/demaconsulting/SarifMark?style=plastic)](https://github.com/demaconsulting/SarifMark/network/members)
[![GitHub stars](https://img.shields.io/github/stars/demaconsulting/SarifMark?style=plastic)](https://github.com/demaconsulting/SarifMark/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/SarifMark?style=plastic)](https://github.com/demaconsulting/SarifMark/graphs/contributors)
[![License](https://img.shields.io/github/license/demaconsulting/SarifMark?style=plastic)](https://github.com/demaconsulting/SarifMark/blob/main/LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/demaconsulting/SarifMark/build_on_push.yaml)](https://github.com/demaconsulting/SarifMark/actions/workflows/build_on_push.yaml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SarifMark&metric=alert_status)](https://sonarcloud.io/dashboard?id=demaconsulting_SarifMark)
[![Security](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SarifMark&metric=security_rating)](https://sonarcloud.io/dashboard?id=demaconsulting_SarifMark)
[![NuGet](https://img.shields.io/nuget/v/DemaConsulting.SarifMark?style=plastic)](https://www.nuget.org/packages/DemaConsulting.SarifMark)

SARIF Report Generation Tool

## Overview

SarifMark is a .NET command-line tool that generates comprehensive markdown reports from SARIF (Static Analysis
Results Interchange Format) files. It processes SARIF files produced by various static analysis tools and converts
them into human-readable markdown reports, making it easy to integrate code quality reporting into your CI/CD
pipelines and documentation workflows.

## Features

- 📄 **SARIF Processing** - Read and parse SARIF 2.1.0 format files
- 📝 **Markdown Reports** - Generate human-readable reports from SARIF data
- 🎯 **Customizable Output** - Configure report depth and custom headings
- 🚀 **CI/CD Integration** - Enforce quality gates and fail builds on issues
- 🌐 **Multi-Platform** - Builds and runs on Windows, Linux, and macOS with .NET 8, 9, and 10
- ✅ **Self-Validation** - Built-in tests without requiring external tools
- 📊 **Detailed Reporting** - Extract tool information, results, and locations
- 🔍 **Linting Enforcement** - markdownlint, cspell, and yamllint enforced on every CI run
- 📋 **Continuous Compliance** - Compliance evidence generated automatically on every CI run,
  following the [Continuous Compliance](https://github.com/demaconsulting/ContinuousCompliance) methodology
- ☁️ **SonarCloud Integration** - Quality gate and security analysis on every build
- 🔗 **Requirements Traceability** - Requirements linked to passing tests with auto-generated trace matrix

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 8.0, 9.0, or 10.0

### Global Installation

Install SarifMark as a global .NET tool for system-wide use:

```bash
dotnet tool install --global DemaConsulting.SarifMark
```

Verify the installation:

```bash
sarifmark --version
```

### Local Installation

Install SarifMark as a local tool in your project (recommended for team projects):

```bash
dotnet new tool-manifest  # if you don't have a tool manifest already
dotnet tool install DemaConsulting.SarifMark
```

Run the tool:

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

### Compatibility

| Component | Version | Status |
| :-------- | :------ | :----- |
| .NET SDK | 8.0 | ✅ Supported |
| .NET SDK | 9.0 | ✅ Supported |
| .NET SDK | 10.0 | ✅ Supported |
| SARIF Format | 2.1.0 | ✅ Supported |
| OS | Windows | ✅ Supported |
| OS | Linux | ✅ Supported |
| OS | macOS | ✅ Supported |

## Usage

### Basic Usage

Run the tool with the `--help` option to see available commands and options:

```bash
sarifmark --help
```

This will display:

```text
Usage: sarifmark [options]

Options:
  -v, --version              Display version information
  -?, -h, --help             Display this help message
  --silent                   Suppress console output
  --validate                 Run self-validation
  --results <file>           Write validation results to file (.trx or .xml)
  --enforce                  Return non-zero exit code if issues found
  --log <file>               Write output to log file
  --sarif <file>             SARIF file to process
  --report <file>            Export analysis results to markdown file
  --depth <depth>            Markdown header depth for report (default: 1)
  --heading <text>           Custom heading for report (default: [ToolName] Analysis)
```

### Quick Start Examples

**Generate a report from a SARIF file:**

```bash
sarifmark --sarif analysis.sarif --report report.md
```

**Generate a report with custom heading:**

```bash
sarifmark --sarif analysis.sarif --report report.md --heading "Code Quality Analysis"
```

**Enforce quality gate in CI/CD:**

```bash
sarifmark --sarif analysis.sarif --enforce
```

**Run self-validation:**

```bash
sarifmark --validate
```

**Run self-validation with test results output:**

```bash
sarifmark --validate --results validation-results.trx
```

### Self-Validation Tests

SarifMark includes built-in self-validation tests that verify the tool's functionality without requiring external
static analysis tools. These tests use mock SARIF data to validate core features and generate test result files
in TRX or JUnit format.

The self-validation suite includes the following tests:

| Test Name | Description |
| :-------- | :---------- |
| `SarifMark_SarifReading` | Verifies reading and parsing SARIF 2.1.0 format files |
| `SarifMark_MarkdownReportGeneration` | Verifies generating markdown reports from SARIF data |
| `SarifMark_Enforcement` | Verifies enforcement mode returns non-zero exit code when issues are found |

These tests provide evidence of the tool's functionality and are particularly useful for:

- Verifying the installation is working correctly
- Running automated tests in CI/CD pipelines without requiring static analysis tools
- Generating test evidence for compliance and traceability requirements

For detailed usage instructions, command-line options, and examples, including tool update instructions, see the
[Usage Guide](https://github.com/demaconsulting/SarifMark/blob/main/docs/guide/guide.md).

## Report Format

The generated markdown report includes:

1. **Report Header** - Custom heading or tool name with "Analysis" suffix
2. **Tool Information** - Tool name and version extracted from SARIF file
3. **File Count** - Total number of files analyzed (sum of artifacts across all runs)
4. **Issues Summary** - Count of issues found in the analysis
5. **Issues List** - Detailed list of issues in compiler-style format with file, line, level, rule ID, and message

Example report structure:

```markdown
# MockTool Analysis

**Tool:** MockTool 1.0.0
**Files:** 2

## Issues

Found 2 issues

src/Program.cs(42): warning [TEST001] Test issue 1
src/Helper.cs(15): error [TEST002] Test issue 2
```

## Self Validation

Running self-validation produces a report containing the following information:

```text
# DEMA Consulting SarifMark

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| SarifMark Version   | <version>                                          |
| Machine Name        | <machine-name>                                     |
| OS Version          | <os-version>                                       |
| DotNet Runtime      | <dotnet-runtime-version>                           |
| Time Stamp          | <timestamp> UTC                                    |

✓ SarifMark_SarifReading - Passed
✓ SarifMark_MarkdownReportGeneration - Passed
✓ SarifMark_Enforcement - Passed

Total Tests: 3
Passed: 3
Failed: 0
```

Each test in the report proves:

- **`SarifMark_SarifReading`** - SARIF file reading and parsing works correctly.
- **`SarifMark_MarkdownReportGeneration`** - Markdown report generation from SARIF data works correctly.
- **`SarifMark_Enforcement`** - Enforcement mode returns a non-zero exit code when issues are found.

See the [Usage Guide](https://github.com/demaconsulting/SarifMark/blob/main/docs/guide/guide.md) for more details
on the self-validation tests.

On validation failure the tool will exit with a non-zero exit code.

## Contributing

Contributions are welcome! We appreciate your interest in improving SarifMark.

Please see our [Contributing Guide](https://github.com/demaconsulting/SarifMark/blob/main/CONTRIBUTING.md) for
development setup, coding standards, and submission guidelines. Also review our
[Code of Conduct](https://github.com/demaconsulting/SarifMark/blob/main/CODE_OF_CONDUCT.md) for community guidelines.

For bug reports, feature requests, and questions, please use [GitHub Issues](https://github.com/demaconsulting/SarifMark/issues).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demaconsulting/SarifMark/blob/main/LICENSE)
file for details.

By contributing to this project, you agree that your contributions will be licensed under the MIT License.

## Support

- 🐛 **Report Bugs**: [GitHub Issues](https://github.com/demaconsulting/SarifMark/issues)
- 💡 **Request Features**: [GitHub Issues](https://github.com/demaconsulting/SarifMark/issues)
- ❓ **Ask Questions**: [GitHub Discussions](https://github.com/demaconsulting/SarifMark/discussions)
- 📖 **Documentation**: [Usage Guide](https://github.com/demaconsulting/SarifMark/blob/main/docs/guide/guide.md)
- 🤝 **Contributing**: [Contributing Guide](https://github.com/demaconsulting/SarifMark/blob/main/CONTRIBUTING.md)

## Security

For security concerns and vulnerability reporting, please see our
[Security Policy](https://github.com/demaconsulting/SarifMark/blob/main/SECURITY.md).

## Acknowledgements

SarifMark is built with the following open-source projects:

- [.NET](https://dotnet.microsoft.com/) - Cross-platform framework for building applications
- [SARIF](https://sarifweb.azurewebsites.net/) - Static Analysis Results Interchange Format specification
- [DemaConsulting.TestResults](https://github.com/demaconsulting/TestResults) - Test results parsing library
