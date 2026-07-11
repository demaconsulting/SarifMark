### SarifResults

![Sarif Structure](SarifView.svg)

#### Purpose

`SarifResults` is the primary record for working with SARIF file content. It holds the
collection of parsed runs and exposes the static `Read` factory method for loading a
SARIF 2.1.0 file, and the `ToMarkdown` method for generating the complete markdown report.

#### Data Model

**Runs**: `IReadOnlyList<SarifRun>` — The collection of all parsed runs from the SARIF
file. At least one element is guaranteed after a successful `Read` call (an empty `runs`
array in the SARIF file causes `InvalidOperationException`).

**HasIssues**: `bool` — Derived; `true` if any run in `Runs` has `HasIssues` equal to
`true`. Used by `Program.ProcessSarifAnalysis` to evaluate the `--enforce` flag.

#### Key Methods

**Read**: Parses a SARIF 2.1.0 file and returns an immutable `SarifResults` record.

- *Parameters*: `string filePath` — path to the SARIF file
- *Returns*: `SarifResults` — fully populated record graph
- *Preconditions*: `filePath` is non-null, non-empty, and non-whitespace; the file exists.
- *Postconditions*: Returns a `SarifResults` containing at least one `SarifRun`, each
  containing zero or more non-suppressed `SarifFinding` records.

`Read` validates the path, reads the file with `File.ReadAllText`, parses it with
`JsonDocument.Parse` (translating `JsonException` to `InvalidOperationException`), calls
`ValidateSarifStructure` to verify `version` and `runs`, then processes each run element
through `ExtractToolInformation`, `ParseResults`, and `ExtractFileCount`.

**ToMarkdown**: Generates a markdown string from all loaded results.

- *Parameters*: `int depth` — heading depth (1–6); `string? heading` — optional custom heading
- *Returns*: `string` — UTF-8 markdown content
- *Preconditions*: `depth` must be between 1 and 6 inclusive.
- *Postconditions*: Returns a non-null string; for single-run files identical to
  `Runs[0].ToMarkdown`; for multi-run files, concatenates indexed run reports.

For single-run files, delegates directly to `Runs[0].ToMarkdown(depth, heading)`. For
multi-run files, emits indexed headings (`"[ToolName] Analysis (#1)"`) and concatenates
all run reports. When a custom heading is provided for a multi-run file, the heading format
becomes `"{customHeading} (#N)"` — consistent with the default multi-run format `"[ToolName] Analysis (#N)"`.
This ensures the custom heading prefix is preserved while the run index suffix still distinguishes
each run.

**ValidateSarifStructure**: Verifies the root JSON element and returns the runs array.

- Throws `InvalidOperationException` if `version` is absent.
- Throws `InvalidOperationException` if `runs` is absent or empty.
- Returns the `runs` JSON array element for iteration.

**ExtractToolInformation** and **ExtractToolVersion**: Extract tool name and version.

`ExtractToolInformation` navigates from the run element to `tool.driver`, throwing
`InvalidOperationException` if either `tool` or `driver` is absent. `ExtractToolVersion`
checks `version`, `semanticVersion`, and `dottedQuadFileVersion` in priority order,
returning the first non-null non-whitespace value or `"Unknown"`.

**ParseResults**: Iterates the `results` array, filters suppressed entries, and
constructs `SarifFinding` records.

If the `results` array is absent or not an array, returns an empty list. For each
element, `IsSuppressed` checks for a non-empty `suppressions` array; suppressed entries
are skipped.

**ExtractFileCount**: Returns the length of the `artifacts` array in the run element; `0`
when the array is absent or not an array.

#### Error Handling

`Read` throws `ArgumentException` when `filePath` is null, empty, or whitespace.
`FileNotFoundException` is thrown when the file does not exist. `InvalidOperationException`
is thrown for JSON parse failures (translated from `JsonException`) and for structural
violations (missing `version`, missing or empty `runs`, missing `tool` or `driver`).

`ToMarkdown` throws `ArgumentOutOfRangeException` when `depth` is outside `[1, 6]`.

The project file includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />`
to allow the test assembly to construct instances directly for unit testing.

#### Dependencies

- **SarifRun** — each element of `Runs` is a `SarifRun` instance produced during `Read`.
- **SarifFinding** — each run's results contain `SarifFinding` instances.
- **System.Text.Json** — `JsonDocument.Parse` is used for all JSON parsing.

#### Callers

- **Program** — calls `SarifResults.Read(context.SarifFile)` and
  `sarifResults.ToMarkdown(depth, heading)` from `ProcessSarifAnalysis`.
- **Validation** — calls `SarifResults.Read` indirectly via `Program.Run` during
  self-validation tests.
