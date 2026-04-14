# SARIF and Reporting

## Overview

The SARIF and reporting layer is responsible for reading SARIF 2.1.0 files and generating
markdown reports from the extracted results. It consists of three records:
`SarifResult` (a single result entry),
`SarifRun` (results from a single tool run), and
`SarifResults` (the full results collection with reading and reporting
logic). This layer satisfies requirements `SarifMark-Sarif-Reading`,
`SarifMark-Sarif-Validation`, `SarifMark-Sarif-ToolInfo`, `SarifMark-Sarif-Results`,
`SarifMark-Sarif-Locations`, `SarifMark-Sarif-FilePaths`,
`SarifMark-Sarif-Processing`, `SarifMark-Sarif-MultiRun`, `SarifMark-Report-Markdown`,
`SarifMark-Report-Depth`, `SarifMark-Report-Counts`, `SarifMark-Report-Locations`,
`SarifMark-Report-Headings`, and `SarifMark-Report-LineBreaks`.

## Architecture

The SARIF and reporting layer uses a three-record design:

- **`SarifResult`** is an immutable record representing a single static analysis finding.
  It stores the rule identifier, severity level, message, optional file URI, and optional
  start line. It is constructed internally by the parsing pipeline. See the SarifResult
  Record document for class-level details.

- **`SarifRun`** is an immutable record representing the results from a single run within a
  SARIF file. It holds tool metadata, the parsed list of results, and the file count for that
  run. It provides the `ToMarkdown` method for generating a markdown report for the run.
  See the SarifRun Record document for class-level details.

- **`SarifResults`** holds the collection of all parsed runs. It provides the static `Read`
  method for loading a SARIF file and the `ToMarkdown` method for generating a markdown
  report. For single-run files it delegates directly to the run's `ToMarkdown`; for multi-run
  files it concatenates the run reports. See the SarifResults Record document for class-level
  details.

## Reading Pipeline

`SarifResults.Read` processes a SARIF file through a pipeline:

1. Path and file existence validation (satisfies `SarifMark-Sarif-FilePaths`)
2. JSON parsing with error translation (satisfies `SarifMark-Sarif-Validation`)
3. SARIF structure validation — `version` and `runs` fields
   (satisfies `SarifMark-Sarif-Validation`)
4. Per-run tool information extraction from `tool.driver`
   (satisfies `SarifMark-Sarif-ToolInfo`)
5. Per-run result parsing with suppression filtering
   (satisfies `SarifMark-Sarif-Results` and `SarifMark-Sarif-Reading`)
6. Construction and return of the `SarifResults` record with all runs
   (satisfies `SarifMark-Sarif-Processing` and `SarifMark-Sarif-MultiRun`)

## Report Generation

`SarifResults.ToMarkdown` generates a markdown string from the loaded results. It
validates the heading depth (1–6), then for a single-run file delegates directly to the
run's `ToMarkdown`. For multi-run files it concatenates the markdown output of all runs,
emitting indexed headings (e.g., `"Tool1 Analysis (#1)"`, `"Tool2 Analysis (#2)"`). Each
run's report includes a configurable heading with tool attribution, and formats each result
with location information and result counts. This satisfies `SarifMark-Report-Markdown`,
`SarifMark-Report-Depth`, `SarifMark-Report-Headings`, `SarifMark-Report-Counts`,
`SarifMark-Report-Locations`, and `SarifMark-Report-LineBreaks`.

## CLI Integration

The requirement `SarifMark-System-SarifRequired` (the tool shall require the `--sarif` parameter
for analysis) is enforced at the application layer rather than within this library. The
`ProcessSarifAnalysis` method in `Program.cs` validates that `--sarif` is provided before
invoking the SARIF reading layer. See the Program Class document for full details.

## Class Details

- **SarifResult record** — immutable value type for a single analysis finding
- **SarifRun record** — immutable value type for a single tool run with markdown generation
- **SarifResults record** — SARIF file reading and markdown report generation
