## WeasyPrint

WeasyPrint (`DemaConsulting.WeasyPrintTool`) is the CI tool that converts the HTML
documents produced by Pandoc into PDF files for inclusion in the SarifMark release
artifacts. It renders each HTML document as a paginated PDF using the CSS-based layout
engine provided by the WeasyPrint Python library.

### Purpose

WeasyPrint was chosen because it produces high-fidelity PDF output from HTML and CSS
sources without requiring a headless browser, making it reliable in CI environments.
The `DemaConsulting.WeasyPrintTool` wrapper provides a consistent .NET tool installation
and invocation pattern aligned with the rest of the Continuous Compliance toolchain.

### Features Used

- **HTML to PDF conversion** — renders an HTML input file to a paginated PDF output file
  using WeasyPrint's CSS layout engine.
- **Multi-document support** — each document collection is converted independently,
  producing separate PDF files for design, user guide, and other collections.

### Integration Pattern

WeasyPrint is invoked as a .NET tool from CI pipeline steps that follow Pandoc HTML
generation:

1. The pipeline installs the tool via `dotnet tool restore`.
2. For each HTML document produced by Pandoc, a pipeline step invokes `weasyprint` with
   the HTML input path and the desired PDF output path.
3. WeasyPrint renders the HTML to PDF and writes the output file.
4. FileAssert subsequently validates each generated PDF file for existence, size, page
   count, and content.
5. The generated PDF files are published as release artifacts.

No application-level code in SarifMark references WeasyPrint directly.
