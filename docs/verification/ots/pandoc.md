## Pandoc

### Verification Approach

Pandoc is used in the SarifMark build pipeline to convert Markdown document collections to HTML using a custom HTML
template. It processes the design document, user guide, build notes, code-quality report, review plan, and review
report. Verification evidence is provided by FileAssert assertions confirming that each HTML output file exists, has
non-trivial size, contains a valid `<title>` element, and includes expected document content.

### Test Scenarios

**Pandoc_BuildNotesHtml**: Pandoc converts the build-notes Markdown collection to an HTML file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes expected build
content. This scenario is verified by FileAssert assertions in the CI pipeline.

**Pandoc_CodeQualityHtml**: Pandoc converts the code-quality report Markdown collection to an HTML file; FileAssert
assertions confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes
expected code-quality content. This scenario is verified by FileAssert assertions in the CI pipeline.

**Pandoc_ReviewPlanHtml**: Pandoc converts the review-plan Markdown collection to an HTML file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes expected
review-plan content. This scenario is verified by FileAssert assertions in the CI pipeline.

**Pandoc_ReviewReportHtml**: Pandoc converts the review-report Markdown collection to an HTML file; FileAssert
assertions confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes
expected review-report content. This scenario is verified by FileAssert assertions in the CI pipeline.

**Pandoc_DesignHtml**: Pandoc converts the design document Markdown collection to an HTML file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes expected design
content. This scenario is verified by FileAssert assertions in the CI pipeline.

**Pandoc_UserGuideHtml**: Pandoc converts the user-guide Markdown collection to an HTML file; FileAssert assertions
confirm the output file exists, has non-trivial size, contains a valid `<title>` element, and includes expected
user-guide content. This scenario is verified by FileAssert assertions in the CI pipeline.

### Requirements Coverage

- **`SarifMark-OTS-Pandoc`**: Pandoc converts each document collection to valid HTML — `Pandoc_BuildNotesHtml`,
  `Pandoc_CodeQualityHtml`, `Pandoc_ReviewPlanHtml`, `Pandoc_ReviewReportHtml`, `Pandoc_DesignHtml`,
  `Pandoc_UserGuideHtml`
