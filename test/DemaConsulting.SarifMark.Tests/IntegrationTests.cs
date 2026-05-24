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
public class IntegrationTests
{
    private readonly string _dllPath;
    private readonly string _testDataPath;

    /// <summary>
    ///     Initialize test by locating the SarifMark DLL and test data.
    /// </summary>
    public IntegrationTests()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = PathHelpers.SafePathCombine(baseDir, "DemaConsulting.SarifMark.dll");
        _testDataPath = PathHelpers.SafePathCombine(baseDir, "TestData");

        Assert.True(File.Exists(_dllPath), $"Could not find SarifMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information.
    /// </summary>
    [Fact]
    public void SarifMark_VersionFlag_OutputsVersion()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.False(string.IsNullOrWhiteSpace(output));
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Copyright", output);
        Assert.Matches(@"\d+\.\d+\.\d+", output);
    }

    /// <summary>
    ///     Test that help flag outputs usage information.
    /// </summary>
    [Fact]
    public void SarifMark_HelpFlag_OutputsUsageInformation()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Contains("Usage: sarifmark", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
        Assert.Contains("--help", output);
        Assert.Contains("--sarif", output);
        Assert.Matches(@"--report(?!-)", output);
    }

    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [Fact]
    public void SarifMark_ValidateFlag_RunsSelfValidation()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Contains("SarifMark version", output);
        Assert.Contains("Total Tests:", output);
        Assert.Contains("Failed: 0", output);
    }

    /// <summary>
    ///     Test that missing sarif parameter shows error.
    /// </summary>
    [Fact]
    public void SarifMark_MissingSarifParameter_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath);

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Contains("--sarif parameter is required", output);
    }

    /// <summary>
    ///     Test that processing a valid SARIF file succeeds.
    /// </summary>
    [Fact]
    public void SarifMark_ValidSarifFile_ProcessesSuccessfully()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Contains("SarifMark version", output);
        Assert.Contains("SARIF File:", output);
        Assert.Contains("Reading SARIF file...", output);
        Assert.Contains("Tool: TestTool", output);
        Assert.Contains("Results: 1", output);
    }

    /// <summary>
    ///     Test that processing a non-existent SARIF file shows error.
    /// </summary>
    [Fact]
    public void SarifMark_NonExistentSarifFile_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", "nonexistent.sarif");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that generating a report file succeeds.
    /// </summary>
    [Fact]
    public void SarifMark_GenerateReport_CreatesReportFile()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

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
            Assert.Equal(0, exitCode);
            Assert.Contains("Writing report to", output);
            Assert.Contains("Report generated successfully", output);
            Assert.True(File.Exists(reportFile), "Report file was not created");

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
    [Fact]
    public void SarifMark_EnforceFlagWithIssues_ReturnsError()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile,
            "--enforce");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Contains("Issues found in SARIF file", output);
    }

    /// <summary>
    ///     Test that silent flag suppresses console output.
    /// </summary>
    [Fact]
    public void SarifMark_SilentFlag_SuppressesOutput()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--silent",
            "--sarif", sarifFile);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.DoesNotContain("SarifMark version", output);
        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that log file parameter writes output to file.
    /// </summary>
    [Fact]
    public void SarifMark_LogFile_WritesOutputToFile()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

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
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(logFile), "Log file was not created");

            var logContent = File.ReadAllText(logFile);
            Assert.Contains("SarifMark version", logContent);
            Assert.Contains("SARIF File:", logContent);
            Assert.Contains("Tool: TestTool", logContent);
        }
        finally
        {
            // Clean up the temporary log file
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that unknown arguments are rejected with error.
    /// </summary>
    [Fact]
    public void SarifMark_UnknownArgument_ShowsError()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--unknown-flag");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Contains("Error:", output);
        Assert.Contains("unknown-flag", output);
    }

    /// <summary>
    ///     Test that depth parameter is configurable.
    /// </summary>
    [Fact]
    public void SarifMark_ReportDepth_IsConfigurable()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

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
                "--depth", "3");

            // Assert
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("### TestTool Analysis", reportContent);
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
    ///     Test that legacy report-depth parameter is still accepted.
    /// </summary>
    [Fact]
    public void SarifMark_LegacyReportDepth_IsAccepted()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-legacy-report-depth-{Guid.NewGuid()}.md");

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
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("### TestTool Analysis", reportContent);
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
    ///     Test that a custom heading appears in the generated report.
    /// </summary>
    [Fact]
    public void SarifMark_CustomHeading_AppearsInReport()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-custom-heading-{Guid.NewGuid()}.md");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--report", reportFile,
                "--heading", "Custom Analysis");

            // Assert
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("# Custom Analysis", reportContent);
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
    ///     Test that a multi-run SARIF file creates a report with sections for each run.
    /// </summary>
    [Fact]
    public void SarifMark_MultiRunSarifFile_CreatesReport()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "multi-run.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-multi-run-report-{Guid.NewGuid()}.md");

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
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("Tool1", reportContent);
            Assert.Contains("Tool2", reportContent);
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
}
