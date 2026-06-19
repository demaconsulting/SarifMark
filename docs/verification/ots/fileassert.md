## FileAssert

### Verification Approach

FileAssert is verified through its built-in `--validate` self-validation mechanism. Running
`fileassert --validate` executes five internal test scenarios that confirm the tool is
installed and all advertised features are functioning correctly: version display, help display,
result file generation, file existence assertion, and content assertion.

### Test Environment

Tests require:

- No network access; all scenarios operate on temporary file fixtures.
- A writable temporary directory for output files.

### Acceptance Criteria

All five self-validation scenarios must pass with exit code 0 and zero failures, confirming
that FileAssert is correctly installed and all advertised features are operational.

### Test Scenarios

**FileAssert_VersionDisplay**: The `FileAssert_VersionDisplay` self-validation scenario
invokes `--version` and confirms the tool outputs a valid version string and exits with
code 0.
This scenario is tested by `FileAssert_VersionDisplay`.

**FileAssert_HelpDisplay**: The `FileAssert_HelpDisplay` self-validation scenario invokes
`--help` and confirms usage information and available options are displayed.
This scenario is tested by `FileAssert_HelpDisplay`.

**FileAssert_Results**: The `FileAssert_Results` self-validation scenario runs test
assertions that produce pass and fail outcomes and writes results to a TRX file, confirming
result file generation with mixed outcomes works correctly.
This scenario is tested by `FileAssert_Results`.

**FileAssert_Exists**: The `FileAssert_Exists` self-validation scenario runs a file
existence assertion via glob pattern and confirms the assertion passes when the expected
file is present.
This scenario is tested by `FileAssert_Exists`.

**FileAssert_Contains**: The `FileAssert_Contains` self-validation scenario runs a content
assertion that checks a file contains expected text and confirms the assertion passes when
the content is present.
This scenario is tested by `FileAssert_Contains`.
