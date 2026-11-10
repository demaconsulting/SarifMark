package SarifMark {
    private import SoftwareItemMetadata::*;

    part def SarifRun {
        doc /* Represents a single SARIF run record and its findings. */

        metadata info : SoftwareInfo {
            description = "Represents a single SARIF run record and its findings.";
            sourceFiles = ("src/DemaConsulting.SarifMark/Sarif/SarifRun.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Sarif/SarifRunTests.cs");
            designFiles = ("docs/design/sarifmark/sarif/sarif-run.md");
            verificationFiles = ("docs/verification/sarifmark/sarif/sarif-run.md");
            requirementFiles = ("docs/reqstream/sarifmark/sarif/sarif-run.yaml");
        }
    }
}
