## VersionMark

VersionMark (`DemaConsulting.VersionMark`) is the CI tool that captures the version
numbers of the .NET tools used in the SarifMark pipeline and renders them as a markdown
document included in the release artifacts. It provides a precise record of the toolchain
versions used to produce each release.

### Purpose

VersionMark was chosen because recording the exact versions of pipeline tools is a
Continuous Compliance requirement: release artifacts must carry enough information to
reproduce the build environment. VersionMark automates this capture step and produces
a structured markdown document that is consistent with the other compliance documents
in the release bundle.

### Features Used

- **Tool-version capture** — reads version metadata for each specified .NET tool by
  invoking it with a version flag and capturing the output.
- **Markdown version report generation** — renders the captured version strings as a
  structured markdown document listing each tool and its version.

### Integration Pattern

VersionMark is invoked as a .NET tool from a CI pipeline step:

1. The pipeline installs the tool via `dotnet tool restore`.
2. A pipeline step invokes `versionmark` with the list of tools whose versions are to
   be captured, supplied via configuration.
3. VersionMark interrogates each tool and writes the version report to the configured
   output path.
4. The generated report is published as a release artifact.

No application-level code in SarifMark references VersionMark directly.
