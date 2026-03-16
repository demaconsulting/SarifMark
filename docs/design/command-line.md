# Command Line

## Overview

The command-line layer is responsible for parsing command-line arguments, routing program
flow to the appropriate subsystem, and managing all output (console, error, and log file).
It consists of two classes: `Program` (the entry point) and
`Context` (the argument and output container). This layer satisfies
requirements `SarifMark-Cli-Interface`, `SarifMark-Cli-Version`, `SarifMark-Cli-Help`,
`SarifMark-Cli-Silent`, `SarifMark-Cli-Log`, `SarifMark-Cli-Enforce`, and
`SarifMark-Cli-InvalidArgs`.

## Architecture

The command-line layer uses a two-class design:

- **`Program`** is the static entry point. It owns `Main`, constructs a `Context`, and
  dispatches execution to the appropriate subsystem via priority-ordered logic. See the
  Program Class document for class-level details.

- **`Context`** is a sealed, disposable container for all parsed command-line state and
  output routing. It accumulates argument values during parsing and provides
  `WriteLine`/`WriteError` methods used throughout the tool. See the Context Class document
  for class-level details.

## Dispatch Model

`Program.Run` evaluates conditions in a fixed priority order and returns after the first
matching condition:

| Priority | Condition          | Action                              |
|----------|--------------------|-------------------------------------|
| 1        | `context.Version`  | Print version string and return     |
| —        | *(always)*         | Print banner                        |
| 2        | `context.Help`     | Print usage and return              |
| 3        | `context.Validate` | Run self-validation and return      |
| 4        | *(default)*        | Run SARIF analysis processing       |

This satisfies requirements `SarifMark-Cli-Version`, `SarifMark-Cli-Help`, and
`SarifMark-Validate-Mode`.

## SARIF Analysis Orchestration

When no informational flag is set, `Program.ProcessSarifAnalysis` validates that `--sarif`
is provided, reads and processes the SARIF file, optionally enforces a quality gate, and
optionally writes a markdown report. This satisfies requirements `SarifMark-Sarif-Required`,
`SarifMark-Enforce-Mode`, and `SarifMark-Report-Markdown`.

## Class Details

- **Program class** — entry point, dispatch, and SARIF analysis orchestration
- **Context class** — argument parsing and output routing
