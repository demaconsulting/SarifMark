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

**SarifResults_Read_EmptyPath_ThrowsArgumentException**: Pass an empty string as the path; assert `ArgumentException`
is thrown.
This scenario is tested by `SarifResults_Read_EmptyPath_ThrowsArgumentException`.

**SarifResults_Read_WhitespacePath_ThrowsArgumentException**: Pass a whitespace-only string as the path; assert
`ArgumentException` is thrown.
This scenario is tested by `SarifResults_Read_WhitespacePath_ThrowsArgumentException`.

**SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException**: Pass a non-existent path; assert
`FileNotFoundException` is thrown.
This scenario is tested by `SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException`.

**SarifResults_Read_InvalidJson_ThrowsInvalidOperationException**: Pass a file containing invalid JSON; assert
`InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_InvalidJson_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingVersion_ThrowsInvalidOperationException**: Pass SARIF missing the required `version`
field; assert `InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_MissingVersion_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingRuns_ThrowsInvalidOperationException**: Pass SARIF missing the `runs` array; assert
`InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_MissingRuns_ThrowsInvalidOperationException`.

**SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException**: Pass SARIF with an empty `runs` array; assert
`InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingTool_ThrowsInvalidOperationException**: Pass SARIF with a run that has no `tool` field;
assert `InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_MissingTool_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingDriver_ThrowsInvalidOperationException**: Pass SARIF with a `tool` object that has no
`driver` field; assert `InvalidOperationException` is thrown.
This scenario is tested by `SarifResults_Read_MissingDriver_ThrowsInvalidOperationException`.

**SarifResults_Read_MissingToolName_UsesUnknown**: Pass SARIF with no tool name field; assert tool name defaults to
`"Unknown"`.
This scenario is tested by `SarifResults_Read_MissingToolName_UsesUnknown`.

**SarifResults_Read_MissingToolVersion_UsesUnknown**: Pass SARIF with no tool version field; assert tool version
defaults to `"Unknown"`.
This scenario is tested by `SarifResults_Read_MissingToolVersion_UsesUnknown`.

**SarifResults_Read_AllVersionFields_PrioritizesVersion**: Pass SARIF with multiple version fields populated; assert
the correct priority order is applied.
This scenario is tested by `SarifResults_Read_AllVersionFields_PrioritizesVersion`.

**SarifResults_Read_SemanticVersionField_ReturnsSemanticVersion**: Pass SARIF with only the semantic version field
populated; assert the semantic version string is returned.
This scenario is tested by `SarifResults_Read_SemanticVersionField_ReturnsSemanticVersion`.

**SarifResults_Read_DottedQuadFileVersionField_ReturnsDottedQuadFileVersion**: Pass SARIF with only the dotted-quad
file version field populated; assert the dotted-quad version string is returned.
This scenario is tested by `SarifResults_Read_DottedQuadFileVersionField_ReturnsDottedQuadFileVersion`.

**SarifResults_Read_VersionAndSemanticVersion_PrioritizesVersion**: Pass SARIF with both version and semantic version
fields; assert the `version` field takes priority.
This scenario is tested by `SarifResults_Read_VersionAndSemanticVersion_PrioritizesVersion`.

**SarifResults_Read_SemanticAndDottedQuad_PrioritizesSemanticVersion**: Pass SARIF with both semantic version and
dotted-quad file version; assert the semantic version takes priority.
This scenario is tested by `SarifResults_Read_SemanticAndDottedQuad_PrioritizesSemanticVersion`.

**SarifResults_Read_EmptyVersionField_FallsBackToSemanticVersion**: Pass SARIF with an empty `version` field and a
populated semantic version; assert the semantic version is used as the fallback.
This scenario is tested by `SarifResults_Read_EmptyVersionField_FallsBackToSemanticVersion`.

**SarifResults_Read_NoResults_ReturnsValidResults**: Pass SARIF with no results array; assert a valid empty
`SarifResults` object is returned.
This scenario is tested by `SarifResults_Read_NoResults_ReturnsValidResults`.

**SarifResults_Read_EmptyResults_ReturnsValidResults**: Pass SARIF with an empty results array; assert a valid
`SarifResults` object with zero findings is returned.
This scenario is tested by `SarifResults_Read_EmptyResults_ReturnsValidResults`.

**SarifResults_Read_WithResults_ReturnsValidResults**: Pass SARIF with results; assert a valid `SarifResults` object
containing the expected findings is returned.
This scenario is tested by `SarifResults_Read_WithResults_ReturnsValidResults`.

