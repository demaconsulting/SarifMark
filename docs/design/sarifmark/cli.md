## Cli

![Cli Structure](CliView.svg)

The `Cli` subsystem provides the command-line interface layer for SarifMark. It is
responsible for accepting user input from the command line and routing all tool output
to the console and an optional log file.

### Overview

The `Cli` subsystem acts as the primary boundary between the user's shell invocation and
the tool's internal logic. It owns argument parsing, output formatting, and error tracking.
All other subsystems receive a `Context` object from `Cli` to read parsed flags and write
output. The subsystem also answers the built-in `help` command. It contains two units:

- **Context**: argument parsing, output channels, and exit-code management.
- **HelpCommand**: the built-in `help` command, answered from embedded help cards.

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
  `SarifFile`, `ReportFile`, `Heading`, `ResultsFile` (string?); `Depth` (int);
  `ExcludeGlobs` (`IReadOnlyList<string>`, default empty); `ExitCode` (int).
  All values are set during `Create` and are immutable after construction.
- *Constraints*: Properties are read-only after construction; `Depth` must be a positive
  integer supplied after `--depth` (or legacy `--report-depth`).

**HelpCommand.TryRun**: Answers the `help` command from the built-in help cards.

- *Type*: In-process .NET instance method (on the shared `HelpCommand.Default` instance)
- *Role*: Provider
- *Contract*: Returns `false`, writing nothing, when the first argument is not `help`. Otherwise
  writes the answer to the console, returns `true`, and sets the exit code (see *Help Command* in
  the system design).
- *Constraints*: Does not throw for user input; does not use `Context`.

**HelpCommand.Footer**: The sentence that points a reader of the usage text to the help command.

- *Type*: In-process .NET instance property
- *Role*: Provider
- *Contract*: Returns the one-line pointer that HelpCards supplies.
- *Constraints*: Reading it never builds the help catalog.

### Design

`Context` and `HelpCommand` do not call each other. `Program.Main` offers the arguments to
`HelpCommand.TryRun` first; when the help command answers, the invocation ends with its exit
code and no `Context` is created. `HelpCommand` writes its answers to the console itself. It is
the one exception to `Context` owning all output, because the `help` command is answered before
a `Context` exists.

For every other invocation, `Context.Create` is the subsystem entry point:

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
