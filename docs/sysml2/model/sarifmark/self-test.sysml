package SarifMark {
    private import SoftwareItemMetadata::*;

    part def SelfTestSubsystem {
        doc /* Self-validation test runner exercised at startup for self-check evidence. */

        metadata info : SoftwareInfo {
            description = "Self-validation test runner exercised at startup for self-check evidence.";
            testFiles = ("test/DemaConsulting.SarifMark.Tests/SelfTest/SelfTestTests.cs");
            designFiles = ("docs/design/sarifmark/self-test.md");
            verificationFiles = ("docs/verification/sarifmark/self-test.md");
            requirementFiles = ("docs/reqstream/sarifmark/self-test.yaml");
        }

        part validation : Validation;
    }
}
