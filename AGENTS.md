# Agent Quick Reference

Project-specific guidance for agents working on SarifMark - a .NET CLI tool for creating markdown reports from SARIF files.

## Standards Application (ALL Agents Must Follow)

Before performing any work, agents must read and apply the relevant standards from `.github/standards/`:

- **`csharp-language.md`** - For C# code development (literate programming, XML docs, dependency injection)
- **`csharp-testing.md`** - For C# test development (AAA pattern, naming, MSTest anti-patterns)
- **`reqstream-usage.md`** - For requirements management (traceability, semantic IDs, source filters)
- **`reviewmark-usage.md`** - For file review management (review-sets, file patterns, enforcement)
- **`software-items.md`** - For software categorization (system/subsystem/unit/OTS classification)
- **`technical-documentation.md`** - For documentation creation and maintenance (structure, Pandoc, README best practices)

Load only the standards relevant to your specific task scope and apply their
quality checks and guidelines throughout your work.

## Agent Delegation Guidelines

The default agent should handle simple, straightforward tasks directly.
Delegate to specialized agents only for specific scenarios:

- **Light development work** (small fixes, simple features) → Call @developer agent
- **Light quality checking** (linting, basic validation) → Call @quality agent
- **Formal feature implementation** (complex, multi-step) → Call the `@implementation` agent
- **Formal bug resolution** (complex debugging, systematic fixes) → Call the `@implementation` agent
- **Formal reviews** (compliance verification, detailed analysis) → Call @code-review agent
- **Template consistency** (downstream repository alignment) → Call @repo-consistency agent

## Available Specialized Agents

- **code-review** - Agent for performing formal reviews using standardized
  review processes
- **developer** - General-purpose software development agent that applies
  appropriate standards based on the work being performed
- **implementation** - Orchestrator agent that manages quality implementations
  through a formal state machine workflow
- **quality** - Quality assurance agent that grades developer work against DEMA
  Consulting standards and Continuous Compliance practices
- **repo-consistency** - Ensures downstream repositories remain consistent with
  the TemplateDotNetTool template patterns and best practices

## Quality Gate Enforcement (ALL Agents Must Verify)

Configuration files and scripts are self-documenting with their design intent and
modification policies in header comments.

1. **Linting Standards**: `./lint.sh` (Unix) or `lint.bat` (Windows) - comprehensive linting suite
2. **Build Quality**: Zero warnings (`TreatWarningsAsErrors=true`)
3. **Static Analysis**: SonarQube/CodeQL passing with no blockers
4. **Requirements Traceability**: `dotnet reqstream --enforce` passing
5. **Test Coverage**: All requirements linked to passing tests
6. **Documentation Currency**: All docs current and generated
7. **File Review Status**: All reviewable files have current reviews

## Continuous Compliance Overview

This repository follows the DEMA Consulting Continuous Compliance
<https://github.com/demaconsulting/ContinuousCompliance> approach, which enforces quality and
compliance gates on every CI/CD run instead of as a last-mile activity.

### Core Principles

- **Requirements Traceability**: Every requirement MUST link to passing tests
- **Quality Gates**: All quality checks must pass before merge
- **Documentation Currency**: All docs auto-generated and kept current
- **Automated Evidence**: Full audit trail generated with every build

## Required Compliance Tools

### Linting Tools (ALL Must Pass)

- **markdownlint-cli2**: Markdown style and formatting enforcement
- **cspell**: Spell-checking across all text files (use `.cspell.yaml` for technical terms)
- **yamllint**: YAML structure and formatting validation
- **Language-specific linters**: Based on repository technology stack

### Quality Analysis

- **SonarQube/SonarCloud**: Code quality and security analysis
- **CodeQL**: Security vulnerability scanning (produces SARIF output)
- **Static analyzers**: Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp, etc.

### Requirements & Compliance

- **ReqStream**: Requirements traceability enforcement (`dotnet reqstream --enforce`)
- **ReviewMark**: File review status enforcement
- **BuildMark**: Tool version documentation
- **VersionMark**: Version tracking across CI/CD jobs

## Project Structure Template

- `docs/` - Documentation and compliance artifacts
  - `reqstream/` - Subsystem requirements YAML files (included by root requirements.yaml)
  - Auto-generated reports (requirements, justifications, trace matrix)
- `src/` - Source code files
- `test/` - Test files
- `.github/workflows/` - CI/CD pipeline definitions (build.yaml, build_on_push.yaml, release.yaml)
- Configuration files: `.editorconfig`, `nuget.config`, `.reviewmark.yaml`, etc.

## Key Configuration Files

### Essential Files (Repository-Specific)

