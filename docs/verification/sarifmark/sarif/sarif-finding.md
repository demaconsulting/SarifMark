### SarifFinding

#### Verification Approach

`SarifFinding` is an immutable record whose only behavior is property storage via its internal constructor.
Tests construct instances via the `InternalsVisibleTo` grant and assert property values. No mocking or file I/O
is required; all test data is constructed inline.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external files, services, or configuration are required; all
test input is constructed inline within each test method.

#### Acceptance Criteria

All `SarifFindingTests` test methods pass, confirming that the internal constructor stores all properties correctly
and that nullable properties correctly accept `null` values. No `SarifFinding` unit requirement may remain without
at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties**: Construct with all properties provided;
assert each property stores the provided value.
This scenario is tested by `SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties`.

**SarifFinding_Constructor_NullUri_UriPropertyIsNull**: Construct with `null` URI; assert `Uri` property is `null`.
This scenario is tested by `SarifFinding_Constructor_NullUri_UriPropertyIsNull`.

**SarifFinding_Constructor_NullStartLine_StartLinePropertyIsNull**: Construct with `null` start line; assert
`StartLine` property is `null`.
This scenario is tested by `SarifFinding_Constructor_NullStartLine_StartLinePropertyIsNull`.
