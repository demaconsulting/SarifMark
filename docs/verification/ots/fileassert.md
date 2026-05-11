## FileAssert Verification

### Overview

FileAssert is used in the SarifMark CI pipeline to assert that generated output files (HTML documents, PDF documents)
exist, have non-trivial size, contain valid structural elements, and include expected content.

### Verification Strategy

FileAssert exposes a `--version` and `--help` flag. Self-validation tests confirm FileAssert is installed and
operational before it is used to validate generated documents. Functional verification is provided by the successful
execution of file assertion steps in the CI pipeline.

### Requirement Coverage

| Requirement ID | Description | Verification Evidence |
|---|---|---|
| `SarifMark-OTS-FileAssert` | CI file assertions pass | `FileAssert_VersionDisplay`, `FileAssert_HelpDisplay` |
