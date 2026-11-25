---
title: Command-line options
summary: Every option of the tool, and the order in which a run handles them.
aliases:
  - options
  - flags
tags:
  - reference
order: 2
see-also:
  - quality-gates
  - troubleshooting
examples:
  - name: usage
    kind: command
    language: shell
    description: Print the short usage text. It ends with a pointer to this help.
    content: '{{tool}} --help'
    expect: "Run '{{tool}} help' for topics and examples."
  - name: silent-log
    kind: command
    language: shell
    description: Hide the console output and keep it in a log file.
    content: '{{tool}} --silent --log sarifmark.log --sarif results.sarif'
  - name: missing-value
    kind: command
    language: shell
    description: An option without its value is an error. Exits 1.
    content: '{{tool}} --sarif'
    exit: 1
    expect: '--sarif requires a filename argument'
---
Options start with `--` (or a single character after `-`) and may be given in any order.

| Option | Meaning |
| --- | --- |
| `-v`, `--version` | Print only the version. |
| `-?`, `-h`, `--help` | Print the short usage text. |
| `--silent` | Hide console output. Errors still set exit code 1. |
| `--log <file>` | Also write the output to a file. |
| `--sarif <file>` | The SARIF file to read; see [[sarif-input]]. |
| `--report <file>` | Write the findings as a Markdown report; see [[reports]]. |
| `--heading <text>` | Heading of the report; default `<tool> Analysis`. |
| `--depth <n>` | First heading level of a report, 1 to 6; default 1. |
| `--exclude <glob>` | Leave out findings in matching files. Repeatable. |
| `--enforce` | Exit 1 when findings remain; see [[quality-gates]]. |
| `--validate` | Run the self-tests; see [[self-validation]]. |
| `--results <file>` | Write the self-test results as `.trx` or `.xml`. |

`--report-depth` is an older name of `--depth`, and `--result` of `--results`; both still work.

**Order.** `--version` wins over everything and prints only the version. Otherwise the tool prints a banner, then
handles the first of `--help`, `--validate`, or the SARIF file. The command that [[#silent-log]] prints writes its
output to the log file only.

**Errors.** An unknown argument, or an option without its value, prints `Error: ...` and exits 1
([[#missing-value]]). A value that is itself an option, as in `--exclude --enforce`, counts as missing.

**Help.** `help` is a command, not an option, and must be the first argument. `--silent` and `--log` do not apply
to it, and an unknown topic exits 2.
