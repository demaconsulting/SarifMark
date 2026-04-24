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
///     Unit tests for the SarifRun record.
/// </summary>
[TestClass]
public class SarifRunTests
{
    /// <summary>
    ///     Test that the internal constructor creates a valid SarifRun instance.
    /// </summary>
    [TestMethod]
    public void SarifRun_InternalConstructor_CreatesValidInstance()
    {
        // Arrange
        var results = new List<SarifFinding> { new SarifFinding("R1", "warning", "msg", null, null) };

        // Act
        var run = new SarifRun("MyTool", "1.0.0", results, 3);

        // Assert
        Assert.AreEqual("MyTool", run.ToolName);
        Assert.AreEqual("1.0.0", run.ToolVersion);
        Assert.AreEqual(1, run.ResultCount);
        Assert.AreEqual(3, run.FileCount);
        Assert.IsNotNull(run.Results);
        Assert.IsTrue(run.HasIssues);
    }

    /// <summary>
    ///     Test that HasIssues returns false when there are no results.
    /// </summary>
    [TestMethod]
    public void SarifRun_HasIssues_NoResults_ReturnsFalse()
    {
        // Arrange
        var run = new SarifRun("Tool", "1.0", [], 0);

        // Assert
        Assert.IsFalse(run.HasIssues);
    }

    /// <summary>
    ///     Test that HasIssues returns true when results are present.
    /// </summary>
    [TestMethod]
    public void SarifRun_HasIssues_WithResults_ReturnsTrue()
    {
        // Arrange
        var results = new List<SarifFinding> { new SarifFinding("R1", "warning", "msg", null, null) };

        // Act
        var run = new SarifRun("Tool", "1.0", results, 0);

        // Assert
        Assert.IsTrue(run.HasIssues);
    }

    /// <summary>
    ///     Test that ToMarkdown with no results shows the correct message.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_NoResults_ShowsFoundNoResults()
    {
        // Arrange
        var run = new SarifRun("Tool", "1.0", [], 0);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("Found no issues", md);
    }

    /// <summary>
    ///     Test that ToMarkdown with results shows the results correctly.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_WithResults_ShowsResults()
    {
        // Arrange
        var results = new List<SarifFinding> { new SarifFinding("RULE1", "warning", "Test message", "file:///test.cs", 5) };
        var run = new SarifRun("Tool", "1.0", results, 1);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 1 issue", md);
        Assert.Contains("RULE1", md);
        Assert.Contains("Test message", md);
        Assert.Contains("file:///test.cs(5):", md);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth 1 produces the correct output structure.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_Depth1_ProducesCorrectOutput()
    {
        // Arrange
        var run = new SarifRun("TestTool", "1.0.0", [], 2);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("# TestTool Analysis", md);
        Assert.Contains("**Tool:** TestTool 1.0.0", md);
        Assert.Contains("**Files:** 2", md);
        Assert.Contains("## Issues", md);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth less than 1 throws ArgumentOutOfRangeException.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var run = new SarifRun("Tool", "1.0", [], 0);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => run.ToMarkdown(0));
    }

    /// <summary>
    ///     Test that ToMarkdown with depth greater than 6 throws ArgumentOutOfRangeException.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var run = new SarifRun("Tool", "1.0", [], 0);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => run.ToMarkdown(7));
    }

    /// <summary>
    ///     Test that ToMarkdown uses the provided custom heading.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_CustomHeading_UsesProvidedHeading()
    {
        // Arrange
        var run = new SarifRun("Tool", "1.0", [], 0);

        // Act
        var md = run.ToMarkdown(1, "My Custom Heading");

        // Assert
        Assert.Contains("# My Custom Heading", md);
    }

    /// <summary>
    ///     Test that ToMarkdown with null heading uses the default heading.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_NullHeading_UsesDefaultHeading()
    {
        // Arrange
        var run = new SarifRun("MyTool", "1.0", [], 0);

        // Act
        var md = run.ToMarkdown(1, null);

        // Assert
        Assert.Contains("# MyTool Analysis", md);
    }

    /// <summary>
    ///     Test that ToMarkdown with whitespace heading uses the default heading.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_WhitespaceHeading_UsesDefaultHeading()
    {
        // Arrange
        var run = new SarifRun("MyTool", "1.0", [], 0);

        // Act
        var md = run.ToMarkdown(1, "   ");

        // Assert
        Assert.Contains("# MyTool Analysis", md);
    }

    /// <summary>
    ///     Test that ToMarkdown shows the file count.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_ShowsFileCount()
    {
        // Arrange
        var run = new SarifRun("TestTool", "1.0.0", [], 5);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("**Files:** 5", md);
    }

    /// <summary>
    ///     Test that ToMarkdown formats results without location correctly.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_ResultWithoutLocation_ShowsNoLocation()
    {
        // Arrange
        var results = new List<SarifFinding> { new("RULE001", "error", "Error without location", null, null) };
        var run = new SarifRun("TestTool", "1.0.0", results, 0);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("(no location): error [RULE001] Error without location", md);
    }

    /// <summary>
    ///     Test that ToMarkdown formats results with URI but no line number correctly.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly()
    {
        // Arrange
        var results = new List<SarifFinding> { new("RULE002", "warning", "Warning with URI only", "src/File.cs", null) };
        var run = new SarifRun("TestTool", "1.0.0", results, 0);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("src/File.cs: warning [RULE002] Warning with URI only", md);
        Assert.DoesNotContain("src/File.cs(", md);
    }

    /// <summary>
    ///     Test that ToMarkdown uses singular form for one result.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_OneResult_UsesSingularForm()
    {
        // Arrange
        var results = new List<SarifFinding> { new("CA1001", "warning", "Test warning", "src/Test.cs", 10) };
        var run = new SarifRun("TestTool", "1.0.0", results, 0);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 1 issue", md);
        Assert.DoesNotContain("Found 1 issues", md);
    }

    /// <summary>
    ///     Test that ToMarkdown uses plural form for multiple results.
    /// </summary>
    [TestMethod]
    public void SarifRun_ToMarkdown_MultipleResults_UsesPluralForm()
    {
        // Arrange
        var results = new List<SarifFinding>
        {
            new("CA1001", "warning", "First issue", "src/A.cs", 1),
            new("CA1002", "error", "Second issue", "src/B.cs", 2),
            new("CA1003", "note", "Third issue", "src/C.cs", 3)
        };
        var run = new SarifRun("TestTool", "1.0.0", results, 0);

        // Act
        var md = run.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 3 issues", md);
        Assert.DoesNotContain("Found 3 issue ", md);
    }
}
