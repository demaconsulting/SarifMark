# SarifMark

## Verification Approach

The SarifMark system is verified through a combination of end-to-end integration tests and unit tests. Integration
tests reside in `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs` and invoke the compiled
`DemaConsulting.SarifMark.dll` end-to-end via the `Runner.Run` helper, which executes
`dotnet {dllPath} {args}` as a subprocess and captures exit codes and console output for assertion. No mocking is
used at the system level; the full compiled binary is exercised against real SARIF test-data files
(`sample.sarif`, `multi-result.sarif`, `multi-run.sarif`, `invalid.sarif`) stored in
`test/DemaConsulting.SarifMark.Tests/TestData/`. Unit tests exercise individual classes directly with console streams
redirected via `StringWriter`. The test framework is xUnit v3, executed via `dotnet test`. Three additional named
scenarios (`SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration`, `SarifMark_Enforcement`) are self-validation
tests invoked through the tool's own `--validate` flag; they are not xUnit test methods but named scenarios reported
in the self-validation output.

## Test Environment

Integration tests execute on Windows, Ubuntu (Linux), and macOS CI runners. Each runner tests against all supported
.NET runtime versions (8, 9, and 10), producing a three-platform by three-runtime matrix. This matrix provides
coverage for requirements `SarifMark-Plt-Windows`, `SarifMark-Plt-Linux`, `SarifMark-Plt-MacOS`,
`SarifMark-Plt-Net8`, `SarifMark-Plt-Net9`, and `SarifMark-Plt-Net10`. No external services or network access are
required to run the test suite.

## Acceptance Criteria

- All integration tests produce the expected exit code (0 for success, 1 for expected error conditions) on all three
  CI runner platforms (Windows, Ubuntu, macOS) against all three supported .NET runtime versions (8, 9, and 10).
- All integration tests produce the expected console output and any expected output files.
- The built-in `--validate` self-test exits with code 0 on all supported platforms and runtimes.
- Every system-level requirement maps to at least one named test scenario (IEC 62304 §5.7.2).

## Test Scenarios

**SarifMark_VersionFlag_OutputsVersion**: Invoking the tool with `--version` must print the version string to the
console and exit with code 0, confirming that the binary is correctly versioned and that the version flag is
recognized.
This scenario is tested by `SarifMark_VersionFlag_OutputsVersion`.

**SarifMark_HelpFlag_OutputsUsageInformation**: Invoking the tool with `--help` must print usage information listing
all supported flags and exit with code 0, confirming that the help text is complete and accessible.
This scenario is tested by `SarifMark_HelpFlag_OutputsUsageInformation`.

**SarifMark_ValidateFlag_RunsSelfValidation**: Invoking the tool with `--validate` must run the built-in
self-validation suite to completion and exit with code 0, confirming that the self-test infrastructure is functional
on the target platform and runtime.
This scenario is tested by `SarifMark_ValidateFlag_RunsSelfValidation`.

**SarifMark_MissingSarifParameter_ShowsError**: Invoking the tool without providing the `--sarif` argument must print
an error message and exit with code 1, confirming that the required parameter is enforced and the error is reported
clearly.
This scenario is tested by `SarifMark_MissingSarifParameter_ShowsError`.

**SarifMark_ValidSarifFile_ProcessesSuccessfully**: Invoking the tool with a valid SARIF file via `--sarif` must
produce analysis output and exit with code 0, confirming that well-formed SARIF input is parsed and processed without
error.
This scenario is tested by `SarifMark_ValidSarifFile_ProcessesSuccessfully`.

**SarifMark_NonExistentSarifFile_ShowsError**: Invoking the tool with a path to a non-existent SARIF file must print
an error message and exit with code 1, confirming that missing-file conditions are detected and reported gracefully.
This scenario is tested by `SarifMark_NonExistentSarifFile_ShowsError`.

**SarifMark_GenerateReport_CreatesReportFile**: Invoking the tool with `--sarif` and `--report {path}` must create
the markdown report file at the specified path, confirming that report generation writes output to disk correctly.
This scenario is tested by `SarifMark_GenerateReport_CreatesReportFile`.

**SarifMark_EnforceFlagWithIssues_ReturnsError**: Invoking the tool with `--enforce` against a SARIF file containing
findings must exit with code 1, confirming that enforcement mode correctly signals a non-zero exit when issues are
present.
This scenario is tested by `SarifMark_EnforceFlagWithIssues_ReturnsError`.

**SarifMark_SilentFlag_SuppressesOutput**: Invoking the tool with `--silent` must produce no output on the console,
confirming that the silent mode suppresses all standard output as required.
This scenario is tested by `SarifMark_SilentFlag_SuppressesOutput`.

