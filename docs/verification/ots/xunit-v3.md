## xUnit v3

### Verification Approach

xUnit v3 (`xunit.v3` and `xunit.runner.visualstudio` packages) is used to discover and execute all unit and
integration tests in `test/DemaConsulting.SarifMark.Tests/`. The VSTest adapter (`xunit.runner.visualstudio`)
produces TRX result files consumed by ReqStream for traceability enforcement. The test framework is verified
implicitly by the successful discovery and execution of the test suite. Representative passing tests from multiple
test classes confirm the framework is operational across all test categories. TRX output is verified by the presence
of the result files produced during `dotnet test --results-directory` invocations in the build pipeline.

### Test Scenarios

**XUnitV3_TestDiscoveryAndExecution**: The test suite runs with `dotnet test` and discovers and executes tests across
all test classes, including `SarifResultsTests`, `ContextTests`, and integration tests, producing passing results.
This confirms xUnit v3 framework discovery and execution are operational.
This scenario is tested by representative test methods across all test classes in
`test/DemaConsulting.SarifMark.Tests/`.

**XUnitV3_TrxOutputGeneration**: The `dotnet test --results-directory` invocation produces TRX result files in the
specified directory, confirming that the `xunit.runner.visualstudio` adapter serializes test results in TRX format
as required by ReqStream for traceability enforcement.
This scenario is verified by the presence of TRX result files in the CI pipeline test-results artifacts.
