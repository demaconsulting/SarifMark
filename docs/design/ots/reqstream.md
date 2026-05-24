## ReqStream

ReqStream (`DemaConsulting.ReqStream`) is the CI tool that enforces requirements
traceability by verifying that every requirement in `requirements.yaml` is linked to
at least one passing test. It processes the requirements file and the TRX test-result
files produced by the test run to generate a requirements report, a justifications
document, and a traceability matrix.

### Purpose

ReqStream was chosen because it provides automated, binding enforcement of the Continuous
Compliance traceability mandate: no requirement may remain untested in a passing build.
Running with `--enforce` makes unproven requirements a build-breaking condition, providing
continuous evidence that the implementation satisfies its specified requirements. This is
the primary traceability mechanism for the SarifMark project.

### Features Used

- **Requirements processing** — reads `requirements.yaml` and all included requirement
  files to build the complete requirements model.
- **TRX consumption** — reads TRX test-result files to determine which test names passed
  in the current build.
- **Traceability enforcement** — with `--enforce`, exits non-zero if any requirement has
  no linked passing test, blocking the CI pipeline.
- **Report generation** — produces a requirements report (Markdown), a justifications
  document, and a traceability matrix as release artifacts.

### Integration Pattern

ReqStream is invoked as a .NET tool from a CI pipeline step after the test run has
completed and the TRX result files are available:

1. The pipeline installs the tool via `dotnet tool restore`.
2. A pipeline step invokes `reqstream` with `requirements.yaml`, the TRX file paths,
   and `--enforce` to enforce full coverage.
3. ReqStream writes the requirements report, justifications, and traceability matrix to
   the configured output paths.
4. A non-zero exit code blocks the pipeline and requires the developer to add tests or
   justifications before merging.
5. The generated reports are published as release artifacts.

No application-level code in SarifMark references ReqStream directly.
