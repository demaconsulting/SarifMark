### SarifFinding

#### Purpose

`SarifFinding` is an immutable record that represents a single static analysis finding
extracted from a SARIF file. It carries the minimum set of fields needed to identify,
describe, and locate a finding in a generated markdown report.

#### Data Model

**RuleId**: `string` — The rule identifier for the finding, e.g. `"CA1234"`. Always
present; sourced from the SARIF result's `ruleId` property.

**Level**: `string` — The severity level of the finding; one of `"error"`, `"warning"`,
or `"note"`. Always present; sourced from the SARIF result's `level` property.

**Message**: `string` — Descriptive message text for the finding. Always present; sourced
from the SARIF result's `message.text` property.

**Uri**: `string?` — File URI where the finding was located; `null` when the SARIF result
contains no location data. Sourced from the first element of the result's `locations` array.

**StartLine**: `int?` — Starting line number within the file; `null` when the SARIF result
contains no physical location data. `Uri` and `StartLine` are both nullable because the
SARIF specification does not require location data for every result.

#### Key Methods

N/A - `SarifFinding` is a positional record with no public instance methods beyond the
auto-generated equality and deconstruction members. All construction is performed by the
`SarifResults.ParseResults` method.

#### Error Handling

`SarifFinding` performs no validation in its constructor; all field validation is the
responsibility of `SarifResults.ParseResults`, which constructs `SarifFinding` instances
only after verifying the relevant JSON properties are present.

The internal constructor is not called directly by external consumers; the project file
includes `<InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" />` to allow the
test assembly to construct instances directly for unit testing.

#### Dependencies

N/A - `SarifFinding` has no dependencies on other units, subsystems, or OTS items.

#### Callers

- **SarifResults** — constructs `SarifFinding` instances inside `ParseResults` during the
  SARIF file parsing pipeline.
