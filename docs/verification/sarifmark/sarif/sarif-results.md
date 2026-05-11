### SarifResults Unit Verification

#### Overview

The `SarifResults` class is verified by the `SarifResultsTests` test class in
`test/DemaConsulting.SarifMark.Tests/Sarif/SarifResultsTests.cs`. The internal constructor is accessed via the
`InternalsVisibleTo` grant. Tests use an `IDisposable` helper that creates a per-test temporary directory under
`Path.GetTempPath()`; SARIF data is written inline as JSON literals. No mocking is required.

#### Requirement Coverage

- **`SarifMark-SarifResults-ValidatePathArgument`**: Path arg null —
  `SarifResults_Read_NullPath_ThrowsArgumentException`
- **`SarifMark-SarifResults-ValidatePathExists`**: File not found —
  `Read_NonExistentFile_ThrowsFileNotFoundException`
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
