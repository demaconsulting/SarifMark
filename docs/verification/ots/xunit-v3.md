## xUnit v3

### Verification Approach

xUnit v3 (`xunit.v3` and `xunit.runner.visualstudio` packages) is used to discover and execute all unit and
integration tests in `test/DemaConsulting.SarifMark.Tests/`. The VSTest adapter (`xunit.runner.visualstudio`)
produces TRX result files consumed by ReqStream for traceability enforcement. The test framework is verified
implicitly by the successful discovery and execution of the test suite. Representative passing tests from multiple
test classes confirm the framework is operational across all test categories. TRX output is verified by the presence
of the result files produced during `dotnet test --results-directory` invocations in the build pipeline.

### Test Scenarios

**XUnitV3_TestDiscovery**: xUnit v3 discovers all test methods marked with [Fact] or [Theory] across all test classes
in `test/DemaConsulting.SarifMark.Tests/`, confirming the framework's test discovery mechanism is operational.
This scenario is evidenced by the successful collection and reporting of all test method names in `dotnet test` output.

**XUnitV3_TestExecution**: xUnit v3 executes all discovered test methods and produces pass/fail results, confirming
that test execution and result reporting work correctly across all test categories (unit, subsystem, and integration
tests).
This scenario is evidenced by representative passing tests across all test classes.

**XUnitV3_TrxOutputGeneration**: The `dotnet test --results-directory` invocation produces TRX result files in the
specified directory, confirming that the `xunit.runner.visualstudio` adapter serializes test results in TRX format
as required by ReqStream for traceability enforcement.
This scenario is verified by the presence of TRX result files in the CI pipeline test-results artifacts.
