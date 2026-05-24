## FileAssert

### Verification Approach

FileAssert is used in the SarifMark CI pipeline to assert that generated output files (HTML documents, PDF documents)
exist, have non-trivial size, contain valid structural elements, and include expected content. FileAssert exposes a
`--version` and `--help` flag. Self-validation tests confirm FileAssert is installed and operational before it is used
to validate generated documents. Functional verification is provided by the successful execution of file assertion
steps in the CI pipeline.

### Test Scenarios

**FileAssert_VersionDisplay**: The CI self-validation step invokes `fileassert --version`; the tool responds with its
version string and exits with code 0, confirming FileAssert is installed and operational.
This scenario is verified by the self-validation CI pipeline step.

**FileAssert_HelpDisplay**: The CI self-validation step invokes `fileassert --help`; the tool responds with its usage
information and exits with code 0, confirming the CLI interface is functioning as expected.
This scenario is verified by the self-validation CI pipeline step.

### Requirements Coverage

- **`SarifMark-OTS-FileAssert`**: CI file assertions pass — `FileAssert_VersionDisplay`, `FileAssert_HelpDisplay`
