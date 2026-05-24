## VersionMark

### Verification Approach

VersionMark is used in the SarifMark CI pipeline to capture the versions of all tools used in the pipeline and
generate a markdown versions document. Verification evidence is provided by successful CI pipeline execution: the
pipeline step that invokes VersionMark completes without error and produces the versions markdown document, confirming
VersionMark executed and captured all configured tool versions.

### Test Scenarios

**VersionMark_CapturesVersions**: The CI pipeline step that invokes VersionMark completes without error, confirming
VersionMark executed and captured the versions of all configured tools used in the pipeline.
This scenario is verified by successful completion of the VersionMark pipeline step in CI.

**VersionMark_GeneratesMarkdownReport**: The VersionMark pipeline step produces the tool-versions markdown document,
confirming that VersionMark generated the expected output file containing the captured version information.
This scenario is verified by the presence of the versions markdown file in CI pipeline artifacts.

### Requirements Coverage

- **`SarifMark-OTS-VersionMark`**: Tool versions captured and markdown report generated —
  `VersionMark_CapturesVersions`, `VersionMark_GeneratesMarkdownReport`
