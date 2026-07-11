## Sarif

![Sarif Structure](SarifView.svg)

The `Sarif` subsystem is responsible for reading SARIF 2.1.0 files and generating
markdown reports from the extracted results. It provides the core analysis capability
of SarifMark through a three-record immutable pipeline.

### Overview

The `Sarif` subsystem processes a SARIF file through a validated JSON parsing pipeline
and produces an immutable record graph from which a markdown report can be generated.
It has no dependency on the `Cli` subsystem and contains no mutable state. The subsystem
contains three units:

- **SarifFinding**: an immutable record representing a single analysis finding (rule ID,
  severity, message, optional location).
- **SarifRun**: an immutable record representing all results from a single tool run,
  plus a `ToMarkdown` method for generating a per-run report.
- **SarifResults**: the root record holding all runs from a SARIF file, providing
  the `Read` static factory method and the `ToMarkdown` method for multi-run aggregation.

### Interfaces

**SarifResults.Read**: Parses a SARIF 2.1.0 file and returns an immutable record graph.

- *Type*: In-process .NET static method
- *Role*: Provider
- *Contract*: Accepts `string filePath`; validates the path, reads and parses the JSON,
  validates SARIF structure, and returns a `SarifResults` record containing one or more
  `SarifRun` records, each containing zero or more `SarifFinding` records.
- *Constraints*: `filePath` must be non-null, non-empty, and refer to an existing file.
  The file must be valid JSON conforming to SARIF 2.1.0. Violations produce
  `ArgumentException`, `FileNotFoundException`, or `InvalidOperationException`.

**SarifResults.ToMarkdown**: Generates a markdown string from the loaded results.

- *Type*: In-process .NET instance method
- *Role*: Provider
- *Contract*: Accepts `int depth` (1–6) and optional `string? heading`; returns a
  UTF-8 markdown string. For single-run files, delegates to `SarifRun.ToMarkdown`.
  For multi-run files, concatenates indexed run reports.
- *Constraints*: `depth` must be between 1 and 6 inclusive; violations throw
  `ArgumentOutOfRangeException`.

**SarifResults.HasIssues**: Aggregated indicator of whether any findings were found.

- *Type*: In-process .NET instance property
- *Role*: Provider
- *Contract*: Returns `true` if any run contains at least one result.
- *Constraints*: None.

### Design

The SARIF reading and reporting pipeline flows through the three units in sequence:

1. `Program.ProcessSarifAnalysis` calls `SarifResults.Read(context.SarifFile)`.
2. `SarifResults.Read` validates the file path, reads the file content, and delegates
   to `JsonDocument.Parse` for JSON parsing (any `JsonException` is translated to
   `InvalidOperationException`).
3. `ValidateSarifStructure` verifies the presence of `version` and a non-empty `runs`
   array, returning the runs JSON element.
4. For each run element, `ExtractToolInformation`, `ParseResults`, and `ExtractFileCount`
   produce a `SarifRun` record. `ParseResults` uses `IsSuppressed` to filter suppressed
   results before constructing `SarifFinding` records.
5. The completed `SarifResults` record is returned to `Program`.
6. `Program` calls `SarifResults.ToMarkdown` when `--report` is specified; the markdown
   string is written to disk with `File.WriteAllText`.
