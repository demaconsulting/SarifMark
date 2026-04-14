# SarifResults Record

## Overview

The `SarifResults` record (`SarifResults.cs`) is the primary public type for working with SARIF
file content. It holds the tool metadata and the parsed list of results, and exposes both the
`Read` static method for file loading and the `ToMarkdown` method for report generation.

## Record Design

`SarifResults` is a `record` with an `internal` constructor. External consumers obtain instances
only through `Read`; the record is immutable once constructed.

## Properties

| Property      | Type                         | Description                             |
|---------------|------------------------------|-----------------------------------------|
| `ToolName`    | `string`                     | Name of the analysis tool               |
| `ToolVersion` | `string`                     | Version of the analysis tool            |
| `FileCount`   | `int`                        | Total number of files analyzed          |
| `Results`     | `IReadOnlyList<SarifResult>` | Collection of non-suppressed results    |
| `ResultCount` | `int`                        | Total number of results (derived count) |

These satisfy requirement `SarifMark-SarifResults-Properties`.

## Read Method

The static `Read(string filePath)` method loads and parses a SARIF 2.1.0 file through a
seven-step pipeline:

1. **Path validation** — throws `ArgumentException` if `filePath` is null, empty, or
   whitespace. This satisfies `SarifMark-SarifResults-ValidatePath`.
2. **File existence** — throws `FileNotFoundException` if the file does not exist on disk.
   This satisfies `SarifMark-SarifResults-ValidatePath`.
3. **JSON parsing** — reads the file with `File.ReadAllText` and parses it with
   `JsonDocument.Parse`. A `JsonException` is translated to `InvalidOperationException`.
   This satisfies `SarifMark-SarifResults-ValidateStructure`.
4. **Structure validation** — delegates to `ValidateSarifStructure` to verify the `version`
   and `runs` fields and return the first run element. This satisfies
   `SarifMark-SarifResults-ValidateStructure`.
5. **Tool extraction** — delegates to `ExtractToolInformation` to retrieve `ToolName` and
   `ToolVersion` from `tool.driver`. This satisfies `SarifMark-SarifResults-ExtractTool`.
6. **Result parsing** — delegates to `ParseResults` to iterate all non-suppressed results.
   This satisfies `SarifMark-SarifResults-ParseResults` and `SarifMark-SarifResults-FilterSuppressions`.
7. **File count extraction** — delegates to `ExtractFileCount` to sum the lengths of the
   `artifacts` arrays across all runs. This satisfies `SarifMark-SarifResults-FileCount`.

Together, steps 1–7 form the complete SARIF reading pipeline.

## ValidateSarifStructure Method

`ValidateSarifStructure` verifies that the root JSON element contains:

- A `version` property (any value is accepted; absence throws `InvalidOperationException`).
- A `runs` array that is non-empty (absence or empty array throws `InvalidOperationException`).

It returns the first element of the `runs` array for further processing. This satisfies
requirement `SarifMark-SarifResults-ValidateStructure`.

## ExtractToolInformation Method

`ExtractToolInformation` navigates from the run element to `tool.driver`, throwing
`InvalidOperationException` if either `tool` or `driver` is absent. It reads the `name` property
from `driver`, defaulting to `"Unknown"` if absent, then delegates to `ExtractToolVersion` for the
version string. This satisfies requirement `SarifMark-SarifResults-ExtractTool`.

## ExtractToolVersion Method

`ExtractToolVersion` checks three fields in the `driver` JSON element in priority order:

| Priority | JSON field               |
|----------|--------------------------|
| 1        | `version`                |
| 2        | `semanticVersion`        |
| 3        | `dottedQuadFileVersion`  |

The first field whose value is non-null and non-whitespace is returned. If none of the three fields
yields a value, `"Unknown"` is returned. This satisfies requirement `SarifMark-SarifResults-VersionPriority`.

## ParseResults Method

`ParseResults` iterates the `results` JSON array within the run element. If the array is absent or
not an array, an empty list is returned. For each element, `IsSuppressed` checks whether a
non-empty `suppressions` array is present; suppressed entries are skipped. Each remaining element
is parsed into a `SarifResult` record. This satisfies requirements `SarifMark-SarifResults-ParseResults`
and `SarifMark-SarifResults-FilterSuppressions`.

## ToMarkdown Method

`ToMarkdown(int depth, string? heading = null)` generates a markdown string from the results:

1. **Depth validation** — throws `ArgumentOutOfRangeException` if `depth` is less than 1 or
   greater than 6. This satisfies `SarifMark-SarifResults-ValidateDepth`.
2. **Header** — calls `AppendHeader` to emit the main heading (using `heading` if provided, or
   `"[ToolName] Analysis"` by default) followed by a `**Tool:**` line with name and version,
   then a `**Files:**` line with the file count. The sub-heading level is `min(depth + 1, 6)`.
   This satisfies `SarifMark-SarifResults-FileCount`.
3. **Issues section** — calls `AppendIssuesSection` to emit the `Issues` sub-heading, the
   result count formatted by `FormatFoundText`, and one line per result formatted by
   `FormatLocation`. Each result line is appended with a trailing two-space markdown hard
   line break (`  `) before the newline, satisfying requirement `SarifMark-Report-LineBreaks`.

This satisfies requirement `SarifMark-SarifResults-ToMarkdown`.

## FormatLocation Method

`FormatLocation(string? uri, int? startLine)` produces the location prefix for each result line,
treating a `uri` that is `null`, empty, or consists only of whitespace as missing:

| `uri`                     | `startLine`  | Output           |
|---------------------------|--------------|------------------|
| null / empty / whitespace | any          | `(no location)`  |
| set                       | null         | `uri`            |
| set                       | set          | `uri(startLine)` |

This satisfies requirement `SarifMark-SarifResults-FormatLocation`.

## FormatFoundText Method

`FormatFoundText(int count, string singularNoun)` produces a grammatically correct summary:

| `count` | Output                        |
|---------|-------------------------------|
| `0`     | Found no {singularNoun}s      |
| `1`     | Found 1 {singularNoun}        |
| `> 1`   | Found {count} {singularNoun}s |

This satisfies requirement `SarifMark-SarifResults-FormatCount`.

## ExtractFileCount Method

`ExtractFileCount(JsonElement root)` sums the lengths of the `artifacts` arrays across every element
in the `runs` array of the SARIF document:

- If the `runs` property is absent or is not an array, `0` is returned.
- For each run element, if an `artifacts` property is present and is an array, its length is added
  to the running total.
- Run elements that lack an `artifacts` property contribute `0` to the total.

The result is stored in the `FileCount` property and emitted as `**Files:** {FileCount}` in the
`AppendHeader` output. This satisfies requirement `SarifMark-SarifResults-FileCount`.

## Cross-References

See the SarifResult Record document for the `SarifResult` record that `ParseResults` produces.
See the Program Class document for how `Read` and `ToMarkdown` are called from
`ProcessSarifAnalysis`.
