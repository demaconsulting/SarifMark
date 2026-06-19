## SelfTest

### Verification Approach

The `SelfTest` subsystem is verified through unit-level tests that create a `Context` by calling
`Context.Create(string[])` with the `--validate` flag and then invoke `Validation.Run(context)` directly in-process.
No subprocess is spawned and no compiled DLL path is required; the subsystem is exercised within the xUnit test runner
using the same process. Tests assert that self-validation runs with exit code 0, produces the expected log output
(including `Total Tests:` and named scenario results), writes result files in the requested format (TRX and JUnit XML),
and that the depth parameter influences the self-validation report. Tests are defined in
`test/DemaConsulting.SarifMark.Tests/SelfTest/SelfTestTests.cs` using the xUnit v3 framework.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Temporary log files and result files (`.trx`, `.xml`) are created in
the OS temporary directory and cleaned up after each test in `finally` blocks. No external services, compiled DLL
paths, or network configuration are required.

### Acceptance Criteria

- All `SelfTestTests` test methods pass.
- `--validate` completes with exit code 0.
- Result files are created with correct format and content.
- The enforcement scenario is exercised within the self-validation suite.
- No `SelfTest` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**SelfTest_ValidateFlag_RunsSelfValidation**: Invoke the tool with `--validate`; assert self-validation runs and exits
with code 0.
This scenario is tested by `SelfTest_ValidateFlag_RunsSelfValidation`.

**SelfTest_ResultsFile_TrxPath_WritesTrxFile**: Invoke `--validate --results {path.trx}`; assert the TRX result file is
created with a `<TestRun` element.
This scenario is tested by `SelfTest_ResultsFile_TrxPath_WritesTrxFile`.

**SelfTest_ResultsFile_XmlPath_WritesJUnitFile**: Invoke `--validate --results {path.xml}`; assert the JUnit XML result
file is created with a `<testsuite` element.
This scenario is tested by `SelfTest_ResultsFile_XmlPath_WritesJUnitFile`.

**SelfTest_EnforcementTest_RunsWithinValidation**: Invoke `--validate`; assert the enforcement scenario
(`SarifMark_Enforcement`) runs within the self-validation suite and is reported in the output.
This scenario is tested by `SelfTest_EnforcementTest_RunsWithinValidation`.
