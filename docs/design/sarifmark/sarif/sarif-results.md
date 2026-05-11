### SarifResults Record

#### Overview

The `SarifResults` record (`SarifResults.cs`) is the primary public type for working with SARIF
file content. It holds the collection of parsed runs and exposes both the `Read` static method
for file loading and the `ToMarkdown` method for report generation.

#### Record Design

`SarifResults` is a `record` with an `internal` constructor. External consumers obtain instances
only through `Read`; the record is immutable once constructed.

The `DemaConsulting.SarifMark` project file includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />`,
which grants the test assembly access to the internal constructor. This enables direct unit testing
of the constructor without relaxing the access restriction for all external consumers.

#### Properties

| Property      | Type                         | Description                                          |
|---------------|------------------------------|------------------------------------------------------|
| `Runs`        | `IReadOnlyList<SarifRun>`    | Collection of all parsed runs                        |
| `HasIssues`   | `bool`                       | True if any run contains results (aggregate)         |

`HasIssues` aggregates across all runs. Per-run data (tool name, version, results, file count) is
accessed via the `Runs` collection.

These satisfy requirements `SarifMark-SarifResults-Properties`, `SarifMark-SarifResults-Runs`,
and `SarifMark-SarifResults-HasIssues`.

#### Read Method

The static `Read(string filePath)` method loads and parses a SARIF 2.1.0 file through a
pipeline:

1. **Path validation** — throws `ArgumentException` if `filePath` is null, empty, or
   whitespace. This satisfies `SarifMark-SarifResults-ValidatePathArgument`.
2. **File existence** — throws `FileNotFoundException` if the file does not exist on disk.
   This satisfies `SarifMark-SarifResults-ValidatePathExists`.
3. **JSON parsing** — reads the file with `File.ReadAllText` and parses it with
   `JsonDocument.Parse`. A `JsonException` is translated to `InvalidOperationException`.
   This satisfies `SarifMark-SarifResults-ValidateStructure`.
4. **Structure validation** — delegates to `ValidateSarifStructure` to verify the `version`
   and `runs` fields and return the runs array element. This satisfies
   `SarifMark-SarifResults-ValidateStructure`.
5. **Per-run processing** — for each element in the runs array, delegates to
   `ExtractToolInformation`, `ParseResults`, and `ExtractFileCount` to create a `SarifRun`.
   This satisfies `SarifMark-SarifResults-ExtractTool`, `SarifMark-SarifResults-ParseResults`,
   `SarifMark-SarifResults-FilterSuppressions`, `SarifMark-SarifResults-FileCount`, and
   `SarifMark-SarifResults-Runs`.
6. **Construction and return** — constructs and returns a `SarifResults` from the list of runs.
   This satisfies `SarifMark-Sarif-Processing`.

#### ValidateSarifStructure Method

`ValidateSarifStructure` verifies that the root JSON element contains:

- A `version` property (any value is accepted; absence throws `InvalidOperationException`).
- A `runs` array that is non-empty (absence or empty array throws `InvalidOperationException`).

It returns the runs array element for iteration. This satisfies requirement
`SarifMark-SarifResults-ValidateStructure`.

#### ExtractToolInformation Method

`ExtractToolInformation` navigates from the run element to `tool.driver`, throwing
`InvalidOperationException` if either `tool` or `driver` is absent. It reads the `name` property
from `driver`, defaulting to `"Unknown"` if absent, then delegates to `ExtractToolVersion` for the
version string. This satisfies requirement `SarifMark-SarifResults-ExtractTool`.

#### ExtractToolVersion Method

`ExtractToolVersion` checks three fields in the `driver` JSON element in priority order:

| Priority | JSON field               |
|----------|--------------------------|
| 1        | `version`                |
| 2        | `semanticVersion`        |
| 3        | `dottedQuadFileVersion`  |

The first field whose value is non-null and non-whitespace is returned. If none of the three fields
yields a value, `"Unknown"` is returned. This satisfies requirement `SarifMark-SarifResults-VersionPriority`.

#### ParseResults Method

`ParseResults` iterates the `results` JSON array within the run element. If the array is absent or
not an array, an empty list is returned. For each element, `IsSuppressed` checks whether a
non-empty `suppressions` array is present; suppressed entries are skipped. Each remaining element
is parsed into a `SarifFinding` record. This satisfies requirements `SarifMark-SarifResults-ParseResults`
and `SarifMark-SarifResults-FilterSuppressions`.

#### ToMarkdown Method

`ToMarkdown(int depth, string? heading = null)` generates a markdown string from the results:

1. **Depth validation** — throws `ArgumentOutOfRangeException` if `depth` is less than 1 or
   greater than 6. This satisfies `SarifMark-SarifResults-ValidateDepth`.
2. **Single-run** — when there is exactly one run, delegates directly to
   `Runs[0].ToMarkdown(depth, heading)`, producing output identical to the pre-multi-run
   behavior. This satisfies `SarifMark-SarifResults-ToMarkdown`.
3. **Multi-run** — when there are multiple runs, concatenates the markdown output of each run
   with headings `"[ToolName] Analysis (#1)"`, `"[ToolName] Analysis (#2)"` etc. (or
   `"[heading] (#1)"` if a custom heading is provided). This satisfies
   `SarifMark-SarifResults-MultiRunMarkdown`.

#### ExtractFileCount Method

`ExtractFileCount(JsonElement runElement)` returns the length of the `artifacts` array of the
given run element:

- If the `artifacts` property is absent or is not an array, `0` is returned.
- Otherwise, the length of the `artifacts` array is returned.

This method is called once per run element during `Read`, so each `SarifRun` in the resulting
collection carries an independent file count from its own artifacts array. This satisfies
requirement `SarifMark-SarifResults-FileCount`.

#### Cross-References

See the SarifRun Record document for `SarifRun`, which is constructed per-run during SARIF parsing.
See the SarifFinding Record document for the `SarifFinding` record that `ParseResults` produces.
See the Program Class document for how `Read` and `ToMarkdown` are called from
`ProcessSarifAnalysis`.
