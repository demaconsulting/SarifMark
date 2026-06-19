### Context

#### Purpose

`Context` is a sealed, disposable container for all parsed command-line state and output
routing. It is the single source of truth for which mode the tool runs in, which files it
reads and writes, and whether any errors have been reported. All other units receive a
`Context` instance to read parsed flags and write output.

#### Data Model

**Version**: `bool` — True when `-v` or `--version` is present; default `false`. Causes
`Program.Run` to print the version string and exit.

**Help**: `bool` — True when `-?`, `-h`, or `--help` is present; default `false`. Causes
`Program.Run` to print usage and exit.

**Silent**: `bool` — True when `--silent` is present; default `false`. Suppresses all output
to `Console.Out` and `Console.Error` while still writing to the log if one is open.

**Validate**: `bool` — True when `--validate` is present; default `false`. Causes
`Program.Run` to invoke `Validation.Run` instead of the analysis path.

**Enforce**: `bool` — True when `--enforce` is present; default `false`. Causes
`ProcessSarifAnalysis` to set the error exit code if any SARIF findings are present.

**SarifFile**: `string?` — Path to the SARIF input file supplied via `--sarif`; `null` when
not provided.

**ReportFile**: `string?` — Path to the markdown report output file supplied via `--report`;
`null` when not provided.

**Depth**: `int` — Heading depth for the generated report supplied via `--depth` or the
legacy alias `--report-depth`; must be a positive integer; default `1`.

**Heading**: `string?` — Custom heading text supplied via `--heading`; `null` when not provided.
When null, the report heading defaults to `"[ToolName] Analysis"`.

**ResultsFile**: `string?` — Path for self-validation results supplied via `--results` or the
legacy alias `--result`; `null` when not provided.

**ExitCode**: `int` — Returns `0` until `WriteError` is called; returns `1` thereafter.
Derived from the internal `_hasErrors` flag.

#### Key Methods

**Create**: Factory method; the only way to construct a `Context`.

- *Parameters*: `string[] args` — command-line arguments from the runtime
- *Returns*: `Context` — fully initialized instance
- *Preconditions*: `args` is not null.
- *Postconditions*: All properties are set from `args`; if `--log` was specified the
  log `StreamWriter` is open and `AutoFlush` is `true`.

`Create` validates that `args` is non-null, constructs an `ArgumentParser`, calls
`ParseArguments`, copies parsed values into the new `Context` via `init`-only setters,
and calls `OpenLogFile` when a log path was specified.

**WriteLine**: Writes a message to the console and optional log.

- *Parameters*: `string message` — text to write
- *Returns*: `void`
- *Preconditions*: None.
- *Postconditions*: `message` has been written to `Console.Out` (unless `Silent`) and to
  the log `StreamWriter` (if open).

**WriteError**: Writes an error message and sets the error exit code.

- *Parameters*: `string message` — error text to write
- *Returns*: `void`
- *Preconditions*: None.
- *Postconditions*: `ExitCode` is `1`; `message` has been written to `Console.Error`
  in red (unless `Silent`) and to the log `StreamWriter` (if open).

**Dispose**: Releases the log file handle.

- *Parameters*: None.
- *Returns*: `void`
- *Preconditions*: None.
- *Postconditions*: Log `StreamWriter` is disposed and set to `null`; any buffered log
  content is flushed to disk.

#### Error Handling

`Create` throws `ArgumentException` for unrecognized tokens and for malformed value-bearing
flags (e.g., `--depth` not followed by a positive integer, or a string flag at end of args).
It throws `InvalidOperationException` if the log file cannot be opened. `ArgumentNullException`
is thrown immediately if `args` is null. These exceptions propagate to `Program.Main`, which
translates them to exit code 1.

The private `ArgumentParser` inner class throws `ArgumentException` on any unrecognized
token. Value-bearing string flags throw `ArgumentException` when they appear as the last
token without a following value.

Value-bearing flags accept the immediately following token as their value without inspecting it further. A token
that begins with `--` (such as `--help`) is treated as a valid value, not as a flag. This is intentional: such
tokens are valid filenames on all supported platforms, and rejecting them would prevent users from writing output
to files whose names happen to match flag names.

#### Dependencies

- **.NET base class library** — `Console`, `StreamWriter`, `Path`, `ArgumentException`,
  `ArgumentNullException`, `IDisposable`.

`Context` itself does not call `PathHelpers` directly. `PathHelpers.SafePathCombine` is used
by `ContextTests` (in the test project) to construct safe file paths for test fixtures, but
that usage is confined to the test boundary and is not part of the `Context` runtime contract.

#### Callers

- **Program** — constructs `Context.Create(args)` inside a `using` block and passes the
  instance to `Validation.Run` and `ProcessSarifAnalysis`.
- **Validation** — constructs additional `Context` instances internally during test execution
  (using `Context.Create` with test-specific argument arrays).
