# Introduction

SarifMark is a .NET command-line tool that generates markdown reports from SARIF (Static Analysis Results
Interchange Format) 2.1.0 files. This document describes the design of the SarifMark software system,
covering the architecture and detailed design of all local software items — system, subsystems, and
units — and the integration and usage design for all OTS software items used in the project pipeline.

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

- **BuildMark**, **FileAssert**, **Pandoc**, **ReqStream**, **ReviewMark**, **SonarMark**,
  **VersionMark**, **WeasyPrint**, **xUnit v3**: integration and usage design for each OTS software
  item used in the project pipeline.

This document does not cover test projects, the CI/CD pipeline configuration, installation procedures,
end-user usage patterns, or the internal design of OTS items.

## Software Structure

The following tree shows how the SarifMark software items are organized across the system, subsystem,
and unit levels:

```text
SarifMark (System)
├── Program (Unit)
├── Cli (Subsystem)
│   └── Context (Unit)
├── Sarif (Subsystem)
│   ├── SarifFinding (Unit)
│   ├── SarifRun (Unit)
│   └── SarifResults (Unit)
├── SelfTest (Subsystem)
│   └── Validation (Unit)
└── Utilities (Subsystem)
    └── PathHelpers (Unit)

OTS Dependencies:
├── BuildMark (OTS)
├── FileAssert (OTS)
├── Pandoc (OTS)
├── ReqStream (OTS)
├── ReviewMark (OTS)
├── SonarMark (OTS)
├── VersionMark (OTS)
├── WeasyPrint (OTS)
└── xUnit v3 (OTS)
```

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown, giving reviewers an
explicit navigation aid from design to code. All source files use the root namespace
`DemaConsulting.SarifMark` regardless of their subdirectory location. This is an intentional
flat-namespace convention: subdirectory names reflect design structure only, not namespace
hierarchy.

```text
src/DemaConsulting.SarifMark/
├── Program.cs                   — entry point and execution orchestrator
├── Cli/
│   └── Context.cs               — command-line argument parser and I/O owner
├── Sarif/
│   ├── SarifFinding.cs          — immutable record for a single analysis finding
│   ├── SarifRun.cs              — immutable record for a single tool run
│   └── SarifResults.cs          — SARIF file reading and markdown report generation
├── SelfTest/
│   └── Validation.cs            — self-validation test runner
└── Utilities/
    └── PathHelpers.cs           — safe path combination utilities

test/DemaConsulting.SarifMark.Tests/
├── Cli/
│   └── ContextTests.cs          — Context unit tests
├── Sarif/
│   ├── SarifFindingTests.cs     — SarifFinding unit tests
│   ├── SarifRunTests.cs         — SarifRun unit tests
│   └── SarifResultsTests.cs     — SarifResults unit tests
├── SelfTest/
│   └── ValidationTests.cs       — Validation unit tests
└── Utilities/
    └── PathHelpersTests.cs      — PathHelpers unit tests
```

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

Review-sets: defined in `.reviewmark.yaml`

## References

- [SARIF 2.1.0 Specification](https://docs.oasis-open.org/sarif/sarif/v2.1.0/sarif-v2.1.0.html)
- [SarifMark releases](https://github.com/demaconsulting/SarifMark/releases)
