## VersionMark

### Verification Approach

VersionMark is verified through its built-in `--validate` self-validation mechanism. Running
`versionmark --validate` executes four internal test scenarios that confirm the tool is
installed and all advertised features are functioning correctly. Self-validation covers version
capture, markdown report generation, and lint validation for both valid and invalid configuration
files.

### Test Environment

Tests require:

- `dotnet --version` is accessible on the PATH (used by `VersionMark_CapturesVersions` to
  capture a real tool version).
- A writable temporary directory for output files.
- No network access.

### Acceptance Criteria

All four self-validation scenarios must pass with exit code 0 and zero failures, confirming
that VersionMark is correctly installed and all advertised features are operational.

### Test Scenarios

**VersionMark_CapturesVersions**: The `VersionMark_CapturesVersions` self-validation scenario
creates a minimal `.versionmark.yaml` that captures the `dotnet` version, runs the capture
command, and confirms the output JSON file contains the expected version key.
This scenario is tested by `VersionMark_CapturesVersions`.

**VersionMark_GeneratesMarkdownReport**: The `VersionMark_GeneratesMarkdownReport`
self-validation scenario creates two version JSON input files, runs the publish command, and
confirms the generated markdown report contains the expected tool version entries.
This scenario is tested by `VersionMark_GeneratesMarkdownReport`.

**VersionMark_LintPassesForValidConfig**: The `VersionMark_LintPassesForValidConfig`
self-validation scenario runs `--lint` against a valid `.versionmark.yaml` configuration file
and confirms exit code 0 with no errors, proving the linter correctly accepts well-formed
configuration.
This scenario is tested by `VersionMark_LintPassesForValidConfig`.

**VersionMark_LintReportsErrorsForInvalidConfig**: The `VersionMark_LintReportsErrorsForInvalidConfig`
self-validation scenario runs `--lint` against a configuration file missing the required `regex`
field and confirms exit code is non-zero, proving the linter correctly rejects malformed
configuration.
This scenario is tested by `VersionMark_LintReportsErrorsForInvalidConfig`.