**SarifMark_LogFile_WritesOutputToFile**: Invoking the tool with `--log {path}` must create the log file at the
specified path and write the expected output content to it, confirming that log-file redirection works correctly.
This scenario is tested by `SarifMark_LogFile_WritesOutputToFile`.

**SarifMark_UnknownArgument_ShowsError**: Invoking the tool with an unrecognized argument must print an error message
and exit with code 1, confirming that invalid command-line input is rejected with a clear diagnostic.
This scenario is tested by `SarifMark_UnknownArgument_ShowsError`.

**SarifMark_ReportDepth_IsConfigurable**: Invoking the tool with `--depth {n}` must produce a generated report whose
top-level headings use the specified heading depth, confirming that report heading depth is configurable.
This scenario is tested by `SarifMark_ReportDepth_IsConfigurable`.

**SarifMark_LegacyReportDepth_IsAccepted**: Invoking the tool with the deprecated `--report-depth {n}` alias must be
accepted and behave identically to `--depth`, confirming backward compatibility with older invocations.
This scenario is tested by `SarifMark_LegacyReportDepth_IsAccepted`.

**SarifMark_SarifReading**: This is a self-validation scenario (invoked via `--validate`) that reads a known SARIF
fixture and asserts that the parsed content matches the expected values, confirming that the SARIF reader produces
correct data structures.
This scenario is tested by `SarifMark_SarifReading`.

**SarifMark_MarkdownReportGeneration**: This is a self-validation scenario (invoked via `--validate`) that generates a
markdown report from a known SARIF fixture and asserts that the output content matches the expected report text,
confirming end-to-end report generation fidelity.
This scenario is tested by `SarifMark_MarkdownReportGeneration`.

**SarifMark_Enforcement**: This is a self-validation scenario (invoked via `--validate`) that runs the tool in
enforcement mode against a SARIF file containing findings and asserts that a non-zero exit code is returned,
confirming that the enforcement logic is correctly exercised from within the self-validation framework.
This scenario is tested by `SarifMark_Enforcement`.

## Requirements Coverage

The following list maps each system-level requirement to the named test scenario(s) that provide verification
evidence.

- **`SarifMark-System-Version`**: Tool displays version on `--version` —
  `SarifMark_VersionFlag_OutputsVersion`
- **`SarifMark-System-Help`**: Tool displays help on `--help` —
  `SarifMark_HelpFlag_OutputsUsageInformation`
- **`SarifMark-System-Validate`**: Tool supports `--validate` mode —
  `SarifMark_ValidateFlag_RunsSelfValidation`
- **`SarifMark-System-SarifRequired`**: Tool requires `--sarif` for analysis —
  `SarifMark_MissingSarifParameter_ShowsError`
- **`SarifMark-System-SarifAnalysis`**: Tool reads and analyses SARIF files —
  `SarifMark_ValidSarifFile_ProcessesSuccessfully`,
  `SarifMark_NonExistentSarifFile_ShowsError`
- **`SarifMark-System-Report`**: Tool generates markdown reports —
  `SarifMark_GenerateReport_CreatesReportFile`
- **`SarifMark-System-Enforce`**: Non-zero exit code in enforcement mode —
  `SarifMark_EnforceFlagWithIssues_ReturnsError`
- **`SarifMark-System-Silent`**: `--silent` suppresses console output —
  `SarifMark_SilentFlag_SuppressesOutput`
- **`SarifMark-System-LogFile`**: `--log` writes output to file —
  `SarifMark_LogFile_WritesOutputToFile`
- **`SarifMark-System-InvalidArgs`**: Unknown arguments rejected with error —
  `SarifMark_UnknownArgument_ShowsError`
- **`SarifMark-System-ReportDepth`**: Configurable heading depth —
  `SarifMark_ReportDepth_IsConfigurable`, `SarifMark_LegacyReportDepth_IsAccepted`
- **`SarifMark-Plt-Windows`**: Runs on Windows —
  `SarifMark_VersionFlag_OutputsVersion` (Windows runner)
- **`SarifMark-Plt-Linux`**: Runs on Linux (Ubuntu) —
  `SarifMark_VersionFlag_OutputsVersion` (Ubuntu runner)
- **`SarifMark-Plt-MacOS`**: Runs on macOS —
  `SarifMark_VersionFlag_OutputsVersion` (macOS runner)
- **`SarifMark-Plt-Net8`**: Supports .NET 8 runtime —
  `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 8)
- **`SarifMark-Plt-Net9`**: Supports .NET 9 runtime —
  `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 9)
- **`SarifMark-Plt-Net10`**: Supports .NET 10 runtime —
  `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 10)
