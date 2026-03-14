# Utilities

## Overview

The utilities layer provides an internal support class used across the tool: `PathHelpers`
for safe path operations. It defends against path-traversal vulnerabilities when
constructing file paths from partially-trusted input.

## PathHelpers Class

The `PathHelpers` class (`PathHelpers.cs`) exposes a single static method,
`SafePathCombine`, designed to guard against path-traversal attacks when building file
paths from input that may not be fully trusted.

### SafePathCombine Method

`SafePathCombine` takes a `basePath` and a `relativePath` and returns a combined path,
subject to two layers of validation:

1. **Pre-combination check**: rejects `relativePath` if it contains `".."` or is a rooted
   (absolute) path.
2. **Post-combination check**: resolves both paths with `Path.GetFullPath` and calls
   `Path.GetRelativePath` to verify the combined path still sits under `basePath`.

If either check fails, `ArgumentException` is thrown. This defense-in-depth approach
guards against edge-cases that might bypass the initial string check while remaining
straightforward to audit.

`PathHelpers` is used by `Validation` when constructing paths inside temporary directories
for self-validation tests.
