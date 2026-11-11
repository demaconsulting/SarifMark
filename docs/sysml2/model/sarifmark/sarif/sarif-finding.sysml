package SarifMark {
    private import SoftwareItemMetadata::*;

    part def SarifFinding {
        doc /* Represents a single SARIF result/finding record. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SarifMark/Sarif/SarifFinding.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Sarif/SarifFindingTests.cs");
            designFiles = ("docs/design/sarifmark/sarif/sarif-finding.md");
            verificationFiles = ("docs/verification/sarifmark/sarif/sarif-finding.md");
            requirementFiles = ("docs/reqstream/sarifmark/sarif/sarif-finding.yaml");
        }
    }
}
