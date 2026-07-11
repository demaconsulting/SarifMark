### Validation

![SelfTest Structure](SelfTestView.svg)

#### Purpose

`Validation` is a static class that provides the self-validation framework for SarifMark.
It runs a built-in suite of three functional tests using a mock SARIF file, reports
pass/fail results to the console, and optionally writes a TRX or JUnit XML results file.

#### Data Model

N/A - `Validation` is a `static` class with no instance or static fields. All state is
local to the `Run` method call and its callees.

#### Key Methods

**Run**: Executes the complete self-validation suite.

- *Parameters*: `Context context` — the active output channel and settings
- *Returns*: `void`
- *Preconditions*: `context` is not null.
- *Postconditions*: All three tests have been executed; pass/fail lines have been written
  to `context`; `context.WriteError` has been called with the failed-test count if any
  tests failed; the results file has been written if `context.ResultsFile` is set.

`Run` validates `context` is not null, calls `PrintValidationHeader`, creates a
`TestResults` collection, calls the three test methods in sequence, prints the summary,
and optionally calls `WriteResultsFile`.

**RunValidationTest** (private shared helper): Creates a temporary directory, writes a
mock SARIF file, constructs a `Context`, calls `Program.Run`, reads outputs, and invokes
a caller-supplied validator lambda.

- *Parameters*: `Context context` — the active output channel and settings;
  `DemaConsulting.TestResults.TestResults testResults` — the shared results collection;
  `string testName` — the name of the test; `string? reportFileName` — optional report
  file name to generate; `Func<int, string, string?, string?> validator` — function
  receiving the exit code, log content, and nullable report content, returning `null` on
  success or an error message string on failure; `IEnumerable<string>? extraArgs` —
  optional extra command-line arguments (default `null`)
- Appends a `TestResult` (pass or fail) to the shared `TestResults` collection; writes a
  `✓` or `✗` status line to the context output.

Each of the three self-validation test methods (`RunSarifReadingTest`, `RunMarkdownReportGenerationTest`,
`RunEnforcementTest`) calls `RunValidationTest` with the appropriate validator lambda.
`RunMarkdownReportGenerationTest` extracts `context.Depth` and constructs
`depthArgs = new[] { "--depth", context.Depth.ToString() }`, passing them as `extraArgs` to
`RunValidationTest`. This ensures report heading depth is consistent between normal analysis
mode and self-validation mode — the same `--depth` value the caller supplied is honoured
inside the self-validation subprocess. Before comparing the generated report against the
expected heading, it also writes the actual first line of the generated report to `context`
as a `"Report heading: ..."` diagnostic line, so the effect of `--depth` on the generated
report is independently observable in the self-validation log rather than only visible
through an internal comparison against the same `context.Depth` value used to build the
expectation.

**WriteResultsFile** (private): Serializes the `TestResults` collection.

- Inspects the extension of `context.ResultsFile`: `.trx` → `TrxSerializer.Serialize`;
  `.xml` → `JUnitSerializer.Serialize`; other → `context.WriteError`.
- Catches `IOException`, `UnauthorizedAccessException`, `ArgumentException`, and
  `NotSupportedException`, routing them to `context.WriteError`.

**CreateMockSarifFile** (private): Writes a hard-coded SARIF 2.1.0 JSON file to a given
path. The file contains a single run from tool `MockTool` (version `1.0.0`) with exactly
two results: `TEST001` (warning, `src/Program.cs` line 42) and `TEST002` (error,
`src/Helper.cs` line 15). All three self-tests validate against this exact structure.

#### Error Handling

`Run` calls `ArgumentNullException.ThrowIfNull(context)` before any other processing.

`RunValidationTest` wraps its body in a `try/catch`; any unhandled exception is passed to
`HandleTestException`, which sets the test outcome to `Failed`, records the exception
message, calls `context.WriteError`, and does not rethrow — preventing a single test
failure from aborting the remaining tests.

`WriteResultsFile` catches file I/O exceptions (`IOException`, `UnauthorizedAccessException`,
`ArgumentException`, `NotSupportedException`) and routes them to `context.WriteError`.
An unknown file extension produces an error message via `context.WriteError`.

The private `TemporaryDirectory` nested class wraps creation failures in
`InvalidOperationException` and silently ignores deletion errors on `Dispose` to allow
graceful cleanup in constrained environments.

#### Dependencies

- **Program** — `Program.Run` is called within `RunValidationTest` to exercise the full
  analysis and enforcement pipelines.
- **PathHelpers** — `PathHelpers.SafePathCombine` is used both directly in
  `RunValidationTest` to construct log, SARIF, and report file paths within the temporary
  directory, and by the `TemporaryDirectory` nested class to construct the temporary
  directory path itself from a GUID-based name.
- **Context** — used both as the external output channel passed to `Run` and as internally
  constructed test contexts passed to `Program.Run` during each test.
- **DemaConsulting.TestResults** — `TestResults`, `TestResult`, `TrxSerializer`, and `JUnitSerializer` types are used
  to collect test results and serialize them in TRX or JUnit XML format.

#### Callers

- **Program** — calls `Validation.Run(context)` when `context.Validate` is `true`.
