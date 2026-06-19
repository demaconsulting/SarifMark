### Context

#### Verification Approach

The `Context` class is tested by calling `Context.Create(string[])` directly via the `InternalsVisibleTo` grant.
All dependencies are standard .NET BCL types (`Console`, `File`) — no mocking framework is required. Console streams
are redirected via `StringWriter` for output assertions. Temporary files are used for log-file tests and are always
cleaned up in `finally` blocks.

Several log-file test methods construct temporary file paths using `PathHelpers.SafePathCombine` rather than
`Path.Combine`, which means the tests indirectly exercise `PathHelpers` as a cross-unit dependency. `PathHelpers`
is documented as an explicit dependency of `Context` in the design documentation; its own correctness is verified
separately in the PathHelpers unit verification document.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Temporary files are created in the OS temporary directory
(`Path.GetTempPath()`) for log-file tests. No external services or network configuration are required.

#### Acceptance Criteria

All `ContextTests` test methods pass, confirming that every command-line argument is parsed correctly, output routing
works as expected, error handling sets the exit code, and resources are disposed cleanly. No `Context` unit
requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**Context_Create_NoArguments_ReturnsDefaultContext**: Create context with no arguments; assert all properties have
their default values.
This scenario is tested by `Context_Create_NoArguments_ReturnsDefaultContext`.

**Context_Create_VersionFlag_SetsVersionTrue**: Pass `--version`; assert `Version` is `true`.
This scenario is tested by `Context_Create_VersionFlag_SetsVersionTrue`.

**Context_Create_HelpFlag_SetsHelpTrue**: Pass `--help`, `-?`, or `-h`; assert `Help` is `true`.
This scenario is tested by `Context_Create_HelpFlag_SetsHelpTrue`.

**Context_Create_SilentFlag_SetsSilentTrue**: Pass `--silent`; assert `Silent` is `true` and console output is
suppressed.
This scenario is tested by `Context_Create_SilentFlag_SetsSilentTrue`.

**Context_Create_ValidateFlag_SetsValidateTrue**: Pass `--validate`; assert `Validate` is `true`.
This scenario is tested by `Context_Create_ValidateFlag_SetsValidateTrue`.

**Context_Create_EnforceFlag_SetsEnforceTrue**: Pass `--enforce`; assert `Enforce` is `true`.
This scenario is tested by `Context_Create_EnforceFlag_SetsEnforceTrue`.

**Context_Create_SarifParameter_SetsSarifFile**: Pass `--sarif {path}`; assert `SarifFile` is set to the provided
path.
This scenario is tested by `Context_Create_SarifParameter_SetsSarifFile`.

**Context_Create_SarifWithoutValue_ThrowsArgumentException**: Pass `--sarif` with no following value; assert
`ArgumentException` is thrown.
This scenario is tested by `Context_Create_SarifWithoutValue_ThrowsArgumentException`.

**Context_Create_ReportParameter_SetsReportFile**: Pass `--report {path}`; assert `ReportFile` is set.
This scenario is tested by `Context_Create_ReportParameter_SetsReportFile`.

**Context_Create_ReportWithoutValue_ThrowsArgumentException**: Pass `--report` with no value; assert
`ArgumentException` is thrown.
This scenario is tested by `Context_Create_ReportWithoutValue_ThrowsArgumentException`.

**Context_Create_DepthParameter_SetsDepth**: Pass `--depth {n}` or `--report-depth {n}`; assert `Depth` is set to
the provided value.
This scenario is tested by `Context_Create_DepthParameter_SetsDepth`.

**Context_Create_HeadingArgument_SetsHeading**: Pass `--heading {text}`; assert `Heading` is set.
This scenario is tested by `Context_Create_HeadingArgument_SetsHeading`.

**Context_Create_ResultsParameter_SetsResultsFile**: Pass `--results {path}`; assert `ResultsFile` is set.
This scenario is tested by `Context_Create_ResultsParameter_SetsResultsFile`.

**Context_Create_ResultLegacyAlias_SetsResultsFile**: Pass `--result {path}`; assert the legacy alias sets
`ResultsFile`.
This scenario is tested by `Context_Create_ResultLegacyAlias_SetsResultsFile`.

