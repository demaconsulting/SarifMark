# Utilities

## Overview

The utilities layer provides internal support classes used across the tool. It currently
consists of a single class: `PathHelpers`, which defends against
path-traversal vulnerabilities when constructing file paths from partially-trusted input.

## PathHelpers

`PathHelpers` is a static internal class exposing a single method, `SafePathCombine`. It
is used by `Validation.TemporaryDirectory` when constructing paths inside a temporary
directory from `Guid`-based file names.

`SafePathCombine` applies two layers of validation to guard against path-traversal attacks:

1. **Pre-combination check** — rejects paths containing `".."` or rooted (absolute) paths.
2. **Post-combination check** — resolves both paths with `Path.GetFullPath` and verifies
   the result still sits under the base path.

See the PathHelpers Class document for class-level details.

## Class Details

- **PathHelpers class** — safe path combination with traversal defense
