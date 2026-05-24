# Installation

## Continuous Compliance

SarifMark follows the Continuous Compliance methodology to ensure every release is auditable and
traceable. The following practices are applied on every build:

- **Requirements Traceability**: Every requirement is linked to passing tests; a trace matrix is
  auto-generated on each release.
- **Linting Enforcement**: markdownlint, cspell, and yamllint are enforced before any build.
- **Automated Audit Documentation**: Each release ships with generated requirements, justifications,
  a trace matrix, and quality reports.
- **CodeQL and SonarCloud**: Security and quality analysis runs on every build.

## Prerequisites

SarifMark requires one of the following .NET SDK versions:

- .NET SDK 8.0
- .NET SDK 9.0
- .NET SDK 10.0

No other runtime dependencies are needed.

## Global Installation

Install SarifMark as a global .NET tool so the `sarifmark` command is available system-wide:

```shell
dotnet tool install --global DemaConsulting.SarifMark
```

## Local Installation

Install SarifMark as a local tool scoped to a repository:

```shell
dotnet new tool-manifest
dotnet tool install DemaConsulting.SarifMark
dotnet sarifmark --version
```

## Updating

To update an existing installation:

**Global:**

```shell
dotnet tool update --global DemaConsulting.SarifMark
```

**Local:**

```shell
dotnet tool update DemaConsulting.SarifMark
```
