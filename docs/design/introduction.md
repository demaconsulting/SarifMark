# Introduction

This document describes the internal design of the SarifMark .NET tool. It provides a
structured account of the key components, their responsibilities, and how they interact to
deliver the tool's capabilities.

## Purpose

The purpose of this document is to:

- Describe the design decisions and structure of the SarifMark tool
- Provide a reference for developers contributing to or reviewing the tool
- Establish traceability between requirements and the components that fulfil them
- Document each conceptual group in sufficient detail to support code review

## Scope

This document covers the design of three primary functional layers within SarifMark:

- The **command-line layer**: the `Program` entry point and `Context` class that handle
  argument parsing, output routing, and program flow control
- The **SARIF and reporting layer**: the `SarifResult` and `SarifResults` classes that
  read SARIF files and generate markdown reports
- The **self-validation layer**: the `Validation` class that provides built-in
  verification of the tool's core functionality

Each functional layer is first described at a **concept level** — covering its purpose,
architecture, and the requirements it satisfies — followed by **class-level documents**
that describe each implementing class in detail.

This document does not cover installation, end-user usage patterns, or the CI/CD pipeline
configuration. Those topics are addressed in other [SarifMark repository][sarifmark-repo] documentation.

## Audience

This document is intended for:

- Software developers implementing features or fixing defects in the tool
- Reviewers conducting formal design and code reviews
- Quality assurance engineers tracing requirements to implementation

Readers are assumed to be familiar with C# and .NET development and general concepts of
command-line tool design.

## Software Structure

The following tree shows how the SarifMark software items are organized across the
system, subsystem, and unit levels:

```text
SarifMark (System)
├── Program (Unit)
├── Cli (Subsystem)
│   └── Context (Unit)
├── Sarif (Subsystem)
│   ├── SarifResult (Unit)
│   └── SarifResults (Unit)
├── SelfTest (Subsystem)
│   └── Validation (Unit)
└── Utilities (Subsystem)
    └── PathHelpers (Unit)
```

Each unit is described in detail in its own chapter within this document.

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown above, giving
reviewers an explicit navigation aid from design to code:

```text
src/DemaConsulting.SarifMark/
├── Program.cs                  — entry point and execution orchestrator
├── Cli/
│   └── Context.cs              — command-line argument parser and I/O owner
├── Sarif/
│   ├── SarifResult.cs          — immutable record for a single analysis finding
│   └── SarifResults.cs         — SARIF file reading and markdown report generation
├── SelfTest/
│   └── Validation.cs           — self-validation test runner
└── Utilities/
    └── PathHelpers.cs          — safe path combination utilities
```

The test project mirrors the same layout under `test/DemaConsulting.SarifMark.Tests/`.

## Relationship to Requirements and Code

Each component described here corresponds to one or more requirements defined in the
`docs/reqstream/` files. Requirements identifiers are referenced inline where relevant to
make traceability explicit.

The source code in `src/DemaConsulting.SarifMark/` is the authoritative implementation.
This document describes the intent and structure of that code; any discrepancy between
this document and the code should be resolved by updating this document to reflect the
actual implementation, or by raising a defect against the code.

[sarifmark-repo]: https://github.com/demaconsulting/SarifMark
