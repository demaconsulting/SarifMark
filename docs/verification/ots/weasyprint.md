## WeasyPrint Verification

### Overview

WeasyPrint is used in the SarifMark CI pipeline to convert HTML documents to PDF. It processes all document types
produced by Pandoc in the pipeline.

### Verification Strategy

Verification evidence is provided by FileAssert assertions confirming that each PDF output file exists, has non-trivial
size, contains at least one page, and includes expected rendered text.

### Requirement Coverage

- **`SarifMark-OTS-WeasyPrint`**: Converts each doc type to valid PDF —
  `WeasyPrint_BuildNotesPdf`, `WeasyPrint_DesignPdf`
