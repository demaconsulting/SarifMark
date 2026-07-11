## Utilities

![Utilities Structure](UtilitiesView.svg)

The `Utilities` subsystem provides shared utility functions for SarifMark. It supplies
reusable, independently testable helpers that do not belong to any specific feature
subsystem.

### Overview

The `Utilities` subsystem contains general-purpose helpers that are consumed by other
subsystems. Its primary responsibility is safe file-path manipulation, protecting callers
from path-traversal vulnerabilities when constructing paths from externally supplied inputs.
The subsystem contains a single unit:

- **PathHelpers**: a static utility class providing a safe path-combination method that
  verifies the resolved path stays within the base directory.

### Interfaces

**PathHelpers.SafePathCombine**: Combines two path segments safely, rejecting traversal.

- *Type*: In-process .NET static method
- *Role*: Provider
- *Contract*: Accepts `string basePath` and `string relativePath`; combines them with
  `Path.Combine`; resolves both the base and the combined path to absolute form; verifies
  the combined path does not escape the base directory; returns the non-resolved combined
  path on success.
- *Constraints*: Both arguments must be non-null (throws `ArgumentNullException`). The
  resolved combined path must not escape the base directory; violations throw
  `ArgumentException` identifying `relativePath` as the problematic parameter.

### Design

The `Utilities` subsystem contains a single unit (`PathHelpers`) so there is no inter-unit
data flow. `PathHelpers.SafePathCombine` is a pure utility method with no side effects:

1. Both arguments are validated for null using `ArgumentNullException.ThrowIfNull`.
2. `Path.Combine(basePath, relativePath)` produces the candidate combined path.
3. Both `basePath` and the candidate are resolved to absolute form with `Path.GetFullPath`.
4. `Path.GetRelativePath(absoluteBase, absoluteCombined)` is called; if the result is
   `".."`, starts with `"..` followed by a directory separator, or is itself rooted
   (absolute), the combined path escapes the base and `ArgumentException` is thrown.
5. On success, the non-resolved combined path (the direct result of step 2) is returned.
