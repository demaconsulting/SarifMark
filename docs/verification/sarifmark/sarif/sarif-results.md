### SarifResults

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

**SarifResults_Read_NullPath_ThrowsArgumentException**: Pass `null` as the path; assert `ArgumentException` is
thrown.
This scenario is tested by `SarifResults_Read_NullPath_ThrowsArgumentException`.

**SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException**: Pass a non-existent path; assert
`FileNotFoundException` is thrown.
This scenario is tested by `SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException`.

**SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException**: Pass SARIF with an empty `runs` array; assert
`InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingToolName_UsesUnknown**: Pass SARIF with no tool name field; assert tool name defaults to
`"Unknown"`.
This scenario is tested by `SarifResults_Read_MissingToolName_UsesUnknown`.

**SarifResults_Read_AllVersionFields_PrioritizesVersion**: Pass SARIF with multiple version fields populated; assert
the correct priority order is applied.
This scenario is tested by `SarifResults_Read_AllVersionFields_PrioritizesVersion`.

**SarifResults_Read_NoResults_ReturnsValidResults**: Pass SARIF with no results array; assert a valid empty
`SarifResults` object is returned.
This scenario is tested by `SarifResults_Read_NoResults_ReturnsValidResults`.

**SarifResults_Read_EmptySuppressions_DoesNotExcludeResult**: Pass a result with an empty suppression list; assert
the result is included (not filtered out).
This scenario is tested by `SarifResults_Read_EmptySuppressions_DoesNotExcludeResult`.

**SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput**: Call `ToMarkdown(1)`; assert the output uses `#` headings
and contains the expected structure.
This scenario is tested by `SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput`.

**SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput**: Call `ToMarkdown(6)`; assert the output uses `######`
headings at maximum depth.
This scenario is tested by `SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput`.

**SarifResults_Read_NoArtifacts_ReturnsZeroFileCount**: Pass SARIF with no `artifacts` section; assert `FileCount`
is 0.
This scenario is tested by `SarifResults_Read_NoArtifacts_ReturnsZeroFileCount`.

**SarifResults_Runs_SingleRun_ReturnsSingleRun**: Pass a single-run SARIF; assert exactly one `SarifRun` is
returned.
This scenario is tested by `SarifResults_Runs_SingleRun_ReturnsSingleRun`.

**SarifResults_HasIssues_NoIssues_ReturnsFalse**: Pass SARIF with no results; assert `HasIssues` is `false`.
This scenario is tested by `SarifResults_HasIssues_NoIssues_ReturnsFalse`.

**SarifResults_ToMarkdown_MultipleRuns_IncludesRunIndices**: Pass a multi-run SARIF; assert run index labels appear
in the generated output.
This scenario is tested by `SarifResults_ToMarkdown_MultipleRuns_IncludesRunIndices`.

**SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels**: Call `ToMarkdown(3)`; assert the output uses `###`
headings.
This scenario is tested by `SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels`.

**SarifResults_ToMarkdown_OneResult_UsesSingularForm**: Pass SARIF with one result; assert the singular count
string is used.
This scenario is tested by `SarifResults_ToMarkdown_OneResult_UsesSingularForm`.

**SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly**: Pass a result with a URI but no line number; assert
only the URI is shown in the output.
This scenario is tested by `SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly`.

**SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading**: Pass a custom heading string; assert it appears in
the generated output.
This scenario is tested by `SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading`.

**SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks**: Pass SARIF with multiple results; assert line breaks
are present between result entries.
This scenario is tested by `SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks`.

**SarifResults_ToMarkdown_ShowsFileCount**: Pass SARIF with artifact entries; assert the file count appears in the
report header.
This scenario is tested by `SarifResults_ToMarkdown_ShowsFileCount`.
