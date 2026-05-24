## xUnit v3

xUnit v3 (`xunit.v3` and `xunit.runner.visualstudio`) is the unit-testing framework used
by the `DemaConsulting.SarifMark.Tests` test project. It discovers and executes all test
methods, and its VSTest adapter writes TRX result files consumed by ReqStream for
requirements traceability.

### Purpose

xUnit v3 was chosen for its first-class support for .NET, its clean attribute-based test
declaration model, and its compatibility with the VSTest adapter required by ReqStream for
TRX output. The v3 release provides improved performance and native support for modern
.NET target frameworks, including .NET 8, 9, and 10, which are all targeted by SarifMark.

### Features Used

- **Test discovery and execution** — xUnit v3 discovers all methods marked with `[Fact]`
  or `[Theory]` attributes in the test assembly and executes them as individual test cases.
- **TRX result file output** — the `xunit.runner.visualstudio` adapter writes TRX result
  files when tests are run via `dotnet test`, providing the test evidence consumed by
  ReqStream.
- **Theory parameterization** — `[Theory]` with `[InlineData]` is used to express
  parameterized test cases without requiring separate test methods.

### Integration Pattern

xUnit v3 is consumed as a NuGet package referenced by the test project:

1. `xunit.v3` is declared as a `PackageReference` in
   `test/DemaConsulting.SarifMark.Tests/DemaConsulting.SarifMark.Tests.csproj`.
2. `xunit.runner.visualstudio` is declared as a `PackageReference` in the same project
   to enable VSTest-compatible execution and TRX output.
3. Tests are executed by `dotnet test` as part of the CI pipeline build step.
4. The TRX output file is written to the configured results path and subsequently
   consumed by ReqStream for traceability enforcement.

No configuration beyond the NuGet package references is required.
