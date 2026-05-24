# OTS Verification

## Verification Strategy

Each OTS item is verified using one of three evidence categories matched to its role in the pipeline:

1. **Self-validation output**: Tools that expose a `--validate` or version/help flag (FileAssert) are exercised in
   the CI pipeline self-validation step; passing output confirms the tool is installed and operational.
2. **Successful CI pipeline completion**: Tools without a self-test mechanism (BuildMark, ReviewMark, SonarMark,
   VersionMark, ReqStream, Pandoc, WeasyPrint) are verified by successful CI pipeline execution — each tool produces
   an artefact (document, report, or exit-code assertion) that confirms functional operation.
3. **Framework operation**: xUnit v3 (the test framework) is verified implicitly — the test suite runs and produces
   passing results, confirming the framework discovers and executes tests correctly.

All verification evidence is collected from the official CI pipeline run. Local runs may not reproduce all evidence
due to environment dependencies (SonarCloud credentials, GitHub API tokens, etc.).

## Qualification Evidence

For each OTS item, the following evidence is collected during CI pipeline execution:

- **FileAssert**: The self-validation pipeline step invokes `fileassert --version` and `fileassert --help`; passing
  output confirms the tool is installed. Subsequent pipeline steps assert generated HTML and PDF output files meet
  structural and content requirements.
- **ReqStream**: The `--enforce` pipeline step completes with exit code 0, confirming that all requirements map to
  at least one passing test in the TRX result files.
- **Pandoc**: FileAssert assertions confirm that each generated HTML file exists, is non-trivial in size, contains
  a valid `<title>` element, and includes expected document content.
- **WeasyPrint**: FileAssert assertions confirm that each generated PDF file exists, is non-trivial in size,
  contains at least one page, and includes expected rendered text.
- **BuildMark**: The build-notes markdown file is present in the pipeline artifacts, confirming successful execution.
- **VersionMark**: The tool-versions markdown file is present, confirming all configured tool versions were captured.
- **SonarMark**: The code-quality markdown file is present, confirming successful SonarCloud connection and report
  generation.
- **ReviewMark**: Both the review plan and review report markdown files are present, confirming successful execution
  of both pipeline steps.
- **xUnit v3**: The test suite produces passing results across all test classes, and TRX result files are generated
  by `dotnet test --results-directory`, confirming test discovery, execution, and result serialization.

## Regression Approach

On any OTS item version upgrade, the CI pipeline is run in full. All three evidence categories are re-evaluated:

- Self-validation tests must pass.
- All CI pipeline steps that produce artifacts must complete without error.
- All FileAssert output assertions on generated HTML and PDF files must continue to hold.

If a tool upgrade changes its output format, API, or CLI interface in a way that affects verification evidence,
the corresponding verification evidence document (`docs/verification/ots/{ots-name}.md`) is reviewed and updated
before the upgrade is accepted into the main branch.
