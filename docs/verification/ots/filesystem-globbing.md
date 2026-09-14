## Microsoft.Extensions.FileSystemGlobbing

### Verification Approach

`Microsoft.Extensions.FileSystemGlobbing` is verified through unit tests in the SarifMark test
suite that exercise the package through `SarifResults.Exclude`. No mocking is applied; the
verification calls the real `Matcher` type to evaluate actual glob patterns against actual
finding URIs.

The tests verify that:

- A single `--exclude` glob pattern removes findings whose `Uri` matches it.
- Multiple `--exclude` glob patterns remove findings matching any one of them.
- Findings whose `Uri` does not match any supplied pattern are retained.
- Findings with a `null` `Uri` are always retained, regardless of supplied patterns.
- Recursive `**` wildcard patterns match nested paths at any depth.
- An empty or absent glob pattern list leaves all findings unchanged (and returns the same
  `SarifResults` instance).
- Filtering is applied independently per `SarifRun` in a multi-run `SarifResults`.
- Run metadata (`ToolName`, `ToolVersion`, `FileCount`) is preserved across filtering.
- Matching is case-insensitive by default.

### Test Environment

Tests require:

- No network access; verification is entirely in-process.
- No real files on disk; `SarifFinding`/`SarifRun`/`SarifResults` fixtures are constructed
  directly via their internal constructors (available to the test assembly through
  `InternalsVisibleTo`), and candidate URIs are arbitrary strings rather than paths to files that
  must exist.
- The `Microsoft.Extensions.FileSystemGlobbing` NuGet package installed as a package reference in
  the production project (`src/DemaConsulting.SarifMark/DemaConsulting.SarifMark.csproj`).

### Acceptance Criteria

The OTS integration is accepted when all linked tests pass with zero failures.

### Test Scenarios

**SinglePatternExclusion**: Constructs a `SarifResults` with one finding whose `Uri` matches a
single supplied glob pattern and confirms the finding is removed. Tested by
`SarifResults_Exclude_SinglePatternMatch_RemovesMatchingFinding`.

**MultiplePatternExclusion**: Constructs a `SarifResults` with findings matching different glob
patterns among several supplied patterns and confirms any matching finding is removed. Tested by
`SarifResults_Exclude_MultiplePatterns_RemovesAnyMatchingFinding`.

**NoMatchRetention**: Constructs a `SarifResults` with findings whose `Uri` values do not match
the supplied glob pattern and confirms all findings are retained. Tested by
`SarifResults_Exclude_NoMatch_RetainsAllFindings`.

**NullUriRetention**: Constructs a `SarifResults` with a finding whose `Uri` is `null` and
confirms it is retained regardless of the supplied glob patterns. Tested by
`SarifResults_Exclude_NullUri_RetainsFinding`.

**RecursiveWildcardMatching**: Constructs a `SarifResults` with a finding at a deeply nested path
and confirms a `**`-prefixed glob pattern matches it. Tested by
`SarifResults_Exclude_RecursiveDoubleStarGlob_MatchesNestedPaths`.

**EmptyPatternListNoOp**: Calls `SarifResults.Exclude` with an empty pattern list and confirms the
same `SarifResults` instance is returned with all findings unchanged. Tested by
`SarifResults_Exclude_EmptyGlobList_ReturnsAllFindings`.

**MultiRunIndependentFiltering**: Constructs a `SarifResults` with multiple `SarifRun` instances
and confirms filtering is applied independently to each run. Tested by
`SarifResults_Exclude_MultiRun_FiltersEachRunIndependently`.

**RunMetadataPreservation**: Confirms `ToolName`, `ToolVersion`, and `FileCount` are unchanged on
each `SarifRun` after filtering. Tested by `SarifResults_Exclude_PreservesRunMetadata`.

**CaseInsensitiveMatching**: Confirms a glob pattern matches a `Uri` that differs from it only in
character case, documenting the observed default `Matcher` behavior. Tested by
`SarifResults_Exclude_DifferentCase_StillMatches`.
