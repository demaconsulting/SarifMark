### Context Unit Verification

#### Verification Approach

The `Context` class is tested by calling `Context.Create(string[])` directly via the `InternalsVisibleTo` grant.
All dependencies are standard .NET BCL types (`Console`, `File`) — no mocking framework is required. Console streams
are redirected via `StringWriter` for output assertions. Temporary files are used for log-file tests and are always
cleaned up in `finally` blocks.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Temporary files are created in the OS temporary directory
(`Path.GetTempPath()`) for log-file tests. No external services or network configuration are required.

#### Acceptance Criteria

All `ContextTests` test methods pass, confirming that every command-line argument is parsed correctly, output routing
works as expected, error handling sets the exit code, and resources are disposed cleanly. No `Context` unit
requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

- `Context_Create_NoArguments_ReturnsDefaultContext`: Create context with no arguments; assert all properties have
  their default values.
- `Context_Create_VersionFlag_SetsVersionTrue`: Pass `--version`; assert `Version` is `true`.
- `Context_Create_HelpFlag_SetsHelpTrue`: Pass `--help`, `-?`, or `-h`; assert `Help` is `true`.
- `Context_Create_SilentFlag_SetsSilentTrue`: Pass `--silent`; assert `Silent` is `true` and console output is
  suppressed.
- `Context_Create_ValidateFlag_SetsValidateTrue`: Pass `--validate`; assert `Validate` is `true`.
- `Context_Create_EnforceFlag_SetsEnforceTrue`: Pass `--enforce`; assert `Enforce` is `true`.
- `Context_Create_SarifParameter_SetsSarifFile`: Pass `--sarif {path}`; assert `SarifFile` is set to the provided
  path.
- `Context_Create_SarifWithoutValue_ThrowsArgumentException`: Pass `--sarif` with no following value; assert
  `ArgumentException` is thrown.
- `Context_Create_ReportParameter_SetsReportFile`: Pass `--report {path}`; assert `ReportFile` is set.
- `Context_Create_ReportWithoutValue_ThrowsArgumentException`: Pass `--report` with no value; assert
  `ArgumentException`.
- `Context_Create_DepthParameter_SetsDepth`: Pass `--depth {n}` or `--report-depth {n}`; assert `Depth` is set to
  the provided value.
- `Context_Create_HeadingArgument_SetsHeading`: Pass `--heading {text}`; assert `Heading` is set.
- `Context_Create_ResultsParameter_SetsResultsFile`: Pass `--results {path}`; assert `ResultsFile` is set.
- `Context_Create_ResultLegacyAlias_SetsResultsFile`: Pass `--result {path}`; assert legacy alias sets `ResultsFile`.
- `Context_Create_ResultsWithoutValue_ThrowsArgumentException`: Pass `--results` with no value; assert
  `ArgumentException`.
- `Context_Create_LogFile_OpensFileSuccessfully`: Pass `--log {path}`; assert the log file is opened and subsequent
  writes go to the file.
- `Context_Create_LogWithoutValue_ThrowsArgumentException`: Pass `--log` with no value; assert `ArgumentException`.
- `Context_Create_UnknownArgument_ThrowsArgumentException`: Pass an unrecognised argument; assert `ArgumentException`.
- `Context_WriteLine_WritesToConsole`: Call `WriteLine` with a message; assert the text appears on the console.
- `Context_WriteLine_WithLogFile_WritesToLog`: Call `WriteLine` with a log file open; assert the text is written to
  the log file.
- `Context_WriteError_WritesToErrorAndSetsExitCode`: Call `WriteError` with a message; assert the text is written to
  stderr and exit code becomes 1.
- `Context_WriteError_WithLogFile_WritesToLog`: Call `WriteError` with a log file open; assert the error text is
  written to the log regardless of silent mode.
- `Context_ExitCode_StartsAtZero_ChangesToOneAfterError`: Verify exit code is 0 initially and becomes 1 after
  `WriteError` is called.
- `Context_Dispose_ProperlyClosesLogFile`: Dispose the context; assert the log `StreamWriter` is closed and no
  further writes are possible.

#### Overview

The `Context` class is verified by the `ContextTests` test class in
`test/DemaConsulting.SarifMark.Tests/Cli/ContextTests.cs`. Tests call `Context.Create(string[])` directly via the
`InternalsVisibleTo` grant. Console streams are redirected via `StringWriter` for output assertions. Temporary files are
used for log-file tests.