- **`lint.sh` / `lint.bat`** - Cross-platform comprehensive linting scripts
- **`.editorconfig`** - Code formatting rules (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.yaml`** - Spell-check configuration and technical term dictionary
- **`.markdownlint-cli2.yaml`** - Markdown linting rules
- **`.yamllint.yaml`** - YAML linting configuration
- **`nuget.config`** - NuGet package sources
- **`package.json`** - Node.js dependencies for linting tools

### Compliance Files

- **`requirements.yaml`** - Root requirements file using `includes:` to reference `docs/reqstream/` files
- **`docs/reqstream/`** - Per-software-unit, platform, and OTS requirements YAML files
- **`.reviewmark.yaml`** - File review definitions and tracking
- CI/CD pipeline files with quality gate enforcement

### Spell Check Word List Policy

**Never** add a word to the `.cspell.yaml` word list in order to silence a spell-checking failure.
Doing so defeats the purpose of spell-checking and reduces the quality of the repository.

- If cspell flags a word that is **misspelled**, fix the spelling in the source file.
- If cspell flags a word that is a **genuine technical term** (tool name, project identifier, etc.) and is
  spelled correctly, raise a **proposal** (e.g. comment in a pull request) explaining why the word
  should be added. The proposal must be reviewed and approved before the word is added to the list.

## Continuous Compliance Workflow

### CI/CD Pipeline Stages (Standard)

1. **Lint**: `./lint.sh` or `lint.bat` - comprehensive linting suite
2. **Build**: Compile with warnings as errors
3. **Analyze**: SonarQube/SonarCloud, CodeQL security scanning
4. **Test**: Execute all tests, generate coverage reports
5. **Validate**: Tool self-validation tests
6. **Document**: Generate requirements reports, trace matrix, build notes
7. **Enforce**: Requirements traceability, file review status
8. **Publish**: Generate final documentation (Pandoc → PDF)

### Quality Gate Enforcement

All stages must pass before merge. Pipeline fails immediately on:

- Any linting errors
- Build warnings or errors
- Security vulnerabilities (CodeQL)
- Requirements without test coverage
- Outdated file reviews
- Missing documentation

## Continuous Compliance Requirements

This repository follows continuous compliance practices from DEMA Consulting
Continuous Compliance <https://github.com/demaconsulting/ContinuousCompliance>.

### Core Requirements Traceability Rules

- **ALL requirements MUST be linked to tests** - Enforced in CI via `dotnet reqstream --enforce`
- **NOT all tests need requirement links** - Tests may exist for corner cases, design validation, failure scenarios
- **Source filters are critical** - Platform/framework requirements need specific test evidence
- Prefer `SarifMark_*` self-validation tests over unit tests for requirements traceability
- When adding features: add requirement + link to test

### Test Source Filters

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

## Agent Report Files

Upon completion, create a report file at `.agent-logs/[agent-name]-[subject]-[unique-id].md` that includes:

- A concise summary of the work performed
- Any important decisions made and their rationale
- Follow-up items, open questions, or TODOs

Store agent logs in the `.agent-logs/` folder so they are ignored via `.gitignore` and excluded from linting and commits.

Files matching `AGENT_REPORT_*.md` are also excluded from git, linting, and spell-checking (legacy naming pattern).

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

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

- **repo-consistency** - Invoke for: ensuring SarifMark stays consistent with TemplateDotNetTool
  template patterns, identifying drift from template standards
- **code-review** - Invoke for: performing formal reviews of named review-sets, producing
  review evidence for the Continuous Compliance pipeline
- **developer** - Invoke for: light development work (small fixes, simple features) applying
  appropriate standards based on task scope
- **quality** - Invoke for: light quality checking, grading developer work against DEMA Consulting
  standards and Continuous Compliance practices
- **implementation** - Invoke for: formal feature implementation (complex, multi-step), formal bug
  resolution (complex debugging, systematic fixes) using state machine orchestration

## Agent Selection Guide

- Fix a bug → call the @developer agent with the **request** to fix the bug and the **context** of the
  bug details; for complex bugs call the @implementation agent
- Add a new feature → call the @implementation agent with the **request** to implement the feature
  and the **context** of the feature details (it orchestrates requirements, development, and testing)
- Write a test → call the @developer agent with the **request** to write the test and the **context** of
  what needs to be tested
- Fix linting or static analysis issues → call the @quality agent with the **request** to fix the issues
  and the **context** of the errors encountered
- Update documentation → call the @developer agent with the **request** to update the documentation and
  the **context** of what needs to change
- Add or update requirements → call the @developer agent with the **request** to add or update requirements
  and the **context** of the feature details
- Ensure test coverage linkage in `requirements.yaml` → call the @developer agent with the **request** to
  ensure test coverage linkage and the **context** of the current coverage gaps
- Run security scanning or address CodeQL alerts → call the @quality agent with the **request** to address
  security scanning or CodeQL alerts and the **context** of the alerts found
- Perform a formal file review → call the @code-review agent with the **request** to perform a formal review and
  the **context** of the review-set name
- Propagate template changes → call the @repo-consistency agent with the **request** to propagate template
  changes and the **context** of the downstream repository