**Context_Create_ResultsWithoutValue_ThrowsArgumentException**: Pass `--results` with no value; assert
`ArgumentException` is thrown.
This scenario is tested by `Context_Create_ResultsWithoutValue_ThrowsArgumentException`.

**Context_Create_LogFile_OpensFileSuccessfully**: Pass `--log {path}`; assert the log file is opened and subsequent
writes go to the file.
This scenario is tested by `Context_Create_LogFile_OpensFileSuccessfully`.

**Context_Create_LogWithoutValue_ThrowsArgumentException**: Pass `--log` with no value; assert `ArgumentException`
is thrown.
This scenario is tested by `Context_Create_LogWithoutValue_ThrowsArgumentException`.

**Context_Create_UnknownArgument_ThrowsArgumentException**: Pass an unrecognized argument; assert
`ArgumentException` is thrown.
This scenario is tested by `Context_Create_UnknownArgument_ThrowsArgumentException`.

**Context_WriteLine_WritesToConsole**: Call `WriteLine` with a message; assert the text appears on the console.
This scenario is tested by `Context_WriteLine_WritesToConsole`.

**Context_WriteLine_WithLogFile_WritesToLog**: Call `WriteLine` with a log file open; assert the text is written to
the log file.
This scenario is tested by `Context_WriteLine_WithLogFile_WritesToLog`.

**Context_WriteError_WritesToErrorAndSetsExitCode**: Call `WriteError` with a message; assert the text is written to
stderr and exit code becomes 1.
This scenario is tested by `Context_WriteError_WritesToErrorAndSetsExitCode`.

**Context_WriteError_WithLogFile_WritesToLog**: Call `WriteError` with a log file open; assert the error text is
written to the log regardless of silent mode.
This scenario is tested by `Context_WriteError_WithLogFile_WritesToLog`.

**Context_ExitCode_StartsAtZero_ChangesToOneAfterError**: Verify exit code is 0 initially and becomes 1 after
`WriteError` is called.
This scenario is tested by `Context_ExitCode_StartsAtZero_ChangesToOneAfterError`.

**Context_Dispose_ProperlyClosesLogFile**: Dispose the context; assert the log `StreamWriter` is closed and no
further writes are possible.
This scenario is tested by `Context_Dispose_ProperlyClosesLogFile`.

**Context_Create_ShortVersionFlag_SetsVersionTrue**: Pass `-v`; assert `Version` is `true`.
This scenario is tested by `Context_Create_ShortVersionFlag_SetsVersionTrue`.

**Context_Create_QuestionMarkHelpFlag_SetsHelpTrue**: Pass `-?`; assert `Help` is `true`.
This scenario is tested by `Context_Create_QuestionMarkHelpFlag_SetsHelpTrue`.

**Context_Create_ShortHelpFlag_SetsHelpTrue**: Pass `-h`; assert `Help` is `true`.
This scenario is tested by `Context_Create_ShortHelpFlag_SetsHelpTrue`.

**Context_Create_InvalidLogFilePath_ThrowsInvalidOperationException**: Pass `--log` with a path whose
parent directory does not exist; assert `InvalidOperationException` is thrown with a message containing
"Failed to open log file".
This scenario is tested by `Context_Create_InvalidLogFilePath_ThrowsInvalidOperationException`.

**Context_WriteLine_SilentMode_DoesNotWriteToConsole**: Create context with `--silent`, redirect stdout to a
`StringWriter`, call `WriteLine`; assert nothing is written to stdout.
This scenario is tested by `Context_WriteLine_SilentMode_DoesNotWriteToConsole`.

**Context_WriteError_SilentMode_DoesNotWriteToConsoleButSetsExitCode**: Create context with `--silent`, redirect
stderr to a `StringWriter`, call `WriteError`; assert nothing is written to stderr but `ExitCode` becomes 1.
This scenario is tested by `Context_WriteError_SilentMode_DoesNotWriteToConsoleButSetsExitCode`.

**Context_WriteLine_SilentModeWithLogFile_WritesToLog**: Create context with `--silent` and `--log {path}`, call
`WriteLine`; assert the message appears in the log file even though silent mode suppresses console output.
This scenario is tested by `Context_WriteLine_SilentModeWithLogFile_WritesToLog`.
