## Cli

### Verification Approach

The `Cli` subsystem is verified through tests that operate at the subsystem boundary by calling `Context.Create(string[])`
directly and asserting the resulting property values and output behavior. Tests are defined in
`test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs` using the xUnit v3 framework. Console streams are redirected via
`StringWriter` for output assertions. The `Cli` subsystem has no dependencies on other tool subsystems, so no mocking of
external boundaries is required.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external services, files, or configuration beyond the standard test
runner are required. Temporary files used for log-file tests are created in the OS temporary directory and cleaned up
after each test.

### Acceptance Criteria

- All `CliTests` test methods pass.
- Every command-line flag and parameter is parsed correctly.
- Output routing operates as expected.
- Error conditions produce the correct exceptions and exit codes.
- No `Cli` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**Cli_Create_VersionFlag_SetsVersionFlag**: Pass `--version` to `Context.Create`; assert `Version` property is `true`.
This scenario is tested by `Cli_Create_VersionFlag_SetsVersionFlag`.

**Cli_Create_HelpFlag_SetsHelpFlag**: Pass `--help` to `Context.Create`; assert `Help` property is `true`.
This scenario is tested by `Cli_Create_HelpFlag_SetsHelpFlag`.

**Cli_Create_SilentFlag_SuppressesOutput**: Pass `--silent` to `Context.Create`; assert console output is suppressed.
This scenario is tested by `Cli_Create_SilentFlag_SuppressesOutput`.

**Cli_Create_LogFile_WritesOutputToFile**: Pass `--log {path}` to `Context.Create`; assert output is written to the
log file.
This scenario is tested by `Cli_Create_LogFile_WritesOutputToFile`.

**Cli_Create_EnforceFlag_SetsEnforceFlag**: Pass `--enforce` to `Context.Create`; assert `Enforce` property is `true`.
This scenario is tested by `Cli_Create_EnforceFlag_SetsEnforceFlag`.

**Cli_WriteError_WithMessage_SetsExitCodeToOne**: Call `WriteError` on the context; assert exit code becomes 1.
This scenario is tested by `Cli_WriteError_WithMessage_SetsExitCodeToOne`.

**Cli_Create_UnknownArgument_ThrowsArgumentException**: Pass an unknown argument to `Context.Create`; assert
`ArgumentException` is thrown.
This scenario is tested by `Cli_Create_UnknownArgument_ThrowsArgumentException`.

**Cli_Create_ValidateFlag_SetsValidateFlag**: Pass `--validate` to `Context.Create`; assert `Validate` property is
`true`.
This scenario is tested by `Cli_Create_ValidateFlag_SetsValidateFlag`.

**Cli_Create_SarifParameter_SetsSarifFilePath**: Pass `--sarif {path}` to `Context.Create`; assert `SarifFile`
property is set.
This scenario is tested by `Cli_Create_SarifParameter_SetsSarifFilePath`.

**Cli_Create_ReportParameter_SetsReportFilePath**: Pass `--report {path}` to `Context.Create`; assert `ReportFile`
property is set.
This scenario is tested by `Cli_Create_ReportParameter_SetsReportFilePath`.

**Cli_Create_DepthParameter_SetsDepth**: Pass `--depth {n}` to `Context.Create`; assert `Depth` property is set.
This scenario is tested by `Cli_Create_DepthParameter_SetsDepth`.

**Cli_Create_HeadingParameter_SetsCustomHeading**: Pass `--heading {text}` to `Context.Create`; assert `Heading`
property is set.
This scenario is tested by `Cli_Create_HeadingParameter_SetsCustomHeading`.

**Cli_Create_ResultsParameter_SetsResultsFilePath**: Pass `--results {path}` to `Context.Create`; assert `ResultsFile`
property is set.
This scenario is tested by `Cli_Create_ResultsParameter_SetsResultsFilePath`.

**Cli_Create_ReportDepthParameter_SetsReportDepth**: Pass `--report-depth {n}` to `Context.Create`; assert the legacy
alias is accepted and `Depth` is set.
This scenario is tested by `Cli_Create_ReportDepthParameter_SetsReportDepth`.

**Cli_Create_ResultLegacyAlias_SetsResultsFilePath**: Pass `--result {path}` to `Context.Create`; assert the legacy
alias is accepted and `ResultsFile` is set.
This scenario is tested by `Cli_Create_ResultLegacyAlias_SetsResultsFilePath`.
