## Microsoft.Extensions.FileSystemGlobbing

### Purpose

`Microsoft.Extensions.FileSystemGlobbing` is a NuGet package produced by Microsoft. It provides a
glob-pattern matching engine (`Matcher`) that determines whether a candidate path matches one or
more include patterns (including `*`, `?`, and recursive `**` wildcards). SarifMark uses it to
implement the `--exclude` CLI option, which removes SARIF findings whose `Uri` matches a
user-supplied glob pattern before enforcement and report generation.

The package was chosen because it is the Microsoft-owned globbing engine already used throughout
the .NET ecosystem (MSBuild item globs, `dotnet watch`, ASP.NET Core static file providers), which
avoids hand-rolling glob parsing and matching logic and gives users glob semantics they are
already familiar with.

### Classification

`Microsoft.Extensions.FileSystemGlobbing` is produced and released independently by Microsoft as
part of the `Microsoft.Extensions` family of packages. It is not part of the SarifMark product and
its internal design requirements do not drive SarifMark requirements. It is therefore classified
as an OTS software item.

### Features Used

- **`Matcher`** — the matching engine. One instance is constructed per call to
  `SarifResults.Exclude`, with one `AddInclude(pattern)` call per supplied `--exclude` pattern.
- **`AddInclude(string pattern)`** — registers a glob pattern that a candidate path must match.
- **`MatcherExtensions.Match(this Matcher, string searchDirectory, IEnumerable<string> files)`** —
  the in-memory matching overload used to test a single candidate `Uri` against the registered
  patterns. This overload never touches the real filesystem; it evaluates the supplied strings
  directly, so SARIF `Uri` values do not need to correspond to files that exist on disk.

### Integration Pattern

`Microsoft.Extensions.FileSystemGlobbing` is referenced as a `PackageReference` in the production
project `src/DemaConsulting.SarifMark/DemaConsulting.SarifMark.csproj`. No initialization or
configuration is required; the `Matcher` type is used directly.

Inside `SarifResults.Exclude`, a single `Matcher` is constructed and one `AddInclude` call is made
per pattern in the supplied glob list. Each finding's non-null `Uri` is then tested individually
against the matcher using a fixed root of `"/"` (via `matcher.Match("/", [uri]).HasMatches`) —
this root value was chosen because it was empirically observed to consistently match relative
paths, `file://` URIs, Unix absolute paths, and Windows drive-letter absolute paths (with either
`/` or `\` separators), whereas the no-root overload resolves paths relative to the current
working directory and silently fails to match absolute paths outside it. Findings are matched one
at a time (rather than batched) because batch matching was observed to mangle Windows
drive-letter prefixes in the returned `Path`/`Stem` values, making result correlation unreliable.
Findings with a `null` `Uri` are always retained and are never passed to the matcher. Matching is
case-insensitive by default (the default `Matcher()` constructor does not specify a
`StringComparison`), which was confirmed during implementation and is documented as the observed
behavior rather than assumed.

`Program.ProcessSarifAnalysis` invokes `SarifResults.Exclude` only when `context.ExcludeGlobs` is
non-empty. No error handling is required beyond what `Matcher`/`AddInclude` already provide;
malformed glob patterns do not throw during registration, and unmatched patterns simply result in
no findings being excluded.

There are no global initialization, thread-affinity, or disposal requirements.
