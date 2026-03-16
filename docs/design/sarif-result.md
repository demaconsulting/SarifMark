# SarifResult Record

## Overview

The `SarifResult` record (`SarifResult.cs`) is an immutable value type that represents a single
static analysis finding extracted from a SARIF file. It carries the minimum set of fields needed
to identify, describe, and locate a result in a generated markdown report.

## Record Design

`SarifResult` is declared as a `record`, making all instances immutable by default. The constructor
is `internal`, so the type cannot be directly instantiated by external consumers; instances are
only produced by the `SarifResults.Read` parsing pipeline. This satisfies requirement
`SarifMark-SR-Internal`.

## Properties

| Property    | Type      | Optional | Description                                        |
|-------------|-----------|----------|----------------------------------------------------|
| `RuleId`    | `string`  | No       | The rule identifier for the result                 |
| `Level`     | `string`  | No       | Severity level (error, warning, or note)           |
| `Message`   | `string`  | No       | Descriptive message text for the result            |
| `Uri`       | `string?` | Yes      | File URI where the result was found, if available  |
| `StartLine` | `int?`    | Yes      | Starting line number within the file, if available |

`Uri` and `StartLine` are both nullable because the SARIF specification does not require location
data for every result. Their optionality satisfies requirements `SarifMark-SR-Uri` and
`SarifMark-SR-StartLine`. The full set of properties satisfies requirement `SarifMark-SR-Properties`.

## Construction

The internal constructor accepts all five properties in positional order and assigns them directly.
Consumers outside the assembly obtain `SarifResult` instances only through `SarifResults.Read`,
which validates and parses the source SARIF JSON before constructing each record. This ensures that
every `SarifResult` in circulation has been produced through the validated parsing pipeline.

## Cross-References

See the SarifResults Record document for `SarifResults.Read`, which constructs `SarifResult`
instances during SARIF file parsing.
