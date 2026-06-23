## SonarMark

### Verification Approach

SonarMark is verified through its built-in `--validate` self-validation mechanism. Running
`sonarmark --validate` executes four internal test scenarios that confirm quality-gate
retrieval, issues retrieval, hot spots retrieval, and markdown report generation are all
functioning correctly using mock API responses — no live SonarCloud connection is required
for self-validation.

### Test Environment

Tests require:

- No network access; all self-validation scenarios use mock API data.
- A writable temporary directory for output files.

### Acceptance Criteria

All four self-validation scenarios must pass with exit code 0 and zero failures, confirming
that SonarMark is correctly installed and all advertised features are operational.

### Test Scenarios

**SonarMark_QualityGateRetrieval**: The `SonarMark_QualityGateRetrieval` self-validation
scenario retrieves mock quality-gate data and confirms the quality-gate status is correctly
parsed and available for report generation.
This scenario is tested by `SonarMark_QualityGateRetrieval`.

**SonarMark_IssuesRetrieval**: The `SonarMark_IssuesRetrieval` self-validation scenario
retrieves mock issues data and confirms the issues list is correctly parsed and available
for report generation.
This scenario is tested by `SonarMark_IssuesRetrieval`.

**SonarMark_HotSpotsRetrieval**: The `SonarMark_HotSpotsRetrieval` self-validation scenario
retrieves mock hot spots data and confirms the hot spots list is correctly parsed and
available for report generation.
This scenario is tested by `SonarMark_HotSpotsRetrieval`.

**SonarMark_MarkdownReportGeneration**: The `SonarMark_MarkdownReportGeneration`
self-validation scenario generates a markdown quality report from mock data and confirms
the report contains the expected quality-gate, issues, and hot spots sections.
This scenario is tested by `SonarMark_MarkdownReportGeneration`.
