## Utilities

### Verification Approach

The `Utilities` subsystem is verified through direct unit tests of the `PathHelpers` static class. Tests are defined in
`test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs` and call `PathHelpers.SafePathCombine` directly. The
subsystem has no dependencies on other tool subsystems; `PathHelpers` is a stateless class using only BCL types, so no
mocking is required at any level.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external services, files, or configuration beyond the standard test
runner are required. Platform-specific tests are guarded by `OperatingSystem.IsWindows()` and are skipped on
non-Windows platforms.

### Acceptance Criteria

- All `UtilitiesTests` test methods pass.
- Valid relative path combinations succeed.
- Boundary conditions (null arguments, absolute-path escapes) throw the correct exceptions.
- No `Utilities` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully**: Call `PathHelpers.SafePathCombine` with a valid base
path and a relative child path; assert the combined path equals the expected value.
This scenario is tested by `Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully`.

### Requirements Coverage

- **`SarifMark-Utilities-SafePathHandling`**: `Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully`