**SarifResults_Read_EmptySuppressions_DoesNotExcludeResult**: Pass a result with an empty suppression list; assert
the result is included (not filtered out).
This scenario is tested by `SarifResults_Read_EmptySuppressions_DoesNotExcludeResult`.

**SarifResults_Read_WithSuppressedResults_ExcludesSuppressedResults**: Pass SARIF containing results with non-empty
suppressions arrays; assert those results are excluded from the output.
This scenario is tested by `SarifResults_Read_WithSuppressedResults_ExcludesSuppressedResults`.

**SarifResults_Read_WithLocations_ReturnsResultsWithLocationData**: Pass SARIF with location data on results; assert
the parsed findings contain the correct URI and line number information.
This scenario is tested by `SarifResults_Read_WithLocations_ReturnsResultsWithLocationData`.

**SarifResults_Read_NoArtifacts_ReturnsZeroFileCount**: Pass SARIF with no `artifacts` section; assert `FileCount`
is 0.
This scenario is tested by `SarifResults_Read_NoArtifacts_ReturnsZeroFileCount`.

**SarifResults_Read_WithArtifacts_ReturnsFileCount**: Pass SARIF with an `artifacts` array; assert `FileCount`
equals the number of entries in the array.
This scenario is tested by `SarifResults_Read_WithArtifacts_ReturnsFileCount`.

**SarifResults_Read_MultipleRuns_EachRunHasOwnFileCount**: Pass a multi-run SARIF where each run has a different
number of artifacts; assert each run reports its own correct file count.
This scenario is tested by `SarifResults_Read_MultipleRuns_EachRunHasOwnFileCount`.

**SarifResults_Read_MultipleRuns_ReturnsAllRuns**: Pass a multi-run SARIF; assert all runs are present in the
returned `SarifResults` object.
This scenario is tested by `SarifResults_Read_MultipleRuns_ReturnsAllRuns`.

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

**SarifResults_ToMarkdown_NoResults_ShowsFoundNoResults**: Call `ToMarkdown` against SARIF with zero results; assert
the output contains `"Found no issues"`.
This scenario is tested by `SarifResults_ToMarkdown_NoResults_ShowsFoundNoResults`.

**SarifResults_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException**: Call `ToMarkdown` with a depth value
less than 1; assert `ArgumentOutOfRangeException` is thrown.
This scenario is tested by `SarifResults_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException`.

**SarifResults_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException**: Call `ToMarkdown` with a depth
value greater than 6; assert `ArgumentOutOfRangeException` is thrown.
This scenario is tested by `SarifResults_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException`.

**SarifResults_ToMarkdown_ResultWithoutLocation_ShowsNoLocation**: Pass a result with no location information;
assert the output contains `"(no location)"`.
This scenario is tested by `SarifResults_ToMarkdown_ResultWithoutLocation_ShowsNoLocation`.

**SarifResults_ToMarkdown_NullHeading_UsesDefaultHeading**: Pass `null` as the heading parameter; assert the
default `"[ToolName] Analysis"` heading is used.
This scenario is tested by `SarifResults_ToMarkdown_NullHeading_UsesDefaultHeading`.

**SarifResults_ToMarkdown_NoHeadingParameter_UsesDefaultHeading**: Call `ToMarkdown` without supplying a heading
parameter; assert the default `"[ToolName] Analysis"` heading is used.
This scenario is tested by `SarifResults_ToMarkdown_NoHeadingParameter_UsesDefaultHeading`.

**SarifResults_ToMarkdown_ZeroFileCount_ShowsZero**: Call `ToMarkdown` against SARIF with no artifacts; assert the
output contains `"**Files:** 0"`.
This scenario is tested by `SarifResults_ToMarkdown_ZeroFileCount_ShowsZero`.

**SarifResults_InternalConstructor_ExposesRunsAndHasIssues**: Construct a `SarifResults` instance via the internal
constructor with known runs; assert `Runs` contains the expected run objects and `HasIssues` reflects the expected
state.
This scenario is tested by `SarifResults_InternalConstructor_ExposesRunsAndHasIssues`.

**SarifResults_HasIssues_WithIssues_ReturnsTrue**: Pass SARIF with findings; assert `HasIssues` is `true`.
This scenario is tested by `SarifResults_HasIssues_WithIssues_ReturnsTrue`.

**SarifResults_HasIssues_AnyRunHasIssues_ReturnsTrue**: Pass a multi-run SARIF where at least one run has findings;
assert `HasIssues` is `true`.
This scenario is tested by `SarifResults_HasIssues_AnyRunHasIssues_ReturnsTrue`.
