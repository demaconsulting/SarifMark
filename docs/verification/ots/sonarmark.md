## SonarMark

### Verification Approach

SonarMark is used in the SarifMark CI pipeline to retrieve quality gate status, issues, and hotspots from SonarCloud
and generate a markdown code-quality report. Verification evidence is provided by successful CI pipeline execution:
the pipeline step that invokes SonarMark completes without error and produces the code-quality markdown document,
confirming SonarMark connected to SonarCloud and rendered the report correctly.

### Test Scenarios

**SonarMark_QualityGateRetrieval**: The CI pipeline step that invokes SonarMark completes without error and produces
the code-quality markdown document, confirming SonarMark successfully connected to SonarCloud and rendered the quality
gate status, issues, and hotspots into the expected report format.
This scenario is verified by successful completion of the SonarMark pipeline step in CI.

### Requirements Coverage

- **`SarifMark-OTS-SonarMark`**: Retrieves quality data and generates markdown report —
  `SonarMark_QualityGateRetrieval`
