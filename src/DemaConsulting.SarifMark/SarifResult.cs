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
///     Represents a single result/issue from a SARIF file.
/// </summary>
public record SarifResult
{
    /// <summary>
    ///     Gets the rule identifier for this result.
    /// </summary>
    public string RuleId { get; }

    /// <summary>
    ///     Gets the level of the result (e.g., "error", "warning", "note").
    /// </summary>
    public string Level { get; }

    /// <summary>
    ///     Gets the message text describing the result.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the file URI where the result was found.
    /// </summary>
    public string? Uri { get; }

    /// <summary>
    ///     Gets the starting line number where the result was found.
    /// </summary>
    public int? StartLine { get; }

    /// <summary>
    ///     Internal constructor for testing purposes.
    /// </summary>
    /// <param name="ruleId">The rule identifier.</param>
    /// <param name="level">The level of the result.</param>
    /// <param name="message">The message text.</param>
    /// <param name="uri">The file URI.</param>
    /// <param name="startLine">The starting line number.</param>
    internal SarifResult(string ruleId, string level, string message, string? uri, int? startLine)
    {
        RuleId = ruleId;
        Level = level;
        Message = message;
        Uri = uri;
        StartLine = startLine;
    }
}
