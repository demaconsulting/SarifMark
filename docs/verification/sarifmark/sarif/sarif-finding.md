### SarifFinding Unit Verification

#### Verification Approach

`SarifFinding` is an immutable record whose only behaviour is property storage via its internal constructor.
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

- `SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties`: Construct with all properties provided;
  assert each property stores the provided value.
- `SarifFinding_Constructor_NullUri_UriPropertyIsNull`: Construct with `null` URI; assert `Uri` property is `null`.
- `SarifFinding_Constructor_NullStartLine_StartLinePropertyIsNull`: Construct with `null` start line; assert
  `StartLine` property is `null`.

#### Overview

The `SarifFinding` record is verified by the `SarifFindingTests` test class in
`test/DemaConsulting.SarifMark.Tests/Sarif/SarifFindingTests.cs`. The internal constructor is accessed via the
`InternalsVisibleTo` grant. All tests exercise construction only; properties are nullable strings or a nullable integer.

#### Requirement Coverage

- **`SarifMark-SarifFinding-Properties`**: Properties —
  `SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties`
- **`SarifMark-SarifFinding-Uri`**: Null URI accepted as null —
  `SarifFinding_Constructor_NullUri_UriPropertyIsNull`
- **`SarifMark-SarifFinding-StartLine`**: Null line —
  `SarifFinding_Constructor_NullStartLine_StartLinePropertyIsNull`
- **`SarifMark-SarifFinding-Internal`**: Internal ctor —
  `SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties`
