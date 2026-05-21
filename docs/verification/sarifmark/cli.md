## CLI Subsystem Verification

### Verification Strategy

The `Cli` subsystem is verified through tests that operate at the subsystem boundary by calling
`Context.Create(string[])` directly and asserting the resulting property values and output behavior. Tests are
defined in `test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs` using the xUnit v3 framework. Console streams are
redirected via `StringWriter` for output assertions. The `Cli` subsystem has no dependencies on other tool subsystems,
so no mocking of external boundaries is required.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external services, files, or configuration beyond the standard
test runner are required. Temporary files used for log-file tests are created in the OS temporary directory and
cleaned up after each test.

### Acceptance Criteria

All `CliTests` test methods pass, confirming that every command-line flag and parameter is parsed correctly,
output routing operates as expected, and error conditions produce the correct exceptions and exit codes. No `Cli`
subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

- `Cli_Create_VersionFlag_SetsVersionFlag`: Pass `--version` to `Context.Create`; assert `Version` property is
  `true`.
- `Cli_Create_HelpFlag_SetsHelpFlag`: Pass `--help` to `Context.Create`; assert `Help` property is `true`.
- `Cli_Create_SilentFlag_SuppressesOutput`: Pass `--silent`; assert console output is suppressed.
- `Cli_Create_LogFile_WritesOutputToFile`: Pass `--log {path}`; assert output is written to the log file.
- `Cli_Create_EnforceFlag_SetsEnforceFlag`: Pass `--enforce`; assert `Enforce` property is `true`.
- `Cli_WriteError_WithMessage_SetsExitCodeToOne`: Call `WriteError` on the context; assert exit code becomes 1.
- `Cli_Create_UnknownArgument_ThrowsArgumentException`: Pass an unknown argument; assert `ArgumentException` is
  thrown.
- `Cli_Create_ValidateFlag_SetsValidateFlag`: Pass `--validate`; assert `Validate` property is `true`.
- `Cli_Create_SarifParameter_SetsSarifFilePath`: Pass `--sarif {path}`; assert `SarifFile` property is set.
- `Cli_Create_ReportParameter_SetsReportFilePath`: Pass `--report {path}`; assert `ReportFile` property is set.
- `Cli_Create_DepthParameter_SetsDepth`: Pass `--depth {n}`; assert `Depth` property is set.
- `Cli_Create_HeadingParameter_SetsCustomHeading`: Pass `--heading {text}`; assert `Heading` property is set.
- `Cli_Create_ResultsParameter_SetsResultsFilePath`: Pass `--results {path}`; assert `ResultsFile` property is set.
- `Cli_Create_ReportDepthParameter_SetsReportDepth`: Pass `--report-depth {n}`; assert the legacy alias is accepted
  and `Depth` is set.
- `Cli_Create_ResultLegacyAlias_SetsResultsFilePath`: Pass `--result {path}`; assert the legacy alias is accepted
  and `ResultsFile` is set.

### Overview

The `Cli` subsystem is verified by the `CliTests` test class in
`test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs`. Tests operate at the subsystem boundary by calling
`Context.Create(string[])` directly and asserting the resulting property values and output behavior. Console streams are
redirected via `StringWriter` for output assertions. No other boundaries are mocked — the `Cli` subsystem has no
dependencies on other tool subsystems.

### Requirement Coverage

- **`SarifMark-Cli-Interface`**: Version and help flags accepted and routed —
  `Cli_Create_VersionFlag_SetsVersionFlag`, `Cli_Create_HelpFlag_SetsHelpFlag`
- **`SarifMark-Cli-Version`**: `--version` sets `Version` property —
  `Cli_Create_VersionFlag_SetsVersionFlag`
- **`SarifMark-Cli-Help`**: `--help` sets `Help` property —
  `Cli_Create_HelpFlag_SetsHelpFlag`
- **`SarifMark-Cli-Silent`**: `--silent` suppresses console output —
  `Cli_Create_SilentFlag_SuppressesOutput`
- **`SarifMark-Cli-Log`**: `--log` writes output to file —
  `Cli_Create_LogFile_WritesOutputToFile`
- **`SarifMark-Cli-Enforce`**: `--enforce` sets `Enforce` property —
  `Cli_Create_EnforceFlag_SetsEnforceFlag`
- **`SarifMark-Cli-WriteError`**: `WriteError` writes to stderr and sets exit code —
  `Cli_WriteError_WithMessage_SetsExitCodeToOne`
- **`SarifMark-Cli-InvalidArgs`**: Unknown argument throws `ArgumentException` —
  `Cli_Create_UnknownArgument_ThrowsArgumentException`
- **`SarifMark-Cli-Validate`**: `--validate` sets `Validate` property —
  `Cli_Create_ValidateFlag_SetsValidateFlag`
- **`SarifMark-Cli-Sarif`**: `--sarif` sets `SarifFile` property —
  `Cli_Create_SarifParameter_SetsSarifFilePath`
- **`SarifMark-Cli-Report`**: `--report` sets `ReportFile` property —
  `Cli_Create_ReportParameter_SetsReportFilePath`
- **`SarifMark-Cli-ReportDepth`**: `--depth` sets `Depth` property —
  `Cli_Create_DepthParameter_SetsDepth`
- **`SarifMark-Cli-Heading`**: `--heading` sets `Heading` property —
  `Cli_Create_HeadingParameter_SetsCustomHeading`
- **`SarifMark-Cli-Results`**: `--results` sets `ResultsFile` property —
  `Cli_Create_ResultsParameter_SetsResultsFilePath`
- **`SarifMark-Cli-ReportDepthLegacyAlias`**: `--report-depth` sets `Depth` property —
  `Cli_Create_ReportDepthParameter_SetsReportDepth`
- **`SarifMark-Cli-ResultLegacyAlias`**: `--result` sets `ResultsFile` property —
  `Cli_Create_ResultLegacyAlias_SetsResultsFilePath`
