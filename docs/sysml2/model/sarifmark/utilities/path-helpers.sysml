package SarifMark {
    private import SoftwareItemMetadata::*;

    part def PathHelpers {
        doc /* File-path resolution and filesystem helper functions. */

        metadata info : SoftwareInfo {
            description = "File-path resolution and filesystem helper functions.";
            sourceFiles = ("src/DemaConsulting.SarifMark/Utilities/PathHelpers.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Utilities/PathHelpersTests.cs");
            designFiles = ("docs/design/sarifmark/utilities/path-helpers.md");
            verificationFiles = ("docs/verification/sarifmark/utilities/path-helpers.md");
            requirementFiles = ("docs/reqstream/sarifmark/utilities/path-helpers.yaml");
        }
    }
}
