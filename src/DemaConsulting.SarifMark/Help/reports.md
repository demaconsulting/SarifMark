---
title: Markdown reports
summary: Write the findings as a Markdown report, and set its heading and depth.
aliases:
  - report
  - markdown
tags:
  - reference
order: 4
see-also:
  - sarif-input
  - quality-gates
examples:
  - name: report
    kind: command
    language: shell
    description: Write the findings of a SARIF file as a Markdown report.
    content: '{{tool}} --sarif results.sarif --report report.md'
    expect: 'Report generated successfully.'
  - name: heading
    kind: command
    language: shell
    description: Use your own heading instead of the tool name.
    content: '{{tool}} --sarif results.sarif --report report.md --heading "Code Quality"'
    expect: 'Report generated successfully.'
  - name: depth
    kind: command
    language: shell
    description: Start the report at heading level 2 to embed it in a larger document.
    content: '{{tool}} --sarif results.sarif --report report.md --depth 2'
    expect: 'Report generated successfully.'
  - name: bad-depth
    kind: command
    language: shell
    description: A depth outside 1 to 6 is an error. Exits 1.
    content: '{{tool}} --sarif results.sarif --report report.md --depth 7'
    exit: 1
    expect: 'requires an integer between 1 and 6'
---
`--report <file>` writes the findings that remain after `--exclude` to a Markdown file.
The command that [[#report]] prints writes one. An existing file is overwritten.
The tool prints `Report generated successfully.` when the file is written.

**Layout.** For each run the report has:

- a heading at level `--depth` (default 1) with the text `<tool> Analysis`, or the text of `--heading`
  ([[#heading]]); when the file has several runs, each heading ends
  with `(#1)`, `(#2)`, and so on
- the lines `**Tool:** <name> <version>` and `**Files:** <count>`
- a subheading `Issues` one level deeper (at most level 6)
- the sentence `Found no issues`, `Found 1 issue`, or `Found <n> issues`
- one line per finding: `<file>(<line>): <level> [<rule>] <message>`, with the file alone when there is no line,
  and `(no location)` when the finding has no location

**Depth.** `--depth` sets the level of the first heading, so the report can be included in a larger document.
The command that [[#depth]] prints sets it to 2. A value outside 1 to 6 is an error;
the command that [[#bad-depth]] prints shows it.

The report is meant to be read, for example in build notes or a pull request. For processing in code, read the
SARIF file itself.
