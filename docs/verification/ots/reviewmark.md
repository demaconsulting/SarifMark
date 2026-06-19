## ReviewMark

### Verification Approach

ReviewMark is verified through its built-in `--validate` self-validation mechanism. Running
`reviewmark --validate` executes nine internal test scenarios that confirm the tool is
installed and all advertised features are functioning correctly in the current environment.
Self-validation covers version display, help display, review plan generation, review report
generation, index scanning, working directory override, enforcement mode, elaboration, and
lint validation.

### Test Environment

Tests require:

- No network access; all scenarios operate on temporary file fixtures.
- A writable temporary directory for output files.

### Acceptance Criteria

All nine self-validation scenarios must pass with exit code 0 and zero failures, confirming
that ReviewMark is correctly installed and all advertised features are operational.

### Test Scenarios

**ReviewMark_VersionDisplay**: The `ReviewMark_VersionDisplay` self-validation scenario
invokes `--version` and confirms the tool outputs a valid version string.
This scenario is tested by `ReviewMark_VersionDisplay`.

**ReviewMark_HelpDisplay**: The `ReviewMark_HelpDisplay` self-validation scenario invokes
`--help` and confirms usage information and available options are displayed.
This scenario is tested by `ReviewMark_HelpDisplay`.

**ReviewMark_ReviewPlanGeneration**: The `ReviewMark_ReviewPlanGeneration` self-validation
scenario generates a review plan from a temporary definition file and confirms the output
contains the expected plan structure.
This scenario is tested by `ReviewMark_ReviewPlanGeneration`.

**ReviewMark_ReviewReportGeneration**: The `ReviewMark_ReviewReportGeneration` self-validation
scenario generates a review report from a temporary definition file and confirms the output
contains the expected report structure.
This scenario is tested by `ReviewMark_ReviewReportGeneration`.

**ReviewMark_IndexScan**: The `ReviewMark_IndexScan` self-validation scenario scans a
directory of PDF evidence files with `--index` and confirms the index.json catalogue is
written with the correct content.
This scenario is tested by `ReviewMark_IndexScan`.

**ReviewMark_WorkingDirectoryOverride**: The `ReviewMark_WorkingDirectoryOverride`
self-validation scenario uses `--dir` to override the working directory and confirms file
operations resolve correctly relative to the specified directory.
This scenario is tested by `ReviewMark_WorkingDirectoryOverride`.

**ReviewMark_Enforce**: The `ReviewMark_Enforce` self-validation scenario invokes
`--enforce` against a review configuration with known issues and confirms exit code is
non-zero, proving enforcement mode correctly rejects stale or missing reviews.
This scenario is tested by `ReviewMark_Enforce`.

**ReviewMark_Elaborate**: The `ReviewMark_Elaborate` self-validation scenario invokes
`--elaborate {id}` and confirms the tool prints the ID, title, fingerprint, and file list
for the specified review set.
This scenario is tested by `ReviewMark_Elaborate`.

**ReviewMark_Lint**: The `ReviewMark_Lint` self-validation scenario invokes `--lint` against
a valid definition file and confirms exit code 0 with no errors reported.
This scenario is tested by `ReviewMark_Lint`.
