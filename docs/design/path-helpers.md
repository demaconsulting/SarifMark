# PathHelpers Class

## Overview

The `PathHelpers` class (`PathHelpers.cs`) is a static internal utility class that provides safe
path operations for use within the tool. It guards against path-traversal vulnerabilities when
constructing file paths from input that may not be fully trusted.

## SafePathCombine Method

`SafePathCombine(string basePath, string relativePath)` combines a base directory path with a
relative path and returns the result, subject to two layers of validation. It is used by the
`TemporaryDirectory` helper inside `Validation` when constructing paths inside a temporary
directory from `Guid`-based file names.

### Null Checks

Both `basePath` and `relativePath` are validated with `ArgumentNullException.ThrowIfNull` before
any other processing. This satisfies requirement `SarifMark-PH-NullCheck`.

### Pre-Combination Check

Before calling `Path.Combine`, the method inspects `relativePath` directly:

- If `relativePath` contains the string `".."`, `ArgumentException` is thrown.
- If `Path.IsPathRooted(relativePath)` returns `true`, `ArgumentException` is thrown.

These checks reject the most common path-traversal patterns before any file-system operations
are performed. This satisfies requirements `SarifMark-PH-PreCombineCheck` and
`SarifMark-PH-RootedCheck`.

### Path Combination

`Path.Combine(basePath, relativePath)` is called after the pre-combination checks pass. Because
`relativePath` has been verified to contain no `".."` segments and is not rooted, the combined
path is expected to remain under `basePath`.

### Post-Combination Check

As a defense-in-depth measure, the method resolves both paths using `Path.GetFullPath` and then
calls `Path.GetRelativePath(fullBasePath, fullCombinedPath)`. If the resulting relative string
starts with `".."` or is itself rooted, `ArgumentException` is thrown. This second check catches
any edge cases that might survive the pre-combination string test on a given operating system.
This satisfies requirement `SarifMark-PH-PostCombineCheck`.

### Return Value

On success, the non-resolved combined path (the direct result of `Path.Combine`) is returned.
This satisfies requirement `SarifMark-PH-SafeCombine`.

## Cross-References

See [validation.md] for the `TemporaryDirectory` nested class that calls `SafePathCombine`.

[validation.md]: validation.md
