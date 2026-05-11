## Program Unit Verification

### Overview

The `Program` class is verified by the `ProgramTests` test class in
`test/DemaConsulting.SarifMark.Tests/ProgramTests.cs`. Tests call `Program.Main(string[])` directly with console streams
redirected via `StringWriter` to assert output and exit codes. Test data files in `TestData/` are used for file-dependent
tests. No mocking is required — `Program` is the top-level orchestrator with no injectable dependencies.

### Requirement Coverage

| Requirement ID | Description | Test Scenario(s) |
|---|---|---|
| `SarifMark-Program-Version` | Version property returns assembly version | `Program_Main_VersionFlag_DisplaysVersionOnly` |
| `SarifMark-Program-Main` | Main with no args returns error; version and help flags handled | `Program_Main_NoArguments_ReturnsError`, `Program_Main_VersionFlag_DisplaysVersionOnly`, `Program_Main_HelpFlag_DisplaysHelp` |
| `SarifMark-Program-Main-Exceptions` | ArgumentException and unknown arguments produce exit code 1 | `Program_Main_NoArguments_ReturnsError`, `Program_Main_UnknownArgument_ReturnsError` |
| `SarifMark-Program-Run` | Version and help processed before SARIF operations | `Program_Main_VersionFlag_DisplaysVersionOnly`, `Program_Main_HelpFlag_DisplaysHelp` |
| `SarifMark-Program-Banner` | Help output includes version and copyright banner | `Program_Main_HelpFlag_DisplaysHelp` |
| `SarifMark-Program-Help` | Help output lists all supported flags | `Program_Main_HelpFlag_DisplaysHelp` |
| `SarifMark-Program-Validation` | `--validate` dispatches to SelfTest | `Program_Main_ValidateFlag_RunsValidation` |
| `SarifMark-Program-SarifArgument` | Missing `--sarif` produces error | `Program_Main_NoArguments_ReturnsError` |
| `SarifMark-Program-SarifReading` | Valid SARIF file processed and results reported | `Program_Main_ValidSarifFile_ProcessesSuccessfully` |
| `SarifMark-Program-EnforcementCheck` | `--enforce` with issues returns exit code 1 | `Program_Main_EnforceFlagWithIssues_ReturnsError` |
| `SarifMark-Program-ReportGeneration` | `--report` creates the report file | `Program_Main_ReportFile_CreatesReport` |
