## Pandoc Verification

### Overview

Pandoc is used in the SarifMark build pipeline to convert Markdown document collections to HTML using a custom HTML
template. It processes the design document, user guide, build notes, code-quality report, review plan, and review
report.

### Verification Strategy

Verification evidence is provided by FileAssert assertions confirming that each HTML output file exists, has
non-trivial size, contains a valid `<title>` element, and includes expected document content.

### Requirement Coverage

| Requirement ID | Description | Verification Evidence |
| --- | --- | --- |
| `SarifMark-OTS-Pandoc` | Pandoc converts each doc to valid HTML | `Pandoc_BuildNotesHtml`, `Pandoc_DesignHtml` |
