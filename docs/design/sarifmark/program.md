# Program Class

## Overview

The `Program` class (`Program.cs`) is the top-level entry point for the SarifMark tool. It is a
static internal class that owns the `Main` method, constructs the `Context`, dispatches to the
appropriate subsystem, and handles top-level exception translation.

## Version Property

The static `Version` property reads the assembly's `AssemblyInformationalVersionAttribute` at
runtime. If that attribute is absent, it falls back to the `AssemblyVersion`; if that is also
unavailable it returns `"0.0.0"`. This satisfies requirement `SarifMark-Program-Version`.

## Main Method

`Main` orchestrates the top-level execution sequence:

1. Constructs a `Context` by calling `Context.Create(args)` inside a `using` block.
2. Calls `Run(context)` to execute the selected mode.
3. Returns `context.ExitCode` to the shell.

`ArgumentException` and `InvalidOperationException` are caught, written to `Console.Error`, and
translated to exit code 1. Any other exception prints its message to `Console.Error` and is
then re-thrown so the runtime generates an event-log entry. This satisfies requirements
`SarifMark-Program-Main` and `SarifMark-Program-Main-Exceptions`.

## Run Method

`Run` implements priority-ordered dispatch. Each step is evaluated in sequence and the method
returns after the first matching condition.

| Priority | Condition          | Action                                 |
|----------|--------------------|----------------------------------------|
| 1        | `context.Version`  | Print version string and return        |
| —        | *(always)*         | Call `PrintBanner`                     |
| 2        | `context.Help`     | Call `PrintHelp` and return            |
| 3        | `context.Validate` | Call `Validation.Run` and return       |
| 4        | *(default)*        | Call `ProcessSarifAnalysis` and return |

This satisfies requirement `SarifMark-Program-Run`.

## PrintBanner Method

`PrintBanner` is a private helper called by `Run` immediately after the version check. It writes
two lines to the context output: the tool version string (e.g. `SarifMark version 1.2.3`) and the
copyright notice (`Copyright (c) DEMA Consulting`), followed by a blank line. This satisfies
requirement `SarifMark-Program-Banner`.

## PrintHelp Method

`PrintHelp` is a private helper called when the help flag is set. It writes a complete usage block
to the context output, listing every supported option with its flag syntax and a brief description:

- `-v, --version` — display version information
- `-?, -h, --help` — display the help message
- `--silent` — suppress console output
- `--validate` — run self-validation
- `--results <file>` — write validation results to a `.trx` or `.xml` file
- `--enforce` — return a non-zero exit code when issues are found
- `--log <file>` — write output to a log file
- `--sarif <file>` — SARIF file to process
- `--report <file>` — export analysis results to a markdown file
- `--depth <depth>` — markdown header depth for the report (default: 1)
- `--heading <text>` — custom heading for the report (default: `[ToolName] Analysis`)

This satisfies requirement `SarifMark-Program-Help`.

## ProcessSarifAnalysis Method

`ProcessSarifAnalysis` is the private orchestrator for the primary SARIF analysis mode. Its
execution sequence is:

1. Validates that `context.SarifFile` is non-null and non-whitespace; if not, calls
   `context.WriteError` and returns.
2. Writes the SARIF file path and a `"Reading SARIF file..."` status message, then calls
   `SarifResults.Read(context.SarifFile)` to load the file. Catches `FileNotFoundException`
   and `InvalidOperationException`, routing them through `context.WriteError` and returning
   on failure.
3. Reports the tool name, tool version, and result count via `context.WriteLine`.
4. If `context.Enforce` is set and the result count is greater than zero, calls
   `context.WriteError("Error: Issues found in SARIF file")`.
5. If `context.ReportFile` is set, writes a `"Writing report to..."` progress message, calls
   `sarifResults.ToMarkdown` and writes the result to the specified file with
   `File.WriteAllText`, then writes `"Report generated successfully."` on success. `IOException`,
   `UnauthorizedAccessException`, `ArgumentException`, and `NotSupportedException` are caught and
   routed through `context.WriteError`.

This satisfies requirement `SarifMark-Program-SarifAnalysis`.

## Cross-References

See the Context Class document for the `Context` class and the SarifResults Record document
for the `SarifResults.Read` and `ToMarkdown` methods used in step 2 and step 5 above.
