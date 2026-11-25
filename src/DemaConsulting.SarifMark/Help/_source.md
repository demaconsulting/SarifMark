---
name: sarifmark
schema: 2
title: SarifMark
summary: Built-in help of SarifMark, which turns SARIF files into Markdown reports.
---
{{tool}} reads the SARIF file of a static analysis tool, lists its findings, writes them as a Markdown report, and
can fail a build when findings remain.

Every operation is an option. The only command is `{{tool}} help`, which must be the first argument and takes topic
names, such as `{{tool}} help command-line`.
