### PathHelpers

![Utilities Structure](UtilitiesView.svg)

#### Purpose

`PathHelpers` is a static internal utility class that provides a single method,
`SafePathCombine`, for combining two path segments while verifying that the result
remains within the base directory. It protects callers from string-level path-traversal
attacks by resolving both paths to absolute form and checking containment before
returning the combined result.

#### Data Model

N/A - `PathHelpers` is a `static` class with no instance or static fields.

#### Key Methods

**SafePathCombine**: Combines two path segments and rejects traversal.

- *Parameters*: `string basePath` — the base directory path; `string relativePath` —
  the relative path to append
- *Returns*: `string` — the non-resolved combined path (direct result of `Path.Combine`)
- *Preconditions*: Both `basePath` and `relativePath` are non-null.
- *Postconditions*: The resolved combined path remains within the resolved base directory.
  Returns the direct (non-resolved) result of `Path.Combine(basePath, relativePath)`.

The method validates both arguments for null using `ArgumentNullException.ThrowIfNull`,
combines them with `Path.Combine`, resolves both the base and the candidate to absolute
form with `Path.GetFullPath`, calls `Path.GetRelativePath(absoluteBase, absoluteCombined)`,
and rejects the result if it is `".."`, starts with `".."` followed by a directory
separator, or is itself rooted (absolute). Valid paths are returned as the direct result
of `Path.Combine`.

The use of `Path.GetRelativePath` for containment checking handles root paths, platform
case-sensitivity, and directory-separator normalization natively, without fragile
pre-combine string inspection of `relativePath`.

#### Error Handling

`SafePathCombine` throws `ArgumentNullException` when either `basePath` or `relativePath`
is null. It throws `ArgumentException` identifying `relativePath` as the problematic
parameter when the resolved combined path escapes the base directory (traversal detected)
or contains an invalid path component. No logging or error accumulation is performed; the
method is a pure utility that throws on invalid input.

#### Dependencies

- **.NET base class library** — `Path.Combine`, `Path.GetFullPath`, `Path.GetRelativePath`,
  `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`,
  `ArgumentNullException.ThrowIfNull`, `ArgumentException`.

#### Callers

- **Validation** — calls `PathHelpers.SafePathCombine` inside the `TemporaryDirectory`
  nested class to construct file paths within the temporary directory from GUID-based names.