#### Isolation Strategy

All dependencies are standard .NET BCL types (`Console`, `File`). No mocking framework is required. Each test is
self-contained and cleans up any temporary files in a `finally` block.

#### Requirement Coverage

- **`SarifMark-Context-Create`**: No-arg context has defaults —
  `Context_Create_NoArguments_ReturnsDefaultContext`
- **`SarifMark-Context-VersionFlag`**: `--version` and `-v` set Version —
  `Context_Create_VersionFlag_SetsVersionTrue`
- **`SarifMark-Context-HelpFlag`**: `-?`, `-h`, `--help` set Help —
  `Context_Create_HelpFlag_SetsHelpTrue`
- **`SarifMark-Context-SilentFlag`**: `--silent` sets Silent and suppresses —
  `Context_Create_SilentFlag_SetsSilentTrue`
- **`SarifMark-Context-ValidateFlag`**: `--validate` sets Validate=true —
  `Context_Create_ValidateFlag_SetsValidateTrue`
- **`SarifMark-Context-EnforceFlag`**: `--enforce` sets `Enforce=true` —
  `Context_Create_EnforceFlag_SetsEnforceTrue`
- **`SarifMark-Context-SarifParam`**: `--sarif` captures file path —
  `Context_Create_SarifParameter_SetsSarifFile`
- **`SarifMark-Context-SarifParam-MissingValue`**: No value —
  `Context_Create_SarifWithoutValue_ThrowsArgumentException`
- **`SarifMark-Context-ReportParam`**: `--report` captures file path —
  `Context_Create_ReportParameter_SetsReportFile`
- **`SarifMark-Context-ReportParam-MissingValue`**: Missing —
  `Context_Create_ReportWithoutValue_ThrowsArgumentException`
- **`SarifMark-Context-ReportDepthParam`**: `--depth` and `--report-depth` validated —
  `Context_Create_DepthParameter_SetsDepth`
- **`SarifMark-Context-HeadingParam`**: `--heading` captures; missing throws —
  `Context_Create_HeadingArgument_SetsHeading`
- **`SarifMark-Context-ResultsParam`**: `--results` captures path —
  `Context_Create_ResultsParameter_SetsResultsFile`
- **`SarifMark-Context-ResultLegacyAlias`**: `--result` sets results —
  `Context_Create_ResultLegacyAlias_SetsResultsFile`
- **`SarifMark-Context-ResultsParam-MissingValue`**: Value —
  `Context_Create_ResultsWithoutValue_ThrowsArgumentException`
- **`SarifMark-Context-LogParam`**: `--log` opens file; invalid path throws —
  `Context_Create_LogFile_OpensFileSuccessfully`
- **`SarifMark-Context-LogParam-MissingValue`**: Missing value —
  `Context_Create_LogWithoutValue_ThrowsArgumentException`
- **`SarifMark-Context-UnknownArgs`**: Unknown argument throws —
  `Context_Create_UnknownArgument_ThrowsArgumentException`
- **`SarifMark-Context-WriteLine-Console`**: Writes to console; silent suppresses —
  `Context_WriteLine_WritesToConsole`
- **`SarifMark-Context-WriteLine-Log`**: Writes to log ignoring silent mode —
  `Context_WriteLine_WithLogFile_WritesToLog`
- **`SarifMark-Context-WriteError`**: Parent requirement; satisfied by child requirements below — —
- **`SarifMark-Context-WriteError-Stderr`**: Writes to stderr —
  `Context_WriteError_WritesToErrorAndSetsExitCode`
- **`SarifMark-Context-WriteError-Log`**: Writes to log regardless —
  `Context_WriteError_WithLogFile_WritesToLog`
- **`SarifMark-Context-WriteError-ExitCode`**: Sets ExitCode to 1 —
  `Context_WriteError_WritesToErrorAndSetsExitCode`
- **`SarifMark-Context-ExitCode`**: Starts at 0; 1 after error —
  `Context_ExitCode_StartsAtZero_ChangesToOneAfterError`
- **`SarifMark-Context-Dispose`**: `Dispose` closes log `StreamWriter` —
  `Context_Dispose_ProperlyClosesLogFile`
