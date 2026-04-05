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
    private string _dllPath = string.Empty;
    private string _testDataPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating the SarifMark DLL and test data.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        var baseDir = AppContext.BaseDirectory;
        _dllPath = PathHelpers.SafePathCombine(baseDir, "DemaConsulting.SarifMark.dll");
        _testDataPath = PathHelpers.SafePathCombine(baseDir, "TestData");

        Assert.IsTrue(File.Exists(_dllPath), $"Could not find SarifMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that missing sarif parameter shows error.
    /// </summary>
    [TestMethod]
    public void Sarif_MissingSarifParameter_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath);

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("--sarif parameter is required", output);
    }

    /// <summary>
    ///     Test that processing a valid SARIF file succeeds.
    /// </summary>
    [TestMethod]
    public void Sarif_ValidSarifFile_ProcessesSuccessfully()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile);

        // Assert
        Assert.AreEqual(0, exitCode);
        Assert.Contains("SarifMark version", output);
        Assert.Contains("SARIF File:", output);
        Assert.Contains("Reading SARIF file...", output);
        Assert.Contains("Tool: TestTool", output);
        Assert.Contains("Results: 1", output);
    }

    /// <summary>
    ///     Test that processing a non-existent SARIF file shows error.
    /// </summary>
    [TestMethod]
    public void Sarif_NonExistentSarifFile_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", "nonexistent.sarif");

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that generating a report file succeeds.
    /// </summary>
    [TestMethod]
    public void Sarif_GenerateReport_CreatesReportFile()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-report-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile);

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.Contains("Writing report to", output);
            Assert.Contains("Report generated successfully", output);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("# TestTool Analysis", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that report depth parameter is configurable.
    /// </summary>
    [TestMethod]
    public void Sarif_ReportDepth_IsConfigurable()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-report-depth-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile,
                "--report-depth", "3");

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("### TestTool Analysis", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that processing an invalid SARIF file shows a format error.
    /// </summary>
    [TestMethod]
    public void Sarif_InvalidSarifFile_ShowsFormatError()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "invalid.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile);

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that a generated report formats multiple results with proper line breaks.
    /// </summary>
    [TestMethod]
    public void Sarif_GenerateReport_FormatsMultipleResultsWithLineBreaks()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "multi-result.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-multi-report-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile);

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("Found 2 issues", reportContent);
            Assert.Contains("first.cs", reportContent);
            Assert.Contains("second.cs", reportContent);

            // Verify results appear on separate lines with proper markdown line breaks
            Assert.MatchesRegex(@"first\.cs.*  \r?\nfile:///path/to/second\.cs", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that a generated report contains result count information.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_ContainsResultCount()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-count-report-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile);

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("Found 1 issue", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that a generated report contains location information for results.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_ContainsLocationInfo()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-location-report-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile);

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("file:///path/to/file.cs", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that a generated report uses a custom heading when provided.
    /// </summary>
    [TestMethod]
    public void Sarif_Report_UsesCustomHeading()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-heading-report-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile,
                "--heading", "Custom Analysis Heading");

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("Custom Analysis Heading", reportContent);
        }
        finally
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }
}
