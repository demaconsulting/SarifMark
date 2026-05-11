## CLI Subsystem Verification

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
