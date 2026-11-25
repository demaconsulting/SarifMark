### HelpCommand

#### Verification Approach

`HelpCommand` is verified with unit tests defined in `HelpCommandTests.cs`. Each test creates a
help command with `HelpCommand.Create`, which writes to in-memory writers instead of the console,
so the tests need no shared console state. A counting replacement for the catalog configuration
calls the real `HelpCommand.Configure` and counts how often the catalog is built. No mocking is
applied: the tests use the real help cards embedded in the tool assembly and the real HelpCards
library, which is a documented dependency of the unit. The validity of the shipped cards is
verified with the HelpCards validator, with warnings treated as errors.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No files, external services, or network access
are required; the cards are read from the tool assembly.

#### Acceptance Criteria

All `HelpCommandTests` test methods pass, confirming that the help verb prints the topic index, a
card path prints the card, an unknown topic exits with code 2 and a hint on the error writer,
search and JSON output find the expected card and carry the tool name and version, arguments that
do not start with the help verb are not answered and never build the catalog, and the shipped
catalog passes validation with warnings treated as errors. No `HelpCommand` unit requirement may
remain without at least one named test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero**: Call `TryRun` with `["help"]`;
assert it answers with exit code 0 and the output lists the command of every root topic.
This scenario is tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero`.

**HelpCommand_TryRun_CardPath_PrintsCardWithExamples**: Call `TryRun` with
`["help", "self-validation"]`; assert it answers with exit code 0 and the output contains the card
title, the examples section, and the description of an example.
This scenario is tested by `HelpCommand_TryRun_CardPath_PrintsCardWithExamples`.

**HelpCommand_TryRun_UnknownTopic_ReturnsTwo**: Call `TryRun` with `["help", "no-such-topic"]`;
assert it answers with exit code 2, writes nothing to the output writer, and the error writer
names the topic and the help command.
This scenario is tested by `HelpCommand_TryRun_UnknownTopic_ReturnsTwo`.

**HelpCommand_TryRun_SearchOption_ListsMatchingCards**: Call `TryRun` with
`["help", "--search", "validation"]`; assert it answers with exit code 0 and the output lists the
self-validation topic.
This scenario is tested by `HelpCommand_TryRun_SearchOption_ListsMatchingCards`.

**HelpCommand_TryRun_JsonOption_WritesJsonDocument**: Call `TryRun` with
`["help", "command-line", "--json"]` and with `["help", "--json"]`; assert the card is a JSON
document of help schema version 1 that names the card, and the index carries the tool name and
the version of the tool.
This scenario is tested by `HelpCommand_TryRun_JsonOption_WritesJsonDocument`.

**HelpCommand_TryRun_NonHelpArguments_ReturnsFalseWithoutBuildingCatalog**: Call `TryRun` with
`["--sarif", "results.sarif"]`, `["--help"]`, and an empty argument list; assert none is answered,
nothing is written, and the catalog is never built. A call with `["help"]` builds the catalog
exactly once.
This scenario is tested by `HelpCommand_TryRun_NonHelpArguments_ReturnsFalseWithoutBuildingCatalog`.

**HelpCommand_CreateOptions_ShippedCatalog_PassesValidation**: Validate the catalog configuration
from `CreateOptions` with warnings treated as errors; assert no defect is reported, which also
proves that the root cards are exactly `RootCardIds`, that every card carries one purpose tag, and
that every command example parses as a command line of the tool.
This scenario is tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`.
