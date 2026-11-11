package SarifMark {
    private import SoftwareItemMetadata::*;

    part def SarifSubsystem {
        doc /* SARIF log data model and result-aggregation types. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Sarif/SarifTests.cs");
            designFiles = ("docs/design/sarifmark/sarif.md");
            verificationFiles = ("docs/verification/sarifmark/sarif.md");
            requirementFiles = ("docs/reqstream/sarifmark/sarif.yaml");
        }

        part sarifFinding : SarifFinding;
        part sarifRun : SarifRun;
        part sarifResults : SarifResults;
    }
}
