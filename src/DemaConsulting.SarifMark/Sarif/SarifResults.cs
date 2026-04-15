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
    ///     Gets the collection of all parsed runs.
    /// </summary>
    public IReadOnlyList<SarifRun> Runs { get; }

    /// <summary>
    ///     Gets a value indicating whether any run contains results.
    /// </summary>
    public bool HasIssues => Runs.Any(r => r.HasIssues);

    /// <summary>
    ///     Internal constructor for SARIF results built from one or more parsed runs.
    /// </summary>
    /// <param name="runs">The collection of parsed runs.</param>
    internal SarifResults(IReadOnlyList<SarifRun> runs)
    {
        Runs = runs;
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

            var runsElement = ValidateSarifStructure(root);
            var runs = new List<SarifRun>();
            foreach (var runElement in runsElement.EnumerateArray())
            {
                var (toolName, toolVersion) = ExtractToolInformation(runElement);
                var results = ParseResults(runElement);
                var fileCount = ExtractFileCount(runElement);
                runs.Add(new SarifRun(toolName, toolVersion, results, fileCount));
            }

            return new SarifResults(runs);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON in SARIF file: {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Validates the SARIF file structure and returns the runs array element.
    /// </summary>
    /// <param name="root">The root JSON element.</param>
    /// <returns>The runs array element.</returns>
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

        if (runsElement.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Invalid SARIF file: 'runs' array is empty.");
        }

        return runsElement;
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

        var toolVersion = ExtractToolVersion(driverElement);

        return (toolName, toolVersion);
    }

    /// <summary>
    ///     Extracts the tool version from a driver element.
    ///     Checks multiple version fields in priority order: version, semanticVersion, dottedQuadFileVersion.
    /// </summary>
    /// <param name="driverElement">The driver JSON element.</param>
    /// <returns>The tool version string, or "Unknown" if no version field is found.</returns>
    private static string ExtractToolVersion(JsonElement driverElement)
    {
        // Priority 1: version field
        if (driverElement.TryGetProperty("version", out var versionElement))
        {
            var version = versionElement.GetString();
            if (!string.IsNullOrWhiteSpace(version))
            {
                return version;
            }
        }

        // Priority 2: semanticVersion field
        if (driverElement.TryGetProperty("semanticVersion", out var semanticVersionElement))
        {
            var semanticVersion = semanticVersionElement.GetString();
            if (!string.IsNullOrWhiteSpace(semanticVersion))
            {
                return semanticVersion;
            }
        }

        // Priority 3: dottedQuadFileVersion field
        if (driverElement.TryGetProperty("dottedQuadFileVersion", out var dottedQuadElement))
        {
            var dottedQuadVersion = dottedQuadElement.GetString();
            if (!string.IsNullOrWhiteSpace(dottedQuadVersion))
            {
                return dottedQuadVersion;
            }
        }

        return "Unknown";
    }

    /// <summary>
    ///     Parses all results from a run element, excluding suppressed results.
    /// </summary>
    /// <param name="runElement">The run JSON element.</param>
    /// <returns>A list of parsed SARIF findings.</returns>
    private static List<SarifFinding> ParseResults(JsonElement runElement)
    {
        var results = new List<SarifFinding>();

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
    ///     Extracts the file count from the 'artifacts' array of the run element.
    /// </summary>
    /// <param name="runElement">The run JSON element.</param>
    /// <returns>The number of artifacts in the run, or zero if the artifacts array is absent.</returns>
    private static int ExtractFileCount(JsonElement runElement)
    {
        if (runElement.TryGetProperty("artifacts", out var artifactsElement) &&
            artifactsElement.ValueKind == JsonValueKind.Array)
        {
            return artifactsElement.GetArrayLength();
        }

        return 0;
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
    /// <returns>A parsed SARIF finding.</returns>
    private static SarifFinding ParseResult(JsonElement resultElement)
    {
        var ruleId = resultElement.TryGetProperty("ruleId", out var ruleIdElement)
            ? ruleIdElement.GetString() ?? string.Empty
            : string.Empty;

        var level = resultElement.TryGetProperty("level", out var levelElement)
            ? levelElement.GetString() ?? "warning"
            : "warning";

        var message = ParseMessage(resultElement);
        var (uri, startLine) = ParseLocation(resultElement);

        return new SarifFinding(ruleId, level, message, uri, startLine);
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
            regionElement.TryGetProperty("startLine", out var startLineElement) &&
            startLineElement.TryGetInt32(out var startLine))
        {
            return startLine;
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

        if (Runs.Count == 1)
        {
            return Runs[0].ToMarkdown(depth, heading);
        }

        // Multi-run: concatenate all runs with indexed headings
        var sb = new StringBuilder();
        for (var i = 0; i < Runs.Count; i++)
        {
            var run = Runs[i];
            var runHeading = !string.IsNullOrWhiteSpace(heading)
                ? $"{heading} (#{i + 1})"
                : $"{run.ToolName} Analysis (#{i + 1})";
            sb.Append(run.ToMarkdown(depth, runHeading));
        }

        return sb.ToString();
    }
}
