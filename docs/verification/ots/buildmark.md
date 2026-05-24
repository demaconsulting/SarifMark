## BuildMark

### Verification Approach

BuildMark is used in the SarifMark CI pipeline to generate a build-notes markdown document containing build metadata,
tool versions, and pipeline artefact listings. BuildMark has no self-test mechanism. Verification evidence is provided
by successful CI pipeline execution: the pipeline step that invokes BuildMark completes without error and produces the
build-notes markdown file, which is subsequently converted to HTML and PDF by Pandoc and WeasyPrint.

### Test Scenarios

**BuildMark_MarkdownReportGeneration**: The CI pipeline step that invokes BuildMark completes without error and
produces the build-notes markdown document containing build metadata, tool versions, and pipeline artefact listings,
confirming BuildMark executed successfully and generated the expected output.
This scenario is verified by successful completion of the BuildMark pipeline step in CI.

### Requirements Coverage

- **`SarifMark-OTS-BuildMark`**: Build notes generated in CI — `BuildMark_MarkdownReportGeneration` (CI pipeline step)
