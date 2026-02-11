# Agent Quick Reference

Project-specific guidance for agents working on SarifMark - a .NET CLI tool for creating markdown reports from SARIF files.

## Available Specialized Agents

- **Requirements Agent** - Develops requirements and ensures test coverage linkage
- **Technical Writer** - Creates accurate documentation following regulatory best practices
- **Software Developer** - Writes production code and self-validation tests in literate style
- **Test Developer** - Creates unit and integration tests following AAA pattern
- **Code Quality Agent** - Enforces linting, static analysis, and security standards
- **Repo Consistency Agent** - Ensures downstream repositories remain consistent with template patterns

## Tech Stack

- C# 12, .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8+BOM, LF endings)
- **`.cspell.json`, `.markdownlint-cli2.jsonc`, `.yamllint.yaml`** - Linting configs

## Requirements (SarifMark-Specific)

- Link ALL requirements to tests (prefer `SarifMark_*` self-validation over unit tests)
- Enforced in CI: `dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce`
- When adding features: add requirement + link to test

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
- **software-developer** - Invoke for: production code features, self-validation tests (SarifMark_*),
  code refactoring, literate programming style
- **test-developer** - Invoke for: unit tests, integration tests, test coverage improvement, AAA
  pattern compliance
