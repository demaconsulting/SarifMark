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
    ///     Gets the total number of results/issues found.
    /// </summary>
    public int ResultCount { get; }

    /// <summary>
    ///     Internal constructor for testing purposes.
    /// </summary>
    /// <param name="toolName">The name of the analysis tool.</param>
    /// <param name="toolVersion">The version of the analysis tool.</param>
    /// <param name="resultCount">The total number of results/issues found.</param>
    internal SarifResults(string toolName, string toolVersion, int resultCount)
    {
        ToolName = toolName;
        ToolVersion = toolVersion;
        ResultCount = resultCount;
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

            // Validate SARIF version
            if (!root.TryGetProperty("version", out _))
            {
                throw new InvalidOperationException("Invalid SARIF file: missing 'version' property.");
            }

            // Get runs array
            if (!root.TryGetProperty("runs", out var runsElement) || runsElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("Invalid SARIF file: missing or invalid 'runs' array.");
            }

            var runs = runsElement.EnumerateArray();
            if (!runs.Any())
            {
                throw new InvalidOperationException("Invalid SARIF file: 'runs' array is empty.");
            }

            // Get the first run
            var firstRun = runs.First();

            // Get tool information
            if (!firstRun.TryGetProperty("tool", out var toolElement))
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

            // Get results count
            var resultCount = 0;
            if (firstRun.TryGetProperty("results", out var resultsElement) &&
                resultsElement.ValueKind == JsonValueKind.Array)
            {
                resultCount = resultsElement.GetArrayLength();
            }

            return new SarifResults(toolName, toolVersion, resultCount);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON in SARIF file: {ex.Message}", ex);
        }
    }
}
