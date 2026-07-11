# Shared Package Dependencies

SarifMark consumes one shared package: a released version of SarifMark itself. This
self-referential dependency exists because the CI pipeline invokes a released version
of the product to generate the CodeQL quality report that is published alongside each
release's compliance documents.

A shared package, as defined in the project's software categorization standard, is a
software package produced within the same program and consumed as a dependency. It is
referenced by its advertised features rather than its internal design or source code.

## Selection Criteria

SarifMark is used in its own CI pipeline because it is the tool specifically designed
to generate markdown quality reports from SARIF files — precisely the task required to
document the CodeQL analysis results. Using a released version ensures that the quality
report in the release artifacts was produced by a stable, independently verified build
rather than the in-development version.

## Version Management Policy

The shared package version is managed through the local tool manifest
(`.config/dotnet-tools.json`), which pins the installed version. Upgrades are applied
by updating the manifest entry and re-running `dotnet tool restore`. Version numbers
are not recorded in design documentation; version information is captured in the project
SBOM produced by the CI pipeline.

## General Integration Approach

The shared package is consumed as a .NET tool invoked from a CI pipeline step. No
application-level code references the shared package directly. The pipeline step
supplies all required parameters — SARIF file path, report output path, custom heading,
and report depth — on the command line. Errors are propagated through non-zero exit
codes and surfaced as CI pipeline step failures.

## Qualification Strategy

Because the shared package is a released version of the same product, the product's
own self-validation test suite constitutes primary qualification evidence. All features
consumed in the CI pipeline — SARIF reading, markdown report generation, custom heading
configuration, and report depth configuration — are directly covered by integration
tests in `test/DemaConsulting.SarifMark.Tests/IntegrationTests.cs`. A passing CI build
is itself evidence that the shared package executed correctly within the pipeline for
the built version. Per-package qualification details are documented in
`docs/design/shared/sarifmark.md`.
