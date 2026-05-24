# Usage

## Getting Started

After installation the `sarifmark` command is available from any terminal.

Generate a markdown report from a SARIF file:

```shell
sarifmark --sarif analysis.sarif --report report.md
```

Display the installed version:

```shell
sarifmark --version
```

Display the built-in help:

```shell
sarifmark --help
```

## Command-Line Options

| Option | Description |
| --- | --- |
| `-v`, `--version` | Display version information |
| `-?`, `-h`, `--help` | Display the help message |
| `--silent` | Suppress console output |
| `--validate` | Run built-in self-validation tests |
| `--results <file>` | Write validation results to a file (`.trx` or `.xml` format) |
| `--enforce` | Return a non-zero exit code if issues are found in the SARIF file |
| `--log <file>` | Write console output to a log file |
| `--sarif <file>` | SARIF file to process (required for analysis) |
| `--report <file>` | Export analysis results to a markdown file |
| `--depth <depth>` | Markdown header depth for the report (default: `1`) |
| `--heading <text>` | Custom heading for the report (default: `[ToolName] Analysis`) |

## Common Usage Patterns

### Generate a Basic Report

```shell
sarifmark --sarif codeql-results.sarif --report quality-report.md
```

### Custom Heading

```shell
sarifmark --sarif analysis.sarif --report report.md --heading "Security Analysis Results"
```

### Adjust Header Depth

Use `--depth` when embedding the report inside a larger document that already has top-level headings:

```shell
sarifmark --sarif analysis.sarif --report report.md --depth 2
```

### Enforce Quality Gates

Return a non-zero exit code when the SARIF file contains issues, causing the CI pipeline to fail:

```shell
sarifmark --sarif analysis.sarif --report report.md --enforce
```

### Self-Validation

Run the built-in validation suite to confirm that SarifMark is working correctly in the current
environment. Results can be written to a `.trx` (Visual Studio Test Results) or `.xml` file for
pipeline consumption:

```shell
sarifmark --validate --results validation.trx
sarifmark --validate --results validation.xml
```

A passing validation run writes a report similar to:

```text
# DEMA Consulting SarifMark

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| SarifMark Version   | <version>                                          |
| Machine Name        | <machine-name>                                     |
| OS Version          | <os-version>                                       |
| DotNet Runtime      | <dotnet-runtime-version>                           |
| Timestamp           | <timestamp> UTC                                    |

✓ SarifMark_SarifReading - Passed
✓ SarifMark_MarkdownReportGeneration - Passed
✓ SarifMark_Enforcement - Passed

Total Tests: 3 / Passed: 3 / Failed: 0
```

### Silent Mode with Log File

Suppress console output and redirect all output to a log file for automated pipelines:

```shell
sarifmark --sarif analysis.sarif --report report.md --silent --log sarifmark.log
```

## Integration Examples

### GitHub Actions CI/CD

The following workflow runs CodeQL analysis and then generates a markdown report with SarifMark:

```yaml
- name: Initialize CodeQL
  uses: github/codeql-action/init@v3
  with:
    languages: csharp

- name: Perform CodeQL Analysis
  uses: github/codeql-action/analyze@v3
  with:
    output: codeql-results.sarif

- name: Install SarifMark
  run: dotnet tool install --global DemaConsulting.SarifMark

- name: Generate Report
  run: sarifmark --sarif codeql-results.sarif --report codeql-report.md --enforce
```

### Multiple SARIF Files

SarifMark processes one SARIF file per invocation. To report on results from multiple tools, run
the command once for each file:

```shell
sarifmark --sarif codeql.sarif   --report codeql-report.md
sarifmark --sarif sonarqube.sarif --report sonarqube-report.md
```

When a SARIF file contains results from multiple runs (multiple tools in a single file), SarifMark
generates indexed headings such as `Analysis (#1)` and `Analysis (#2)` to distinguish each run.

## Report Format

Generated markdown reports include the following sections for each run in the SARIF file:

- **Tool Information**: The name and version of the analysis tool that produced the results.
- **File Count**: The total number of files analyzed.
- **Summary**: A total count of issues found.
- **Issue Details**: For each finding — file location, line number, severity, rule ID, and message.

## Exit Codes

| Code | Meaning |
| --- | --- |
| `0` | Success — processing completed without errors |
| `1` | Error — invalid arguments, file not found, or processing error |
| Non-zero (with `--enforce`) | Issues found in the SARIF file |
