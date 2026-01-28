# Development Quick Start

Quick reference for developers and AI agents working on SarifMark.

## Prerequisites

- [.NET SDK 8.0, 9.0, or 10.0][dotnet-sdk]
- [Node.js][nodejs] (for linting tools)

## Quick Commands

```bash
# First time setup
npm install                                    # Install linting tools
dotnet tool restore                            # Install .NET tools
dotnet restore                                 # Restore dependencies

# Build and test
dotnet build --configuration Release           # Build (zero warnings required)
dotnet test --configuration Release            # Run all tests

# Code quality
dotnet format                                  # Format code
dotnet format --verify-no-changes              # Verify formatting
npx markdownlint-cli2 "**/*.md"                # Lint markdown
npx cspell "**/*.{cs,md,json,yaml,yml}"        # Spell check
npx yamllint .                                 # Lint YAML

# Requirements traceability
dotnet test --configuration Release --logger "trx;LogFileName=test-results.trx"
dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce

# Coverage
dotnet test --configuration Release --collect "XPlat Code Coverage"

# Package
dotnet pack --configuration Release
```

## Project Structure

```text
SarifMark/
├── src/DemaConsulting.SarifMark/    # Main CLI tool
├── test/DemaConsulting.SarifMark.Tests/  # Tests
├── requirements.yaml                # Requirements (linked to tests)
├── .editorconfig                    # Code style rules
└── .github/workflows/               # CI/CD pipelines
```

## Code Standards

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior`
- **XML Docs**: Required on ALL members (public/internal/private)
- **Requirements**: All features must have requirements linked to tests
- **Zero Warnings**: Build must complete with zero warnings
- **Coverage**: Maintain >80% code coverage

## Pre-Commit Checklist

1. ✅ `dotnet build --configuration Release` (zero warnings)
2. ✅ `dotnet test --configuration Release` (all pass)
3. ✅ `dotnet format --verify-no-changes` (formatting)
4. ✅ `npx markdownlint-cli2 "**/*.md"` (markdown)
5. ✅ `npx cspell "**/*.{cs,md,json,yaml,yml}"` (spelling)
6. ✅ Requirements enforcement passes

## Common Tasks

### Adding a Feature

1. Add requirement to `requirements.yaml`
2. Write tests (link to requirement)
3. Implement feature
4. Update documentation
5. Run pre-commit checklist

### Fixing a Bug

1. Write failing test
2. Fix bug
3. Verify test passes
4. Run pre-commit checklist

### Updating Dependencies

1. Update package references
2. Run full build and test suite
3. Update `requirements.yaml` if needed
4. Check for security vulnerabilities

## Resources

- [AGENTS.md](AGENTS.md) - Agent-specific guidance
- [CONTRIBUTING.md](CONTRIBUTING.md) - Detailed contribution guide
- [README.md](README.md) - Project overview

[dotnet-sdk]: https://dotnet.microsoft.com/download
[nodejs]: https://nodejs.org/
