## TestResults

### Verification Approach

`DemaConsulting.TestResults` is verified through unit tests in `ValidationTests.cs` that exercise
the package's advertised features within the SarifMark self-validation pipeline. Tests run against
the real `TestResults`, `TestResult`, `TrxSerializer`, and `JUnitSerializer` types without
mocking, providing direct integration evidence that each advertised feature functions as required.

### Test Scenarios

**Validation_Run_ValidContext_PrintsSummary**: Running self-validation with a valid context
produces summary lines reporting "Total Tests: 3", "Passed: 3", and "Failed: 0", confirming that
the TestResults collection is correctly created, populated with three result entries, and that
the `Results.Count` and outcome filtering work as expected.

**Validation_Run_ValidContext_RunsAllTests**: The log output contains all three scenario test
names with passing indicators, confirming that individual TestResult entries with Passed outcomes
are correctly added to the collection.

**Validation_Run_ValidContext_VerifiesSarifReadingOutput**: The log output contains
"✓ SarifMark_SarifReading - Passed", confirming that a TestResult entry for the SARIF-reading
scenario is correctly recorded with the Passed outcome.

**Validation_Run_ValidContext_VerifiesReportGenerationOutput**: The log output contains
"✓ SarifMark_MarkdownReportGeneration - Passed", confirming that a TestResult entry for the
report-generation scenario is correctly recorded with the Passed outcome.

**Validation_Run_ValidContext_VerifiesEnforcementOutput**: The log output contains
"✓ SarifMark_Enforcement - Passed", confirming that a TestResult entry for the enforcement
scenario is correctly recorded with the Passed outcome.

**Validation_Run_WithTrxResultsFile_WritesResultsFile**: Running self-validation with a `.trx`
results file path produces a file containing "TestRun" and "SarifMark Self-Validation",
confirming that `TrxSerializer` correctly serializes the TestResults collection to TRX XML
format.

**Validation_Run_WithXmlResultsFile_WritesResultsFile**: Running self-validation with a `.xml`
results file path produces a file containing "testsuites" and "SarifMark Self-Validation",
confirming that `JUnitSerializer` correctly serializes the TestResults collection to JUnit XML
format.
