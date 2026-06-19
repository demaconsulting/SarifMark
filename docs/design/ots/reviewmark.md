## ReviewMark

ReviewMark (`DemaConsulting.ReviewMark`) is the CI tool that generates a review plan and
a review report from the `.reviewmark.yaml` review configuration. It documents which files
have been formally reviewed, by whom, and when, providing audit evidence for the code
review process.

### Purpose

ReviewMark was chosen because it provides structured, automated enforcement of formal
review coverage across the repository. It ensures that every file included in a review
set has been reviewed before it is considered compliant, and it produces review artifacts
that can be included in release documentation. The tool integrates directly with the
Continuous Compliance workflow and its output format is consistent with the other
compliance documents in the SarifMark release bundle.

### Features Used

- **Review plan generation** — generates a markdown document listing all files included
  in each review set and their current review status.
- **Review report generation** — generates a markdown document summarizing completed
  reviews with reviewer, date, and outcome information.
- **Review coverage enforcement** — when run with enforcement flags, fails the pipeline
  if required files have not been reviewed.

### Integration Pattern

ReviewMark is invoked as a .NET tool from CI pipeline steps:

1. The pipeline installs the tool via `dotnet tool restore`.
2. A pipeline step invokes `reviewmark` with the `.reviewmark.yaml` configuration and
   the review evidence store.
3. ReviewMark writes the review plan and review report to the configured output paths.
4. The generated documents are published as release artifacts.
5. The `--validate` flag runs ReviewMark's built-in self-validation suite, confirming all
   advertised features are operational. The CI pipeline invokes
   `dotnet reviewmark --validate --results {path}` to collect self-validation evidence.

The review configuration in `.reviewmark.yaml` defines the review sets and their member
files. No application-level code in SarifMark references ReviewMark directly.
