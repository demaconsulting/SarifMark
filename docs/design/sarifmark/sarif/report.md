### Report

#### Purpose

The `Report` unit represents the markdown report output format produced by SarifMark from a parsed SARIF file.
It is not a discrete class but rather the design contract that governs the structure and content of the generated
markdown: heading depth selection, per-run sections, result counts, location formatting, file count header, line
breaks, and custom heading support. The report output is the primary deliverable of SarifMark and is the artifact
consumed by documentation pipelines and human reviewers.

#### Data Model

A generated report is a UTF-8 string composed of one or more run sections. Each section contains:

**Heading line**: A markdown heading at the requested depth using `#` × `depth` characters, followed by either the
custom heading text or the default `"[ToolName] Analysis"` label. For multi-run SARIF files each heading is suffixed
with `" (#N)"` where `N` is the 1-based run index.

**Tool attribution line**: A line identifying the tool name and tool version in the form
`"**Tool:** ToolName ToolVersion"` (bold label, tool name and version space-separated).

**File count line**: A line recording the number of files analyzed in the form `"**Files:** N"` (bold label),
where `N` is the integer count of entries in the SARIF `artifacts` array for the run. `N` is `0` when the array
is absent.

**Issues sub-heading**: A markdown heading at `depth + 1` (capped at 6) titled `"Issues"`.

**Issues count summary**: A grammatically correct sentence of the form `"Found no issues"`,
`"Found 1 issue"`, or `"Found N issues"` depending on the number of non-suppressed results.

**Result lines**: One line per non-suppressed finding, formatted as
`"location: severity [ruleId] message  "` (two trailing spaces as a markdown hard line break).
The location prefix is produced by `FormatLocation`:

- `"(no location)"` — when the URI is null, empty, or whitespace
- `"uri"` — when the URI is set but `startLine` is null
- `"uri(startLine)"` — when both URI and start line are set

Suppressed results (those with a non-empty `suppressions` array in the SARIF JSON) are excluded and do not
contribute to the count or result lines.

#### Key Methods

**SarifResults.ToMarkdown**: Top-level entry point for report generation.

- *Parameters*: `int depth` — heading depth (1–6); `string? heading` — optional custom heading
- *Returns*: `string` — the complete UTF-8 markdown report
- *Preconditions*: `depth` must be between 1 and 6 inclusive.
- *Report contract (single run)*: Delegates directly to `Runs[0].ToMarkdown(depth, heading)`; the output is
  identical to the single-run format described above.
- *Report contract (multi-run)*: Iterates all runs; for each run emits an indexed heading
  (`"[ToolName] Analysis (#N)"` or `"{customHeading} (#N)"`) and appends the per-run body. Run sections are
  separated by a blank line.

**SarifRun.ToMarkdown**: Per-run report generator.

- *Parameters*: `int depth` — heading depth (1–6); `string? heading` — optional custom heading
- *Returns*: `string` — the per-run markdown section
- *Report contract*: Calls `AppendHeader` to emit the heading, tool attribution, and file count lines; then calls
  `AppendIssuesSection` to emit the issues sub-heading, the count summary from `FormatFoundText`, and one
  `FormatLocation`-prefixed line per result, each terminated with two trailing spaces.

**SarifRun.FormatLocation**: Location prefix formatter consumed by each result line.

- *Parameters*: `string? uri`; `int? startLine`
- *Returns*: `string` — the location prefix string
- *Report contract*: Returns `"(no location)"` when `uri` is null/empty/whitespace; `uri` when `startLine` is
  null; `"uri(startLine)"` when both are present.

**SarifRun.FormatFoundText**: Issue count summary formatter.

- *Parameters*: `int count`; `string singularNoun`
- *Returns*: `string` — grammatically correct summary
- *Report contract*: Returns `"Found no {noun}s"` for zero, `"Found 1 {noun}"` for one, or
  `"Found {count} {noun}s"` for any other count.

**SarifResults.ValidateDepth** (via `SarifResults.ToMarkdown` guard): Enforces the depth precondition before any
output is produced, throwing `ArgumentOutOfRangeException` when `depth` is outside `[1, 6]`.

#### Error Handling

`SarifResults.ToMarkdown` and `SarifRun.ToMarkdown` throw `ArgumentOutOfRangeException` with the message
`"Depth must be between 1 and 6"` when `depth` is less than 1 or greater than 6. This validation is applied before
any output is written, ensuring no partial report is emitted on invalid input.

No other error conditions arise at report generation time; all structural validation (missing `version`, empty
`runs`, missing `tool`/`driver`) occurs during `SarifResults.Read` and is reported via `InvalidOperationException`
before report generation is attempted.

#### Dependencies

- **SarifResults** — owns the `ToMarkdown` entry point and the multi-run aggregation logic.
- **SarifRun** — owns per-run report formatting via `ToMarkdown`, `FormatLocation`, and `FormatFoundText`.
- **SarifFinding** — each result line is derived from a `SarifFinding` instance held in `SarifRun.Results`.

#### Callers

- **Program** — calls `sarifResults.ToMarkdown(depth, heading)` from `ProcessSarifAnalysis` when `--report` is
  specified and writes the returned string to disk with `File.WriteAllText`.
