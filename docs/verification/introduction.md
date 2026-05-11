# Introduction

## Purpose

This document describes how each SarifMark requirement is verified. It maps requirements to named test scenarios and
provides traceability from requirements through to test implementation, enabling reviewers to confirm test completeness
without reading source code.

## Scope

This document covers all in-house software items comprising the SarifMark system — including all subsystems and units —
and all OTS software items used in the build pipeline and test infrastructure. It does not cover installation
procedures, end-user guides, or CI/CD pipeline configuration.

## Companion Artifact Structure

For each in-house software item, the companion artifacts are organized as follows:

- Requirements: `docs/reqstream/sarifmark.yaml` and `docs/reqstream/sarifmark/{item}.yaml`
- Design: `docs/design/sarifmark.md` and `docs/design/sarifmark/{item}.md`
- Verification: this document
- Source: `src/DemaConsulting.SarifMark/`
- Tests: `test/DemaConsulting.SarifMark.Tests/`

For each OTS software item, the companion artifacts are organized as follows:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Verification: `docs/verification/ots/{ots-name}.md`

Review-sets for formal review coverage are defined in `.reviewmark.yaml`.

## Audience

This document is intended for compliance reviewers and quality assurance personnel confirming that all requirements have
test coverage and that the test approach is sound.
