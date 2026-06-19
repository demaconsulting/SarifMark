### PathHelpers

#### Verification Approach

`PathHelpers` is a stateless static class. Tests call `PathHelpers.SafePathCombine` directly with various argument
combinations covering the normal path, null-argument boundary, and absolute-path-escape boundary. No mocking, no
injected dependencies, and no shared file-system state is required between tests. The Windows-specific absolute
path test (`C:\Windows\System32`) is wrapped in `OperatingSystem.IsWindows()` in the test code, ensuring it is
skipped on non-Windows platforms.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. The Windows-specific absolute path test (`C:\Windows\System32`)
is wrapped in `OperatingSystem.IsWindows()` and is skipped on non-Windows platforms. No external services or
configuration are required.

#### Acceptance Criteria

All `PathHelpersTests` test methods pass, confirming that valid relative path combinations return the correct result
and that boundary conditions (null base path, absolute path escape) throw the correct exceptions. No `PathHelpers`
unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully**: Call with a valid base path and relative child
path; assert the combined result equals the expected absolute path.
This scenario is tested by `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException**: Pass `null` as the base path; assert
`ArgumentNullException` is thrown.
This scenario is tested by `PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_AbsoluteChildPath_ThrowsException**: Pass a child segment that resolves outside the
base path (e.g. an absolute path); assert an exception is thrown to prevent path traversal.
This scenario is tested by `PathHelpers_SafePathCombine_AbsoluteChildPath_ThrowsException`.
