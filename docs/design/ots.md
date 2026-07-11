# OTS Dependencies

SarifMark uses eleven OTS software items: ten DEMA Consulting pipeline tools and the
xUnit v3 testing framework. All eleven items are consumed as .NET tools or NuGet packages
and are managed through the local tool manifest and the project dependency lock files.
Per-item integration designs are documented in the `ots/` sub-folder.

## Selection Criteria

OTS items are selected according to the following criteria:

All OTS packages must carry an OSI-approved open-source license that is compatible with
the MIT License under which SarifMark is distributed. Each package must demonstrate
active maintenance through regular releases and publicly available source code.

DEMA Consulting pipeline tools (BuildMark, DemaConsulting.TestResults, FileAssert, Pandoc, ReqStream, ReviewMark,
SonarMark, SysML2Tools, VersionMark, WeasyPrint) are preferred because they are designed specifically
for the Continuous Compliance workflow and provide documented compliance evidence that
integrates directly with the project's traceability pipeline.

Testing framework selection (xUnit v3) is based on native TRX output support, compatibility
with the VSTest adapter required by ReqStream, and the established ecosystem around xUnit
in the .NET community.

## Version Management Policy

OTS package versions are managed through Dependabot pull requests for NuGet and Node.js
packages. The local tool manifest (`.config/dotnet-tools.json`) pins all .NET tool
versions; upgrades are applied by updating the manifest entry and re-running
`dotnet tool restore`.

Version numbers are not recorded in design documentation; version information is captured
in the project SBOM produced by the CI pipeline. Major version upgrades trigger a design
review to assess whether the integration pattern documented in `ots/{item}.md` remains
accurate. Node.js tool versions are pinned directly in `package.json`, and .NET tool
versions are pinned in the local tool manifest.

## General Integration Approach

All OTS items are consumed as CLI tools invoked from CI/CD pipeline scripts, as a NuGet
package referenced directly by the main project (DemaConsulting.TestResults, used by the
SelfTest subsystem to collect and serialize self-validation results), or as a NuGet package
referenced by the test project (xUnit v3). No wrapper classes are introduced at the
application level; tools are invoked directly via `dotnet tool run` or their shell
command, and packages are referenced through standard NuGet project references.

Each tool reads its configuration from a dedicated file (e.g., `.reviewmark.yaml` for
ReviewMark, `requirements.yaml` for ReqStream) or receives all parameters from the
pipeline script. Errors are propagated through non-zero exit codes and surfaced as
CI pipeline step failures.

## Qualification Strategy

OTS items are qualified through two mechanisms. First, each item's own published CI
pipeline or test results serve as vendor-provided evidence that the item functions
as documented. Second, local integration tests defined in the
`test/DemaConsulting.SarifMark.Tests/` project confirm that the required features
work correctly in the SarifMark context; these tests are linked to requirements via
ReqStream and must pass in every CI run.

A passing CI build is itself evidence that all OTS items executed correctly within the
pipeline for the built version.
