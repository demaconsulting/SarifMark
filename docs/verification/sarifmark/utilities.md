## Utilities

### Verification Approach

The `Utilities` subsystem is verified through direct unit tests of the `PathHelpers` static class. Tests are defined in
`test/DemaConsulting.SarifMark.Tests/Utilities/UtilitiesTests.cs` and call `PathHelpers.SafePathCombine` directly. The
subsystem has no dependencies on other tool subsystems; `PathHelpers` is a stateless class using only BCL types, so no
mocking is required at any level.

### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No external services, files, or configuration beyond the standard test
runner are required. All tests are cross-platform by design and require no runtime platform guards, relying on
.NET's `Path.Combine`, `Path.GetFullPath`, and `Path.GetRelativePath` cross-platform semantics.

### Acceptance Criteria

- All `UtilitiesTests` test methods pass.
- Valid relative path combinations succeed.
- Boundary conditions (null arguments, absolute-path escapes) throw the correct exceptions.
- No `Utilities` subsystem requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

### Test Scenarios

**Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully**: Call `PathHelpers.SafePathCombine` with a valid base
path and a relative child path; assert the combined path equals the expected value.
This scenario is tested by `Utilities_SafePathHandling_ValidPaths_CombinesSuccessfully`.

**Utilities_SafePathHandling_PathTraversal_ThrowsException**: Call `PathHelpers.SafePathCombine` with a path traversal
component (`../../../etc/passwd`); assert `ArgumentException` is thrown, confirming path traversal attacks are rejected
at the subsystem boundary.
This scenario is tested by `Utilities_SafePathHandling_PathTraversal_ThrowsException`.

**Utilities_SafePathHandling_AbsolutePath_ThrowsException**: Call `PathHelpers.SafePathCombine` with an absolute child
path (`/etc/passwd`); assert `ArgumentException` is thrown, confirming absolute path injection is rejected. The
Windows-specific case (`C:\Windows\System32`) is only exercised on Windows; the Unix-style absolute path
(`/etc/passwd`) is exercised on all platforms.
This scenario is tested by `Utilities_SafePathHandling_AbsolutePath_ThrowsException`.

**Utilities_SafePathHandling_NullRelativePath_ThrowsException**: Call `PathHelpers.SafePathCombine` with a `null`
relative path; assert `ArgumentNullException` is thrown, confirming null-argument validation works at the subsystem
boundary.
This scenario is tested by `Utilities_SafePathHandling_NullRelativePath_ThrowsException`.
