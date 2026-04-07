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
///     Subsystem tests for the SARIF reading subsystem.
/// </summary>
[TestClass]
public class SarifTests
{
    private string _testDataPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating test data.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData");
    }

    /// <summary>
    ///     Test that missing sarif parameter shows error.
    /// </summary>
    [TestMethod]
    public void Sarif_MissingSarifParameter_ShowsError()
    {
        // Arrange
        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var exitCode = Program.Main([]);
            var output = outWriter.ToString() + errWriter.ToString();

            // Assert
            Assert.AreEqual(1, exitCode);
            Assert.Contains("--sarif parameter is required", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that processing a valid SARIF file succeeds.
    /// </summary>
    [TestMethod]
    public void Sarif_ValidSarifFile_ProcessesSuccessfully()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);

        // Assert
        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual(1, results.ResultCount);
    }

    /// <summary>
    ///     Test that processing a non-existent SARIF file throws FileNotFoundException.
    /// </summary>
    [TestMethod]
    public void Sarif_NonExistentSarifFile_ShowsError()
    {
        // Arrange - No special setup needed

        // Act / Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => SarifResults.Read("nonexistent.sarif"));
    }

    /// <summary>
    ///     Test that generating a report file succeeds.
    /// </summary>
    [TestMethod]
    public void Sarif_GenerateReport_CreatesReportFile()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(1);

        // Assert
        Assert.Contains("# TestTool Analysis", reportContent);
    }

    /// <summary>
    ///     Test that report depth parameter is configurable.
    /// </summary>
    [TestMethod]
    public void Sarif_ReportDepth_IsConfigurable()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(3);

        // Assert
        Assert.Contains("### TestTool Analysis", reportContent);
    }

    /// <summary>
    ///     Test that processing an invalid SARIF file throws InvalidOperationException.
    /// </summary>
    [TestMethod]
    public void Sarif_InvalidSarifFile_ShowsFormatError()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "invalid.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act / Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => SarifResults.Read(sarifFile));
    }

    /// <summary>
    ///     Test that a generated report formats multiple results with proper line breaks.
    /// </summary>
    [TestMethod]
    public void Sarif_GenerateReport_FormatsMultipleResultsWithLineBreaks()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "multi-result.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 2 issues", reportContent);
        Assert.Contains("first.cs", reportContent);
        Assert.Contains("second.cs", reportContent);

        // Verify results appear on separate lines with proper markdown line breaks
        Assert.MatchesRegex(@"first\.cs.*  \r?\nfile:///path/to/second\.cs", reportContent);
    }

    /// <summary>
    ///     Test that a generated report contains result count information.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_ContainsResultCount()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 1 issue", reportContent);
    }

    /// <summary>
    ///     Test that a generated report contains location information for results.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_ContainsLocationInfo()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(1);

        // Assert
        Assert.Contains("file:///path/to/file.cs", reportContent);
    }

    /// <summary>
    ///     Test that a generated report uses a custom heading when provided.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_UsesCustomHeading()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var results = SarifResults.Read(sarifFile);
        var reportContent = results.ToMarkdown(1, "Custom Analysis Heading");

        // Assert
        Assert.Contains("Custom Analysis Heading", reportContent);
    }
}
