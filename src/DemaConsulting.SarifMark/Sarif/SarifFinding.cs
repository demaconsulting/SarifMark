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

namespace DemaConsulting.SarifMark;

/// <summary>
///     Represents a single SARIF finding from a SARIF file.
/// </summary>
public record SarifFinding
{
    /// <summary>
    ///     Gets the rule identifier that links this finding back to the tool rule definition,
    ///     allowing report readers to look up the rule documentation for the flagged issue.
    /// </summary>
    public string RuleId { get; }

    /// <summary>
    ///     Gets the severity level (error, warning, or note) that determines how the finding
    ///     is classified and displayed in the generated markdown report.
    /// </summary>
    public string Level { get; }

    /// <summary>
    ///     Gets the descriptive message text that explains what the finding means and how
    ///     to address the flagged issue.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the file URI where the finding was detected, enabling report readers to
    ///     navigate directly to the affected source file. Null when no physical location
    ///     is associated with the finding.
    /// </summary>
    public string? Uri { get; }

    /// <summary>
    ///     Gets the starting line number within the file, providing precise location
    ///     information for direct navigation to the affected code. Null when no line
    ///     information is available in the SARIF data.
    /// </summary>
    public int? StartLine { get; }

    /// <summary>
    ///     Internal constructor to enforce that instances are only created through the validated parsing pipeline.
    /// </summary>
    /// <remarks>
    ///     The constructor is <see langword="internal"/> to restrict instantiation to the parsing pipeline:
    ///     only <c>SarifResults.ParseResults</c> should construct <see cref="SarifFinding"/> instances,
    ///     ensuring every finding passes through the validated SARIF extraction logic before it reaches
    ///     report generation. The project file includes
    ///     <c>&lt;InternalsVisibleTo Include="DemaConsulting.SarifMark.Tests" /&gt;</c>,
    ///     which grants the test assembly permission to construct instances directly for unit testing
    ///     without relaxing the access restriction for production consumers.
    /// </remarks>
    /// <param name="ruleId">The rule identifier.</param>
    /// <param name="level">The level of the finding.</param>
    /// <param name="message">The message text.</param>
    /// <param name="uri">The file URI.</param>
    /// <param name="startLine">The starting line number.</param>
    internal SarifFinding(string ruleId, string level, string message, string? uri, int? startLine)
    {
        RuleId = ruleId;
        Level = level;
        Message = message;
        Uri = uri;
        StartLine = startLine;
    }
}
