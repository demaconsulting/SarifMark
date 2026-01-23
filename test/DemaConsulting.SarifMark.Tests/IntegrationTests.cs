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
///     Integration tests that run the SarifMark application through dotnet.
/// </summary>
[TestClass]
public class IntegrationTests
{
    private string _dllPath = string.Empty;
    private string _testDataPath = string.Empty;

    /// <summary>
    ///     Safely combines two paths, ensuring the second path doesn't contain path traversal sequences.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePath">The relative path to combine.</param>
    /// <returns>The combined path.</returns>
    private static string SafePathCombine(string basePath, string relativePath)
    {
        // Ensure the relative path doesn't contain path traversal sequences
        if (relativePath.Contains("..") || Path.IsPathRooted(relativePath))
        {
            throw new ArgumentException($"Invalid path component: {relativePath}", nameof(relativePath));
        }

        return Path.Combine(basePath, relativePath);
    }

    /// <summary>
    ///     Initialize test by locating the SarifMark DLL and test data.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = SafePathCombine(baseDir, "DemaConsulting.SarifMark.dll");
        _testDataPath = SafePathCombine(baseDir, "TestData");

        Assert.IsTrue(File.Exists(_dllPath), $"Could not find SarifMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Run the application with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version is output
        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that help flag outputs usage information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpFlag_OutputsUsageInformation()
    {
        // Run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify usage information
        Assert.Contains("Usage: sarifmark", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
        Assert.Contains("--help", output);
        Assert.Contains("--sarif", output);
        Assert.Contains("--report", output);
    }

    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ValidateFlag_RunsSelfValidation()
    {
        // Run the application with --validate flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Verify validation runs (exit code may be 0 or 1 depending on test results)
        Assert.IsTrue(exitCode is 0 or 1);

        // Verify validation output contains expected content
        Assert.Contains("SarifMark version", output);
        Assert.Contains("Total Tests:", output);
    }

    /// <summary>
    ///     Test that missing sarif parameter shows error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_MissingSarifParameter_ShowsError()
    {
        // Run the application without required parameters
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath);

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("--sarif parameter is required", output);
    }

    /// <summary>
    ///     Test that processing a valid SARIF file succeeds.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ValidSarifFile_ProcessesSuccessfully()
    {
        // Locate the test SARIF file
        var sarifFile = SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Run the application with the SARIF file
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile);

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify output contains expected information
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
    public void IntegrationTest_NonExistentSarifFile_ShowsError()
    {
        // Run the application with a non-existent file
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", "nonexistent.sarif");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that generating a report file succeeds.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_GenerateReport_CreatesReportFile()
    {
        // Locate the test SARIF file
        var sarifFile = SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Create a temporary report file path
        var reportFile = SafePathCombine(Path.GetTempPath(), $"test-report-{Guid.NewGuid()}.md");

        try
        {
            // Run the application with report generation
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile);

            // Verify success
            Assert.AreEqual(0, exitCode);

            // Verify output
            Assert.Contains("Writing report to", output);
            Assert.Contains("Report generated successfully", output);

            // Verify report file was created
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            // Verify report content
            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("# TestTool Analysis", reportContent);
        }
        finally
        {
            // Clean up the temporary report file
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }

    /// <summary>
    ///     Test that enforce flag with issues returns error exit code.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_EnforceFlagWithIssues_ReturnsError()
    {
        // Locate the test SARIF file (which has 1 result)
        var sarifFile = SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Run the application with --enforce flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile,
            "--enforce");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Issues found in SARIF file", output);
    }
}
