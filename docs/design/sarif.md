# SARIF and Reporting

## Overview

The SARIF and reporting layer is responsible for reading SARIF 2.1.0 files and generating
markdown reports from the extracted results. It consists of two records: `SarifResult`
(a single result entry) and `SarifResults` (the full results collection with reading and
reporting logic).

## SarifResult Record

The `SarifResult` record (`SarifResult.cs`) is an immutable value type representing a
single static analysis finding extracted from a SARIF file.

### SarifResult Properties

| Property    | Type      | Description                                     |
|-------------|-----------|-------------------------------------------------|
| `RuleId`    | `string`  | The rule identifier for the result              |
| `Level`     | `string`  | Severity level (e.g., `"error"`, `"warning"`)   |
| `Message`   | `string`  | Descriptive message for the result              |
| `Uri`       | `string?` | File URI where the result was found             |
| `StartLine` | `int?`    | Starting line number in the file                |

`SarifResult` is constructed internally and is not directly instantiated by consumers of
the public API. It satisfies requirements `SarifMark-Sarif-Results` and
`SarifMark-Sarif-Locations`.

## SarifResults Record

The `SarifResults` record (`SarifResults.cs`) is the primary public type for working with
SARIF file content. It holds tool metadata and the list of results, and provides both file
reading and markdown report generation.

### SarifResults Properties

| Property      | Type                        | Description                          |
|---------------|-----------------------------|--------------------------------------|
| `ToolName`    | `string`                    | Name of the analysis tool            |
| `ToolVersion` | `string`                    | Version of the analysis tool         |
| `Results`     | `IReadOnlyList<SarifResult>`| Collection of extracted results      |
| `ResultCount` | `int`                       | Total number of results              |

This satisfies requirements `SarifMark-Sarif-ToolInfo` and `SarifMark-Sarif-Results`.

### Read Method

The static `Read` method loads and parses a SARIF file:

1. Validates the file path is non-null and non-empty, throwing `ArgumentException` for
   invalid paths. This satisfies `SarifMark-Sarif-FilePaths`.
2. Checks that the file exists, throwing `FileNotFoundException` if not. This satisfies
   `SarifMark-Sarif-FilePaths`.
3. Reads the JSON content and parses it with `JsonDocument`. A `JsonException` is
   translated to `InvalidOperationException`. This satisfies `SarifMark-Sarif-Validation`.
4. Delegates to `ValidateSarifStructure` to verify the `version` and `runs` fields, then
   returns the first run element. This satisfies `SarifMark-Sarif-Validation`.
5. Delegates to `ExtractToolInformation` to retrieve `name` and `version` from the
   `tool.driver` object. This satisfies `SarifMark-Sarif-ToolInfo`.
6. Delegates to `ParseResults` to iterate and parse all non-suppressed results. This
   satisfies `SarifMark-Sarif-Results` and `SarifMark-Sarif-Reading`.

### Version Extraction

`ExtractToolVersion` checks three fields in priority order: `version`,
`semanticVersion`, and `dottedQuadFileVersion`. The first non-empty value is used; if
none is found, `"Unknown"` is returned. This satisfies `SarifMark-Sarif-ToolInfo`.

### Suppression Filtering

`ParseResults` skips any result element that contains a non-empty `suppressions` array.
This ensures suppressed findings do not appear in reports.

### ToMarkdown Method

The `ToMarkdown` method generates a markdown string from the SARIF results:

1. Validates that `depth` is between 1 and 6, throwing `ArgumentOutOfRangeException` for
   invalid values. This satisfies `SarifMark-Rpt-Depth`.
2. Calls `AppendHeader` to write the main heading (using `customHeading` if provided, or
   `"[ToolName] Analysis"` by default) and tool information. This satisfies
   `SarifMark-Rpt-Headings`.
3. Calls `AppendIssuesSection` to write the sub-heading, result count using
   `FormatFoundText` (with singular/plural handling), and individual result lines. This
   satisfies `SarifMark-Rpt-Counts` and `SarifMark-Rpt-Markdown`.

### Location Formatting

`FormatLocation` produces the location prefix for each result line. If the URI is empty,
it returns `"(no location)"`. If the URI is set but there is no line number, it returns
the URI alone. If both URI and start line are present, it returns `"uri(line)"`. This
satisfies `SarifMark-Rpt-Locations`.

### Result Formatting

Each result is formatted as a single line ending with two trailing spaces (`  `), which
forces a hard line break in rendered markdown. This satisfies `SarifMark-Rpt-LineBreaks`.

## CLI Integration

The requirement `SarifMark-Sarif-Required` (the tool shall require the `--sarif` parameter
for analysis) is enforced at the command-line layer rather than within this library. The
`ProcessSarifAnalysis` method in `Program.cs` validates that `--sarif` is provided before
invoking the SARIF reading layer. See [command-line.md] for full details.

[command-line.md]: command-line.md
