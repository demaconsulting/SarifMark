package SarifMark {
    part def UtilitiesSubsystem {
        doc /* File-path resolution and filesystem helper utilities. */

        comment testRef /* Test: test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs */
        comment designRef /* Design: docs/design/sarifmark/utilities.md */
        comment verificationRef /* Verification: docs/verification/sarifmark/utilities.md */
        comment reqRef /* Requirements: docs/reqstream/sarifmark/utilities.yaml */

        part pathHelpers : PathHelpers;
    }
}
