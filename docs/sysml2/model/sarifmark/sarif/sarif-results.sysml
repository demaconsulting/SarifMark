package SarifMark {
    private import SoftwareItemMetadata::*;

    part def SarifResults {
        doc /* Loads and aggregates SARIF log results for assertion. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SarifMark/Sarif/SarifResults.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Sarif/SarifResultsTests.cs");
            designFiles = ("docs/design/sarifmark/sarif/sarif-results.md");
            verificationFiles = ("docs/verification/sarifmark/sarif/sarif-results.md");
            requirementFiles = ("docs/reqstream/sarifmark/sarif/sarif-results.yaml");
        }
    }
}
