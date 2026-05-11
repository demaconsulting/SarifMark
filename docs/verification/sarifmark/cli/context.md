### Context Unit Verification

#### Overview

The `Context` class is verified by the `ContextTests` test class in
`test/DemaConsulting.SarifMark.Tests/Cli/ContextTests.cs`. Tests call `Context.Create(string[])` directly via the
`InternalsVisibleTo` grant. Console streams are redirected via `StringWriter` for output assertions. Temporary files are
used for log-file tests.

#### Isolation Strategy

All dependencies are standard .NET BCL types (`Console`, `File`). No mocking framework is required. Each test is
self-contained and cleans up any temporary files in a `finally` block.

#### Requirement Coverage

| Requirement ID | Description | Test Scenario(s) |
|---|---|---|
| `SarifMark-Context-Create` | No-arg context has defaults | `Context_Create_NoArguments_ReturnsDefaultContext` |
| `SarifMark-Context-VersionFlag` | `--version` and `-v` set Version | `Context_Create_VersionFlag_SetsVersionTrue` |
| `SarifMark-Context-HelpFlag` | `-?`, `-h`, `--help` set Help | `Context_Create_HelpFlag_SetsHelpTrue` |
| `SarifMark-Context-SilentFlag` | `--silent` sets Silent and suppresses | `Context_Create_SilentFlag_SetsSilentTrue` |
| `SarifMark-Context-ValidateFlag` | `--validate` sets Validate=true | `Context_Create_ValidateFlag_SetsValidateTrue` |
| `SarifMark-Context-EnforceFlag` | `--enforce` sets `Enforce=true` | `Context_Create_EnforceFlag_SetsEnforceTrue` |
| `SarifMark-Context-SarifParam` | `--sarif` captures file path | `Context_Create_SarifParameter_SetsSarifFile` |
| `SarifMark-Context-SarifParam-MissingValue` | No value | `Context_Create_SarifWithoutValue_ThrowsArgumentException` |
| `SarifMark-Context-ReportParam` | `--report` captures file path | `Context_Create_ReportParameter_SetsReportFile` |
| `SarifMark-Context-ReportParam-MissingValue` | Missing | `Context_Create_ReportWithoutValue_ThrowsArgumentException` |
| `SarifMark-Context-ReportDepthParam` | `--depth` and `--report-depth` validated | `Context_Create_DepthParameter_SetsDepth` |
| `SarifMark-Context-HeadingParam` | `--heading` captures; missing throws | `Context_Create_HeadingArgument_SetsHeading` |
| `SarifMark-Context-ResultsParam` | `--results` captures path | `Context_Create_ResultsParameter_SetsResultsFile` |
| `SarifMark-Context-ResultLegacyAlias` | `--result` sets results | `Context_Create_ResultLegacyAlias_SetsResultsFile` |
| `SarifMark-Context-ResultsParam-MissingValue` | Value | `Context_Create_ResultsWithoutValue_ThrowsArgumentException` |
| `SarifMark-Context-LogParam` | `--log` opens file; invalid path throws | `Context_Create_LogFile_OpensFileSuccessfully` |
| `SarifMark-Context-LogParam-MissingValue` | Missing value | `Context_Create_LogWithoutValue_ThrowsArgumentException` |
| `SarifMark-Context-UnknownArgs` | Unknown argument throws | `Context_Create_UnknownArgument_ThrowsArgumentException` |
| `SarifMark-Context-WriteLine-Console` | Writes to console; silent suppresses | `Context_WriteLine_WritesToConsole` |
| `SarifMark-Context-WriteLine-Log` | Writes to log ignoring silent mode | `Context_WriteLine_WithLogFile_WritesToLog` |
| `SarifMark-Context-WriteError` | Parent requirement; satisfied by child requirements below | — |
| `SarifMark-Context-WriteError-Stderr` | Writes to stderr | `Context_WriteError_WritesToErrorAndSetsExitCode` |
| `SarifMark-Context-WriteError-Log` | Writes to log regardless | `Context_WriteError_WithLogFile_WritesToLog` |
| `SarifMark-Context-WriteError-ExitCode` | Sets ExitCode to 1 | `Context_WriteError_WritesToErrorAndSetsExitCode` |
| `SarifMark-Context-ExitCode` | Starts at 0; 1 after error | `Context_ExitCode_StartsAtZero_ChangesToOneAfterError` |
| `SarifMark-Context-Dispose` | `Dispose` closes log `StreamWriter` | `Context_Dispose_ProperlyClosesLogFile` |
