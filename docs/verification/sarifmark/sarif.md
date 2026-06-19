## Sarif

### Verification Approach

The `Sarif` subsystem is verified through tests that call `SarifResults.Read` and `SarifResults.ToMarkdown` with real
test data SARIF files. Tests are defined in `test/DemaConsulting.SarifMark.Tests/Sarif/SarifTests.cs` using the xUnit v3
framework. No mocking is required — the subsystem operates on file I/O with no injectable dependencies beyond the file
path argument.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Test data SARIF files (`sample.sarif`, `multi-result.sarif`,
`multi-run.sarif`, `invalid.sarif`) in `test/DemaConsulting.SarifMark.Tests/TestData/` are required. No external
services or network configuration are required.

### Acceptance Criteria

- All `SarifTests` test methods pass.
- Valid SARIF files are parsed correctly.
- Invalid inputs throw the expected exceptions.
- Generated markdown reports contain the expected structure and content.
- No `Sarif` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**Sarif_Read_ValidSarifFile_ProcessesSuccessfully**: Read `sample.sarif`; assert tool name, version, result count, and
file count are correct.
This scenario is tested by `Sarif_Read_ValidSarifFile_ProcessesSuccessfully`.

**Sarif_Read_NonExistentFile_ThrowsFileNotFoundException**: Pass a non-existent path to `SarifResults.Read`; assert
`FileNotFoundException` is thrown.
This scenario is tested by `Sarif_Read_NonExistentFile_ThrowsFileNotFoundException`.

**Sarif_Read_InvalidSarifFile_ThrowsInvalidOperationException**: Pass `invalid.sarif` (malformed JSON); assert
`InvalidOperationException` is thrown.
This scenario is tested by `Sarif_Read_InvalidSarifFile_ThrowsInvalidOperationException`.

**Sarif_Read_MultiRunSarifFile_ProcessesAllRuns**: Read `multi-run.sarif`; assert the run count equals 2.
This scenario is tested by `Sarif_Read_MultiRunSarifFile_ProcessesAllRuns`.

**Sarif_GenerateReport_LocationInfo_ContainsLocationInfo**: Generate a markdown report from a SARIF file with location
data; assert the file URI appears in the output.
This scenario is tested by `Sarif_GenerateReport_LocationInfo_ContainsLocationInfo`.

**Sarif_GenerateReport_FileCount_ContainsFileCount**: Generate a markdown report from a SARIF file with artifact data;
assert the file count is present in the report header.
This scenario is tested by `Sarif_GenerateReport_FileCount_ContainsFileCount`.

**Sarif_GenerateReport_DefaultDepth_ProducesMarkdownContent**: Read `sample.sarif` and call `ToMarkdown(1)`; assert the
report contains the `# TestTool Analysis` heading, confirming the default heading format is correct.
This scenario is tested by `Sarif_GenerateReport_DefaultDepth_ProducesMarkdownContent`.

**Sarif_GenerateReport_ReportDepth_IsConfigurable**: Read `sample.sarif` and call `ToMarkdown(3)`; assert the report
uses `### TestTool Analysis` headings, confirming the depth parameter controls heading level.
This scenario is tested by `Sarif_GenerateReport_ReportDepth_IsConfigurable`.

**Sarif_GenerateReport_MultipleResults_FormatsWithLineBreaks**: Read `multi-result.sarif` and call `ToMarkdown(1)`;
assert both file paths appear and are separated by proper markdown line breaks (two trailing spaces).
This scenario is tested by `Sarif_GenerateReport_MultipleResults_FormatsWithLineBreaks`.

**Sarif_GenerateReport_ResultCount_ContainsResultCount**: Read `sample.sarif` and generate a report; assert the
singular "Found 1 issue" string appears in the output.
This scenario is tested by `Sarif_GenerateReport_ResultCount_ContainsResultCount`.

**Sarif_GenerateReport_CustomHeading_UsesCustomHeading**: Read `sample.sarif` and call `ToMarkdown(1, "Custom Analysis
Heading")`; assert the custom heading string appears in the output instead of the default tool-name heading.
This scenario is tested by `Sarif_GenerateReport_CustomHeading_UsesCustomHeading`.
