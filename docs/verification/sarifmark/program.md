## Program Unit Verification

### Verification Approach

`Program` is the top-level entry point with no injectable dependencies. Tests call `Program.Main(string[])` directly
with console streams redirected via `StringWriter` to capture output. The compiled assembly is exercised as-is; no
mocking framework is required.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Test data SARIF files (`sample.sarif`, `multi-result.sarif`) in
`test/DemaConsulting.SarifMark.Tests/TestData/` are used for file-dependent tests. No external services or
configuration are required.

### Acceptance Criteria

All `ProgramTests` test methods pass, confirming that every execution path — version display, help display, SARIF
processing, error handling, report generation, and enforcement — produces the expected exit code and console output.
No `Program` unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

- `Program_Main_NoArguments_ReturnsError`: Invoke `Program.Main` with no arguments; assert exit code is 1 and an
  error message is written to the console.
- `Program_Main_VersionFlag_DisplaysVersionOnly`: Invoke with `--version`; assert the version string is the only
  output and exit code is 0.
- `Program_Main_HelpFlag_DisplaysHelp`: Invoke with `--help`; assert usage text includes the tool version, copyright
  banner, and all supported flags; assert exit code is 0.
- `Program_Main_UnknownArgument_ReturnsError`: Invoke with an unrecognised argument; assert exit code is 1.
- `Program_Main_ValidateFlag_RunsValidation`: Invoke with `--validate`; assert self-validation runs and exit code
  is 0.
- `Program_Main_ValidSarifFile_ProcessesSuccessfully`: Invoke with `--sarif {valid path}`; assert analysis output
  contains tool name and finding summary; assert exit code is 0.
- `Program_Main_EnforceFlagWithIssues_ReturnsError`: Invoke with `--enforce` and a SARIF file containing findings;
  assert exit code is 1.
- `Program_Main_ReportFile_CreatesReport`: Invoke with `--sarif` and `--report {path}`; assert the report file is
  created on disk.

### Overview

The `Program` class is verified by the `ProgramTests` test class in
`test/DemaConsulting.SarifMark.Tests/ProgramTests.cs`. Tests call `Program.Main(string[])` directly with console streams
redirected via `StringWriter` to assert output and exit codes. Test data files in `TestData/` are used for file-dependent
tests. No mocking is required — `Program` is the top-level orchestrator with no injectable dependencies.

### Requirement Coverage

- **`SarifMark-Program-Version`**: Version property returns assembly version —
  `Program_Main_VersionFlag_DisplaysVersionOnly`
- **`SarifMark-Program-Main`**: Main with no args returns error; version and help flags handled —
  `Program_Main_NoArguments_ReturnsError`,
  `Program_Main_VersionFlag_DisplaysVersionOnly`,
  `Program_Main_HelpFlag_DisplaysHelp`
- **`SarifMark-Program-Main-Exceptions`**: ArgumentException and unknown arguments produce exit code 1 —
  `Program_Main_NoArguments_ReturnsError`,
  `Program_Main_UnknownArgument_ReturnsError`
- **`SarifMark-Program-Run`**: Version and help processed before SARIF operations —
  `Program_Main_VersionFlag_DisplaysVersionOnly`,
  `Program_Main_HelpFlag_DisplaysHelp`
- **`SarifMark-Program-Banner`**: Help output includes version and copyright banner —
  `Program_Main_HelpFlag_DisplaysHelp`
- **`SarifMark-Program-Help`**: Help output lists all supported flags —
  `Program_Main_HelpFlag_DisplaysHelp`
- **`SarifMark-Program-Validation`**: `--validate` dispatches to SelfTest —
  `Program_Main_ValidateFlag_RunsValidation`
- **`SarifMark-Program-SarifArgument`**: Missing `--sarif` produces error —
  `Program_Main_NoArguments_ReturnsError`
- **`SarifMark-Program-SarifReading`**: Valid SARIF file processed and results reported —
  `Program_Main_ValidSarifFile_ProcessesSuccessfully`
- **`SarifMark-Program-EnforcementCheck`**: `--enforce` with issues returns exit code 1 —
  `Program_Main_EnforceFlagWithIssues_ReturnsError`
- **`SarifMark-Program-ReportGeneration`**: `--report` creates the report file —
  `Program_Main_ReportFile_CreatesReport`
