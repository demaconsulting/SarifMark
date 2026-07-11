package SarifMark {
    part def SarifSubsystem {
        doc /* SARIF log data model and result-aggregation types. */

        comment testRef /* Test: test/DemaConsulting.SarifMark.Tests/Sarif/SarifTests.cs */
        comment designRef /* Design: docs/design/sarifmark/sarif.md */
        comment verificationRef /* Verification: docs/verification/sarifmark/sarif.md */
        comment reqRef /* Requirements: docs/reqstream/sarifmark/sarif.yaml */

        part sarifFinding : SarifFinding;
        part sarifRun : SarifRun;
        part sarifResults : SarifResults;
    }
}
