# Command Line

## Overview

The command-line layer is responsible for parsing command-line arguments, routing program
flow to the appropriate subsystem, and managing all output (console, error, and log file).
It consists of two classes: `Program` (the entry point) and `Context` (the argument and
output container).

## Program Class

The `Program` class (`Program.cs`) is the top-level entry point for the tool. It owns the
`Main` method, constructs the `Context`, dispatches to the appropriate mode, and handles
top-level exception translation.

### Version Property

The static `Version` property reads the assembly's `AssemblyInformationalVersionAttribute`
at runtime and falls back to `AssemblyVersion` or `"0.0.0"` if neither is available. This
satisfies requirement `SarifMark-Cli-Version`.

### Main Method

`Main` creates a `Context` from the command-line arguments, calls `Run`, and returns
`context.ExitCode`. `ArgumentException` and `InvalidOperationException` are caught and
written to `Console.Error`, returning exit code 1. Unexpected exceptions are re-thrown to
generate event-log entries. This satisfies requirements `SarifMark-Cli-Interface` and
`SarifMark-Cli-InvalidArgs`.

### Run Method

`Run` implements priority-ordered dispatch:

| Priority | Condition         | Action                            |
|----------|-------------------|-----------------------------------|
| 1        | `context.Version` | Print version string and return   |
| —        | Print banner      | Always executed after priority 1  |
| 2        | `context.Help`    | Print usage and return            |
| 3        | `context.Validate`| Run self-validation and return    |
| 4        | Default           | Run SARIF analysis processing     |

This dispatch order satisfies requirements `SarifMark-Cli-Version`, `SarifMark-Cli-Help`,
and `SarifMark-Val-Mode`.

### SARIF Analysis Orchestration

`ProcessSarifAnalysis` is a private helper called from `Run`. It validates that `--sarif`
is provided, reads the SARIF file, optionally checks enforcement, and optionally writes a
markdown report. This satisfies requirements `SarifMark-Sarif-Required`,
`SarifMark-Enf-Mode`, and `SarifMark-Rpt-Markdown`.

## Context Class

The `Context` class (`Context.cs`) is a sealed, disposable container for all parsed
command-line state and output routing. It is constructed via the `Create` factory method.

### Properties

| Property      | Type      | Default | Description                                |
|---------------|-----------|---------|--------------------------------------------|
| `Version`     | `bool`    | `false` | `-v` / `--version` flag                    |
| `Help`        | `bool`    | `false` | `-?`, `-h`, `--help` flag                  |
| `Silent`      | `bool`    | `false` | `--silent` flag                            |
| `Validate`    | `bool`    | `false` | `--validate` flag                          |
| `Enforce`     | `bool`    | `false` | `--enforce` flag                           |
| `SarifFile`   | `string?` | `null`  | `--sarif <file>`                           |
| `ReportFile`  | `string?` | `null`  | `--report <file>`                          |
| `ReportDepth` | `int`     | `1`     | `--report-depth <depth>`                   |
| `Heading`     | `string?` | `null`  | `--heading <text>`                         |
| `ResultsFile` | `string?` | `null`  | `--results <file>`                         |
| `ExitCode`    | `int`     | `0`/`1` | 0 for success, 1 if errors reported        |

This satisfies requirements `SarifMark-Cli-Interface`, `SarifMark-Cli-Version`,
`SarifMark-Cli-Help`, `SarifMark-Cli-Silent`, `SarifMark-Cli-Log`, `SarifMark-Cli-Enforce`,
and `SarifMark-Sarif-Required`.

### ArgumentParser

The private `ArgumentParser` class performs the actual token-by-token parsing. Unknown
arguments throw `ArgumentException`, satisfying `SarifMark-Cli-InvalidArgs`.

### WriteLine and WriteError

`WriteLine` writes to `Console.Out` unless `Silent` is set, and also writes to the log
file if one was opened. `WriteError` additionally sets `_hasErrors = true` (making
`ExitCode` return 1) and writes to `Console.Error` in red. This satisfies
`SarifMark-Cli-Silent` and `SarifMark-Enf-ExitCode`.

### Log File

The `OpenLogFile` method opens a `StreamWriter` with `AutoFlush = true`. If opening fails,
an `InvalidOperationException` is thrown with contextual information. The writer is
disposed when `Context` is disposed, satisfying `SarifMark-Cli-Log`.
