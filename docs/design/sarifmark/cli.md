## Cli

![Cli Structure](CliView.svg)

The `Cli` subsystem provides the command-line interface layer for SarifMark. It is
responsible for accepting user input from the command line and routing all tool output
to the console and an optional log file.

### Overview

The `Cli` subsystem acts as the primary boundary between the user's shell invocation and
the tool's internal logic. It owns argument parsing, output formatting, and error tracking.
All other subsystems receive a `Context` object from `Cli` to read parsed flags and write
output. The subsystem contains a single unit:

- **Context**: argument parsing, output channels, and exit-code management.

### Interfaces

**Context.Create**: Factory method that constructs a fully initialized `Context` from
command-line arguments.

- *Type*: In-process .NET static method
- *Role*: Provider
- *Contract*: Accepts `string[] args`; returns a `Context` with all flags and parameters
  parsed, and the log file opened if `--log` was specified.
- *Constraints*: `args` must not be null. Throws `ArgumentException` for unrecognized or
  malformed arguments; throws `InvalidOperationException` if the log file cannot be opened.

**Context.WriteLine**: Writes a message to the console and optional log file.

- *Type*: In-process .NET instance method
- *Role*: Provider
- *Contract*: Accepts `string message`; writes to `Console.Out` unless `Silent` is `true`;
  always writes to the log `StreamWriter` if one is open.
- *Constraints*: None.

**Context.WriteError**: Writes an error message and sets the non-zero exit code.

- *Type*: In-process .NET instance method
- *Role*: Provider
- *Contract*: Accepts `string message`; unconditionally sets the internal error flag (causing
  `ExitCode` to return `1`); writes to `Console.Error` in red unless `Silent` is `true`;
  always writes to the log if open.
- *Constraints*: None.

**Context properties**: The read-only parsed state exposed to the application layer.

- *Type*: In-process .NET instance properties
- *Role*: Provider
- *Contract*: Exposes `Version`, `Help`, `Silent`, `Validate`, `Enforce` (bool);
  `SarifFile`, `ReportFile`, `Heading`, `ResultsFile` (string?); `Depth` (int); `ExitCode` (int).
  All values are set during `Create` and are immutable after construction.
- *Constraints*: Properties are read-only after construction; `Depth` must be a positive
  integer supplied after `--depth` (or legacy `--report-depth`).

### Design

The `Cli` subsystem contains a single unit (`Context`) so there is no inter-unit data flow
to describe. `Context.Create` is the subsystem entry point:

1. `Create` receives `string[] args` and delegates to the private `ArgumentParser` inner class.
2. `ArgumentParser.ParseArguments` iterates tokens in order; value-bearing flags consume the
   following token.
3. On any unrecognized token, `ParseArgument` throws `ArgumentException`.
4. The fully parsed state is transferred into a new `Context` instance through `init`-only
   property setters.
5. If `--log` was specified, `OpenLogFile` is called to open the log `StreamWriter` with
   `AutoFlush = true`.
6. The `Context` instance is returned to the caller and used for all subsequent output.

All subsystems that need to produce output hold a reference to the same `Context` instance
created in step 6; no subsystem constructs its own `Context`.
