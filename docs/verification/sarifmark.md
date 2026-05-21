# SarifMark System Verification

## Verification Strategy

The SarifMark system is verified through a combination of end-to-end integration tests and unit tests. Integration
tests are defined in `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs` and invoke the compiled
`DemaConsulting.SarifMark.dll` via the `Runner.Run` helper, asserting exit codes and console output against expected
values. Unit tests exercise individual classes directly with console streams redirected via `StringWriter`. The test
framework is xUnit v3, executed via `dotnet test`. Platform verification tests run the full integration suite on
Windows, Ubuntu (Linux), and macOS CI runners against .NET 8, 9, and 10.

## Overview

The SarifMark system is verified through end-to-end integration tests and unit tests. Integration tests exercise the
published `DemaConsulting.SarifMark.dll` by invoking `dotnet {dllPath} {args}` via the `Runner.Run` helper and asserting
exit codes and console output. Unit tests exercise individual classes directly with console streams redirected via
`StringWriter`.

## Test Environments

Integration tests execute on Windows, Ubuntu (Linux), and macOS CI runners. Each runner tests against all supported
.NET runtime versions (8, 9, and 10). This matrix provides coverage for requirements
`SarifMark-Plt-Windows`, `SarifMark-Plt-Linux`, `SarifMark-Plt-MacOS`, `SarifMark-Plt-Net8`, `SarifMark-Plt-Net9`, and
`SarifMark-Plt-Net10`.

## Integration Test Approach

All system-level integration tests reside in
`test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs` and use the `Runner.Run` helper to invoke the compiled DLL
end-to-end. Test data SARIF files (`sample.sarif`, `multi-result.sarif`, `multi-run.sarif`, `invalid.sarif`) reside in
`test/DemaConsulting.SarifMark.Tests/TestData/`. Self-validation tests (`SarifMark_SarifReading`,
`SarifMark_MarkdownReportGeneration`, `SarifMark_Enforcement`) are invoked via the `--validate` flag and run inside the
tool itself; they are not xUnit test methods but named scenarios reported in the self-validation output.

## Acceptance Criteria

The system-level test suite is considered passing when all integration tests produce the expected exit code (0 for
success, 1 for expected error conditions), the expected console output, and the expected output files on all three CI
runner platforms (Windows, Ubuntu, macOS) running all three supported .NET runtime versions (8, 9, and 10). The
built-in `--validate` self-test must exit with code 0 on all supported platforms and runtimes. Every system-level
requirement must map to at least one named test scenario (IEC 62304 §5.7.2).

## System-Level Test Scenarios

The following named scenarios correspond to xUnit test methods in `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs`
and self-validation tests executed via `--validate`:

- `SarifMark_VersionFlag_OutputsVersion`: Invoke the tool with `--version`; assert the version string is printed to
  the console and exit code is 0.
- `SarifMark_HelpFlag_OutputsUsageInformation`: Invoke with `--help`; assert usage information lists all supported
  flags and exit code is 0.
- `SarifMark_ValidateFlag_RunsSelfValidation`: Invoke with `--validate`; assert the self-validation suite runs to
  completion and exit code is 0.
- `SarifMark_MissingSarifParameter_ShowsError`: Invoke without `--sarif`; assert an error message is printed and exit
  code is 1.
- `SarifMark_ValidSarifFile_ProcessesSuccessfully`: Invoke with a valid SARIF file via `--sarif`; assert analysis
  output is produced and exit code is 0.
- `SarifMark_NonExistentSarifFile_ShowsError`: Invoke with a non-existent SARIF path; assert an error message is
  printed and exit code is 1.
- `SarifMark_GenerateReport_CreatesReportFile`: Invoke with `--sarif` and `--report {path}`; assert the markdown
  report file is created on disk.
- `SarifMark_EnforceFlagWithIssues_ReturnsError`: Invoke with `--enforce` and a SARIF file containing findings;
  assert exit code is 1.
- `SarifMark_SilentFlag_SuppressesOutput`: Invoke with `--silent`; assert no output is written to the console.
- `SarifMark_LogFile_WritesOutputToFile`: Invoke with `--log {path}`; assert the log file is created and contains the
  expected output.
- `SarifMark_UnknownArgument_ShowsError`: Invoke with an unrecognised argument; assert an error message is printed and
  exit code is 1.
- `SarifMark_ReportDepth_IsConfigurable`: Invoke with `--depth {n}`; assert the generated report uses the heading
  depth matching the specified value.
- `SarifMark_LegacyReportDepth_IsAccepted`: Invoke with `--report-depth {n}`; assert the deprecated alias is accepted
  and behaves identically to `--depth`.
- `SarifMark_SarifReading`: Self-validation scenario that reads a known SARIF fixture and asserts parsed content
  is correct (runs via `--validate`).
- `SarifMark_MarkdownReportGeneration`: Self-validation scenario that generates a markdown report from a known SARIF
  fixture and asserts the output content (runs via `--validate`).
- `SarifMark_Enforcement`: Self-validation scenario that asserts non-zero exit code when enforcement mode is used with
  a SARIF file containing findings (runs via `--validate`).

## Requirement Coverage

The following list maps each system-level requirement to the named test scenario(s) that provide
verification evidence.

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
