## DemaConsulting.HelpCards

### Purpose

`DemaConsulting.HelpCards` is a NuGet package produced by DEMA Consulting. It provides the built-in
help of the tool: it turns Markdown help cards embedded in the tool assembly into a validated
catalog, serves `sarifmark help` from that catalog for people and AI agents, and supplies the test
helpers that prove the cards are valid and that their examples work.

### Classification

Like the other DEMA Consulting tools that SarifMark uses, `DemaConsulting.HelpCards` is produced by
another repository with its own requirements and release lifecycle, and SarifMark does not change
it. It is therefore classified as an OTS software item.

### Features Used

- **`HelpSource.FromAssembly`** — exposes the `Help` folder that the package build targets embed
  in the tool assembly.
- **`HelpCli`** and **`HelpCliOptions`** — the command-line helper that answers the `help` command,
  and the pointer sentence (`Footer`) for the usage text. `GetCatalogConfiguration` gives the tests
  the composition that the tool ships.
- **`HelpCatalogBuilder`** — `AddSource`, `TagVocabulary`, and `ExpectedRoots` fix the cards, the
  allowed tags, and the set of topics.
- **`HelpCardValidator`** and **`HelpExampleRunner`** — test helpers that validate the cards and
  run every command example against the built tool.
- **MSBuild targets** shipped in the package — embed `Help/**` as resources of the tool assembly.

### Integration Pattern

`HelpCommand` is the only unit that uses the package in the shipped tool; creating its `HelpSource`
and `HelpCli` performs no I/O and needs no disposal. `Program.Main` calls `HelpCommand.Default.TryRun`
before `Context.Create`, and the package builds the catalog only when the first argument is `help`.

The tool project references the package directly, because only a project with its own package
reference embeds cards; the test project receives the test helpers through its project reference to
the tool. The package is a pre-release, so its exact version is pinned in the project file;
upgrading it requires re-running the help tests and a review of this design.
