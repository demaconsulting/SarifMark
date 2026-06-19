## BuildMark

BuildMark (`DemaConsulting.BuildMark`) is the CI tool responsible for generating
build-notes documentation from GitHub Actions workflow run metadata. It runs as a step
in the SarifMark CI pipeline and produces a markdown build-notes document that is
included in the release artifacts.

### Purpose

BuildMark was chosen because it is designed specifically for the Continuous Compliance
workflow and produces structured build-notes documentation in a format consistent with
the other documents in the SarifMark release bundle. It provides evidence that the build
pipeline executed correctly by capturing workflow run identifiers, trigger information,
and run timing from the GitHub Actions API.

### Features Used

- **GitHub Actions metadata query** — BuildMark queries the GitHub API for the current
  workflow run to extract run ID, trigger event, branch, and timestamp.
- **Markdown build-notes generation** — BuildMark renders the captured metadata as a
  structured markdown document suitable for inclusion in the release artifacts.

### Integration Pattern

BuildMark is invoked as a .NET tool from a CI pipeline step after the main build and
test steps have completed:

1. The pipeline installs the tool via `dotnet tool restore` from the local tool manifest.
2. The `buildmark` command is invoked with the GitHub API token and repository details
   supplied as environment variables from the CI context.
3. BuildMark writes the build-notes markdown file to the configured output path.
4. The generated markdown file is published as a release artifact.

No application-level code in SarifMark references BuildMark directly.

### Error Handling

BuildMark is invoked as a CI pipeline step; a nonzero exit code from any `dotnet buildmark`
invocation causes the CI step to fail and stops the pipeline immediately. No special wrapper
or retry logic is applied. Failures are surfaced directly through the GitHub Actions step
status and must be investigated by inspecting the step log.
