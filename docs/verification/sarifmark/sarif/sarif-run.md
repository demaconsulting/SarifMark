### SarifRun

#### Verification Approach

The `SarifRun` record is tested by constructing instances via the internal constructor (accessed through the
`InternalsVisibleTo` grant) and invoking `HasIssues` and `ToMarkdown`. No mocking is required — `SarifRun` is an
immutable record with no external dependencies; all test data is constructed inline.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external files, services, or configuration are required; all
test input is constructed inline within each test method.

#### Acceptance Criteria

All `SarifRunTests` test methods pass, confirming that property access, `HasIssues` logic, `ToMarkdown` output at
various depths, and argument validation all behave correctly. No `SarifRun` unit requirement may remain without at
least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**SarifRun_InternalConstructor_CreatesValidInstance**: Construct a `SarifRun` with all properties; assert each
property stores the provided value.
This scenario is tested by `SarifRun_InternalConstructor_CreatesValidInstance`.

**SarifRun_HasIssues_NoResults_ReturnsFalse**: Construct with an empty results collection; assert `HasIssues` is
`false`.
This scenario is tested by `SarifRun_HasIssues_NoResults_ReturnsFalse`.

**SarifRun_ToMarkdown_NoResults_ShowsFoundNoResults**: Construct a `SarifRun` with an empty results collection and
call `ToMarkdown(1)`; assert the output contains `Found no issues`, confirming that the zero-result count phrasing
is used when a run has no findings.
This scenario is tested by `SarifRun_ToMarkdown_NoResults_ShowsFoundNoResults`.

**SarifRun_ToMarkdown_Depth1_ProducesCorrectOutput**: Call `ToMarkdown(1)`; assert the output uses `#` headings and
contains the expected tool name and result summary.
This scenario is tested by `SarifRun_ToMarkdown_Depth1_ProducesCorrectOutput`.

**SarifRun_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException**: Call `ToMarkdown(0)`; assert
`ArgumentOutOfRangeException` is thrown.
This scenario is tested by `SarifRun_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException`.

**SarifRun_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException**: Call `ToMarkdown(7)`; assert
`ArgumentOutOfRangeException` is thrown.
This scenario is tested by `SarifRun_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException`.

**SarifRun_ToMarkdown_ResultWithoutLocation_ShowsNoLocation**: Generate output for a result with no location data;
assert the location section is absent from the output.
This scenario is tested by `SarifRun_ToMarkdown_ResultWithoutLocation_ShowsNoLocation`.

**SarifRun_HasIssues_WithResults_ReturnsTrue**: Construct a `SarifRun` with a non-empty results collection; assert
`HasIssues` is `true`, covering the path where a run contains findings.
This scenario is tested by `SarifRun_HasIssues_WithResults_ReturnsTrue`.

**SarifRun_ToMarkdown_WithResults_ShowsResults**: Call `ToMarkdown(1)` on a run containing one result with location
data; assert the output contains the result count, rule ID, message, and formatted location string.
This scenario is tested by `SarifRun_ToMarkdown_WithResults_ShowsResults`.

**SarifRun_ToMarkdown_ShowsFileCount**: Call `ToMarkdown(1)` on a run with a non-zero file count; assert the output
contains the `**Files:**` line with the correct count, confirming file count reporting is included in the report.
This scenario is tested by `SarifRun_ToMarkdown_ShowsFileCount`.

**SarifRun_ToMarkdown_NullHeading_UsesDefaultHeading**: Call `ToMarkdown(1, null)`; assert the output uses the
default heading derived from the tool name (e.g. `# MyTool Analysis`), confirming that a null heading argument
falls back to the auto-generated default.
This scenario is tested by `SarifRun_ToMarkdown_NullHeading_UsesDefaultHeading`.

**SarifRun_ToMarkdown_WhitespaceHeading_UsesDefaultHeading**: Call `ToMarkdown(1, "   ")`; assert the output uses
the default heading derived from the tool name, confirming that a whitespace-only heading argument is treated as
absent and falls back to the auto-generated default.
This scenario is tested by `SarifRun_ToMarkdown_WhitespaceHeading_UsesDefaultHeading`.

**SarifRun_ToMarkdown_OneResult_UsesSingularForm**: Generate output for a run with one result; assert the result
count uses the singular form.
This scenario is tested by `SarifRun_ToMarkdown_OneResult_UsesSingularForm`.

**SarifRun_ToMarkdown_MultipleResults_UsesPluralForm**: Call `ToMarkdown(1)` on a run with three results; assert the
output contains `Found 3 issues` (plural) and does not contain `Found 3 issue`, confirming that the plural form
is used when more than one result is present.
This scenario is tested by `SarifRun_ToMarkdown_MultipleResults_UsesPluralForm`.

**SarifRun_ToMarkdown_Depth6_IssuesHeadingCappedAtSix**: Call `ToMarkdown(6)`; assert the output uses `######` for
both the tool heading and the Issues sub-heading, confirming that the Issues sub-heading depth is capped at 6
(i.e. `Math.Min(depth + 1, 6)`) and no seven-hash heading is generated.
This scenario is tested by `SarifRun_ToMarkdown_Depth6_IssuesHeadingCappedAtSix`.

**SarifRun_ToMarkdown_CustomHeading_UsesProvidedHeading**: Call `ToMarkdown(1, "Custom Heading")`; assert the output
uses the provided custom heading text instead of the default tool-name label, confirming that the heading parameter
is correctly applied.
This scenario is tested by `SarifRun_ToMarkdown_CustomHeading_UsesProvidedHeading`.

**SarifRun_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly**: Call `ToMarkdown(1)` on a run containing a result with a
URI but no start-line number; assert the output contains only the URI without a line number suffix, confirming that
partial location information is gracefully rendered.
This scenario is tested by `SarifRun_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly`.
