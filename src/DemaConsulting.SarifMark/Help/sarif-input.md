---
title: SARIF input
summary: Which SARIF files the tool reads, and which parts of them it uses.
aliases:
  - sarif
  - input
  - suppression
  - suppressions
tags:
  - reference
order: 3
see-also:
  - reports
  - troubleshooting
examples:
  - name: summary
    kind: command
    language: shell
    description: Read a SARIF file and print the tool and the number of findings of each run.
    content: '{{tool}} --sarif results.sarif'
    expect: |
      Tool: CodeQL 2.20.0
      Results: 2
---
SARIF (Static Analysis Results Interchange Format) is the JSON format in which static analysis tools such as
CodeQL, SonarQube, and the .NET analyzers write their findings. {{tool}} does not write SARIF; it reads the file
that such a tool produced. `--sarif <file>` names one file per run.

**Required structure.** The file is SARIF 2.1.0 JSON with a `version` property and a non-empty `runs` array. Each
run needs `tool.driver`. A file that is not JSON, or lacks one of these, is an error;
see [[troubleshooting]].

**What is read.** For each run:

- the tool name (`tool.driver.name`) and version (`version`, else `semanticVersion`, else
  `dottedQuadFileVersion`); a missing value reads as `Unknown`
- the number of entries of `artifacts`, reported as the number of files
- for each result: `ruleId`, `level` (default `warning`), `message.text`, and the `uri` and `startLine` of the
  first location

**What is left out.** A result with a non-empty `suppressions` array is suppressed in the source and is not counted
or reported. `--exclude` leaves out more findings by file; see [[quality-gates]].

**Several runs.** A file with several runs, for example from several analyzers, is reported run by run. The command
that [[#summary]] prints shows one `Tool:` and one `Results:` line per run.
