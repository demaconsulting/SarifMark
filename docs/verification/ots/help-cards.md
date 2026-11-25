## DemaConsulting.HelpCards

### Verification Approach

`DemaConsulting.HelpCards` is a runtime library with no self-validation mode, so it is verified
through tests in the SarifMark test suite that use the package with the help cards embedded in the
tool. No mocking is applied. The `HelpCommand` unit tests drive the command-line helper of the
package with in-memory writers and check the answers, the exit codes, and the deferred creation of
the catalog; the same tests run the validator of the package against the shipped cards. The
integration tests run the compiled tool to confirm that the same behavior holds in a separate
process and that every command example of every card gives its stated result.

### Test Environment

Tests require:

- No network access; the cards are read from the tool assembly.
- A writable temporary directory, in which the example runner creates a scratch folder per card.
- The `DemaConsulting.HelpCards` NuGet package installed as a package reference in the
  production project (`src/DemaConsulting.SarifMark/DemaConsulting.SarifMark.csproj`).

### Acceptance Criteria

The OTS integration is accepted when all linked tests pass with zero failures: the helper answers
the help verb and leaves other arguments alone, the shipped catalog passes validation with
warnings treated as errors, and every command example gives the exit code and output that its card
states.

### Test Scenarios

**CommandAnswers**: Verifies that the helper prints the topic index, a card with its examples, and
a usage error with exit code 2 for an unknown topic, and that arguments other than the help verb
are not answered and never build the catalog. Tested by
`HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero`,
`HelpCommand_TryRun_CardPath_PrintsCardWithExamples`, `HelpCommand_TryRun_UnknownTopic_ReturnsTwo`,
and `HelpCommand_TryRun_NonHelpArguments_ReturnsFalseWithoutBuildingCatalog`.

**CatalogSearchAndJson**: Verifies that the catalog lists the cards that match a search and writes
a versioned JSON document. Tested by `HelpCommand_TryRun_SearchOption_ListsMatchingCards` and
`HelpCommand_TryRun_JsonOption_WritesJsonDocument`.

**Validation**: Verifies that the validator accepts the shipped cards with warnings treated as
errors, including the expected topics, the purpose tags, and the command lines of the examples.
Tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`.

**ExampleRunner**: Verifies that the runner calls the test's delegate for every command example
in a scratch folder per card and compares the exit code and the output fragment that each example
states. Tested by `SarifMark_HelpExamples_AllCommandExamples_GiveStatedResults`.

**Embedding**: Verifies that the cards embedded in the assembly by the package build targets are
complete: the test output holds no card files, so a validated catalog, a printed card, and every
command example running against the compiled tool prove the embedded copy. Tested by
`HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`,
`HelpCommand_TryRun_CardPath_PrintsCardWithExamples`, and
`SarifMark_HelpExamples_AllCommandExamples_GiveStatedResults`.
