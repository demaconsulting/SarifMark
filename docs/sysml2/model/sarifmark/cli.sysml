package SarifMark {
    private import SoftwareItemMetadata::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing and execution context. */

        metadata info : SoftwareInfo {
            description = "Command-line argument parsing and execution context.";
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/sarifmark/cli.md");
            verificationFiles = ("docs/verification/sarifmark/cli.md");
            requirementFiles = ("docs/reqstream/sarifmark/cli.yaml");
        }

        part context : Context;
    }
}
