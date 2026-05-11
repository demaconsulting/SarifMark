## xUnit v3 Verification

### Overview

xUnit v3 (`xunit.v3` and `xunit.runner.visualstudio` packages) is used to discover and execute
all unit and integration tests in `test/DemaConsulting.SarifMark.Tests/`. The VSTest adapter
(`xunit.runner.visualstudio`) produces TRX result files consumed by ReqStream for traceability
enforcement.

### Verification Strategy

The test framework is verified implicitly by the successful discovery and execution of the test
suite. Representative passing tests from multiple test classes confirm the framework is operational
across all test categories. TRX output is verified by the presence of the result files produced
during `dotnet test --results-directory` invocations in the build pipeline.

### Requirement Coverage

| Requirement ID | Description | Verification Evidence |
|---|---|---|
| `SarifMark-OTS-XUnitV3-Discovery` | xUnit v3 discovers and executes unit tests | `SarifResults_Read_NoResults_ReturnsValidResults` |
| `SarifMark-OTS-XUnitV3-Discovery` | xUnit v3 discovers and executes unit tests | `SarifResults_Read_WithResults_ReturnsValidResults` |
| `SarifMark-OTS-XUnitV3-Discovery` | xUnit v3 discovers and executes unit tests | `SarifResults_ToMarkdown_NoResults_ShowsFoundNoResults` |
| `SarifMark-OTS-XUnitV3-Discovery` | xUnit v3 discovers and executes unit tests | `Context_Create_VersionFlag_SetsVersionTrue` |
| `SarifMark-OTS-XUnitV3-Discovery` | xUnit v3 discovers and executes unit tests | `Context_Create_HelpFlag_SetsHelpTrue` |
| `SarifMark-OTS-XUnitV3-TrxOutput` | xUnit v3 writes TRX result files | `SarifResults_Read_NoResults_ReturnsValidResults` |
| `SarifMark-OTS-XUnitV3-TrxOutput` | xUnit v3 writes TRX result files | `SarifResults_Read_WithResults_ReturnsValidResults` |
| `SarifMark-OTS-XUnitV3-TrxOutput` | xUnit v3 writes TRX result files | `SarifResults_ToMarkdown_NoResults_ShowsFoundNoResults` |
| `SarifMark-OTS-XUnitV3-TrxOutput` | xUnit v3 writes TRX result files | `Context_Create_VersionFlag_SetsVersionTrue` |
| `SarifMark-OTS-XUnitV3-TrxOutput` | xUnit v3 writes TRX result files | `Context_Create_HelpFlag_SetsHelpTrue` |
