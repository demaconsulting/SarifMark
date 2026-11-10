package SarifMark {
    import OtsDependencies::*;
    import SharedDependencies::*;
    private import SoftwareItemMetadata::*;

    doc
    /*
     * SarifMark is a .NET command-line application for validating and asserting
     * SARIF (Static Analysis Results Interchange Format) log content.
     */

    part def SarifMarkSystem {
        doc /* The SarifMark system as a whole. */

        metadata info : SoftwareInfo {
            description = "The SarifMark system as a whole.";
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

        part sarifMarkSharedPackage : SarifMarkSharedPackage;
    }
}
