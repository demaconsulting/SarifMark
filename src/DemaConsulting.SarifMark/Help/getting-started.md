---
title: Getting started
summary: Install the tool, check that it works, and write a first report.
aliases:
  - start
  - quickstart
  - examples
tags:
  - tutorial
order: 1
see-also:
  - command-line
  - sarif-input
  - reports
examples:
  - name: version
    kind: command
    language: shell
    description: Print the version and exit.
    content: '{{tool}} --version'
  - name: report
    kind: command
    language: shell
    description: Read a SARIF file and write its findings as a Markdown report.
    content: '{{tool}} --sarif results.sarif --report report.md'
    expect: 'Report generated successfully.'
  - name: topic
    kind: command
    language: shell
    description: Print a help topic, here the list of options.
    content: '{{tool}} help command-line'
    expect: '--sarif'
---
{{tool}} {{version}} is a .NET tool.
It reads the SARIF file that a static analysis tool, such as CodeQL, writes and turns its findings into a Markdown
report. The examples read `results.sarif`, a SARIF file of such a tool.

1. **Install it.** Install the tool from NuGet, globally or in a local tool manifest:

   ```bash
   dotnet tool install -g DemaConsulting.SarifMark
   ```

2. **Check it.** The command that [[#version]] prints shows the version.
   `{{tool}} --validate` runs the built-in self-tests; see [[self-validation]].
3. **Write a report.** The command that [[#report]] prints reads the SARIF file,
   prints the tool name and the number of findings, and writes the report.
   [[sarif-input]] says what is read, and [[reports]] describes the report.
4. **Gate the build.** Add `--enforce` to exit 1 when findings remain; see [[quality-gates]].

Print any help topic with `{{tool}} help <topic>`,
as in the command that [[#topic]] prints.
Add `--search <words>` to find a topic, and `--json` to get any help answer as JSON.
