package SarifMark_HelpCommand {
    private import SoftwareItemMetadata::*;

    part def HelpCommand {
        doc /* Built-in help command: answers the help verb from embedded help cards before option parsing. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SarifMark/Cli/HelpCommand.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/Cli/HelpCommandTests.cs");
            designFiles = ("docs/design/sarifmark/cli/help-command.md");
            verificationFiles = ("docs/verification/sarifmark/cli/help-command.md");
            requirementFiles = ("docs/reqstream/sarifmark/cli/help-command.yaml");
        }
    }
}
