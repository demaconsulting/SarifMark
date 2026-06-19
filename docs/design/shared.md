## Shared Package Dependencies

SarifMark uses one DemaConsulting shared package: `DemaConsulting.TestResults`. This package is
produced by a different repository within the same program and is consumed as a NuGet runtime
dependency. The per-package integration design is documented in the `shared/` sub-folder.

### Selection Criteria

`DemaConsulting.TestResults` was selected because it provides a purpose-built test result model
and serializers that integrate directly with the self-validation feature of SarifMark. It is an
MIT-licensed package produced within the DEMA Consulting program, ensuring consistent
compatibility with the project's compliance workflow and traceability pipeline.

### Version Management Policy

The shared package version is managed through Dependabot pull requests for NuGet packages.
Version numbers are not recorded in design documentation; version information is captured in
the project SBOM produced by the CI pipeline. Major version upgrades trigger a design review
to assess whether the integration pattern documented in `shared/test-results.md` remains
accurate.

### General Integration Approach

`DemaConsulting.TestResults` is consumed as a NuGet package referenced by the main project.
SarifMark creates a `TestResults` collection at the start of self-validation, appends
individual `TestResult` entries as each scenario completes, and serializes the collection to
either TRX or JUnit XML format when a results file path is supplied. No wrapper classes are
introduced; the package's public types are used directly in `SelfTest/Validation.cs`.
