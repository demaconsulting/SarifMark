# Shared Package Verification

## Verification Strategy

The `DemaConsulting.TestResults` shared package is verified through integration evidence obtained
from the SarifMark unit test suite. Because the package is a DemaConsulting-produced shared
package, vendor-provided test evidence is available from the originating repository; however,
the consuming repository also produces direct verification through unit tests in
`ValidationTests.cs` that exercise each advertised feature within the SarifMark context.

## Qualification Evidence

For the TestResults shared package, the following evidence is collected during each test run:

- **TestResults collection creation and population**: Tests in `ValidationTests` confirm that a
  named `TestResults` collection is created and populated with the correct number of results
  during self-validation, and that totals are accurately reported in the summary output.
- **TestResult outcome recording**: Tests confirm that each self-validation scenario produces a
  `TestResult` entry with the correct Passed outcome and that the result name appears in the log.
- **TRX serialization**: `Validation_Run_WithTrxResultsFile_WritesResultsFile` confirms that
  `TrxSerializer` produces a file containing valid TRX XML content.
- **JUnit XML serialization**: `Validation_Run_WithXmlResultsFile_WritesResultsFile` confirms
  that `JUnitSerializer` produces a file containing valid JUnit XML content.

## Regression Approach

On any TestResults package version upgrade, the full test suite is run. All tests in
`ValidationTests.cs` that exercise the TestResults advertised features must continue to pass. If
an upgrade changes the API or serialized output format, the integration design
(`docs/design/shared/test-results.md`) and this document are reviewed and updated before the
upgrade is accepted into the main branch.
