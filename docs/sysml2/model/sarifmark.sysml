package SarifMark {
    public import SarifMark_Program::*;
    public import SarifMark_CliSubsystem::*;
    public import SarifMark_SarifSubsystem::*;
    public import SarifMark_SelfTestSubsystem::*;
    public import SarifMark_UtilitiesSubsystem::*;
    private import OtsDependencies::*;
    private import SharedPackages::*;
    private import SoftwareItemMetadata::*;

    doc
    /*
     * SarifMark is a .NET command-line application for validating and asserting
     * SARIF (Static Analysis Results Interchange Format) log content.
     */

    part def SarifMarkSystem {
        doc /* The SarifMark system as a whole. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs");
            designFiles = ("docs/design/sarifmark.md");
            verificationFiles = ("docs/verification/sarifmark.md");
            requirementFiles = ("docs/reqstream/sarifmark.yaml");
        }

        part program : Program;
        part cli : CliSubsystem;
        part sarif : SarifSubsystem;
        part selfTest : SelfTestSubsystem;
        part utilities : UtilitiesSubsystem;

        part testResults : TestResultsPackage;
        part sysml2tools : SysML2ToolsTool;
        part filesystemGlobbing : FileSystemGlobbingPackage;
        part helpCards : HelpCardsPackage;

        part sarifMarkSharedPackage : SarifMarkSharedPackage;
    }
}
