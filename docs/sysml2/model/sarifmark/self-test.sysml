package SarifMark {
    part def SelfTestSubsystem {
        doc /* Self-validation test runner exercised at startup for self-check evidence. */

        comment testRef /* Test: test/DemaConsulting.SarifMark.Tests/SelfTest/SelfTestTests.cs */
        comment designRef /* Design: docs/design/sarifmark/self-test.md */
        comment verificationRef /* Verification: docs/verification/sarifmark/self-test.md */
        comment reqRef /* Requirements: docs/reqstream/sarifmark/self-test.yaml */

        part validation : Validation;
    }
}
