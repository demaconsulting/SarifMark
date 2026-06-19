## ReviewMark

### Verification Approach

ReviewMark is used in the SarifMark CI pipeline to generate a review plan and a review report from the
`.reviewmark.yaml` review-set configuration. Verification evidence is provided by successful CI pipeline execution:
the pipeline steps that invoke ReviewMark complete without error and produce both the review plan and review report
markdown files, which are subsequently converted to HTML and PDF.

### Test Scenarios

**ReviewMark_ReviewPlanGeneration**: The CI pipeline step that invokes ReviewMark to generate the review plan
completes without error and produces the review plan markdown file, confirming ReviewMark read the
`.reviewmark.yaml` configuration and generated the expected output.
This scenario is verified by successful completion of the ReviewMark plan pipeline step in CI.

**ReviewMark_ReviewReportGeneration**: The CI pipeline step that invokes ReviewMark to generate the review report
completes without error and produces the review report markdown file, confirming ReviewMark processed the
review-set configuration and generated the expected report output.
This scenario is verified by successful completion of the ReviewMark report pipeline step in CI.
