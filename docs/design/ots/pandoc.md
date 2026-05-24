## Pandoc

Pandoc (`DemaConsulting.PandocTool`) is the CI tool that converts Markdown source
documents to HTML as part of the SarifMark documentation build pipeline. It combines
multiple Markdown files — including title metadata, introduction, design sections, and
generated reports — into a single structured HTML document using a custom HTML template.

### Purpose

Pandoc was chosen for its mature support for combining multiple Markdown inputs, YAML
front matter, Mermaid diagrams, and custom HTML templates into cohesive documents. It is
the industry-standard tool for this workflow and is widely supported in CI environments.
The `DemaConsulting.PandocTool` wrapper ensures a consistent installation and invocation
pattern aligned with the Continuous Compliance toolchain.

### Features Used

- **Multi-file Markdown concatenation** — Pandoc reads the ordered list of Markdown files
  specified in each `definition.yaml` and concatenates them into a single HTML document.
- **YAML front matter processing** — title, subtitle, author, and keyword metadata from
  `title.txt` are embedded in the generated HTML.
- **Custom HTML template rendering** — a custom `template.html` controls the document
  structure, styling, and navigation.
- **Table of contents generation** — enabled via `table-of-contents: true` in
  `definition.yaml`; Pandoc generates a linked table of contents from the heading
  structure.
- **Section numbering** — enabled via `number-sections: true`; headings are
  automatically numbered in the output.

### Integration Pattern

Pandoc is invoked as a .NET tool wrapper from CI pipeline steps:

1. The pipeline installs the tool via `dotnet tool restore`.
2. For each document collection (design, user guide, etc.), a pipeline step invokes
   `pandoc` with the corresponding `definition.yaml` as input.
3. Pandoc reads all `input-files` entries in the order listed, applies the specified
   template, and writes the HTML output to the `generated/` folder.
4. FileAssert subsequently validates each generated HTML file.

No application-level code in SarifMark references Pandoc directly.
