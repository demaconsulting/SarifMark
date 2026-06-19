## ReqStream

### Verification Approach

ReqStream is used in the SarifMark CI pipeline to enforce requirements traceability. It reads requirements YAML files
and TRX test result files, verifies that every requirement maps to at least one passing test, and fails the pipeline if
any requirement is untested. Verification evidence is provided by successful CI pipeline execution with `--enforce`
mode: the pipeline step completes with exit code 0, confirming that ReqStream parsed all requirements, matched them to
passing test results, and found no untested requirements.

### Test Scenarios

**ReqStream_EnforcementMode**: The CI pipeline step invokes ReqStream with `--enforce`; the step completes with exit
code 0, confirming that ReqStream parsed all requirements YAML files, matched every requirement to at least one passing
test result in the TRX files, and found no untested requirements.
This scenario is verified by successful completion of the ReqStream enforcement pipeline step in CI.
