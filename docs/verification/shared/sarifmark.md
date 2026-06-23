## SarifMark (Shared Package)

### Verification Approach

SarifMark is verified as a shared package through the product's own self-validation test suite.
Because the shared package is a released version of the same product, every feature consumed
in the CI pipeline is directly covered by the integration tests in
`test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs`. These tests exercise SarifMark
end-to-end against real SARIF input files and confirm that SARIF reading, report generation,
custom heading application, and report depth configuration all function correctly.

In addition to the test suite, the CI pipeline step itself serves as operational confirmation:
the step must complete with exit code 0 and the generated report must be present at the
expected output path before the pipeline proceeds to publish release artifacts.

### Test Environment

The self-validation tests require:

- No external services; all tests operate on local SARIF test data files bundled with the
  test project.
- A writable temporary directory for output report files.
- A built version of `DemaConsulting.SarifMark.dll` in the test assembly's output directory.

The CI pipeline step additionally requires that the CodeQL SARIF output file
`artifacts/csharp.sarif` is present before the SarifMark step executes.

### Acceptance Criteria

All relevant self-validation integration tests must pass with exit code 0 and zero failures.
The CI pipeline step must complete with exit code 0 and the file
`docs/code_quality/generated/codeql-quality.md` must be present and non-empty in the
pipeline workspace after the step runs.

### Test Scenarios

**SarifMark_ValidSarifFile_ProcessesSuccessfully**: SarifMark is invoked with a valid SARIF
input file via `--sarif`. The test confirms the process exits with code 0, reads the SARIF
file successfully, and reports the tool name and result count without error.
This scenario is tested by `SarifMark_ValidSarifFile_ProcessesSuccessfully`.

**SarifMark_GenerateReport_CreatesReportFile**: SarifMark is invoked with `--sarif` and
`--report` pointing to a temporary output path. The test confirms the process exits with
code 0, creates the report file, and the file contains the expected tool heading.
This scenario is tested by `SarifMark_GenerateReport_CreatesReportFile`.

**SarifMark_ValidSarif_NoIssues_GeneratesReport**: SarifMark is invoked with a SARIF file
that contains no findings. The test confirms the process exits with code 0 and the generated
report correctly indicates that no issues were found.
This scenario is tested by `SarifMark_ValidSarif_NoIssues_GeneratesReport`.

**SarifMark_CustomHeading_AppearsInReport**: SarifMark is invoked with `--sarif`, `--report`,
and `--heading "Custom Analysis"`. The test confirms the process exits with code 0 and the
generated report contains the specified custom heading as the top-level section title.
This scenario is tested by `SarifMark_CustomHeading_AppearsInReport`.

**SarifMark_LegacyReportDepth_IsAccepted**: SarifMark is invoked with `--sarif`, `--report`,
and `--report-depth 3`. The test confirms the process exits with code 0, the report file is
created, and the heading in the report uses the `###` level corresponding to depth 3.
This scenario is tested by `SarifMark_LegacyReportDepth_IsAccepted`.
