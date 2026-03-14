# Self-Validation

## Overview

The self-validation layer provides built-in verification of the tool's core functionality.
It is invoked when the `--validate` flag is passed and can write results to a TRX or
JUnit XML file when `--results` is also provided. This satisfies requirements
`SarifMark-Val-Mode` and `SarifMark-Val-ResultFiles`.

## Validation Class

The `Validation` class (`Validation.cs`) exposes a single public method, `Run`, and
organizes all test execution internally.

### Run Method

`Run` orchestrates the self-validation sequence:

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
3. Constructs a `Context` with `--silent`, `--log <file>`, `--sarif <file>`, and
   `--report <file>`.
4. Calls `Program.Run` and verifies exit code is 0.
5. Checks the report file contains `"MockTool Analysis"` and `"Found 2 issues"`.

The test name is `SarifMark_MarkdownReportGeneration`, satisfying `SarifMark-Rpt-Markdown`.

### RunEnforcementTest

`RunEnforcementTest` verifies enforcement mode:

1. Creates a `TemporaryDirectory`.
2. Writes the mock SARIF file.
3. Constructs a `Context` with `--silent`, `--log <file>`, `--sarif <file>`, and
   `--enforce`.
4. Calls `Program.Run` and verifies exit code is non-zero.
5. Checks the log contains `"Error: Issues found in SARIF file"`.

The test name is `SarifMark_Enforcement`, satisfying `SarifMark-Enf-Mode` and
`SarifMark-Enf-ExitCode`.

### WriteResultsFile

`WriteResultsFile` inspects the file extension of `context.ResultsFile`:

- `.trx` → `TrxSerializer.Serialize`. This satisfies `SarifMark-Val-TrxFormat`.
- `.xml` → `JUnitSerializer.Serialize`. This satisfies `SarifMark-Val-JUnitFormat`.
- Other → writes an error via `context.WriteError`.

The serialized content is written with `File.WriteAllText`.

### TemporaryDirectory

`TemporaryDirectory` is a private nested class implementing `IDisposable`. It creates a
uniquely-named subdirectory under `Path.GetTempPath()` using `PathHelpers.SafePathCombine`
and a `Guid`-based name. On `Dispose`, it deletes the directory recursively, ignoring
`IOException` and `UnauthorizedAccessException` to allow graceful cleanup even in
constrained environments.
