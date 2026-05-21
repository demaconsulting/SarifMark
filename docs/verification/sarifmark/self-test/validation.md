### Validation Unit Verification

#### Verification Approach

The `Validation` class is tested by calling `Validation.Run` directly with a `--silent --log` context so that all
output is captured in a log file. Temporary log and result files are created in the OS temporary directory via
`PathHelpers.SafePathCombine`. The nested `TemporaryDirectory` class is accessed via the `InternalsVisibleTo` grant.
All I/O uses real file-system operations; no mocking framework is required. Tests clean up all temporary files and
directories in `finally` blocks.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Temporary log files (`.log`) and result files (`.trx`, `.xml`) are
created in the OS temporary directory (`Path.GetTempPath()`). The embedded SARIF fixture used by the validation
scenarios is written to a `TemporaryDirectory` instance during each test run. No external services or network
configuration are required.

#### Acceptance Criteria

All `ValidationTests` test methods pass, confirming that the validation header, all three built-in test scenarios,
result file generation, error handling for unsupported result file formats, and `TemporaryDirectory` lifecycle
management behave correctly. No `Validation` unit requirement may remain without at least one named test scenario
(IEC 62304 §5.5.2).

#### Test Scenarios

- `Validation_Run_ValidContext_RunsAllTests`: Run validation with a valid context; assert all three scenarios
  (`SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration`, `SarifMark_Enforcement`) appear in the output.
- `Validation_Run_NullContext_ThrowsArgumentNullException`: Pass `null` as the context; assert
  `ArgumentNullException` is thrown.
- `Validation_Run_ValidContext_PrintsValidationHeader`: Run validation; assert the header contains the SarifMark
  version, machine name, OS version, .NET runtime, and timestamp.
- `Validation_Run_ValidContext_VerifiesSarifReadingOutput`: Run validation; assert the `SarifMark_SarifReading`
  scenario output is present and indicates a pass.
- `Validation_Run_ValidContext_VerifiesReportGenerationOutput`: Run validation; assert the
  `SarifMark_MarkdownReportGeneration` scenario output is present and indicates a pass.
- `Validation_Run_ValidContext_VerifiesEnforcementOutput`: Run validation; assert the `SarifMark_Enforcement`
  scenario output is present and indicates a pass.
- `Validation_Run_ValidContext_PrintsSummary`: Run validation; assert the summary line contains "Total Tests: 3",
  "Passed: 3", and "Failed: 0".
- `Validation_Run_WithTrxResultsFile_WritesResultsFile`: Run with `--results {path.trx}`; assert the TRX file is
  created and contains a `<TestRun` element and `SarifMark Self-Validation` text.
- `Validation_Run_WithXmlResultsFile_WritesResultsFile`: Run with `--results {path.xml}`; assert the JUnit XML file
  is created and contains a `<testsuites` element and `SarifMark Self-Validation` text.
- `Validation_Run_WithUnsupportedResultsFileExtension_WritesError`: Run with `--results {path.csv}`; assert an
  "Unsupported results file format" error is written.
- `Validation_Run_WithNonExistentResultsDirectory_WritesError`: Run with `--results` pointing to a non-existent
  directory; assert an "Error: Failed to write results file" error is written.
- `Validation_TemporaryDirectory_Create_DirectoryExists`: Construct a `TemporaryDirectory` instance; assert the
  directory exists on disk.
- `Validation_TemporaryDirectory_Dispose_DirectoryDeleted`: Dispose a `TemporaryDirectory` instance; assert the
  directory is deleted from disk.

#### Overview

The `Validation` class is verified by the `ValidationTests` test class in
`test/DemaConsulting.SarifMark.Tests/SelfTest/ValidationTests.cs`. Tests run `Validation.Run` with a `--silent --log`
context to capture all output. Temporary log and results files are created via `PathHelpers.SafePathCombine`. The nested
`TemporaryDirectory` class is exposed via `InternalsVisibleTo`. All tests clean up in `finally` blocks.

#### Requirement Coverage

- **`SarifMark-Validation-Run`**: Valid context runs all three tests —
  `Validation_Run_ValidContext_RunsAllTests`
- **`SarifMark-Validation-NullCheck`**: Null context throws `ArgumentNullException` —
  `Validation_Run_NullContext_ThrowsArgumentNullException`
- **`SarifMark-Validation-Header`**: Validation header includes SarifMark version, machine name,
  OS version, .NET runtime, and timestamp —
  `Validation_Run_ValidContext_PrintsValidationHeader`
- **`SarifMark-Validation-SarifReadingTest`**: `SarifMark_SarifReading` test runs and passes —
  `Validation_Run_ValidContext_RunsAllTests`,
  `Validation_Run_ValidContext_VerifiesSarifReadingOutput`
- **`SarifMark-Validation-ReportGenerationTest`**: `SarifMark_MarkdownReportGeneration` test runs
  and passes —
  `Validation_Run_ValidContext_RunsAllTests`,
  `Validation_Run_ValidContext_VerifiesReportGenerationOutput`
- **`SarifMark-Validation-EnforcementTest`**: `SarifMark_Enforcement` test runs and passes —
  `Validation_Run_ValidContext_RunsAllTests`,
  `Validation_Run_ValidContext_VerifiesEnforcementOutput`
- **`SarifMark-Validation-Summary`**: Summary contains "Total Tests: 3", "Passed: 3", "Failed: 0" —
  `Validation_Run_ValidContext_PrintsSummary`
- **`SarifMark-Validation-TrxResultsFile`**: TRX file created with `TestRun` and
  `SarifMark Self-Validation` —
  `Validation_Run_WithTrxResultsFile_WritesResultsFile`
- **`SarifMark-Validation-XmlResultsFile`**: XML file created with `testsuites` and
  `SarifMark Self-Validation` —
  `Validation_Run_WithXmlResultsFile_WritesResultsFile`
- **`SarifMark-Validation-UnsupportedResultsFile`**: Unsupported extension produces
  "Unsupported results file format" error —
  `Validation_Run_WithUnsupportedResultsFileExtension_WritesError`
- **`SarifMark-Validation-WriteResultsFile-IOFailure`**: Non-existent directory produces
  "Error: Failed to write results file" error —
  `Validation_Run_WithNonExistentResultsDirectory_WritesError`
- **`SarifMark-Validation-TempDir`**: `TemporaryDirectory` exists after construction; deleted after
  disposal —
  `Validation_TemporaryDirectory_Create_DirectoryExists`,
  `Validation_TemporaryDirectory_Dispose_DirectoryDeleted`
