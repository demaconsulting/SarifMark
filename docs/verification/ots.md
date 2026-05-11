# OTS Software Verification

## Overview

OTS software items used in the SarifMark build pipeline and test infrastructure are verified through three evidence
categories:

1. **Self-validation output**: Tools that expose a `--validate` flag (FileAssert) are exercised in the CI pipeline
   self-validation step; passing output confirms the tool is installed and operational.
2. **Successful CI pipeline completion**: Tools without a self-test mechanism (BuildMark, ReviewMark, SonarMark,
   VersionMark, ReqStream, Pandoc, WeasyPrint) are verified by successful CI pipeline execution — each tool produces an
   artefact (document, report, or exit-code assertion) that confirms functional operation.
3. **Framework operation**: xUnit v3 (the test framework) is verified implicitly — the
   test suite runs and produces passing results, confirming the framework discovers and executes tests correctly.

All OTS evidence is collected from the official CI pipeline run. Local runs may not reproduce all evidence due to
environment dependencies (SonarCloud credentials, GitHub API tokens, etc.).

## OTS Items

The following OTS items are covered in subsequent sections:

- BuildMark — build notes and pipeline report generation
- FileAssert — output file assertion for CI verification
- xUnit v3 — unit and integration test framework
- Pandoc — document conversion (Markdown to HTML)
- ReqStream — requirements traceability and enforcement
- ReviewMark — review plan and report generation
- SonarMark — SonarCloud quality gate and issues reporting
- VersionMark — tool version capture and reporting
- WeasyPrint — document conversion (HTML to PDF)
