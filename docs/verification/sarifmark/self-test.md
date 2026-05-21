## SelfTest Subsystem Verification

### Verification Strategy

The `SelfTest` subsystem is verified through tests that invoke the subsystem via the `--validate` code path of the
compiled DLL using the `Runner.Run` helper. Tests assert that self-validation runs, produces the expected output,
writes result files in the requested format, and exercises the enforcement scenario. Tests are defined in
`test/DemaConsulting.SarifMark.Tests/SelfTest/SelfTestTests.cs` using the xUnit v3 framework.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. The compiled DLL must be available to the `Runner.Run` helper.
Temporary result files (`.trx`, `.xml`) are created in the OS temporary directory and cleaned up after each test.
No external services or network configuration are required.

### Acceptance Criteria

All `SelfTestTests` test methods pass, confirming that `--validate` completes with exit code 0, that result files
are created with correct format and content, and that the enforcement scenario is exercised within the self-validation
suite. No `SelfTest` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

- `SelfTest_ValidateFlag_RunsSelfValidation`: Invoke the tool with `--validate`; assert self-validation runs and
  exits with code 0.
- `SelfTest_ResultsFile_TrxPath_WritesTrxFile`: Invoke `--validate --results {path.trx}`; assert the TRX result
  file is created with a `<TestRun` element.
- `SelfTest_ResultsFile_XmlPath_WritesJUnitFile`: Invoke `--validate --results {path.xml}`; assert the JUnit XML
  result file is created with a `<testsuite` element.
- `SelfTest_EnforcementTest_RunsWithinValidation`: Invoke `--validate`; assert the enforcement scenario
  (`SarifMark_Enforcement`) runs within the self-validation suite and is reported in the output.

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
