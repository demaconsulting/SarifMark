# SarifRun Record

## Overview

The `SarifRun` record (`SarifRun.cs`) represents the results extracted from a single run within a
SARIF file. It holds the tool metadata and the parsed list of results for that run, and exposes
the `ToMarkdown` method for generating a markdown report for the run.

## Record Design

`SarifRun` is a `record` with an `internal` constructor. External consumers obtain instances only
through `SarifResults.Read`; the record is immutable once constructed.

The `DemaConsulting.SarifMark` project file includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />`,
which grants the test assembly access to the internal constructor. This enables direct unit testing
of the constructor without relaxing the access restriction for all external consumers.

The `SarifRun` type and its sibling types (`SarifResults`, `SarifFinding`) are placed in the root
`DemaConsulting.SarifMark` namespace rather than a `.Sarif` sub-namespace. This is an intentional
design decision to keep the public and internal API surface flat and consistent, avoiding the need
for additional `using` directives in consuming code.

## Properties

| Property      | Type                          | Description                                     |
|---------------|-------------------------------|-------------------------------------------------|
| `ToolName`    | `string`                      | Name of the analysis tool                       |
| `ToolVersion` | `string`                      | Version of the analysis tool                    |
| `FileCount`   | `int`                         | Total number of files analyzed in this run      |
| `Results`     | `IReadOnlyList<SarifFinding>` | Collection of non-suppressed results            |
| `ResultCount` | `int`                         | Total number of results (derived count)         |
| `HasIssues`   | `bool`                        | True if any results are present (derived)       |

These satisfy requirements `SarifMark-SarifRun-Properties` and `SarifMark-SarifRun-HasIssues`.

## ToMarkdown Method

`ToMarkdown(int depth, string? heading = null)` generates a markdown string from the run results:

1. **Depth validation** — throws `ArgumentOutOfRangeException` if `depth` is less than 1 or
   greater than 6. This satisfies `SarifMark-SarifRun-ValidateDepth`.
2. **Header** — calls `AppendHeader` to emit the main heading (using `heading` if provided, or
   `"[ToolName] Analysis"` by default) followed by a `**Tool:**` line with name and version,
   then a `**Files:**` line with the file count.
3. **Issues section** — calls `AppendIssuesSection` to emit the `Issues` sub-heading at
   `depth + 1` (capped at `6` to remain within the valid markdown heading range), the result
   count formatted by `FormatFoundText`, and one line per result formatted by `FormatLocation`.
   Each result line is appended with a trailing two-space markdown hard line break.

This satisfies requirement `SarifMark-SarifRun-ToMarkdown`.

## FormatLocation Method

`FormatLocation(string? uri, int? startLine)` produces the location prefix for each result line,
treating a `uri` that is `null`, empty, or consists only of whitespace as missing:

| `uri`                     | `startLine`  | Output           |
|---------------------------|--------------|------------------|
| null / empty / whitespace | any          | `(no location)`  |
| set                       | null         | `uri`            |
| set                       | set          | `uri(startLine)` |

This satisfies requirement `SarifMark-SarifRun-FormatLocation`.

## FormatFoundText Method

`FormatFoundText(int count, string singularNoun)` produces a grammatically correct summary:

| `count` | Output                        |
|---------|-------------------------------|
| `0`     | Found no {singularNoun}s      |
| `1`     | Found 1 {singularNoun}        |
| `> 1`   | Found {count} {singularNoun}s |

This satisfies requirement `SarifMark-SarifRun-FormatCount`.

## Cross-References

See the SarifResults Record document for `SarifResults.Read`, which constructs `SarifRun`
instances during SARIF file parsing.
See the SarifFinding Record document for the `SarifFinding` record that each run's results contain.
