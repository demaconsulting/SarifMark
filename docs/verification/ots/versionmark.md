## VersionMark Verification

### Overview

VersionMark is used in the SarifMark CI pipeline to capture the versions of all tools used in the pipeline and generate
a markdown versions document.

### Verification Strategy

Verification evidence is provided by successful CI pipeline execution: the pipeline step that invokes VersionMark
completes without error and produces the versions markdown document, confirming VersionMark executed and captured all
configured tool versions.

### Requirement Coverage

- **`SarifMark-OTS-VersionMark`**: Tool versions captured and markdown report generated —
  `VersionMark_CapturesVersions`, `VersionMark_GeneratesMarkdownReport`
