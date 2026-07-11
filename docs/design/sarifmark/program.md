## Program

![SarifMark Structure](SarifMarkView.svg)

### Purpose

`Program` is the top-level entry point for the SarifMark tool. It is a static internal
class that owns the `Main` method, constructs the `Context`, dispatches to the appropriate
subsystem based on the parsed flags, and handles top-level exception translation.

### Data Model

**Version**: `string` — The assembly informational version string; derived at runtime from
`AssemblyInformationalVersionAttribute`, falling back to `AssemblyVersion`, then to `"0.0.0"`.
This property is read-only and is recomputed on every access (no caching).

### Key Methods

**Main**: Top-level entry point invoked by the .NET runtime.

- *Parameters*: `string[] args` — command-line arguments supplied by the shell
- *Returns*: `int` — exit code; 0 for success, 1 for any error
- *Preconditions*: None.
- *Postconditions*: Returns 0 when execution completes without error; returns 1 when
  `ArgumentException` or `InvalidOperationException` is caught; rethrows any other
  exception after printing its message to `Console.Error` with an `"Unexpected error:"` prefix.

`Main` constructs a `Context` inside a `using` block (ensuring disposal), calls `Run`,
and returns `context.ExitCode`.

**Run**: Selects and executes the correct execution mode based on the parsed context.

- *Parameters*: `Context context` — fully initialized context
- *Returns*: `void`
- *Preconditions*: `context` is not null.
- *Postconditions*: Exactly one execution path has been invoked (version, help, validate,
  or analysis); `context.ExitCode` reflects the outcome.

`Run` evaluates conditions in priority order: version flag → print version and return;
then always print the banner; help flag → print help and return; validate flag → call
`Validation.Run` and return; default → call `ProcessSarifAnalysis`.

**ProcessSarifAnalysis**: Orchestrates the primary SARIF analysis execution path.

- *Parameters*: `Context context` — context with analysis mode active
- *Returns*: `void`
- *Preconditions*: `context.Version`, `context.Help`, and `context.Validate` are all false.
- *Postconditions*: If `context.SarifFile` is a valid path to an existing SARIF file, the
  results have been reported to the context output; if `context.ReportFile` is set, the
  markdown report has been written to disk.

The method validates that `context.SarifFile` is non-null and non-whitespace; calls
`SarifResults.Read`; checks `context.Enforce` against `sarifResults.HasIssues`; and
conditionally writes the markdown report using `sarifResults.ToMarkdown` and
`File.WriteAllText`.

### Error Handling

`Main` catches `ArgumentException` and `InvalidOperationException`, writes the message to
`Console.Error`, and returns exit code 1. Any other exception is printed to `Console.Error`
with an `"Unexpected error:"` prefix and rethrown, allowing the .NET runtime to produce a
stack trace and terminate with a non-zero exit code.

`ProcessSarifAnalysis` catches `FileNotFoundException` and `InvalidOperationException` from
`SarifResults.Read`, routing them through `context.WriteError`. File-write failures
(`IOException`, `UnauthorizedAccessException`, `ArgumentException`, `NotSupportedException`)
are similarly caught and routed through `context.WriteError`.

### Dependencies

- **Context** — receives parsed arguments and provides the output channel for all tool output.
- **SarifResults** — reads SARIF files and generates markdown in analysis mode.
- **Validation** — runs self-validation tests in validate mode.

### Callers

N/A - entry point, called by the .NET host environment via `Main`. `Program.Run` is also
called internally by `Validation` when executing the self-validation tests.
