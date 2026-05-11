# SarifMark System Verification

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

## Requirement Coverage

The following table maps each system-level requirement to the named test scenario(s) that provide verification evidence.

| Requirement ID | Description | Test Scenario(s) |
|---|---|---|
| `SarifMark-System-Version` | Tool displays version on `--version` | `SarifMark_VersionFlag_OutputsVersion` |
| `SarifMark-System-Help` | Tool displays help on `--help` | `SarifMark_HelpFlag_OutputsUsageInformation` |
| `SarifMark-System-Validate` | Tool supports `--validate` mode | `SarifMark_ValidateFlag_RunsSelfValidation` |
| `SarifMark-System-SarifRequired` | Tool requires `--sarif` for analysis | `SarifMark_MissingSarifParameter_ShowsError` |
| `SarifMark-System-SarifAnalysis` | Tool reads and analyses SARIF files | `SarifMark_ValidSarifFile_ProcessesSuccessfully`, `SarifMark_NonExistentSarifFile_ShowsError` |
| `SarifMark-System-Report` | Tool generates markdown reports | `SarifMark_GenerateReport_CreatesReportFile` |
| `SarifMark-System-Enforce` | Non-zero exit code in enforcement mode | `SarifMark_EnforceFlagWithIssues_ReturnsError` |
| `SarifMark-System-Silent` | `--silent` suppresses console output | `SarifMark_SilentFlag_SuppressesOutput` |
| `SarifMark-System-LogFile` | `--log` writes output to file | `SarifMark_LogFile_WritesOutputToFile` |
| `SarifMark-System-InvalidArgs` | Unknown arguments rejected with error | `SarifMark_UnknownArgument_ShowsError` |
| `SarifMark-System-ReportDepth` | Configurable heading depth | `SarifMark_ReportDepth_IsConfigurable`, `SarifMark_LegacyReportDepth_IsAccepted` |
| `SarifMark-Plt-Windows` | Runs on Windows | `SarifMark_VersionFlag_OutputsVersion` (Windows runner) |
| `SarifMark-Plt-Linux` | Runs on Linux (Ubuntu) | `SarifMark_VersionFlag_OutputsVersion` (Ubuntu runner) |
| `SarifMark-Plt-MacOS` | Runs on macOS | `SarifMark_VersionFlag_OutputsVersion` (macOS runner) |
| `SarifMark-Plt-Net8` | Supports .NET 8 runtime | `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 8) |
| `SarifMark-Plt-Net9` | Supports .NET 9 runtime | `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 9) |
| `SarifMark-Plt-Net10` | Supports .NET 10 runtime | `SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration` (.NET 10) |
