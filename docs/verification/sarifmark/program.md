## Program

### Verification Approach

`Program` is the top-level entry point with no injectable dependencies. Tests call `Program.Main(string[])` directly
with console streams redirected via `StringWriter` to capture output. The compiled assembly is exercised as-is; no
mocking framework is required.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Test data SARIF files (`sample.sarif`, `multi-result.sarif`) in
`test/DemaConsulting.SarifMark.Tests/TestData/` are used for file-dependent tests. No external services or
configuration are required.

### Acceptance Criteria

All `ProgramTests` test methods pass, confirming that every execution path — argument parsing, operation dispatch,
version display, help display, SARIF processing, error handling, report generation, and enforcement — produces the
expected exit code and console output.
No `Program` unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**Program_Main_NoArguments_ReturnsError**: Invoke `Program.Main` with no arguments; assert exit code is 1 and an
error message is written to stderr (`Console.Error`).
This scenario is tested by `Program_Main_NoArguments_ReturnsError`.

**Program_Main_VersionFlag_DisplaysVersionOnly**: Invoke with `--version`; assert the version string is the only
output and exit code is 0.
This scenario is tested by `Program_Main_VersionFlag_DisplaysVersionOnly`.

**Program_Main_HelpFlag_DisplaysHelp**: Invoke with `--help`; assert usage text includes the tool version, copyright
banner, and all supported flags; assert exit code is 0.
This scenario is tested by `Program_Main_HelpFlag_DisplaysHelp`.

**Program_Main_UnknownArgument_ReturnsError**: Invoke with an unrecognized argument; assert exit code is 1.
This scenario is tested by `Program_Main_UnknownArgument_ReturnsError`.

**Program_Main_ValidateFlag_RunsValidation**: Invoke with `--validate`; assert self-validation runs and exits with
code 0, and the TRX results file is created and contains a `<TestRun` element, confirming that the validation
pipeline executed to completion.
This scenario is tested by `Program_Main_ValidateFlag_RunsValidation`.

**Program_Main_ValidSarifFile_ProcessesSuccessfully**: Invoke with `--sarif {valid path}`; assert analysis output
contains tool name and finding summary; assert exit code is 0.
This scenario is tested by `Program_Main_ValidSarifFile_ProcessesSuccessfully`.

**Program_Main_EnforceFlagWithIssues_ReturnsError**: Invoke with `--enforce` and a SARIF file containing findings;
assert exit code is 1.
This scenario is tested by `Program_Main_EnforceFlagWithIssues_ReturnsError`.

**Program_Main_ReportFile_CreatesReport**: Invoke with `--sarif` and `--report {path}`; assert the report file is
created on disk.
This scenario is tested by `Program_Main_ReportFile_CreatesReport`.
