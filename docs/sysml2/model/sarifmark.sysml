package SarifMark {
    import OtsDependencies::*;
    import SharedDependencies::*;

    doc
    /*
     * SarifMark is a .NET command-line application for validating and asserting
     * SARIF (Static Analysis Results Interchange Format) log content.
     */

    part def SarifMarkSystem {
        doc /* The SarifMark system as a whole. */

        comment testRef /* Test: test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs */
        comment designRef /* Design: docs/design/sarifmark.md */
        comment verificationRef /* Verification: docs/verification/sarifmark.md */
        comment reqRef /* Requirements: docs/reqstream/sarifmark.yaml */

        part program : Program;
        part cli : CliSubsystem;
        part sarif : SarifSubsystem;
        part selfTest : SelfTestSubsystem;
        part utilities : UtilitiesSubsystem;

        part testResults : TestResultsPackage;

        part sarifMarkSharedPackage : SarifMarkSharedPackage;
    }
}
