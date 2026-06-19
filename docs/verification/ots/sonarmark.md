## SonarMark

### Verification Approach

SonarMark is used in the SarifMark CI pipeline to retrieve quality gate status, issues, and hotspots from SonarCloud
and generate a markdown code-quality report. Verification evidence is provided by successful CI pipeline execution:
the pipeline step that invokes SonarMark completes without error and produces the code-quality markdown document,
confirming SonarMark connected to SonarCloud and rendered the report correctly.

### Test Environment

SonarMark executes as a CI pipeline step with access to the SonarCloud API using the
project token configured in the CI environment. No local test execution is possible;
all verification is performed by CI pipeline runs against the live SonarCloud instance
for the `demaconsulting/SarifMark` project.

### Acceptance Criteria

All SonarMark CI pipeline steps complete without error and produce the expected code-quality
markdown document. The generated document exists, has non-trivial size, and contains the
quality-gate status, issues list, and hot spots sections. Every SonarMark OTS requirement
maps to at least one named pipeline scenario (IEC 62304 §5.5.2).

### Test Scenarios

**SonarMark_QualityGateRetrieval**: The CI pipeline step that invokes SonarMark completes without error and produces
the code-quality markdown document, confirming SonarMark successfully connected to SonarCloud and rendered the quality
gate status, issues, and hotspots into the expected report format.
This scenario is verified by successful completion of the SonarMark pipeline step in CI.

**SonarMark_IssuesRetrieval**: The CI pipeline step that invokes SonarMark completes without error, confirming
SonarMark successfully retrieved the issues list from SonarCloud and rendered it in the code-quality markdown report.
This scenario is verified by successful completion of the SonarMark pipeline step in CI.

**SonarMark_HotSpotsRetrieval**: The CI pipeline step that invokes SonarMark completes without error, confirming
SonarMark successfully retrieved the hot spots list from SonarCloud and rendered it in the code-quality markdown
report. This scenario is verified by successful completion of the SonarMark pipeline step in CI.

**SonarMark_MarkdownReportGeneration**: The CI pipeline step that invokes SonarMark produces the code-quality
markdown document, confirming SonarMark successfully generated a complete markdown report containing quality-gate,
issues, and hot spots sections.
This scenario is verified by successful completion of the SonarMark pipeline step in CI.
