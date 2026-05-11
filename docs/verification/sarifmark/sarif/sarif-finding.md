### SarifFinding Unit Verification

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
