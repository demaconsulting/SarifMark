## DemaConsulting.HelpCards

### Purpose

`DemaConsulting.HelpCards` is a NuGet package produced by DEMA Consulting. It turns Markdown help
cards embedded in an assembly into a validated catalog, serves a command-line `help` command from
that catalog for people and AI agents, and supplies test helpers that prove the cards are valid and
that their command examples work. SarifMark uses it in the `HelpCommand` unit to answer
`sarifmark help`.

The package was chosen because it gives the tool a short usage text that points to focused help
topics with worked examples, which a person or an AI agent can read, search, and request as JSON,
and because its build targets embed the cards in the assembly, so the .NET tool package needs no
loose files.

### Classification

Although `DemaConsulting.HelpCards` is produced by DEMA Consulting — the same organization that
produces SarifMark — it is an independently developed and separately released package with its own
requirements, versioning, and release lifecycle. SarifMark consumes it through its public API and
does not change it. It is therefore classified as an OTS software item.

### Features Used

- **`HelpSource.FromAssembly`** — exposes the `Help` folder that the package build targets embed
  in the tool assembly. Creating the source reads nothing.
- **`HelpCli.Create`**, **`HelpCli.TryRun`**, and **`HelpCli.Footer`** — the command-line helper
  that answers `help`, `help <topic>`, `--example`, `--search`, and `--json`, and the constant
  pointer sentence for the usage text.
- **`HelpCliOptions`** — `ToolName` and `Version` are set once and the helper gives both to the
  catalog builder; `ConfigureCatalog` adds the sources and settings that belong to the tool;
  `UsageLines` and `IndexPreamble` shape the topic index; `GetCatalogConfiguration` gives the
  tests the identical composition.
- **`HelpCatalog.Create`** with **`HelpCatalogBuilder.AddSource`**, **`TagVocabulary`**, and
  **`ExpectedRoots`** — builds and validates the catalog, and fixes the allowed tags and the set
  of topics.
- **`HelpCardValidator.AssertValid`** with **`HelpValidationOptions.WarningsAreErrors`** — a unit
  test proves the cards are valid, including the expected topics, the purpose tags, and the
  command lines of the examples.
- **`HelpExampleRunner.Run`**, **`HelpExampleOutcome`**, and
  **`HelpExampleRunReport.AssertSucceeded`** — a system test runs every command example against
  the built tool and compares the exit code (`exit`) and the output fragment (`expect`) that the
  card states. The runner never starts a process, so the test supplies a delegate that runs the
  built tool in the card's scratch folder.
- **MSBuild targets** shipped in the package — embed `Help/**` as resources of the tool assembly.

### Integration Pattern

`DemaConsulting.HelpCards` is referenced as a `PackageReference` in the production project
`src/DemaConsulting.SarifMark/DemaConsulting.SarifMark.csproj`; only a project with its own
package reference embeds cards. The test project receives the validator and example-runner
helpers through its project reference to the tool. The package is a pre-release, so its exact
version is pinned in the project file; an upgrade requires the card validation and
example-runner tests to pass.

`HelpCommand` is the only unit that uses the package in the shipped tool. It keeps one static
`HelpSource` and one `HelpCli`; creating them performs no I/O and needs no disposal.
`Program.Main` calls `HelpCommand.Default.TryRun` before `Context.Create`. The package answers
only when the first argument is `help`, and otherwise returns `false` without building the
catalog, so no other run reads or parses a card.

The cards use schema 2 (`schema: 2` in `_source.md`), and a command example states its result
with `exit` and `expect`. The help answer is written by the package directly to the console, not
through `Context`, so `--silent` and `--log` do not apply to the `help` command.

There are no global initialization, thread-affinity, or disposal requirements.
