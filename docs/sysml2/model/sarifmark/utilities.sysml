package SarifMark {
    private import SoftwareItemMetadata::*;

    part def UtilitiesSubsystem {
        doc /* File-path resolution and filesystem helper utilities. */

        metadata info : SoftwareInfo {
            description = "File-path resolution and filesystem helper utilities.";
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs");
            designFiles = ("docs/design/sarifmark/utilities.md");
            verificationFiles = ("docs/verification/sarifmark/utilities.md");
            requirementFiles = ("docs/reqstream/sarifmark/utilities.yaml");
        }

        part pathHelpers : PathHelpers;
    }
}
