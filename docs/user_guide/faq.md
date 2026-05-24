# FAQ

## Frequently Asked Questions

### What is SARIF?

SARIF (Static Analysis Results Interchange Format) is an open standard for the output of static
analysis tools. It provides a common schema so that results from different tools can be consumed
by a single reader. See the [SARIF website](https://sarifweb.azurewebsites.net/) for the full
specification.

### Which tools produce SARIF output?

Many popular analysis tools support SARIF output, including CodeQL, SonarQube, ESLint, Pylint,
Semgrep, Checkmarx, and Trivy.

### Can I process multiple SARIF files in one run?

SarifMark processes one SARIF file per invocation. To report on multiple files, run the command
once for each file. If a single SARIF file contains results from multiple runs, SarifMark handles
them automatically and distinguishes each run with indexed headings.

### How do I use SarifMark in a CI/CD pipeline?

Refer to *Integration Examples* in the Usage section for a complete GitHub Actions workflow example.

### What does `--enforce` do?

The `--enforce` flag processes the SARIF file normally and generates the report, but returns a
non-zero exit code if any issues are found. This allows pipelines to fail automatically when
analysis detects problems.

### Can I customize the report output?

Yes. Use `--heading` to specify a custom top-level heading and `--depth` to set the markdown header
depth, which is useful when embedding the report inside a larger document.

### Which .NET versions are supported?

SarifMark supports .NET SDK 8.0, 9.0, and 10.0.

### How do I update SarifMark?

Refer to *Updating* in the Installation section for global and local update commands.

## Troubleshooting

### SARIF File Not Found

Verify the path passed to `--sarif` is correct and that the file exists. Use an absolute path if
you are unsure of the current working directory.

### Invalid SARIF Format

Ensure the SARIF file conforms to the SARIF 2.1.0 specification. Run the file through the SARIF
validator at the SARIF website before passing it to SarifMark.

### Missing `--sarif` Parameter

Analysis mode requires `--sarif`. Provide a valid SARIF file path or use `--validate` to run
self-validation without a SARIF file.

## Support

- **GitHub Issues**: <https://github.com/demaconsulting/SarifMark/issues>
- **GitHub Discussions**: <https://github.com/demaconsulting/SarifMark/discussions>
- **Documentation**: <https://github.com/demaconsulting/SarifMark>
