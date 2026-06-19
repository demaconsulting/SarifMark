### Report

#### Verification Approach

The `Report` unit is tested indirectly through `SarifResultsTests`, which exercises `SarifResults.ToMarkdown` and
`SarifRun.ToMarkdown` directly via the `InternalsVisibleTo` grant. Test instances are constructed inline using the
internal constructors of `SarifResults`, `SarifRun`, and `SarifFinding` — no file I/O or mocking is required for
the report-format scenarios. Each test asserts the structural and content properties of the generated markdown
string to confirm the report output contract.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external files, services, or network configuration are
required; all test input is constructed inline within each test method. Tests that exercise `SarifMark-Report-Depth`
construct `SarifResults` instances in memory and call `ToMarkdown` with specific depth values.

#### Acceptance Criteria

All `SarifResultsTests` test methods covering report-level requirements pass, confirming that markdown generation,
configurable heading depth, result counts, location formatting, custom headings, line breaks, and file count output
all conform to the report output contract. No `Report` unit requirement may remain without at least one named test
scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**Report generates markdown output**: Call `ToMarkdown(1)` on a populated `SarifResults` instance; assert the
returned string is non-empty and contains the expected heading and tool attribution.
This scenario is tested by `SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput`.

**Report heading depth is configurable**: Call `ToMarkdown(3)` on a `SarifResults` instance; assert the generated
output uses `###` headings at the specified depth.
This scenario is tested by `SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels`.

**Report depth validation — below range**: Call `ToMarkdown(0)`; assert `ArgumentOutOfRangeException` is thrown
before any output is produced.
This scenario is tested by `SarifResults_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException`.

**Report depth validation — above range**: Call `ToMarkdown(7)`; assert `ArgumentOutOfRangeException` is thrown
before any output is produced.
This scenario is tested by `SarifResults_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException`.

**Report depth at maximum boundary**: Call `ToMarkdown(6)`; assert the output uses `######` headings and is
structurally correct.
This scenario is tested by `SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput`.

**Report displays result count in singular form**: Call `ToMarkdown(1)` on a `SarifResults` instance containing
exactly one finding; assert the count summary uses `"Found 1 issue"` rather than `"Found 1 issues"`.
This scenario is tested by `SarifResults_ToMarkdown_OneResult_UsesSingularForm`.

**Report displays location with URI but no line number**: Call `ToMarkdown(1)` on a result that has a URI but no
start line; assert the generated output contains only the URI without a line-number suffix.
This scenario is tested by `SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly`.

**Report uses a custom heading**: Pass a non-null custom heading string to `ToMarkdown`; assert the custom text
appears in the generated output in place of the default `"[ToolName] Analysis"` label.
This scenario is tested by `SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading`.

**Report enforces line breaks between multiple results**: Call `ToMarkdown(1)` on a `SarifResults` instance with
two or more findings; assert that trailing two-space hard line breaks are present between result entries.
This scenario is tested by `SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks`.

**Report includes the file count**: Call `ToMarkdown(1)` on a `SarifResults` instance whose run has a non-zero
artifact count; assert the file count appears in the report header.
This scenario is tested by `SarifResults_ToMarkdown_ShowsFileCount`.
