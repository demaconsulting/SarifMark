# Shared Package Verification

## Verification Strategy

Each shared package is verified through the product's own self-validation test suite,
supplemented by confirmation that the CI pipeline step completes successfully. Because
the one shared package consumed by SarifMark is a released version of the same product,
every feature consumed in the CI pipeline is directly covered by the integration tests
in `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs`.

Self-validation tests exercise SarifMark end-to-end against local SARIF test data files
and confirm that SARIF reading, markdown report generation, custom heading application,
and report depth configuration all function correctly without requiring any external
services. The CI pipeline step additionally confirms that the released version operates
correctly in the pipeline environment by completing with exit code 0 and producing the
expected report file.

## Qualification Evidence

For the SarifMark shared package, the following evidence is collected:

- **Self-validation integration tests**: The integration tests in
  `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs` cover all features consumed
  by the CI pipeline step. All relevant test methods must pass in every CI run, and the
  TRX result files are linked to requirements via the ReqStream trace matrix.
- **CI pipeline step completion**: The `Generate CodeQL Quality Report with SarifMark`
  pipeline step must complete with exit code 0. The generated file
  `docs/code_quality/generated/codeql-quality.md` must be present and non-empty in the
  pipeline workspace, confirming the shared package read the SARIF input and wrote the
  markdown report successfully.

## Regression Approach

On any shared package version upgrade, the CI pipeline is run in full. All self-validation
integration tests must pass. The pipeline step producing the CodeQL quality report must
complete without error and the output file must be present. If a version upgrade changes
the CLI interface or output format in a way that affects the pipeline step, the
corresponding documentation (`docs/verification/shared/sarifmark.md`) is reviewed and
updated before the upgrade is accepted into the main branch.
