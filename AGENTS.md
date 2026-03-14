# Agent Quick Reference

Project-specific guidance for agents working on SarifMark - a .NET CLI tool for creating markdown reports from SARIF files.

## Available Specialized Agents

- **Requirements Agent** - Develops requirements and ensures test coverage linkage
- **Technical Writer** - Creates accurate documentation following regulatory best practices
- **Software Developer** - Writes production code and self-validation tests in literate style
- **Test Developer** - Creates unit and integration tests following AAA pattern
- **Code Quality Agent** - Enforces linting, static analysis, and security standards
- **Code Review Agent** - Assists in performing formal file reviews
- **Repo Consistency Agent** - Ensures downstream repositories remain consistent with template patterns

## Agent Selection Guide

- Fix a bug → **Software Developer**
- Add a new feature → **Requirements Agent** → **Software Developer** → **Test Developer**
- Write a test → **Test Developer**
- Fix linting or static analysis issues → **Code Quality Agent**
- Update documentation → **Technical Writer**
- Add or update requirements → **Requirements Agent**
- Ensure test coverage linkage in `requirements.yaml` → **Requirements Agent**
- Run security scanning or address CodeQL alerts → **Code Quality Agent**
- Perform a formal file review → **Code Review Agent**
- Propagate template changes → **Repo Consistency Agent**

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.json`, `.markdownlint-cli2.jsonc`, `.yamllint.yaml`** - Linting configs

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

## Linting (SarifMark-Specific)

- **README.md**: Absolute URLs only (shipped in NuGet package)
- **Other .md**: Reference-style links `[text][ref]` with `[ref]: url` at end
- **All linters must pass locally**: markdownlint, cspell, yamllint (see CI workflows for commands)

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

- **requirements-agent** - Invoke for: creating/reviewing requirements in requirements.yaml, ensuring
  proper test coverage linkage, determining test strategy (unit/integration/self-validation)
- **technical-writer** - Invoke for: documentation updates/reviews, markdown/spell/YAML linting,
  regulatory documentation best practices
- **repo-consistency-agent** - Invoke for: ensuring SarifMark stays consistent with TemplateDotNetTool
  template patterns, identifying drift from template standards
- **code-quality-agent** - Invoke for: linting issues, static analysis, security scanning, quality
  gates enforcement, requirements traceability verification
- **code-review-agent** - Invoke for: performing formal reviews of named review-sets, producing
  review evidence for the Continuous Compliance pipeline
- **software-developer** - Invoke for: production code features, self-validation tests (SarifMark_*),
  code refactoring, literate programming style
- **test-developer** - Invoke for: unit tests, integration tests, test coverage improvement, AAA
  pattern compliance
