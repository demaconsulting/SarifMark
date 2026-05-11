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

namespace DemaConsulting.SarifMark.Tests;

/// <summary>
///     Unit tests for the SarifFinding record.
/// </summary>
public class SarifFindingTests
{
    /// <summary>
    ///     Test that the constructor stores all properties when all properties are provided.
    /// </summary>
    [Fact]
    public void SarifFinding_Constructor_AllPropertiesProvided_StoresAllProperties()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", "src/File.cs", 42);

        // Assert
        Assert.Equal("RULE001", result.RuleId);
        Assert.Equal("error", result.Level);
        Assert.Equal("Error message", result.Message);
        Assert.Equal("src/File.cs", result.Uri);
        Assert.Equal(42, result.StartLine);
    }

    /// <summary>
    ///     Test that the Uri property is null when a null uri is provided to the constructor.
    /// </summary>
    [Fact]
    public void SarifFinding_Constructor_NullUri_UriPropertyIsNull()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", null, null);

        // Assert
        Assert.Null(result.Uri);
    }

    /// <summary>
    ///     Test that the StartLine property is null when a null start line is provided to the constructor.
    /// </summary>
    [Fact]
    public void SarifFinding_Constructor_NullStartLine_StartLinePropertyIsNull()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", "src/File.cs", null);

        // Assert
        Assert.Null(result.StartLine);
    }
}
