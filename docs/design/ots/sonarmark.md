## SonarMark

SonarMark (`DemaConsulting.SonarMark`) is the CI tool that retrieves quality-gate and
metrics data from SonarCloud and renders it as a markdown document included in the
SarifMark release artifacts. It provides a snapshot of the project's code quality
status at the time of each release.

### Purpose

SonarMark was chosen because SarifMark uses SonarCloud for continuous static analysis
quality monitoring, and SonarMark provides a convenient, consistent way to capture the
SonarCloud quality-gate result, security rating, and key metrics as a human-readable
markdown document. This makes the quality evidence visible alongside the other compliance
documents in the release bundle without manual intervention.

### Features Used

- **Quality-gate retrieval** — queries the SonarCloud API for the current quality-gate
  status (passed or failed) of the configured project.
- **Issues retrieval** — retrieves the count and breakdown of open issues by severity.
- **Hot spots retrieval** — retrieves the count of open security hot spots.
- **Markdown report generation** — renders all retrieved data as a structured markdown
  document.

### Integration Pattern

SonarMark is invoked as a .NET tool from a CI pipeline step after the SonarCloud
analysis has completed:

1. The pipeline installs the tool via `dotnet tool restore`.
2. A pipeline step invokes `sonarmark` with the SonarCloud project key and an API token
   supplied as CI environment secrets.
3. SonarMark queries the SonarCloud API and writes the quality report to the configured
   output path.
4. The generated report is published as a release artifact.
5. The `--validate` flag runs SonarMark's built-in self-validation suite using mock API
   data, confirming all advertised features are operational without requiring a live
   SonarCloud connection. The CI pipeline invokes
   `dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx` to collect self-validation evidence.

No application-level code in SarifMark references SonarMark directly.
