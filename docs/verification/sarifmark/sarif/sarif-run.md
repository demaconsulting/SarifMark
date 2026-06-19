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

**SarifRun_ToMarkdown_OneResult_UsesSingularForm**: Generate output for a run with one result; assert the result
count uses the singular form.
This scenario is tested by `SarifRun_ToMarkdown_OneResult_UsesSingularForm`.
