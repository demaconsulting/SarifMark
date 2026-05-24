### SarifRun

#### Purpose

`SarifRun` is an immutable record that represents all results extracted from a single run
within a SARIF file. It holds the tool metadata and the parsed list of findings, and
provides the `ToMarkdown` method for generating a per-run markdown report.

#### Data Model

**ToolName**: `string` — Name of the analysis tool sourced from `tool.driver.name`; defaults
to `"Unknown"` if the property is absent.

**ToolVersion**: `string` — Version of the analysis tool; resolved in priority order from
`tool.driver.version`, then `tool.driver.semanticVersion`, then
`tool.driver.dottedQuadFileVersion`; defaults to `"Unknown"` if none of the three fields
is present.

**Results**: `IReadOnlyList<SarifFinding>` — The collection of non-suppressed analysis
findings for this run. Suppressed results (those with a non-empty `suppressions` array) are
excluded during parsing.

**ResultCount**: `int` — Derived count; equals `Results.Count`.

**FileCount**: `int` — Number of files analyzed in this run; derived from the length of the
`artifacts` array in the run element; `0` when the array is absent.

**HasIssues**: `bool` — Derived; `true` when `ResultCount` is greater than `0`.

#### Key Methods

**ToMarkdown**: Generates a markdown string for this run's results.

- *Parameters*: `int depth` — heading depth (1–6); `string? heading` — optional custom
  heading text (default: `null`, which produces `"[ToolName] Analysis"`)
- *Returns*: `string` — UTF-8 markdown content
- *Preconditions*: `depth` must be between 1 and 6 inclusive.
- *Postconditions*: Returns a non-null string containing the heading, tool attribution,
  file count, issue count summary, and one line per finding.

`ToMarkdown` calls `AppendHeader` to emit the main heading and tool/file lines, then calls
`AppendIssuesSection` to emit the issues sub-heading at `depth + 1` (capped at 6),
the count summary from `FormatFoundText`, and one formatted line per result from
`FormatLocation`. Each result line ends with a trailing two-space markdown hard line break.

**FormatLocation**: Formats the location prefix for a single finding line.

- *Parameters*: `string? uri` — file URI; `int? startLine` — start line
- *Returns*: `string` — location string for insertion into the report

When `uri` is null, empty, or whitespace, returns `"(no location)"`. When `uri` is set and
`startLine` is null, returns `uri`. When both are set, returns `"uri(startLine)"`.

**FormatFoundText**: Produces a grammatically correct issues summary.

- *Parameters*: `int count` — number of findings; `string singularNoun` — noun in singular form
- *Returns*: `string` — `"Found no {noun}s"`, `"Found 1 {noun}"`, or `"Found {count} {noun}s"`

#### Error Handling

`ToMarkdown` throws `ArgumentOutOfRangeException` when `depth` is less than 1 or greater
than 6. The internal constructor performs no validation; all field validation is the
responsibility of `SarifResults.Read`, which constructs `SarifRun` instances only after
verifying the JSON structure.

The project file includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />`
to allow the test assembly to construct instances directly for unit testing.

#### Dependencies

- **SarifFinding** — the `Results` collection holds `SarifFinding` instances produced by
  `SarifResults.ParseResults`.

#### Callers

- **SarifResults** — constructs `SarifRun` instances inside `Read` during per-run processing,
  and delegates to `SarifRun.ToMarkdown` from `SarifResults.ToMarkdown`.
