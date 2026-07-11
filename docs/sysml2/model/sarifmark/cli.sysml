package SarifMark {
    part def CliSubsystem {
        doc /* Command-line argument parsing and execution context. */

        comment testRef /* Test: test/DemaConsulting.SarifMark.Tests/Cli/CliTests.cs */
        comment designRef /* Design: docs/design/sarifmark/cli.md */
        comment verificationRef /* Verification: docs/verification/sarifmark/cli.md */
        comment reqRef /* Requirements: docs/reqstream/sarifmark/cli.yaml */

        part context : Context;
    }
}
