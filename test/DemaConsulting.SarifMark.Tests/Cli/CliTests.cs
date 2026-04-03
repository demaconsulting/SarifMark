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
///     Subsystem tests for the Command-Line Interface subsystem.
/// </summary>
[TestClass]
public class CliTests
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
    ///     Test that version flag outputs version information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Assert
        Assert.AreEqual(0, exitCode);
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
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Assert
        Assert.AreEqual(0, exitCode);
        Assert.Contains("Usage: sarifmark", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
        Assert.Contains("--help", output);
        Assert.Contains("--sarif", output);
        Assert.MatchesRegex(@"--report(?!-)", output);
    }

    /// <summary>
    ///     Test that silent flag suppresses console output.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_SilentFlag_SuppressesOutput()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--silent",
            "--sarif", sarifFile);

        // Assert
        Assert.AreEqual(0, exitCode);
        Assert.DoesNotContain("SarifMark version", output);
        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that log file parameter writes output to file.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_LogFile_WritesOutputToFile()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--log", logFile,
                "--sarif", sarifFile);

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(logFile), "Log file was not created");

            var logContent = File.ReadAllText(logFile);
            Assert.Contains("SarifMark version", logContent);
            Assert.Contains("SARIF File:", logContent);
            Assert.Contains("Tool: TestTool", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that enforce flag with issues returns error exit code.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_EnforceFlagWithIssues_ReturnsError()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile,
            "--enforce");

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Issues found in SARIF file", output);
    }

    /// <summary>
    ///     Test that unknown arguments are rejected with error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_UnknownArgument_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--unknown-flag");

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Error:", output);
        Assert.Contains("unknown-flag", output);
    }
}
