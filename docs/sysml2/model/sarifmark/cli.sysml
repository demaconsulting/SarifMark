package SarifMark_CliSubsystem {
    public import SarifMark_Context::*;
    public import SarifMark_HelpCommand::*;
    private import SoftwareItemMetadata::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing, execution context, and the built-in help command. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/sarifmark/cli.md");
            verificationFiles = ("docs/verification/sarifmark/cli.md");
            requirementFiles = ("docs/reqstream/sarifmark/cli.yaml");
        }

        part context : Context;
        part helpCommand : HelpCommand;
    }
}
