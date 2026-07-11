## SelfTest

![SelfTest Structure](SelfTestView.svg)

The `SelfTest` subsystem provides the self-validation framework for SarifMark.
It runs a built-in suite of tests to demonstrate the tool is functioning correctly in the
deployment environment, without requiring any external static analysis tooling.

### Overview

The `SelfTest` subsystem is invoked when the user passes `--validate` on the command line.
It exercises the tool's own SARIF-reading, report-generation, and enforcement capabilities
using a mock SARIF file, and reports a pass/fail summary. It can also write test results to
a TRX or JUnit XML file for integration with CI/CD pipelines. The subsystem contains a
single unit:

- **Validation**: self-validation test orchestrator; creates mock SARIF files, invokes
  `Program.Run`, inspects outputs, records results, and optionally writes a results file.

### Interfaces

**Validation.Run**: Executes the complete self-validation suite.

- *Type*: In-process .NET static method
- *Role*: Provider
- *Contract*: Accepts a `Context` instance; runs all three validation tests
  (`SarifMark_SarifReading`, `SarifMark_MarkdownReportGeneration`,
  `SarifMark_Enforcement`); writes pass/fail lines to the context output; writes a
  results file if `context.ResultsFile` is set; calls `context.WriteError` for the
  failed-test count if any tests fail.
- *Constraints*: `context` must not be null; throws `ArgumentNullException` if it is.

### Design

The `SelfTest` subsystem contains a single unit (`Validation`) so there is no inter-unit
data flow to describe. The overall test execution flow is:

1. `Program.Run` calls `Validation.Run(context)` when `context.Validate` is `true`.
2. `Validation.Run` calls `PrintValidationHeader` to emit an information table (version,
   machine, OS, .NET runtime, timestamp).
3. A `TestResults` collection is created to accumulate results.
4. Each of the three test methods (`RunSarifReadingTest`, `RunMarkdownReportGenerationTest`,
   `RunEnforcementTest`) is called in sequence; each delegates to the shared
   `RunValidationTest` helper, passing a test-specific validator lambda.
5. `RunValidationTest` creates a temporary directory, writes a mock SARIF file, constructs
   a `Context`, calls `Program.Run`, reads the outputs, invokes the validator, and records
   the pass/fail outcome.
6. A summary of passed and failed tests is written to the context output.
7. If `context.ResultsFile` is set, `WriteResultsFile` serializes the results in the
   appropriate format (.trx or .xml).

The `SelfTest` subsystem depends on `Utilities.PathHelpers` for safe path construction
within the temporary directory, and calls back into `Program.Run` to exercise the full
analysis pipeline.
