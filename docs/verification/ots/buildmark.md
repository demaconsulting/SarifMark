## BuildMark

### Verification Approach

BuildMark is verified through its built-in `--validate` self-validation mechanism. Running
`dotnet buildmark --validate` executes five internal test scenarios that confirm the tool
is installed and functioning correctly in the current environment.

Most scenarios exercise specific advertised features using mock data or the local Git repository
and do not require live GitHub API access. The exception is the `BuildMark_IssueTracking`
scenario, which connects to the GitHub API and therefore requires a valid GitHub API token
(sourced from `GH_TOKEN`, `GITHUB_TOKEN`, or the `gh` CLI) and network access to
`api.github.com`. Environments without API access must skip or exclude that scenario.

### Test Environment

Tests require:

- A local Git repository (used by `BuildMark_GitIntegration` to read version tags).
- Network access to the GitHub API (used by `BuildMark_IssueTracking`); the test uses
  a token sourced from `GH_TOKEN`, `GITHUB_TOKEN`, or the `gh` CLI.
- A writable temporary directory for output files.

### Acceptance Criteria

All five self-validation scenarios must pass with exit code 0 and zero failures in an environment
that provides GitHub API access. In environments without API access, the four scenarios that do
not require live network access (`BuildMark_MarkdownReportGeneration`, `BuildMark_GitIntegration`,
`BuildMark_KnownIssuesReporting`, `BuildMark_RulesRouting`) must pass; `BuildMark_IssueTracking`
may be skipped or excluded.

### Test Scenarios

**BuildMark_MarkdownReportGeneration**: Invokes `dotnet buildmark --validate`; the
`BuildMark_MarkdownReportGeneration` scenario generates a markdown build-notes document
from mock data and confirms the report contains the expected content.
This scenario is tested by `BuildMark_MarkdownReportGeneration`.

**BuildMark_GitIntegration**: The `BuildMark_GitIntegration` self-validation scenario reads
version tags and commits from the local Git repository and confirms the Git connector
returns expected data.
This scenario is tested by `BuildMark_GitIntegration`.

**BuildMark_IssueTracking**: The `BuildMark_IssueTracking` self-validation scenario connects
to the GitHub API, retrieves issue and pull request data, and confirms the integration
returns expected results.
This scenario is tested by `BuildMark_IssueTracking`.

**BuildMark_KnownIssuesReporting**: The `BuildMark_KnownIssuesReporting` self-validation
scenario generates a report with the `--include-known-issues` flag and confirms the Known
Issues section is included in the output.
This scenario is tested by `BuildMark_KnownIssuesReporting`.

**BuildMark_RulesRouting**: The `BuildMark_RulesRouting` self-validation scenario applies
routing rules and confirms that items are assigned to the correct report sections.
This scenario is tested by `BuildMark_RulesRouting`.
