# Introduction

This document describes how each software item in SarifMark is verified.

## Purpose

This document describes how each SarifMark requirement is verified. It maps requirements to named test scenarios and
provides traceability from requirements through to test implementation, enabling reviewers to confirm test completeness
without reading source code.

## Scope

This document covers all in-house software items comprising the SarifMark system — including all subsystems and units —
all OTS software items used in the build pipeline, and all shared package dependencies
(released version of SarifMark used in the CI pipeline to generate the CodeQL quality report).
OTS packages used by the test infrastructure (xUnit v3, DemaConsulting.TestResults) are also covered because they are
third-party items whose correct operation must be independently confirmed; the test project itself is out of scope.
It does not cover installation procedures, end-user guides, or CI/CD pipeline configuration.

Out of scope: build pipeline scripts and the test project itself (`test/DemaConsulting.SarifMark.Tests/`).

## Companion Artifact Structure

Local items have parallel artifacts in:

- Requirements: `docs/reqstream/sarifmark.yaml`,
  `docs/reqstream/sarifmark[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/sarifmark.md`,
  `docs/design/sarifmark[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/sarifmark.md`,
  `docs/verification/sarifmark[/{subsystem-name}...]/{item}.md`
- Source: `src/DemaConsulting.SarifMark[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DemaConsulting.SarifMark.Tests[/{SubsystemName}...]/{Item}Tests.cs`

OTS items have integration/usage design documentation parallel to system folders:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Shared package items have integration/usage design documentation parallel to system and OTS folders:

- Requirements: `docs/reqstream/shared/{name}.yaml`
- Design: `docs/design/shared/{name}.md`
- Verification: `docs/verification/shared/{name}.md`

Review-sets are defined in `.reviewmark.yaml`.

## Audience

This document is intended for compliance reviewers and quality assurance personnel confirming that all requirements have
test coverage and that the test approach is sound.

## References

- [SarifMark releases](https://github.com/demaconsulting/SarifMark/releases)
