## ReqStream Verification

### Overview

ReqStream is used in the SarifMark CI pipeline to enforce requirements traceability. It reads requirements YAML files
and TRX test result files, verifies that every requirement maps to at least one passing test, and fails the pipeline if
any requirement is untested.

### Verification Strategy

Verification evidence is provided by successful CI pipeline execution with `--enforce` mode: the pipeline step
completes with exit code 0, confirming that ReqStream parsed all requirements, matched them to passing test results, and
found no untested requirements.

### Requirement Coverage

| Requirement ID | Description | Verification Evidence |
|---|---|---|
| `SarifMark-OTS-ReqStream` | Enforcement mode passes in CI | `ReqStream_EnforcementMode` (CI pipeline step) |
