# Self-Validation

## Overview

The self-validation layer provides built-in verification of the tool's core functionality.
It is invoked when the `--validate` flag is passed and can write results to a TRX or
JUnit XML file when `--results` is also provided. This satisfies requirements
`SarifMark-Validate-Mode` and `SarifMark-Validate-ResultFiles`.

## Validation Class

The `Validation` class (`Validation.cs`) is declared `internal static`, limiting its use to the
`DemaConsulting.SarifMark` assembly. It exposes a single public method, `Run`, and organizes
all test execution internally.

### Run Method

Before executing the sequence, `Run` validates its input by calling
`ArgumentNullException.ThrowIfNull(context)`, throwing `ArgumentNullException` immediately if
`context` is null. This satisfies requirement `SarifMark-Validation-NullCheck`.

`Run` then orchestrates the self-validation sequence:

1. Calls `PrintValidationHeader` to emit a markdown table with tool version, machine
   name, OS version, .NET runtime, and timestamp.
2. Creates a `TestResults` collection named `"SarifMark Self-Validation"`.
3. Calls `RunSarifReadingTest`, `RunMarkdownReportGenerationTest`, and
   `RunEnforcementTest` to execute the three functional tests.
4. Prints a summary of passed and failed tests, calling `context.WriteError` for the
   failed count if any tests failed.
5. If `context.ResultsFile` is set, calls `WriteResultsFile` to persist the results.

### RunSarifReadingTest

`RunSarifReadingTest` verifies end-to-end SARIF reading:

1. Creates a `TemporaryDirectory`.
2. Writes a mock SARIF file containing two results from a tool named `MockTool`.
3. Constructs a `Context` with `--silent`, `--log <file>`, and `--sarif <file>`.
4. Calls `Program.Run` and verifies exit code is 0.
5. Checks the log contains `"Tool: MockTool 1.0.0"` and `"Results: 2"`.

The test name is `SarifMark_SarifReading`, satisfying `SarifMark-Sarif-Reading`.

### RunMarkdownReportGenerationTest

`RunMarkdownReportGenerationTest` verifies report generation end-to-end:

1. Creates a `TemporaryDirectory`.
2. Writes the same mock SARIF file used in the reading test.
3. Computes `depthArgs = new[] { "--depth", context.Depth.ToString() }` and
   `headingPrefix = new string('#', context.Depth)`.
4. Constructs a `Context` with `--silent`, `--log <file>`, `--sarif <file>`,
   `--report <file>`, and `depthArgs` as extra arguments.
5. Calls `Program.Run` and verifies exit code is 0.
6. Checks the report file contains `"{headingPrefix} MockTool Analysis"` and `"Found 2 issues"`.

The test name is `SarifMark_MarkdownReportGeneration`, satisfying `SarifMark-Report-Markdown`.

### RunEnforcementTest

`RunEnforcementTest` verifies enforcement mode by delegating to `RunValidationTest` with
`--enforce` as an extra argument and a validator that:

1. Verifies exit code is non-zero.
2. Checks the log contains `"Error: Issues found in SARIF file"`.

The test name is `SarifMark_Enforcement`, satisfying `SarifMark-Enforce-Mode` and
`SarifMark-Enforce-ExitCode`.

### RunValidationTest

`RunValidationTest` is a private shared helper used by `RunSarifReadingTest`,
`RunMarkdownReportGenerationTest`, and `RunEnforcementTest`. It accepts a test name, an
optional report file name, a caller-supplied `validator` function, and an optional
`extraArgs` collection, and:

1. Creates a `TemporaryDirectory`.
2. Creates a mock SARIF file and builds a command-line argument list with `--silent`,
   `--log`, and `--sarif`. If a `reportFileName` is provided, adds `--report` to the
   argument list. Any `extraArgs` are appended last.
3. Constructs a `Context` and calls `Program.Run`, capturing the exit code.
4. Reads the log and (if present) report file contents and passes the exit code, log
   content, and report content to the `validator` function.
5. Records the test as passed or failed in the `TestResults` collection and prints a `✓`
   or `✗` status line to the context output.

This design avoids duplication across the three test methods while keeping each test's
validation logic distinct and independently readable.

### WriteResultsFile

`WriteResultsFile` inspects the file extension of `context.ResultsFile`:

- `.trx` → `TrxSerializer.Serialize`. This satisfies `SarifMark-Validate-TrxFormat`.
- `.xml` → `JUnitSerializer.Serialize`. This satisfies `SarifMark-Validate-JUnitFormat`.
- Other → writes an error via `context.WriteError`.

On success, writes `"Results written to <path>"` via `context.WriteLine`. Catches
`IOException`, `UnauthorizedAccessException`, `ArgumentException`, and
`NotSupportedException` and routes them through `context.WriteError`.

The serialized content is written with `File.WriteAllText`.

### CreateMockSarifFile

`CreateMockSarifFile(string filePath)` writes a hard-coded SARIF 2.1.0 JSON file to `filePath`.
The file contains a single run from a tool named `MockTool` (version `1.0.0`) with exactly two
results: `TEST001` (warning, `src/Program.cs` line 42) and `TEST002` (error, `src/Helper.cs`
line 15). All three self-tests validate against this specific structure, so any change to
the mock file must be reflected in the corresponding validator lambdas.

### CreateTestResult

`CreateTestResult(string testName)` allocates a new `TestResult` object with `Name` set to
`testName`, `ClassName` set to `"Validation"`, and `CodeBase` set to `"SarifMark"`. Centralizing
creation ensures every test result carries consistent metadata without repetition across the
three test methods.

### FinalizeTestResult

`FinalizeTestResult(TestResult test, DateTime startTime, TestResults testResults)` sets
`test.Duration` to the elapsed time since `startTime` and appends `test` to `testResults.Results`.
It is always called at the end of `RunValidationTest` — whether the test passed, failed, or threw —
to ensure every started test is recorded with a valid duration.

### HandleTestException

`HandleTestException(TestResult test, Context context, string testName, Exception ex)` sets
`test.Outcome` to `Failed`, records the exception message as `test.ErrorMessage`, and calls
`context.WriteError` with a `✗`-prefixed failure line. It is invoked from the `catch` block in
`RunValidationTest` to handle any unhandled exception as a test failure rather than propagating
it as an unhandled crash.

### TemporaryDirectory

`TemporaryDirectory` is a private nested class implementing `IDisposable`. It creates a
uniquely-named subdirectory under `Path.GetTempPath()` using `PathHelpers.SafePathCombine`
and a `sarifmark_validation_`-prefixed GUID name. Creation failures (`IOException`,
`UnauthorizedAccessException`, `ArgumentException`) are wrapped in `InvalidOperationException`.
On `Dispose`, it checks whether the directory still exists using `Directory.Exists` before
deleting it recursively, ignoring `IOException` and `UnauthorizedAccessException` to allow
graceful cleanup even in constrained environments.
