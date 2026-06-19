## BuildMark

### Verification Approach

BuildMark is used in the SarifMark CI pipeline to generate a build-notes markdown document containing build metadata
(workflow run ID, trigger event, branch, and timestamp) from the GitHub Actions API. Tool-version information is
captured separately by VersionMark, not by BuildMark.

BuildMark provides a built-in self-test mechanism: the pipeline invokes `dotnet buildmark --validate` and writes the
results to a `.trx` file. Verification evidence is provided by two observable outcomes: (1) the self-validation step
completes without error and produces a passing `.trx` results file, and (2) the subsequent build-notes generation step
completes without error and produces the build-notes markdown file, which is subsequently converted to HTML and PDF by
Pandoc and WeasyPrint.

### Test Scenarios

**BuildMark_SelfValidation**: The CI pipeline step `dotnet buildmark --validate` completes without error and produces a
passing `.trx` results file, confirming BuildMark's built-in self-test passed in the current environment.
This scenario is verified by successful completion of the BuildMark self-validation pipeline step in CI.

**BuildMark_MarkdownReportGeneration**: The CI pipeline step that invokes `dotnet buildmark` to generate the
build-notes document completes without error and produces the build-notes markdown file containing build metadata
(workflow run ID, trigger event, branch, and timestamp), confirming BuildMark executed successfully and generated
the expected output.
This scenario is verified by successful completion of the build-notes generation pipeline step in CI.
