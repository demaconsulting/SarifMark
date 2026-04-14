# Cli Subsystem

The `Cli` subsystem provides the command-line interface for SarifMark.
It is responsible for accepting user input from the command line and routing output to
the console and an optional log file.

## Overview

The `Cli` subsystem acts as the primary boundary between the user's shell invocation and
the tool's internal logic. It owns argument parsing, output formatting, and error tracking.
All other subsystems receive a `Context` object from the `Cli` subsystem to read parsed
flags and write output.

## Units

The `Cli` subsystem contains the following software unit:

| Unit      | File             | Responsibility                                    |
|-----------|------------------|---------------------------------------------------|
| `Context` | `Cli/Context.cs` | Argument parsing, output channels, and exit code. |

## Interfaces

The `Cli` subsystem exposes the following interface to the rest of the tool:

**Methods:**

| Interface            | Direction | Description                                                   |
|----------------------|-----------|---------------------------------------------------------------|
| `Context.Create`     | Outbound  | Factory method constructing a `Context` from `string[] args`. |
| `Context.WriteLine`  | Outbound  | Writes a message to console and optional log file.            |
| `Context.WriteError` | Outbound  | Writes an error to stderr and sets the error exit code.       |

**Parsed flags and parameters** (set by `Create`, read by the application layer):

| Property      | Type      | CLI flag(s)              | Description                                   |
|---------------|-----------|--------------------------|-----------------------------------------------|
| `Version`     | `bool`    | `-v`, `--version`        | Version query flag                            |
| `Help`        | `bool`    | `-?`, `-h`, `--help`     | Help flag                                     |
| `Silent`      | `bool`    | `--silent`               | Suppress console output flag                  |
| `Validate`    | `bool`    | `--validate`             | Self-validation mode flag                     |
| `Enforce`     | `bool`    | `--enforce`              | Enforcement mode flag                         |
| `SarifFile`   | `string?` | `--sarif <file>`         | Path to the SARIF input file                  |
| `ReportFile`  | `string?` | `--report <file>`        | Path for the markdown report output file      |
| `ReportDepth` | `int`     | `--depth <depth>`        | Markdown heading depth for the report         |
| `Heading`     | `string?` | `--heading <text>`       | Custom heading text for the report            |
| `ResultsFile` | `string?` | `--results <file>`       | Path for the self-validation results file     |
| `ExitCode`    | `int`     | *(derived)*              | 0 until `WriteError` is called, then 1        |

## Interactions

The `Cli` subsystem has no dependencies on other tool subsystems. It uses only .NET base
class library types. The `Program` unit at system level creates the `Context` and passes it
to all subsystems that need to produce output.

## Class Details

- **Context class** — argument parsing and output routing
