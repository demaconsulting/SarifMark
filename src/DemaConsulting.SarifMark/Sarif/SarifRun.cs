// Copyright (c) DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text;

namespace DemaConsulting.SarifMark;

/// <summary>
///     Represents the results from a single run within a SARIF file.
/// </summary>
public record SarifRun
{
    /// <summary>
    ///     Gets the name of the analysis tool.
    /// </summary>
    public string ToolName { get; }

    /// <summary>
    ///     Gets the version of the analysis tool.
    /// </summary>
    public string ToolVersion { get; }

    /// <summary>
    ///     Gets the collection of results/issues found.
    /// </summary>
    public IReadOnlyList<SarifResult> Results { get; }

    /// <summary>
    ///     Gets the total number of results/issues found.
    /// </summary>
    public int ResultCount => Results.Count;

    /// <summary>
    ///     Gets the total number of files analyzed.
    /// </summary>
    public int FileCount { get; }

    /// <summary>
    ///     Gets a value indicating whether any results are present.
    /// </summary>
    public bool HasIssues => ResultCount > 0;

    /// <summary>
    ///     Internal constructor to enforce that instances are only created through the validated parsing pipeline.
    /// </summary>
    /// <param name="toolName">The name of the analysis tool.</param>
    /// <param name="toolVersion">The version of the analysis tool.</param>
    /// <param name="results">The collection of results/issues.</param>
    /// <param name="fileCount">The total number of files analyzed.</param>
    internal SarifRun(string toolName, string toolVersion, IReadOnlyList<SarifResult> results, int fileCount = 0)
    {
        ToolName = toolName;
        ToolVersion = toolVersion;
        Results = results;
        FileCount = fileCount;
    }

    /// <summary>
    ///     Converts the SARIF run results to markdown format.
    /// </summary>
    /// <param name="depth">The heading depth level (1-6) for the report title.</param>
    /// <param name="heading">Optional custom heading. If null, defaults to "[ToolName] Analysis".</param>
    /// <returns>Markdown representation of the SARIF run results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when depth is not between 1 and 6.</exception>
    public string ToMarkdown(int depth, string? heading = null)
    {
        if (depth < 1 || depth > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(depth), depth, "Depth must be between 1 and 6");
        }

        var mainHeading = new string('#', depth);
        var subHeadingDepth = Math.Min(depth + 1, 6);
        var subHeading = new string('#', subHeadingDepth);
        var sb = new StringBuilder();

        AppendHeader(sb, mainHeading, heading);
        AppendIssuesSection(sb, subHeading);

        return sb.ToString();
    }

    /// <summary>
    ///     Appends the header section with custom or default heading and tool information.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="heading">The markdown heading prefix (e.g., "#", "##", "###").</param>
    /// <param name="customHeading">Optional custom heading text. If null, defaults to "[ToolName] Analysis".</param>
    private void AppendHeader(StringBuilder sb, string heading, string? customHeading)
    {
        // Use custom heading or default to "[ToolName] Analysis"
        var headingText = customHeading ?? $"{ToolName} Analysis";
        sb.AppendLine($"{heading} {headingText}");
        sb.AppendLine();

        // Add tool info on separate line
        sb.AppendLine($"**Tool:** {ToolName} {ToolVersion}");

        // Add file count on separate line
        sb.AppendLine($"**Files:** {FileCount}");
        sb.AppendLine();
    }

    /// <summary>
    ///     Appends the issues section with count and details.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="subHeading">The markdown heading prefix for the Issues section.</param>
    private void AppendIssuesSection(StringBuilder sb, string subHeading)
    {
        sb.AppendLine($"{subHeading} Issues");
        sb.AppendLine();

        sb.AppendLine(FormatFoundText(Results.Count, "issue"));
        sb.AppendLine();

        if (Results.Count > 0)
        {
            foreach (var result in Results)
            {
                var locationInfo = FormatLocation(result.Uri, result.StartLine);
                sb.AppendLine($"{locationInfo}: {result.Level} [{result.RuleId}] {result.Message}  ");
            }

            sb.AppendLine();
        }
    }

    /// <summary>
    ///     Formats a count with proper pluralization and "Found" prefix.
    /// </summary>
    /// <param name="count">The count value.</param>
    /// <param name="singularNoun">The singular form of the noun.</param>
    /// <returns>Formatted text like "Found no issues", "Found 1 issue", or "Found 5 issues".</returns>
    private static string FormatFoundText(int count, string singularNoun)
    {
        return count switch
        {
            0 => $"Found no {singularNoun}s",
            1 => $"Found 1 {singularNoun}",
            _ => $"Found {count} {singularNoun}s"
        };
    }

    /// <summary>
    ///     Formats the location information for a result.
    /// </summary>
    /// <param name="uri">The file URI.</param>
    /// <param name="startLine">The starting line number.</param>
    /// <returns>Formatted location string.</returns>
    private static string FormatLocation(string? uri, int? startLine)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return "(no location)";
        }

        return startLine.HasValue ? $"{uri}({startLine})" : uri;
    }
}
