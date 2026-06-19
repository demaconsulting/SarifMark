## TestResults

`DemaConsulting.TestResults` is the shared package used by the SarifMark self-validation feature
to collect individual test outcomes and serialize them to standard result formats for CI pipeline
consumption.

### Advertised Features Consumed

- **TestResults collection** — a named container for individual test outcomes; SarifMark creates
  one collection named "SarifMark Self-Validation" to aggregate all self-validation results.
- **TestResult entry** — an individual test record carrying name, class name, code base, outcome
  (Passed or Failed), optional error message, and duration; one entry is created per
  self-validation scenario.
- **TrxSerializer** — serializes a `TestResults` collection to TRX format, enabling the results
  file to be consumed by Visual Studio Test Explorer and CI pipeline tooling.
- **JUnitSerializer** — serializes a `TestResults` collection to JUnit XML format for consumption
  by CI pipeline tooling that expects JUnit-format reports.

### Integration Pattern

`DemaConsulting.TestResults` is referenced as a runtime NuGet dependency in the
`DemaConsulting.SarifMark` project file. It is consumed directly in `SelfTest/Validation.cs`:

1. At the start of a validation run, a `DemaConsulting.TestResults.TestResults` object is created
   with the name "SarifMark Self-Validation".
2. Before each self-validation scenario executes, a `DemaConsulting.TestResults.TestResult` is
   created with the test name, class, and code base.
3. When the scenario completes or throws an exception, the test result's outcome is set to
   `DemaConsulting.TestResults.TestOutcome.Passed` or `TestOutcome.Failed`, and an optional error
   message is recorded.
4. The elapsed duration is computed from the wall-clock start time and the result is appended to
   the collection via `testResults.Results.Add(test)`.
5. If a results file path was supplied on the command line (via `--results`), the collection is
   serialized using `TrxSerializer.Serialize` (for `.trx` files) or `JUnitSerializer.Serialize`
   (for `.xml` files) from the `DemaConsulting.TestResults.IO` namespace and written to disk.

No wrapper class is introduced; the package's public types are used directly.

### Assumptions

- The `TestResults` and `TestResult` types are mutable; properties may be set after construction.
- `TestOutcome.Passed` and `TestOutcome.Failed` are the only outcome values used by SarifMark.
- `TrxSerializer.Serialize` and `JUnitSerializer.Serialize` return complete, well-formed
  serialized strings; no post-processing is required before writing to disk.
- The serializers do not throw exceptions for a well-formed `TestResults` collection.
