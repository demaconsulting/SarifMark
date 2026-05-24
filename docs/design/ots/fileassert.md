## FileAssert

FileAssert (`DemaConsulting.FileAssert`) is the CI tool used to validate generated HTML
and PDF documents against acceptance criteria. It asserts that each generated file exists,
has a non-trivial size, is structurally valid, and contains expected content strings,
providing independent confirmation that the document generation pipeline succeeded.

### Purpose

FileAssert was chosen because it provides a declarative, assertion-based approach to
document validation that integrates directly with the CI pipeline. It serves as the
primary evidence mechanism for the Pandoc and WeasyPrint OTS items by independently
confirming that their generated output files are correct and complete. FileAssert also
provides self-validation evidence for its own tool qualification.

### Features Used

- **File existence assertion** — verifies that each expected output file has been created
  by the preceding pipeline step.
- **File size assertion** — rejects zero-byte or suspiciously small files that would
  indicate an empty or truncated document.
- **HTML structure assertion** — verifies that generated HTML files contain a valid
  `<title>` element and expected document content.
- **PDF content assertion** — verifies that generated PDF files contain at least one page
  and include expected text strings from the rendered document.
- **Self-validation** — FileAssert's version display and help display commands are used
  to confirm the tool is operational before relying on it for document assertions.

### Integration Pattern

FileAssert is invoked as a .NET tool from CI pipeline steps that follow document
generation steps:

1. The pipeline installs the tool via `dotnet tool restore`.
2. After Pandoc generates HTML files, a FileAssert step runs assertions for each HTML
   output file, checking existence, size, `<title>` presence, and content strings.
3. After WeasyPrint generates PDF files, a FileAssert step runs assertions for each PDF
   output file, checking existence, size, page count, and content strings.
4. A non-zero exit code from FileAssert causes the CI step to fail, blocking the
   pipeline.

No application-level code in SarifMark references FileAssert directly.
