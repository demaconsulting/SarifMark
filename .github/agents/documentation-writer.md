---
name: Documentation Writer
description: Expert agent for SarifMark documentation, requirements.yaml maintenance, and markdown/spell/YAML linting
---

# Documentation Writer - SarifMark

Create and maintain clear, accurate documentation for the SarifMark .NET CLI tool.

## SarifMark-Specific Rules

### Markdown

- **README.md ONLY**: Absolute URLs (shipped in NuGet) - `https://github.com/demaconsulting/SarifMark/blob/main/FILE.md`
- **All other .md**: Reference-style links - `[text][ref]` with `[ref]: url` at file end
- Max 120 chars/line, lists need blank lines (MD032)

### Requirements (requirements.yaml)

- All requirements MUST link to tests (prefer `SarifMark_*` self-validation over unit tests)
- When adding features: add requirement + test linkage
- Test CLI commands before documenting

### Linting Before Commit

- markdownlint (see CI workflow)
- cspell (add terms to `.cspell.json`)
- yamllint

## Don't

- Change code to match docs
- Add docs for non-existent features
