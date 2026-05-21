### PathHelpers Unit Verification

#### Verification Approach

`PathHelpers` is a stateless static class. Tests call `PathHelpers.SafePathCombine` directly with various argument
combinations covering the normal path, null-argument boundary, and absolute-path-escape boundary. No mocking, no
injected dependencies, and no shared file-system state is required between tests.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. The Windows-specific absolute path test (`C:\Windows\System32`)
is wrapped in `OperatingSystem.IsWindows()` and is skipped on non-Windows platforms. No external services or
configuration are required.

#### Acceptance Criteria

All `PathHelpersTests` test methods pass, confirming that valid relative path combinations return the correct result
and that boundary conditions (null base path, absolute path escape) throw the correct exceptions. No `PathHelpers`
unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

- `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully`: Call with a valid base path and relative child path;
  assert the combined result equals the expected absolute path.
- `PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`: Pass `null` as the base path; assert
  `ArgumentNullException` is thrown.
- `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully` (post-combine absolute check): Call with a child
  segment that resolves outside the base path (e.g. an absolute path); assert an exception is thrown to prevent
  path traversal.

#### Overview

The `PathHelpers` class is verified by the `PathHelpersTests` test class in
`test/DemaConsulting.SarifMark.Tests/Utilities/PathHelpersTests.cs`. `PathHelpers` is a stateless static class; all
tests call `PathHelpers.SafePathCombine` directly. No mocking is required.

#### Isolation Strategy

The Windows-specific absolute path check (`C:\Windows\System32`) is wrapped in `OperatingSystem.IsWindows()` in the
test code, ensuring it is skipped on non-Windows platforms.

#### Requirement Coverage

- **`SarifMark-PathHelpers-SafeCombine`**: Relative combine —
  `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully`
- **`SarifMark-PathHelpers-NullCheck`**: Null arg —
  `PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`
- **`SarifMark-PathHelpers-PostCombineCheck`**: Path check —
  `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully`
