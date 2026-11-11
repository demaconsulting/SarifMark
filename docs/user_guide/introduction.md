# Introduction

This guide describes how to install, configure, and use SarifMark.

## Purpose

SarifMark is a .NET command-line tool that reads SARIF (Static Analysis Results
Interchange Format) files produced by various static analysis tools and generates
comprehensive markdown reports. It is designed to integrate seamlessly into CI/CD
pipelines for automated quality reporting, making analysis results accessible and
actionable for development teams.

The workflow below shows how SarifMark is used: run a code analysis tool such as CodeQL to produce
a SARIF file, run the `sarifmark` command locally or in a CI pipeline, and use the resulting
markdown report and pass/fail result in code quality documents, as a CI quality gate, or as
compliance evidence.

![SarifMark workflow: a code analysis tool such as CodeQL produces a SARIF file, SarifMark runs from
the command line or a CI pipeline, produces a markdown report and a pass/fail result, which are used
in code quality documents, as a CI quality gate, or as compliance evidence](workflow.svg)

## Scope

This guide covers:

- Installation and setup of SarifMark
- Command-line options and usage patterns
- CI/CD pipeline integration examples
- Report format and customization options
- Troubleshooting common issues

SarifMark requires .NET SDK 10.0 or 11.0. No other runtime dependencies are needed.

## References

- [SARIF specification](https://sarifweb.azurewebsites.net/)
- [SarifMark releases](https://github.com/demaconsulting/SarifMark/releases)
- [.NET download](https://dotnet.microsoft.com/download)
- [Continuous Compliance](https://github.com/demaconsulting/ContinuousCompliance)
