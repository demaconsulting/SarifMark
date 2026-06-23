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
and that boundary conditions (null base path, absolute path escape, directory traversal) throw the correct exceptions.
No `PathHelpers` unit requirement may remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully**: Call with a valid base path and relative child
path; assert the combined result equals the expected absolute path.
This scenario is tested by `PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_SimpleFilename_CombinesSuccessfully**: Call with a valid base path and a simple filename
(e.g. `file.txt`); assert the combined result equals the expected path, confirming that single-component relative
paths are accepted.
This scenario is tested by `PathHelpers_SafePathCombine_SimpleFilename_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_PathWithSubdirectories_CombinesSuccessfully**: Call with a valid base path and a
multi-component relative path (e.g. `documents/work/report.pdf`); assert the combined result equals the expected path,
confirming that relative paths with multiple directory components are accepted.
This scenario is tested by `PathHelpers_SafePathCombine_PathWithSubdirectories_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_GuidBasedFilename_CombinesSuccessfully**: Call with a temp directory base path and a
GUID-based relative filename (e.g. `test-{guid}.tmp`); assert the combined result equals the expected path,
confirming that dynamically generated filenames are accepted.
This scenario is tested by `PathHelpers_SafePathCombine_GuidBasedFilename_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_FilenameWithEmbeddedDots_CombinesSuccessfully**: Call with a valid base path and a
relative path containing embedded `..` as a substring (e.g. `v1..0.sarif`); assert the combined result equals the
expected path, confirming that filenames containing `..` as part of the name (not as a traversal segment) are
accepted.
This scenario is tested by `PathHelpers_SafePathCombine_FilenameWithEmbeddedDots_CombinesSuccessfully`.

**PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException**: Pass `null` as the base path; assert
`ArgumentNullException` is thrown.
This scenario is tested by `PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException**: Pass `null` as the relative path;
assert `ArgumentNullException` is thrown with `ParamName` equal to `relativePath`.
This scenario is tested by `PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException**: Pass a child segment that is an absolute path
(e.g. `/etc/passwd` on Unix or `C:\Windows\System32` on Windows); assert `ArgumentException` is thrown to prevent
escape from the base directory, confirming that absolute-path inputs are rejected.
This scenario is tested by `PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_PathWithParentDirectory_ThrowsArgumentException**: Pass a relative child
path that uses `..` segments to escape the base directory (e.g., `../etc/passwd`); assert
`ArgumentException` is thrown with `ParamName` equal to `relativePath` and a message containing
`"Invalid path component"`, confirming that directory traversal outside the base directory is rejected.
This scenario is tested by `PathHelpers_SafePathCombine_PathWithParentDirectory_ThrowsArgumentException`.
