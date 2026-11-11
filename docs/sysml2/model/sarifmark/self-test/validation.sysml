package SarifMark {
    private import SoftwareItemMetadata::*;

    part def Validation {
        doc /* Self-validation test runner. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SarifMark/SelfTest/Validation.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/SelfTest/ValidationTests.cs");
            designFiles = ("docs/design/sarifmark/self-test/validation.md");
            verificationFiles = ("docs/verification/sarifmark/self-test/validation.md");
            requirementFiles = ("docs/reqstream/sarifmark/self-test/validation.yaml");
        }
    }
}
