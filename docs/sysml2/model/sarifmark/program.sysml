package SarifMark {
    private import SoftwareItemMetadata::*;

    part def Program {
        doc /* Entry point and execution orchestrator. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SarifMark/Program.cs");
            testFiles = ("test/DemaConsulting.SarifMark.Tests/ProgramTests.cs");
            designFiles = ("docs/design/sarifmark/program.md");
            verificationFiles = ("docs/verification/sarifmark/program.md");
            requirementFiles = ("docs/reqstream/sarifmark/program.yaml");
        }
    }
}
