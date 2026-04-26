# Utilities Subsystem

The `Utilities` subsystem provides shared utility functions for SarifMark.
It supplies reusable, independently testable helpers that are consumed by other subsystems.

## Overview

The `Utilities` subsystem contains general-purpose helpers that do not belong to any
specific feature subsystem. Its primary responsibility is safe file-path manipulation,
protecting callers from path-traversal vulnerabilities when constructing paths from
external inputs.

## Units

The `Utilities` subsystem contains the following software unit:

| Unit          | File                       | Responsibility                              |
|---------------|----------------------------|---------------------------------------------|
| `PathHelpers` | `Utilities/PathHelpers.cs` | Safe path combination and traversal checks. |

## Interfaces

The `Utilities` subsystem exposes the following interface to the rest of the tool:

| Interface                     | Direction | Description                                         |
|-------------------------------|-----------|-----------------------------------------------------|
| `PathHelpers.SafePathCombine` | Outbound  | Combines two path segments, rejecting traversal.    |

**Error contract for `SafePathCombine`**: throws `ArgumentNullException` when either argument
is `null`; throws `ArgumentException` when the resolved path escapes the base directory or
contains an invalid path component.

## Interactions

`PathHelpers` has no dependencies on other tool units or subsystems. It uses only .NET base
class library types (`Path`, `ArgumentNullException`).

## Class Details

- **PathHelpers class** — safe path combination utilities
