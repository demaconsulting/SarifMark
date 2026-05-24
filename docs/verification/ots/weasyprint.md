## WeasyPrint

### Verification Approach

WeasyPrint is used in the SarifMark CI pipeline to convert HTML documents to PDF. It processes all document types
produced by Pandoc in the pipeline. Verification evidence is provided by FileAssert assertions confirming that each
PDF output file exists, has non-trivial size, contains at least one page, and includes expected rendered text.

### Test Scenarios

**WeasyPrint_BuildNotesPdf**: WeasyPrint converts the build-notes HTML document to a PDF file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains at least one page, and includes expected rendered text.
This scenario is verified by FileAssert assertions in the CI pipeline.

**WeasyPrint_CodeQualityPdf**: WeasyPrint converts the code-quality report HTML document to a PDF file; FileAssert
assertions confirm the output file exists, has non-trivial size, contains at least one page, and includes expected
rendered text. This scenario is verified by FileAssert assertions in the CI pipeline.

**WeasyPrint_ReviewPlanPdf**: WeasyPrint converts the review-plan HTML document to a PDF file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains at least one page, and includes expected rendered text.
This scenario is verified by FileAssert assertions in the CI pipeline.

**WeasyPrint_ReviewReportPdf**: WeasyPrint converts the review-report HTML document to a PDF file; FileAssert
assertions confirm the output file exists, has non-trivial size, contains at least one page, and includes expected
rendered text. This scenario is verified by FileAssert assertions in the CI pipeline.

**WeasyPrint_DesignPdf**: WeasyPrint converts the design document HTML to a PDF file; FileAssert assertions confirm
the output file exists, has non-trivial size, contains at least one page, and includes expected rendered text.
This scenario is verified by FileAssert assertions in the CI pipeline.

**WeasyPrint_UserGuidePdf**: WeasyPrint converts the user-guide HTML document to a PDF file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains at least one page, and includes expected rendered text.
This scenario is verified by FileAssert assertions in the CI pipeline.

### Requirements Coverage

- **`SarifMark-OTS-WeasyPrint`**: Converts each document type to a valid PDF — `WeasyPrint_BuildNotesPdf`,
  `WeasyPrint_CodeQualityPdf`, `WeasyPrint_ReviewPlanPdf`, `WeasyPrint_ReviewReportPdf`, `WeasyPrint_DesignPdf`,
  `WeasyPrint_UserGuidePdf`
