# PathHelpers Class

## Overview

`PathHelpers` is a static internal utility class that provides a safe path-combination method. It
protects callers against path-traversal attacks by verifying the resolved combined path stays
within the base directory. Note that `Path.GetFullPath` normalizes `.`/`..` segments but does
not resolve symlinks or reparse points, so this check guards against string-level traversal
only.

## SafePathCombine Method

```csharp
internal static string SafePathCombine(string basePath, string relativePath)
```

Combines `basePath` and `relativePath` safely, ensuring the resulting path remains within
the base directory. It is used by the `TemporaryDirectory` helper inside `Validation` when
constructing paths inside a temporary directory from `Guid`-based file names.

### Null Checks

Both `basePath` and `relativePath` are validated with `ArgumentNullException.ThrowIfNull` before
any other processing. This satisfies requirement `SarifMark-PathHelpers-NullCheck`.

### Path Combination

`Path.Combine(basePath, relativePath)` is called to produce the candidate path, preserving
the caller's relative/absolute style.

### Post-Combination Security Check

The method resolves both `basePath` and the candidate to absolute form with `Path.GetFullPath`,
then calls `Path.GetRelativePath(absoluteBase, absoluteCombined)` and rejects the input if
the result is exactly `".."`, starts with `".."` followed by `Path.DirectorySeparatorChar`
or `Path.AltDirectorySeparatorChar`, or is itself rooted (absolute). These conditions indicate
the combined path escapes the base directory. This satisfies requirement
`SarifMark-PathHelpers-PostCombineCheck`.

### Return Value

On success, the non-resolved combined path (the direct result of `Path.Combine`) is returned.
This satisfies requirement `SarifMark-PathHelpers-SafeCombine`.

## Design Decisions

- **`Path.GetRelativePath` for containment check**: Using `GetRelativePath` to verify
  containment handles root paths (e.g. `/`, `C:\`), platform case-sensitivity, and
  directory-separator normalization natively. The containment test treats `..` as an
  escaping segment only when it is the entire relative result or is followed by a directory
  separator, avoiding false positives for valid in-base names such as `..data`.
- **Post-combine canonical-path check**: Resolving paths after combining handles all traversal
  patterns — `../`, embedded `/../`, absolute-path overrides, and platform edge cases —
  without fragile pre-combine string inspection of `relativePath`.
- **ArgumentException on invalid input**: Callers receive a specific `ArgumentException`
  identifying `relativePath` as the problematic parameter, making debugging straightforward.
- **No logging or error accumulation**: `SafePathCombine` is a pure utility method that throws
  on invalid input; it does not interact with the `Context` or any output mechanism.

## Cross-References

See the Self-Validation document for the `TemporaryDirectory` nested class that calls
`SafePathCombine`.
