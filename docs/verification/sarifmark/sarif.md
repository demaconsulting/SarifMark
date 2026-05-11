## SARIF and Reporting Subsystem Verification

### Overview

The `Sarif` subsystem is verified by the `SarifTests` test class in
`test/DemaConsulting.SarifMark.Tests/Sarif/SarifTests.cs`. Tests exercise `SarifResults.Read` and
`SarifResults.ToMarkdown` via real test data SARIF files. Test data includes `sample.sarif` (single result, two files,
tool TestTool 1.0.0), `multi-result.sarif` (two results), `multi-run.sarif` (two runs), and `invalid.sarif` (invalid
JSON).

### Requirement Coverage

| Requirement ID | Description | Test Scenario(s) |
| --- | --- | --- |
| `SarifMark-Sarif-Reading` | Valid SARIF processed correctly | `Sarif_Read_ValidSarifFile_ProcessesSuccessfully` |
| `SarifMark-Sarif-Validation` | File not found | `Sarif_Read_NonExistentFile_ThrowsFileNotFoundException` |
| `SarifMark-Sarif-Validation` | Bad JSON input | `Sarif_Read_InvalidSarifFile_ThrowsInvalidOperationException` |
| `SarifMark-Sarif-ToolInfo` | Tool name and version extracted | `Sarif_Read_ValidSarifFile_ProcessesSuccessfully` |
| `SarifMark-Sarif-Results` | Results extracted from SARIF file | `Sarif_Read_ValidSarifFile_ProcessesSuccessfully` |
| `SarifMark-Sarif-Locations` | Location info present | `Sarif_GenerateReport_LocationInfo_ContainsLocationInfo` |
| `SarifMark-Sarif-FilePaths` | Non-existent file throws | `Sarif_Read_NonExistentFile_ThrowsFileNotFoundException` |
| `SarifMark-Sarif-Processing` | Valid SARIF processed | `Sarif_Read_ValidSarifFile_ProcessesSuccessfully` |
| `SarifMark-Sarif-FileCount` | File count in generated report | `Sarif_GenerateReport_FileCount_ContainsFileCount` |
| `SarifMark-Sarif-MultiRun` | Multi-run SARIF returns all runs | `Sarif_Read_MultiRunSarifFile_ProcessesAllRuns` |
