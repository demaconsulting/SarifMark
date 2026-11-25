---
title: Quality gates and exclusions
summary: Fail a build when findings remain, and leave out findings by file.
aliases:
  - enforce
  - exclude
  - gate
tags:
  - reference
order: 5
see-also:
  - reports
  - troubleshooting
examples:
  - name: enforce
    kind: command
    language: shell
    description: Exit 1 because the SARIF file has findings.
    content: '{{tool}} --sarif results.sarif --enforce'
    exit: 1
    expect: 'Error: Issues found in SARIF file'
  - name: clean
    kind: command
    language: shell
    description: A SARIF file without findings passes the gate.
    content: '{{tool}} --sarif clean.sarif --enforce'
    expect: 'Results: 0'
  - name: exclude
    kind: command
    language: shell
    description: Leave out the findings in generated files below any obj folder.
    content: '{{tool}} --sarif results.sarif --exclude "**/obj/**"'
    expect: 'Excluded 1 finding(s) matching --exclude patterns.'
  - name: ci-step
    kind: snippet
    language: yaml
    save-as: sarifmark-step.yaml
    description: A GitHub Actions step that writes the report and fails the job on findings.
    content: |
      - name: Report and gate static analysis findings
        run: >-
          dotnet sarifmark --sarif results.sarif --report analysis.md
          --exclude "**/obj/**" --enforce
---
**Enforce.** `--enforce` makes the run exit 1 when any finding remains after exclusions, and prints
`Error: Issues found in SARIF file`, as the command that [[#enforce]] prints shows.
Suppressed results never count. The report of `--report` is still written, so a failed build keeps the list of
findings. A file without findings exits 0, as the command
that [[#clean]] prints shows.

**Exclude.** `--exclude <glob>` leaves out the findings whose file matches the pattern.
The command that [[#exclude]] prints shows it. The rules:

- the pattern is matched against the file URI of the finding's first location, compared without regard to case
- `*` matches within one folder and `**` matches any number of folders
- the option can be given several times; a finding is left out when it matches any pattern
- a finding without a location is never left out
- exclusions apply before `--enforce` and before the report, and the tool prints how many findings were left out

Quote a pattern in the shell (`"**/obj/**"`) so that the shell does not expand it.

**In a pipeline.** Run the analyzer first, then {{tool}} on its SARIF file.
The step that [[#ci-step]] prints writes the report and fails the job
when findings remain; publish the report as a build artifact or in the job summary.
