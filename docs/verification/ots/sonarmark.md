## SonarMark Verification

### Overview

SonarMark is used in the SarifMark CI pipeline to retrieve quality gate status, issues, and hotspots from SonarCloud
and generate a markdown code-quality report.

### Verification Strategy

Verification evidence is provided by successful CI pipeline execution: the pipeline step that invokes SonarMark
completes without error and produces the code-quality markdown document, confirming SonarMark connected to SonarCloud
and rendered the report correctly.

### Requirement Coverage

| Requirement ID | Description | Verification Evidence |
| --- | --- | --- |
| `SarifMark-OTS-SonarMark` | Retrieves quality data and generates markdown report | `SonarMark_QualityGateRetrieval` |
