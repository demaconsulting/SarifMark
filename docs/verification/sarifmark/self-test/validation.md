### Validation Unit Verification

#### Overview

The `Validation` class is verified by the `ValidationTests` test class in
`test/DemaConsulting.SarifMark.Tests/SelfTest/ValidationTests.cs`. Tests run `Validation.Run` with a `--silent --log`
context to capture all output. Temporary log and results files are created via `PathHelpers.SafePathCombine`. The nested
`TemporaryDirectory` class is exposed via `InternalsVisibleTo`. All tests clean up in `finally` blocks.

#### Requirement Coverage

| Requirement ID | Description | Test Scenario(s) |
|---|---|---|
| `SarifMark-Validation-Run` | Valid context runs all three tests | `Validation_Run_ValidContext_RunsAllTests` |
| `SarifMark-Validation-NullCheck` | Null context throws `ArgumentNullException` | `Validation_Run_NullContext_ThrowsArgumentNullException` |
| `SarifMark-Validation-Header` | Validation header includes SarifMark version, machine name, OS version, .NET runtime, and timestamp | `Validation_Run_ValidContext_PrintsValidationHeader` |
| `SarifMark-Validation-SarifReadingTest` | `SarifMark_SarifReading` test runs and passes | `Validation_Run_ValidContext_RunsAllTests`, `Validation_Run_ValidContext_VerifiesSarifReadingOutput` |
| `SarifMark-Validation-ReportGenerationTest` | `SarifMark_MarkdownReportGeneration` test runs and passes | `Validation_Run_ValidContext_RunsAllTests`, `Validation_Run_ValidContext_VerifiesReportGenerationOutput` |
| `SarifMark-Validation-EnforcementTest` | `SarifMark_Enforcement` test runs and passes | `Validation_Run_ValidContext_RunsAllTests`, `Validation_Run_ValidContext_VerifiesEnforcementOutput` |
| `SarifMark-Validation-Summary` | Summary contains "Total Tests: 3", "Passed: 3", "Failed: 0" | `Validation_Run_ValidContext_PrintsSummary` |
| `SarifMark-Validation-TrxResultsFile` | TRX file created with `TestRun` and `SarifMark Self-Validation` | `Validation_Run_WithTrxResultsFile_WritesResultsFile` |
| `SarifMark-Validation-XmlResultsFile` | XML file created with `testsuites` and `SarifMark Self-Validation` | `Validation_Run_WithXmlResultsFile_WritesResultsFile` |
| `SarifMark-Validation-UnsupportedResultsFile` | Unsupported extension produces "Unsupported results file format" error | `Validation_Run_WithUnsupportedResultsFileExtension_WritesError` |
| `SarifMark-Validation-WriteResultsFile-IOFailure` | Non-existent directory produces "Error: Failed to write results file" error | `Validation_Run_WithNonExistentResultsDirectory_WritesError` |
| `SarifMark-Validation-TempDir` | `TemporaryDirectory` exists after construction; deleted after disposal | `Validation_TemporaryDirectory_Create_DirectoryExists`, `Validation_TemporaryDirectory_Dispose_DirectoryDeleted` |
