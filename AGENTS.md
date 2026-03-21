# Agent Quick Reference

Project-specific guidance for agents working on SarifMark - a .NET CLI tool for creating markdown reports from SARIF files.

## Available Specialized Agents

- **requirements** agent - Develops requirements and ensures test coverage linkage
- **technical-writer** agent - Creates accurate documentation following regulatory best practices
- **software-developer** agent - Writes production code and self-validation tests in literate style
- **test-developer** agent - Creates unit and integration tests following AAA pattern
- **code-quality** agent - Enforces linting, static analysis, and security standards
- **code-review** agent - Assists in performing formal file reviews
- **repo-consistency** agent - Ensures downstream repositories remain consistent with template patterns

## Agent Selection Guide

- Fix a bug → call the @software-developer agent with the **request** to fix the bug and the **context** of the
  bug details
- Add a new feature → call the @requirements agent with the **request** to define the feature requirements and the
  **context** of the feature details, then call the @software-developer agent with the **request** to implement the
  feature and the **context** of the requirements, then call the @test-developer agent with the **request** to add
  tests and the **context** of the feature implemented
- Write a test → call the @test-developer agent with the **request** to write the test and the **context** of
  what needs to be tested
- Fix linting or static analysis issues → call the @code-quality agent with the **request** to fix the issues
  and the **context** of the errors encountered
- Update documentation → call the @technical-writer agent with the **request** to update the documentation and
  the **context** of what needs to change
- Add or update requirements → call the @requirements agent with the **request** to add or update requirements
  and the **context** of the feature details
- Ensure test coverage linkage in `requirements.yaml` → call the @requirements agent with the **request** to
  ensure test coverage linkage and the **context** of the current coverage gaps
- Run security scanning or address CodeQL alerts → call the @code-quality agent with the **request** to address
  security scanning or CodeQL alerts and the **context** of the alerts found
- Perform a formal file review → call the @code-review agent with the **request** to perform a formal review and
  the **context** of the review-set name
- Propagate template changes → call the @repo-consistency agent with the **request** to propagate template
  changes and the **context** of the downstream repository

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.yaml`, `.markdownlint-cli2.yaml`, `.yamllint.yaml`** - Linting configs

### Spell check word list policy

**Never** add a word to the `.cspell.yaml` word list in order to silence a spell-checking failure.
Doing so defeats the purpose of spell-checking and reduces the quality of the repository.

- If cspell flags a word that is **misspelled**, fix the spelling in the source file.
- If cspell flags a word that is a **genuine technical term** (tool name, project identifier, etc.) and is
  spelled correctly, raise a **proposal** (e.g. comment in a pull request) explaining why the word
  should be added. The proposal must be reviewed and approved before the word is added to the list.

## Requirements (SarifMark-Specific)

- Link ALL requirements to tests (prefer `SarifMark_*` self-validation over unit tests)
- Not all tests need to be linked to requirements (tests may exist for corner cases, design testing, failure-testing, etc.)
- Enforced in CI: `dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce`
- When adding features: add requirement + link to test

## Test Source Filters

Test links in `requirements.yaml` can include a source filter prefix to restrict which test results count as
evidence. This is critical for platform and framework requirements - **do not remove these filters**.

- `windows@TestName` - proves the test passed on a Windows platform
- `ubuntu@TestName` - proves the test passed on a Linux (Ubuntu) platform
- `macos@TestName` - proves the test passed on a macOS platform
- `net8.0@TestName` - proves the test passed under the .NET 8 target framework
- `net9.0@TestName` - proves the test passed under the .NET 9 target framework
- `net10.0@TestName` - proves the test passed under the .NET 10 target framework
- `dotnet8.x@TestName` - proves the self-validation test ran on a machine with .NET 8.x runtime
- `dotnet9.x@TestName` - proves the self-validation test ran on a machine with .NET 9.x runtime
- `dotnet10.x@TestName` - proves the self-validation test ran on a machine with .NET 10.x runtime

## Testing (SarifMark-Specific)

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` (for requirements traceability)
- **MSTest v4**: Use `Assert.HasCount()`, `Assert.IsEmpty()`, `Assert.DoesNotContain()` (not old APIs)
- **Console Tests**: Always save/restore `Console.Out` in try/finally

## Code Style (SarifMark-Specific)

- **XML Docs**: On ALL members (public/internal/private) with spaces after `///` in summaries
- **Errors**: `ArgumentException` for parsing, `InvalidOperationException` for runtime, Write* only after success
- **No code duplication**: Extract to properties/methods

## Markdown Link Style

- **AI agent markdown files** (`.github/agents/*.agent.md`): Use inline links `[text](url)` so URLs are visible
  in agent context
- **README.md**: Use absolute URLs (shipped in NuGet package)
- **All other markdown files**: Use reference-style links `[text][ref]` with `[ref]: url` at document end

## Build & Quality (Quick Reference)

```bash
# Standard build/test
dotnet build --configuration Release && dotnet test --configuration Release

# Pre-finalization checklist (in order):
# 1. Build/test (zero warnings required)
# 2. code_review tool
# 3. codeql_checker tool
# 4. All linters (markdownlint, cspell, yamllint)
# 5. Requirements: dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce
```

## Custom Agents

Delegate tasks to specialized agents for better results:

- **requirements** - Invoke for: creating/reviewing requirements in requirements.yaml, ensuring
  proper test coverage linkage, determining test strategy (unit/integration/self-validation)
- **technical-writer** - Invoke for: documentation updates/reviews, markdown/spell/YAML linting,
  regulatory documentation best practices
- **repo-consistency** - Invoke for: ensuring SarifMark stays consistent with TemplateDotNetTool
  template patterns, identifying drift from template standards
- **code-quality** - Invoke for: linting issues, static analysis, security scanning, quality
  gates enforcement, requirements traceability verification
- **code-review** - Invoke for: performing formal reviews of named review-sets, producing
  review evidence for the Continuous Compliance pipeline
- **software-developer** - Invoke for: production code features, self-validation tests (SarifMark_*),
  code refactoring, literate programming style
- **test-developer** - Invoke for: unit tests, integration tests, test coverage improvement, AAA
  pattern compliance
