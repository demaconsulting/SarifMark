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
using System.Text.Json;

namespace DemaConsulting.SarifMark;

/// <summary>
///     Represents the results from reading a SARIF file.
/// </summary>
public record SarifResults
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
    ///     Internal constructor for testing purposes.
    /// </summary>
    /// <param name="toolName">The name of the analysis tool.</param>
    /// <param name="toolVersion">The version of the analysis tool.</param>
    /// <param name="results">The collection of results/issues.</param>
    internal SarifResults(string toolName, string toolVersion, IReadOnlyList<SarifResult> results)
    {
        ToolName = toolName;
        ToolVersion = toolVersion;
        Results = results;
    }

    /// <summary>
    ///     Reads a SARIF file and extracts the results.
    /// </summary>
    /// <param name="filePath">The path to the SARIF file.</param>
    /// <returns>A SarifResults record containing the extracted information.</returns>
    /// <exception cref="ArgumentException">Thrown when the file path is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the SARIF file is invalid or malformed.</exception>
    public static SarifResults Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"SARIF file not found: {filePath}", filePath);
        }

        try
        {
            var json = File.ReadAllText(filePath);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var firstRun = ValidateSarifStructure(root);
            var (toolName, toolVersion) = ExtractToolInformation(firstRun);
            var results = ParseResults(firstRun);

            return new SarifResults(toolName, toolVersion, results);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON in SARIF file: {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Validates the SARIF file structure and returns the first run element.
    /// </summary>
    /// <param name="root">The root JSON element.</param>
    /// <returns>The first run element.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the SARIF structure is invalid.</exception>
    private static JsonElement ValidateSarifStructure(JsonElement root)
    {
        if (!root.TryGetProperty("version", out _))
        {
            throw new InvalidOperationException("Invalid SARIF file: missing 'version' property.");
        }

        if (!root.TryGetProperty("runs", out var runsElement) || runsElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("Invalid SARIF file: missing or invalid 'runs' array.");
        }

        var runs = runsElement.EnumerateArray();
        if (!runs.Any())
        {
            throw new InvalidOperationException("Invalid SARIF file: 'runs' array is empty.");
        }

        return runs.First();
    }

    /// <summary>
    ///     Extracts tool information from a run element.
    /// </summary>
    /// <param name="runElement">The run JSON element.</param>
    /// <returns>A tuple containing the tool name and version.</returns>
    /// <exception cref="InvalidOperationException">Thrown when tool information is missing.</exception>
    private static (string ToolName, string ToolVersion) ExtractToolInformation(JsonElement runElement)
    {
        if (!runElement.TryGetProperty("tool", out var toolElement))
        {
            throw new InvalidOperationException("Invalid SARIF file: missing 'tool' property in run.");
        }

        if (!toolElement.TryGetProperty("driver", out var driverElement))
        {
            throw new InvalidOperationException("Invalid SARIF file: missing 'driver' property in tool.");
        }

        var toolName = driverElement.TryGetProperty("name", out var nameElement)
            ? nameElement.GetString() ?? "Unknown"
            : "Unknown";

        var toolVersion = driverElement.TryGetProperty("version", out var toolVersionElement)
            ? toolVersionElement.GetString() ?? "Unknown"
            : "Unknown";

        return (toolName, toolVersion);
    }

    /// <summary>
    ///     Parses all results from a run element, excluding suppressed results.
    /// </summary>
    /// <param name="runElement">The run JSON element.</param>
    /// <returns>A list of parsed SARIF results.</returns>
    private static List<SarifResult> ParseResults(JsonElement runElement)
    {
        var results = new List<SarifResult>();

        if (!runElement.TryGetProperty("results", out var resultsElement) ||
            resultsElement.ValueKind != JsonValueKind.Array)
        {
            return results;
        }

        foreach (var resultElement in resultsElement.EnumerateArray())
        {
            // Skip suppressed results
            if (IsSuppressed(resultElement))
            {
                continue;
            }

            results.Add(ParseResult(resultElement));
        }

        return results;
    }

    /// <summary>
    ///     Determines if a result is suppressed.
    /// </summary>
    /// <param name="resultElement">The result JSON element.</param>
    /// <returns>True if the result has suppressions; otherwise, false.</returns>
    private static bool IsSuppressed(JsonElement resultElement)
    {
        if (resultElement.TryGetProperty("suppressions", out var suppressionsElement) &&
            suppressionsElement.ValueKind == JsonValueKind.Array)
        {
            return suppressionsElement.EnumerateArray().Any();
        }

        return false;
    }

    /// <summary>
    ///     Parses a single result element.
    /// </summary>
    /// <param name="resultElement">The result JSON element.</param>
    /// <returns>A parsed SARIF result.</returns>
    private static SarifResult ParseResult(JsonElement resultElement)
    {
        var ruleId = resultElement.TryGetProperty("ruleId", out var ruleIdElement)
            ? ruleIdElement.GetString() ?? string.Empty
            : string.Empty;

        var level = resultElement.TryGetProperty("level", out var levelElement)
            ? levelElement.GetString() ?? "warning"
            : "warning";

        var message = ParseMessage(resultElement);
        var (uri, startLine) = ParseLocation(resultElement);

        return new SarifResult(ruleId, level, message, uri, startLine);
    }

    /// <summary>
    ///     Parses the message from a result element.
    /// </summary>
    /// <param name="resultElement">The result JSON element.</param>
    /// <returns>The message text.</returns>
    private static string ParseMessage(JsonElement resultElement)
    {
        if (resultElement.TryGetProperty("message", out var messageElement) &&
            messageElement.TryGetProperty("text", out var messageTextElement))
        {
            return messageTextElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    /// <summary>
    ///     Parses location information from a result element.
    /// </summary>
    /// <param name="resultElement">The result JSON element.</param>
    /// <returns>A tuple containing the URI and start line.</returns>
    private static (string? Uri, int? StartLine) ParseLocation(JsonElement resultElement)
    {
        if (!resultElement.TryGetProperty("locations", out var locationsElement) ||
            locationsElement.ValueKind != JsonValueKind.Array)
        {
            return (null, null);
        }

        var firstLocation = locationsElement.EnumerateArray().FirstOrDefault();
        if (firstLocation.ValueKind == JsonValueKind.Undefined ||
            !firstLocation.TryGetProperty("physicalLocation", out var physicalLocationElement))
        {
            return (null, null);
        }

        var uri = ParseUri(physicalLocationElement);
        var startLine = ParseStartLine(physicalLocationElement);

        return (uri, startLine);
    }

    /// <summary>
    ///     Parses the URI from a physical location element.
    /// </summary>
    /// <param name="physicalLocationElement">The physical location JSON element.</param>
    /// <returns>The URI string or null.</returns>
    private static string? ParseUri(JsonElement physicalLocationElement)
    {
        if (physicalLocationElement.TryGetProperty("artifactLocation", out var artifactLocationElement) &&
            artifactLocationElement.TryGetProperty("uri", out var uriElement))
        {
            return uriElement.GetString();
        }

        return null;
    }

    /// <summary>
    ///     Parses the start line from a physical location element.
    /// </summary>
    /// <param name="physicalLocationElement">The physical location JSON element.</param>
    /// <returns>The start line number or null.</returns>
    private static int? ParseStartLine(JsonElement physicalLocationElement)
    {
        if (physicalLocationElement.TryGetProperty("region", out var regionElement) &&
            regionElement.TryGetProperty("startLine", out var startLineElement))
        {
            return startLineElement.GetInt32();
        }

        return null;
    }

    /// <summary>
    ///     Converts the SARIF results to markdown format.
    /// </summary>
    /// <param name="depth">The heading depth level (1-6) for the report title.</param>
    /// <param name="heading">Optional custom heading. If null, defaults to "[ToolName] Analysis".</param>
    /// <returns>Markdown representation of the SARIF results.</returns>
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
        AppendResultsSection(sb, subHeading);

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
        sb.AppendLine();
    }

    /// <summary>
    ///     Appends the results section with count and details.
    /// </summary>
    private void AppendResultsSection(StringBuilder sb, string subHeading)
    {
        sb.AppendLine($"{subHeading} Results");
        sb.AppendLine();

        sb.AppendLine(FormatFoundText(Results.Count, "result"));
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
    /// <returns>Formatted text like "Found no results", "Found 1 result", or "Found 5 results".</returns>
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
        if (string.IsNullOrEmpty(uri))
        {
            return "(no location)";
        }

        return startLine.HasValue ? $"{uri}({startLine})" : uri;
    }
}
