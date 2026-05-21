## Utilities Subsystem Verification

### Verification Strategy

The `Utilities` subsystem is verified through direct unit tests of the `PathHelpers` static class. Tests are defined
in `test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs` and call `PathHelpers.SafePathCombine` directly.
The subsystem has no dependencies on other tool subsystems; `PathHelpers` is a stateless class using only BCL types,
so no mocking is required at any level.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external services, files, or configuration beyond the standard
test runner are required. Platform-specific tests are guarded by `OperatingSystem.IsWindows()` and are skipped on
non-Windows platforms.

### Acceptance Criteria

All `UtilitiesTests` test methods pass, confirming that valid relative path combinations succeed and that boundary
conditions (null arguments, absolute-path escapes) throw the correct exceptions. No `Utilities` subsystem requirement
may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

- `Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully`: Call `PathHelpers.SafePathCombine` with a valid base
  path and a relative child path; assert the combined path equals the expected value.

### Overview

The `Utilities` subsystem is verified by the `UtilitiesTests` test class in
`test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs`. The subsystem has no dependencies on other tool
subsystems; `PathHelpers` is a stateless static class using only BCL types. No mocking is required at any level.

### Requirement Coverage

| Requirement ID | Description | Test Scenario(s) |
| --- | --- | --- |
| `SarifMark-Utilities-SafePathHandling` | Safe combine | `Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully` |
