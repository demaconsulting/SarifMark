# SARIF and Reporting

## Overview

The SARIF and reporting layer is responsible for reading SARIF 2.1.0 files and generating
markdown reports from the extracted results. It consists of two records:
`SarifResult` (a single result entry) and
`SarifResults` (the full results collection with reading and reporting
logic). This layer satisfies requirements `SarifMark-Sarif-Reading`,
`SarifMark-Sarif-Validation`, `SarifMark-Sarif-ToolInfo`, `SarifMark-Sarif-Results`,
`SarifMark-Sarif-Locations`, `SarifMark-Sarif-FilePaths`, `SarifMark-Sarif-Required`,
`SarifMark-Sarif-Processing`, `SarifMark-Rpt-Markdown`, `SarifMark-Rpt-Depth`,
`SarifMark-Rpt-Counts`, `SarifMark-Rpt-Locations`, `SarifMark-Rpt-Headings`, and
`SarifMark-Rpt-LineBreaks`.

## Architecture

The SARIF and reporting layer uses a two-record design:

- **`SarifResult`** is an immutable record representing a single static analysis finding.
  It stores the rule identifier, severity level, message, optional file URI, and optional
  start line. It is constructed internally by the parsing pipeline. See the SarifResult
  Record document for class-level details.

- **`SarifResults`** holds tool metadata and the parsed list of results. It provides the
  static `Read` method for loading a SARIF file and the `ToMarkdown` method for generating
  a markdown report. See the SarifResults Record document for class-level details.

## Reading Pipeline

`SarifResults.Read` processes a SARIF file through a six-step pipeline:

1. Path and file existence validation (satisfies `SarifMark-Sarif-FilePaths`)
2. JSON parsing with error translation (satisfies `SarifMark-Sarif-Validation`)
3. SARIF structure validation — `version` and `runs` fields
   (satisfies `SarifMark-Sarif-Validation`)
4. Tool information extraction from `tool.driver`
   (satisfies `SarifMark-Sarif-ToolInfo`)
5. Result parsing with suppression filtering
   (satisfies `SarifMark-Sarif-Results` and `SarifMark-Sarif-Reading`)
6. Construction and return of the `SarifResults` record
   (satisfies `SarifMark-Sarif-Processing`)

## Report Generation

`SarifResults.ToMarkdown` generates a markdown string from the loaded results. It
validates the heading depth (1–6), emits a configurable heading with tool attribution, and
formats each result with location information and result counts. This satisfies
`SarifMark-Rpt-Markdown`, `SarifMark-Rpt-Depth`, `SarifMark-Rpt-Headings`,
`SarifMark-Rpt-Counts`, `SarifMark-Rpt-Locations`, and `SarifMark-Rpt-LineBreaks`.

## CLI Integration

The requirement `SarifMark-Sarif-Required` (the tool shall require the `--sarif` parameter
for analysis) is enforced at the command-line layer rather than within this library. The
`ProcessSarifAnalysis` method in `Program.cs` validates that `--sarif` is provided before
invoking the SARIF reading layer. See the Command Line document for full details.

## Class Details

- **SarifResult record** — immutable value type for a single analysis finding
- **SarifResults record** — SARIF file reading and markdown report generation
