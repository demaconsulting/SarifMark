# Introduction

SarifMark is a .NET command-line tool that generates markdown reports from SARIF (Static Analysis Results
Interchange Format) 2.1.0 files. This document describes the design of the SarifMark software system,
covering the architecture and detailed design of all local software items — system, subsystems, and
units — the integration and usage design for all OTS software items used in the project pipeline, and
the integration and usage design for all shared package dependencies.

## Purpose

This document defines the design for each software item in SarifMark. It provides full architectural
and detailed design for local items (system, subsystems, and units), and integration and usage design
for OTS software items. A reviewer should be able to understand how each item satisfies its
requirements without reading source code. This document also establishes traceability between
requirements and the components that fulfil them, and provides a reference for developers
contributing to or reviewing the tool.

## Scope

This document covers the following software items:

Local items:

- **SarifMark**: system, subsystem, and unit design for all components of the SarifMark tool, including
  the command-line interface, SARIF reading and reporting, self-validation, and path utilities.

OTS items:

- **BuildMark**, **DemaConsulting.TestResults**, **FileAssert**, **Pandoc**, **ReqStream**, **ReviewMark**, **SonarMark**,
  **VersionMark**, **WeasyPrint**, **xUnit v3**: integration and usage design for each OTS software
  item used in the project pipeline.

Shared packages:

- **SarifMark (released version)**: integration and usage design for the released version of SarifMark
  consumed in the CI pipeline to generate the CodeQL quality report.

This document does not cover test projects, the CI/CD pipeline configuration, installation procedures,
end-user usage patterns, or the internal design of OTS items.

## Software Structure

- **SarifMark** (System) - .NET CLI tool that generates markdown reports from SARIF 2.1.0 files
  - Program (Unit) - system-level entry point and execution dispatcher
  - **Cli** (Subsystem) - command-line argument parsing and execution context
    - Context (Unit) - argument parser, I/O owner, and exit-code manager
  - **Sarif** (Subsystem) - SARIF file reading and markdown report generation
    - SarifFinding (Unit) - immutable record for a single analysis finding
    - SarifRun (Unit) - immutable record for a single tool run
    - SarifResults (Unit) - SARIF file reader and markdown report generator
  - **SelfTest** (Subsystem) - end-to-end self-validation of tool capabilities
    - Validation (Unit) - self-validation test runner
  - **Utilities** (Subsystem) - shared path-safety helpers
    - PathHelpers (Unit) - safe path combination utilities

**OTS Dependencies:**

- BuildMark (OTS)
- DemaConsulting.TestResults (OTS)
- FileAssert (OTS)
- Pandoc (OTS)
- ReqStream (OTS)
- ReviewMark (OTS)
- SonarMark (OTS)
- VersionMark (OTS)
- WeasyPrint (OTS)
- xUnit v3 (OTS)

**Shared Package Dependencies:**

- SarifMark (Shared Package)

## Folder Layout

- **src/** - source files and projects
  - **DemaConsulting.SarifMark/** - main project source
    - **Cli/** - command-line argument parsing
    - **Sarif/** - SARIF reading and report generation
    - **SelfTest/** - self-validation test runner
    - **Utilities/** - path-safety helpers
- **test/** - test projects
  - **DemaConsulting.SarifMark.Tests/** - unit and integration tests
    - **Cli/** - tests for the Cli subsystem
    - **Sarif/** - tests for the Sarif subsystem
    - **SelfTest/** - tests for the SelfTest subsystem
    - **Utilities/** - tests for the Utilities subsystem
- **docs/design/ots/** - OTS item integration and usage design documents
- **docs/design/shared/** - shared package integration and usage design documents

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/sarifmark.yaml`,
  `docs/reqstream/sarifmark[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/sarifmark.md`,
  `docs/design/sarifmark[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/sarifmark.md`,
  `docs/verification/sarifmark[/{subsystem-name}...]/{item}.md`
- Source: `src/DemaConsulting.SarifMark[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DemaConsulting.SarifMark.Tests[/{SubsystemName}...]/{Item}Tests.cs`

OTS items have integration and usage design documentation parallel to system folders:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Shared package items have integration and usage design documentation parallel to system and OTS folders:

- Requirements: `docs/reqstream/shared/{name}.yaml`
- Design: `docs/design/shared/{name}.md`
- Verification: `docs/verification/shared/{name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

- [SARIF 2.1.0 Specification](https://docs.oasis-open.org/sarif/sarif/v2.1.0/sarif-v2.1.0.html)
- [SarifMark releases](https://github.com/demaconsulting/SarifMark/releases)
