## ReqStream

### Verification Approach

ReqStream is verified through its built-in `--validate` self-validation mechanism. Running
`reqstream --validate` executes six internal test scenarios that confirm the tool is
installed and functioning correctly in the current environment, including requirements
processing, traceability matrix generation, report export, tag filtering, enforcement mode,
and lint validation.

### Test Environment

Tests require:

- No network access; all scenarios operate on temporary file fixtures.
- A writable temporary directory for output files.

### Acceptance Criteria

All six self-validation scenarios must pass with exit code 0 and zero failures, confirming
that ReqStream is correctly installed and all advertised features are operational.

### Test Scenarios

**ReqStream_RequirementsProcessing**: The `ReqStream_RequirementsProcessing` self-validation
scenario processes a requirements YAML file and confirms all requirements are parsed into
the complete model.
This scenario is tested by `ReqStream_RequirementsProcessing`.

**ReqStream_TraceMatrix**: The `ReqStream_TraceMatrix` self-validation scenario generates a
traceability matrix document and confirms the matrix correctly links requirements to tests.
This scenario is tested by `ReqStream_TraceMatrix`.

**ReqStream_ReportExport**: The `ReqStream_ReportExport` self-validation scenario exports a
requirements report and a justifications document and confirms both output files are
created with expected content.
This scenario is tested by `ReqStream_ReportExport`.

**ReqStream_TagsFiltering**: The `ReqStream_TagsFiltering` self-validation scenario filters
requirements by tag and confirms only matching requirements are included in the output.
This scenario is tested by `ReqStream_TagsFiltering`.

**ReqStream_EnforcementMode**: The `ReqStream_EnforcementMode` self-validation scenario
invokes ReqStream with `--enforce` against a requirements set where all requirements are
covered, confirming exit code 0 and that enforcement mode correctly signals success.
This scenario is tested by `ReqStream_EnforcementMode`.

**ReqStream_Lint**: The `ReqStream_Lint` self-validation scenario invokes `--lint` against
a requirements YAML file and confirms the file is structurally valid with no errors reported.
This scenario is tested by `ReqStream_Lint`.
