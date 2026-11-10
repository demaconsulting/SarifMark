package SarifMark {
    private import SoftwareItemMetadata::*;

    part def Context {
        doc /* Command-line argument parser and execution context owner. */

        metadata info : SoftwareInfo {
            description = "Command-line argument parser and execution context owner.";
            sourceFiles = ("src/DemaConsulting.SarifMark/Cli/Context.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Cli/ContextTests.cs");
            designFiles = ("docs/design/sarifmark/cli/context.md");
            verificationFiles = ("docs/verification/sarifmark/cli/context.md");
            requirementFiles = ("docs/reqstream/sarifmark/cli/context.yaml");
        }
    }
}
