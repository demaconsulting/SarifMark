## SarifMark (Shared Package)

SarifMark (`DemaConsulting.SarifMark`) is used as a shared package in its own CI build pipeline.
A released version of SarifMark is invoked to generate the CodeQL quality report that is
published as part of each release's documentation artifacts.

### Advertised Features Consumed

- **SARIF file reading** — accepts a SARIF 2.1.0 input file via the `--sarif` parameter and
  parses all tool runs and findings contained in the file.
- **Markdown report generation** — writes a structured markdown quality report to the output
  path specified by the `--report` parameter.
- **Custom heading configuration** — accepts a custom heading string via the `--heading`
  parameter that is used as the top-level section title of the generated report.
- **Report depth configuration** — accepts a heading depth value via the `--report-depth`
  parameter that controls the markdown heading level (`#`, `##`, `###`, etc.) used in the
  generated report.

### Integration Pattern

SarifMark is invoked as a .NET tool from a CI pipeline step after the CodeQL analysis has
completed and produced a SARIF output file:

1. The pipeline installs the released version of SarifMark via `dotnet tool restore`.
2. A pipeline step invokes `dotnet sarifmark` with the SARIF file path, report output path,
   custom heading, and report depth:

   ```text
   dotnet sarifmark
     --sarif artifacts/csharp.sarif
     --report docs/code_quality/generated/codeql-quality.md
     --heading "SarifMark CodeQL Analysis"
     --report-depth 1
   ```

3. SarifMark reads the SARIF file, processes all tool runs and findings, and writes the
   markdown quality report to the configured output path.
4. The generated report is published as a release artifact alongside the other compliance
   documents.

No application-level code in SarifMark references the shared package directly; it is consumed
exclusively through the CI pipeline step.

### Assumptions

The self-referential nature of this dependency is intentional: the shared package is an earlier
released version of the same product. All features consumed in the CI pipeline are part of
SarifMark's own advertised feature set and are fully covered by the product's self-validation
test suite. Accordingly, no separate qualification effort is required — the product's own
passing test suite constitutes sufficient evidence that the consumed features behave as specified.
It is assumed that the released version behaves identically to the version under test for the
features exercised in the CI pipeline step.
