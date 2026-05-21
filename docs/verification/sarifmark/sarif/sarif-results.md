### SarifResults Unit Verification

#### Verification Approach

`SarifResults` is tested by calling `Read` and `ToMarkdown` directly via the `InternalsVisibleTo` grant. A
per-test `IDisposable` helper creates a temporary directory under `Path.GetTempPath()` where SARIF fixtures are
written inline as JSON literals. No mocking is required — all I/O uses real file-system operations.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. Per-test temporary directories are created in the OS temporary
directory (`Path.GetTempPath()`) and cleaned up via `IDisposable` after each test. No external services or network
configuration are required.

#### Acceptance Criteria

All `SarifResultsTests` test methods pass, confirming that SARIF reading, validation, tool extraction, result
parsing, suppression filtering, markdown generation, multi-run handling, and argument validation behave correctly.
No `SarifResults` unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

- `SarifResults_Read_NullPath_ThrowsArgumentException`: Pass `null` as the path; assert `ArgumentException`.
- `SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException`: Pass a non-existent path; assert
  `FileNotFoundException`.
- `SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException`: Pass SARIF with an empty `runs` array; assert
  `InvalidOperationException`.
- `SarifResults_Read_MissingToolName_UsesUnknown`: Pass SARIF with no tool name field; assert tool name defaults to
  `"Unknown"`.
- `SarifResults_Read_AllVersionFields_PrioritizesVersion`: Pass SARIF with multiple version fields populated; assert
  the correct priority order is applied.
- `SarifResults_Read_NoResults_ReturnsValidResults`: Pass SARIF with no results array; assert a valid empty
  `SarifResults` object is returned.
- `SarifResults_Read_EmptySuppressions_DoesNotExcludeResult`: Pass a result with an empty suppression list; assert
  the result is included (not filtered out).
- `SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput`: Call `ToMarkdown(1)`; assert the output uses `#` headings
  and contains the expected structure.
- `SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput`: Call `ToMarkdown(6)`; assert the output uses `######`
  headings at maximum depth.
- `SarifResults_Read_NoArtifacts_ReturnsZeroFileCount`: Pass SARIF with no `artifacts` section; assert `FileCount`
  is 0.
- `SarifResults_Runs_SingleRun_ReturnsSingleRun`: Pass a single-run SARIF; assert exactly one `SarifRun` is
  returned.
- `SarifResults_HasIssues_NoIssues_ReturnsFalse`: Pass SARIF with no results; assert `HasIssues` is `false`.
- `SarifResults_ToMarkdown_MultipleRuns_IncludesRunIndices`: Pass a multi-run SARIF; assert run index labels appear
  in the generated output.
- `SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels`: Call `ToMarkdown(3)`; assert the output uses `###`
  headings.
- `SarifResults_ToMarkdown_OneResult_UsesSingularForm`: Pass SARIF with one result; assert the singular count
  string is used.
- `SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly`: Pass a result with a URI but no line number; assert
  only the URI is shown in the output.
- `SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading`: Pass a custom heading string; assert it appears in
  the generated output.
- `SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks`: Pass SARIF with multiple results; assert line breaks
  are present between result entries.
- `SarifResults_ToMarkdown_ShowsFileCount`: Pass SARIF with artifact entries; assert the file count appears in the
  report header.

#### Overview

The `SarifResults` class is verified by the `SarifResultsTests` test class in
`test/DemaConsulting.SarifMark.Tests/Sarif/SarifResultsTests.cs`. The internal constructor is accessed via the
`InternalsVisibleTo` grant. Tests use an `IDisposable` helper that creates a per-test temporary directory under
`Path.GetTempPath()`; SARIF data is written inline as JSON literals. No mocking is required.

#### Requirement Coverage

- **`SarifMark-SarifResults-ValidatePathArgument`**: Path arg null —
  `SarifResults_Read_NullPath_ThrowsArgumentException`
- **`SarifMark-SarifResults-ValidatePathExists`**: File not found —
  `SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException`
- **`SarifMark-SarifResults-ValidateStructure`**: Structure —
  `SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException`
- **`SarifMark-SarifResults-ExtractTool`**: Extract tool info —
  `SarifResults_Read_MissingToolName_UsesUnknown`
- **`SarifMark-SarifResults-VersionPriority`**: Version priority —
  `SarifResults_Read_AllVersionFields_PrioritizesVersion`
- **`SarifMark-SarifResults-ParseResults`**: Parse SARIF results —
  `SarifResults_Read_NoResults_ReturnsValidResults`
- **`SarifMark-SarifResults-FilterSuppressions`**: Filtering —
  `SarifResults_Read_EmptySuppressions_DoesNotExcludeResult`
- **`SarifMark-SarifResults-ToMarkdown`**: Markdown output —
  `SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput`
- **`SarifMark-SarifResults-ValidateDepth`**: Depth range check —
  `SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput`
- **`SarifMark-SarifResults-FileCount`**: File count per run —
  `SarifResults_Read_NoArtifacts_ReturnsZeroFileCount`
- **`SarifMark-SarifResults-Runs`**: Single and multiple runs —
  `SarifResults_Runs_SingleRun_ReturnsSingleRun`
- **`SarifMark-SarifResults-HasIssues`**: HasIssues false/true —
  `SarifResults_HasIssues_NoIssues_ReturnsFalse`
- **`SarifMark-SarifResults-MultiRunMarkdown`**: Run indices —
  `SarifResults_ToMarkdown_MultipleRuns_IncludesRunIndices`
- **`SarifMark-Report-Markdown`**: Markdown report with heading —
  `SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput`
- **`SarifMark-Report-Depth`**: Configurable depth heading —
  `SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels`
- **`SarifMark-Report-Counts`**: Report contains result count —
  `SarifResults_ToMarkdown_OneResult_UsesSingularForm`
- **`SarifMark-Report-Locations`**: Location info in report —
  `SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly`
- **`SarifMark-Report-Headings`**: Custom heading in report —
  `SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading`
- **`SarifMark-Report-LineBreaks`**: Line breaks enforced —
  `SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks`
- **`SarifMark-Report-FileCount`**: Report header includes file count —
  `SarifResults_ToMarkdown_ShowsFileCount`
