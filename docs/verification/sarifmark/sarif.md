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

**Sarif_Read_MultiRunSarifFile_ProcessesAllRuns**: Read `multi-run.sarif`; assert all runs are returned with correct
tool metadata.
This scenario is tested by `Sarif_Read_MultiRunSarifFile_ProcessesAllRuns`.

**Sarif_GenerateReport_LocationInfo_ContainsLocationInfo**: Generate a markdown report from a SARIF file with location
data; assert file path and line number appear in the output.
This scenario is tested by `Sarif_GenerateReport_LocationInfo_ContainsLocationInfo`.

**Sarif_GenerateReport_FileCount_ContainsFileCount**: Generate a markdown report from a SARIF file with artifact data;
assert the file count is present in the report header.
This scenario is tested by `Sarif_GenerateReport_FileCount_ContainsFileCount`.

### Requirements Coverage

- **`SarifMark-Sarif-Reading`**: `Sarif_Read_ValidSarifFile_ProcessesSuccessfully`
- **`SarifMark-Sarif-Validation`**: `Sarif_Read_NonExistentFile_ThrowsFileNotFoundException`,
  `Sarif_Read_InvalidSarifFile_ThrowsInvalidOperationException`
- **`SarifMark-Sarif-ToolInfo`**: `Sarif_Read_ValidSarifFile_ProcessesSuccessfully`
- **`SarifMark-Sarif-Results`**: `Sarif_Read_ValidSarifFile_ProcessesSuccessfully`
- **`SarifMark-Sarif-Locations`**: `Sarif_GenerateReport_LocationInfo_ContainsLocationInfo`
- **`SarifMark-Sarif-FilePaths`**: `Sarif_Read_NonExistentFile_ThrowsFileNotFoundException`
- **`SarifMark-Sarif-Processing`**: `Sarif_Read_ValidSarifFile_ProcessesSuccessfully`
- **`SarifMark-Sarif-FileCount`**: `Sarif_GenerateReport_FileCount_ContainsFileCount`
- **`SarifMark-Sarif-MultiRun`**: `Sarif_Read_MultiRunSarifFile_ProcessesAllRuns`
