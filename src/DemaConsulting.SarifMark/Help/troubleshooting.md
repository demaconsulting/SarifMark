---
title: Errors and exit codes
summary: What the exit codes mean, and how to fix the common error messages.
aliases:
  - errors
  - exit-codes
tags:
  - troubleshooting
order: 7
see-also:
  - command-line
  - sarif-input
  - quality-gates
examples:
  - name: no-sarif
    kind: command
    language: shell
    description: A report without a SARIF file to read. Exits 1.
    content: '{{tool}} --report report.md'
    exit: 1
    expect: 'Error: --sarif parameter is required'
  - name: missing-file
    kind: command
    language: shell
    description: A SARIF file that does not exist. Exits 1.
    content: '{{tool}} --sarif missing.sarif'
    exit: 1
    expect: 'SARIF file not found'
  - name: broken
    kind: command
    language: shell
    description: A file that is not valid JSON. Exits 1.
    content: '{{tool}} --sarif broken.sarif'
    exit: 1
    expect: 'Invalid JSON in SARIF file'
  - name: unknown-option
    kind: command
    language: shell
    description: An unknown argument. Exits 1.
    content: '{{tool}} --unknown'
    exit: 1
    expect: "Error: Unsupported argument '--unknown'"
  - name: silent-error
    kind: command
    language: shell
    description: An argument error is printed even with --silent. Exits 1.
    content: '{{tool}} --silent --unknown'
    exit: 1
    expect: "Error: Unsupported argument '--unknown'"
  - name: unknown-topic
    kind: command
    language: shell
    description: A help topic that does not exist. Exits 2.
    content: '{{tool}} help no-such-topic'
    exit: 2
    expect: "Unknown help card 'no-such-topic'"
---
**Exit codes.**

| Code | Meaning |
| --- | --- |
| 0 | Success. |
| 1 | An error, a failed self-test, or findings with `--enforce`. |
| 2 | A wrong `help` argument or an unknown help topic. |

Errors are printed to standard error as `Error: ...` and set the exit code. Errors found during a run, such as a
missing SARIF file, are hidden by `--silent` and kept in the `--log` file. Errors in the arguments, and a log file
that cannot be opened, are found before the run starts: they are always printed and never reach the log file.
The command that [[#silent-error]] prints shows one despite `--silent`.

**Common messages.**

- `--sarif parameter is required`: every run other than `--help`, `--version`, and `--validate` reads a SARIF
  file. Add `--sarif <file>`.
  The command that [[#no-sarif]] prints shows the error.
- `SARIF file not found`: the path is wrong. A relative path is relative to the current folder.
  The command that [[#missing-file]] prints shows the error.
- `Invalid JSON in SARIF file`: the file is not JSON, often because the analyzer stopped early. Run the analyzer
  again. The command that [[#broken]] prints shows the error.
- `Invalid SARIF file: ...`: the JSON lacks `version`, a non-empty `runs` array, or `tool.driver`; see
  [[sarif-input]].
- `Failed to write report`: the folder of `--report` does not exist or the file is locked.
- `Failed to open log file`: the folder of `--log` does not exist or the file is locked.
- `Unsupported argument`: check the spelling against [[command-line]]. `help` must come first.
  The command that [[#unknown-option]] prints shows the error.
- `requires a filename argument` and similar: the option before it needs a value.
- `Unknown help card`: the topic does not exist. The message lists the topics.
  The command that [[#unknown-topic]] prints shows the error.
- `Issues found in SARIF file`: `--enforce` found findings; see [[quality-gates]].
