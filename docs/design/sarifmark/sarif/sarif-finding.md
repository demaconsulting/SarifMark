# SarifFinding Record

## Overview

The `SarifFinding` record (`SarifFinding.cs`) is an immutable value type that represents a single
static analysis finding extracted from a SARIF file. It carries the minimum set of fields needed
to identify, describe, and locate a finding in a generated markdown report. The fully qualified
type name is `DemaConsulting.SarifMark.SarifFinding`.

## Record Design

`SarifFinding` is declared as a `record`, making all instances immutable by default. The constructor
is `internal`, so the type cannot be directly instantiated by consumers outside the
`DemaConsulting.SarifMark` assembly; instances are only produced by the `SarifResults.Read`
parsing pipeline. This satisfies requirement `SarifMark-SarifFinding-Internal`.

## Properties

| Property    | Type      | Optional | Description                                        |
|-------------|-----------|----------|----------------------------------------------------|
| `RuleId`    | `string`  | No       | The rule identifier for the finding                |
| `Level`     | `string`  | No       | Severity level (error, warning, or note)           |
| `Message`   | `string`  | No       | Descriptive message text for the finding           |
| `Uri`       | `string?` | Yes      | File URI where the finding was found, if available |
| `StartLine` | `int?`    | Yes      | Starting line number within the file, if available |

`Uri` and `StartLine` are both nullable because the SARIF specification does not require location
data for every result. Their optionality satisfies requirements `SarifMark-SarifFinding-Uri` and
`SarifMark-SarifFinding-StartLine`. The full set of properties satisfies requirement `SarifMark-SarifFinding-Properties`.

## Construction

The internal constructor accepts all five properties in positional order and assigns them directly.
Consumers outside the assembly obtain `SarifFinding` instances only through `SarifResults.Read`,
which validates and parses the source SARIF JSON before constructing each record. This ensures that
every `SarifFinding` in circulation has been produced through the validated parsing pipeline.

The `DemaConsulting.SarifMark` project file includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />`,
which grants the test assembly access to the internal constructor. This enables direct unit testing
of the constructor without relaxing the access restriction for all external consumers.

## Cross-References

See the SarifResults Record document for `SarifResults.Read`, which constructs `SarifFinding`
instances during SARIF file parsing.
