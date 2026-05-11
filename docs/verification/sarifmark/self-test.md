## SelfTest Subsystem Verification

### Overview

The `SelfTest` subsystem is verified by the `SelfTestTests` test class in
`test/DemaConsulting.SarifMark.Tests/SelfTest/SelfTestTests.cs`. Tests invoke the `SelfTest` subsystem through the
`--validate` code path, asserting that self-validation runs, produces output, and writes result files.

### Requirement Coverage

- **`SarifMark-Validate-Mode`**: `--validate` runs self-validation and exits 0 —
  `SelfTest_ValidateFlag_RunsSelfValidation`
- **`SarifMark-Validate-ResultFiles`**: TRX results file written; JUnit XML file written —
  `SelfTest_ResultsFile_TrxPath_WritesTrxFile`,
  `SelfTest_ResultsFile_XmlPath_WritesJUnitFile`
- **`SarifMark-Validate-TrxFormat`**: TRX file contains `<TestRun` element —
  `SelfTest_ResultsFile_TrxPath_WritesTrxFile`
- **`SarifMark-Validate-JUnitFormat`**: XML file contains `<testsuite` element —
  `SelfTest_ResultsFile_XmlPath_WritesJUnitFile`
- **`SarifMark-Enforce-Mode`**: Enforcement test runs within validation —
  `SelfTest_EnforcementTest_RunsWithinValidation`
- **`SarifMark-Enforce-ExitCode`**: Enforcement test verifies non-zero exit code when issues found —
  `SelfTest_EnforcementTest_RunsWithinValidation`
