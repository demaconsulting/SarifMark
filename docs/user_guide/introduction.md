# Introduction

This guide describes how to install, configure, and use SarifMark.

## Purpose

SarifMark is a .NET command-line tool that reads SARIF (Static Analysis Results
Interchange Format) files produced by various static analysis tools and generates
comprehensive markdown reports. It is designed to integrate seamlessly into CI/CD
pipelines for automated quality reporting, making analysis results accessible and
actionable for development teams.

The following diagram shows how SarifMark turns a SARIF file into a report:

![SarifMark architecture: the Program entry point uses Cli, Utilities, SelfTest and Sarif to turn a SARIF
file into a markdown report and an exit code.](architecture.svg)

## Scope

This guide covers:

- Installation and setup of SarifMark
- Command-line options and usage patterns
- CI/CD pipeline integration examples
- Report format and customization options
- Troubleshooting common issues

SarifMark requires .NET SDK 8.0, 9.0, or 10.0. No other runtime dependencies are needed.

## References

- [SARIF specification](https://sarifweb.azurewebsites.net/)
- [SarifMark releases](https://github.com/demaconsulting/SarifMark/releases)
- [.NET download](https://dotnet.microsoft.com/download)
- [Continuous Compliance](https://github.com/demaconsulting/ContinuousCompliance)
