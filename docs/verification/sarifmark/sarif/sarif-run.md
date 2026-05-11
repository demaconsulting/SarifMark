### SarifRun Unit Verification

#### Overview

The `SarifRun` record is verified by the `SarifRunTests` test class in
`test/DemaConsulting.SarifMark.Tests/Sarif/SarifRunTests.cs`. The internal constructor is accessed via the
`InternalsVisibleTo` grant. Tests cover property access, `HasIssues` logic, `ToMarkdown` output at various depths, and
all argument validation paths.

#### Requirement Coverage

- **`SarifMark-SarifRun-ToolName`**: `ToolName` property exposed —
  `SarifRun_InternalConstructor_CreatesValidInstance`
- **`SarifMark-SarifRun-ToolVersion`**: `ToolVersion` property exposed —
  `SarifRun_InternalConstructor_CreatesValidInstance`
- **`SarifMark-SarifRun-Results`**: `Results` and `ResultCount` exposed —
  `SarifRun_InternalConstructor_CreatesValidInstance`
- **`SarifMark-SarifRun-FileCount`**: `FileCount` property exposed —
  `SarifRun_InternalConstructor_CreatesValidInstance`
- **`SarifMark-SarifRun-HasIssues`**: `HasIssues` false/true for results —
  `SarifRun_HasIssues_NoResults_ReturnsFalse`
- **`SarifMark-SarifRun-ToMarkdown`**: Depth-1 full markdown output —
  `SarifRun_ToMarkdown_Depth1_ProducesCorrectOutput`
- **`SarifMark-SarifRun-ValidateDepth`**: Depth < 1 or > 6 throws —
  `SarifRun_ToMarkdown_Depth6_IssuesHeadingCappedAtSix`
- **`SarifMark-SarifRun-FormatLocation`**: Location format —
  `SarifRun_ToMarkdown_ResultWithoutLocation_ShowsNoLocation`
- **`SarifMark-SarifRun-FormatCount`**: 0/1/many results count format —
  `SarifRun_ToMarkdown_OneResult_UsesSingularForm`
