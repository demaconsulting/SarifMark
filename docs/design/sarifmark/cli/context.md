# Context Class

## Overview

The `Context` class (`Context.cs`) is a sealed, disposable container for all parsed command-line
state and output routing. It is the single source of truth for which mode the tool runs in, which
files it reads and writes, and whether any errors have been reported.

## Class Design

`Context` is declared `sealed` and implements `IDisposable`. The constructor is private; all
instances are created through the `Create` static factory method. This ensures every `Context` is
fully initialized before use. This satisfies requirement `SarifMark-Context-Create`.

## Create Factory Method

`Create(string[] args)` is the sole public entry point for constructing a `Context`. It:

1. Validates that `args` is non-null.
2. Constructs an `ArgumentParser` and calls `ParseArguments(args)`.
3. Copies all parsed values into a new `Context` using `init`-only property setters.
4. If a log file was specified, calls `OpenLogFile`.
5. Returns the fully configured instance.

This satisfies requirements `SarifMark-Context-Create` through `SarifMark-Context-LogParam`.

## Properties

| Property      | Type      | Default | CLI flag(s)              | Description                                 |
|---------------|-----------|---------|--------------------------|---------------------------------------------|
| `Version`     | `bool`    | `false` | `-v`, `--version`        | Version query flag                          |
| `Help`        | `bool`    | `false` | `-?`, `-h`, `--help`     | Help flag                                   |
| `Silent`      | `bool`    | `false` | `--silent`               | Suppress console output                     |
| `Validate`    | `bool`    | `false` | `--validate`             | Self-validation mode flag                   |
| `Enforce`     | `bool`    | `false` | `--enforce`              | Enforcement mode flag                       |
| `SarifFile`   | `string?` | `null`  | `--sarif <file>`         | Path to the SARIF file                      |
| `ReportFile`  | `string?` | `null`  | `--report <file>`        | Path to the markdown report output file     |
| `ReportDepth` | `int`     | `1`     | `--report-depth <depth>` | Markdown heading depth for the report       |
| `Heading`     | `string?` | `null`  | `--heading <text>`       | Custom heading text for the report          |
| `ResultsFile` | `string?` | `null`  | `--results <file>`, `--result <file>` | Path for the self-validation results file (`--result` is a legacy alias for `--results`) |
| `ExitCode`    | `int`     | `0`/`1` | *(derived)*              | 0 until `WriteError` is called, then 1      |

These properties satisfy requirements `SarifMark-Context-VersionFlag`, `SarifMark-Context-HelpFlag`,
`SarifMark-Context-SilentFlag`, `SarifMark-Context-ValidateFlag`, `SarifMark-Context-EnforceFlag`,
`SarifMark-Context-SarifParam`, `SarifMark-Context-ReportParam`, `SarifMark-Context-ReportDepthParam`,
`SarifMark-Context-HeadingParam`, `SarifMark-Context-ResultsParam`, `SarifMark-Context-ResultLegacyAlias`, and `SarifMark-Context-ExitCode`.

## ArgumentParser Inner Class

`ArgumentParser` is a private, sealed nested class responsible for token-by-token command-line
parsing. Its `ParseArguments(string[] args)` method iterates through tokens in order and delegates
each to `ParseArgument`. Value-bearing flags (e.g. `--sarif`, `--report-depth`) consume the
following token as their argument value.

Any unrecognized token causes `ParseArgument` to throw `ArgumentException` with a message
identifying the unsupported argument. This satisfies requirement `SarifMark-Context-UnknownArgs`.

`--report-depth` requires a positive integer value; non-integer or non-positive values also throw
`ArgumentException`. This satisfies requirement `SarifMark-Context-ReportDepthParam`.

## WriteLine Method

`WriteLine(string message)` writes to `Console.Out` unless `Silent` is `true`. If a log file is
open, the message is also written to the log `StreamWriter` regardless of the `Silent` flag. This
satisfies requirement `SarifMark-Context-WriteLine`.

## WriteError Method

`WriteError(string message)` unconditionally sets the private `_hasErrors` flag to `true`, which
causes `ExitCode` to return `1`. Unless `Silent` is `true`, it writes the message to
`Console.Error` with the console foreground color temporarily set to red. If a log file is open,
the message is also written there. This satisfies requirements `SarifMark-Context-WriteError` and
`SarifMark-Context-ExitCode`.

## OpenLogFile Method

`OpenLogFile(string logFile)` opens a `StreamWriter` over the specified path with
`AutoFlush = true`, ensuring log entries are flushed to disk immediately even if the process
terminates unexpectedly. If the file cannot be opened for any reason, the underlying exception is
caught and wrapped in an `InvalidOperationException` with a message that identifies the failing
file path. This satisfies requirement `SarifMark-Context-LogParam`.

## Dispose Method

`Dispose()` disposes the log `StreamWriter` if one was opened and sets the reference to `null`.
This ensures file handles are released and any remaining buffered content is flushed on disposal.
This satisfies requirement `SarifMark-Context-Dispose`.

## Cross-References

See the Program Class document for how `Context` is constructed and consumed by `Program.Main`
and `Program.Run`.
