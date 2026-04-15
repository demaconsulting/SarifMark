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
[TestClass]
public class SarifFindingTests
{
    /// <summary>
    ///     Test that the internal constructor stores all properties correctly.
    /// </summary>
    [TestMethod]
    public void SarifFinding_InternalConstructor_StoresAllProperties()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", "src/File.cs", 42);

        // Assert
        Assert.AreEqual("RULE001", result.RuleId);
        Assert.AreEqual("error", result.Level);
        Assert.AreEqual("Error message", result.Message);
        Assert.AreEqual("src/File.cs", result.Uri);
        Assert.AreEqual(42, result.StartLine);
    }

    /// <summary>
    ///     Test that Uri can be null.
    /// </summary>
    [TestMethod]
    public void SarifFinding_Uri_CanBeNull()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", null, null);

        // Assert
        Assert.IsNull(result.Uri);
    }

    /// <summary>
    ///     Test that StartLine can be null.
    /// </summary>
    [TestMethod]
    public void SarifFinding_StartLine_CanBeNull()
    {
        // Arrange & Act
        var result = new SarifFinding("RULE001", "error", "Error message", "src/File.cs", null);

        // Assert
        Assert.IsNull(result.StartLine);
    }
}
