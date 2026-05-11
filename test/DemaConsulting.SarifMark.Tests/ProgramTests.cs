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
///     Unit tests for the Program class.
/// </summary>
public class ProgramTests
{
    /// <summary>
    ///     Test that Main with no arguments returns error due to missing sarif parameter.
    /// </summary>
    [Fact]
    public void Program_Main_NoArguments_ReturnsError()
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
            var result = Program.Main([]);

            // Assert
            Assert.Equal(1, result);
            Assert.Contains("SarifMark version", outWriter.ToString());
            Assert.Contains("--sarif parameter is required", errWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Main with --version flag displays version only.
    /// </summary>
    [Fact]
    public void Program_Main_VersionFlag_DisplaysVersionOnly()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--version"]);

            // Assert
            Assert.Equal(0, result);
            var output = outWriter.ToString().Trim();
            Assert.Matches(@"^\d+\.\d+\.\d+(?:-[\w.-]+)?(?:\+[\w.-]+)?$", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with --help flag displays help with banner.
    /// </summary>
    [Fact]
    public void Program_Main_HelpFlag_DisplaysHelp()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--help"]);

            // Assert
            Assert.Equal(0, result);
            var output = outWriter.ToString();
            Assert.Contains("SarifMark version", output);
            Assert.Contains("Copyright", output);
            Assert.Contains("Usage:", output);
            Assert.Contains("--version", output);
            Assert.Contains("--help", output);
            Assert.Contains("--silent", output);
            Assert.Contains("--validate", output);
            Assert.Contains("--results", output);
            Assert.Contains("--enforce", output);
            Assert.Contains("--log", output);
            Assert.Contains("--sarif", output);
            Assert.Matches(@"--report(?!-)", output);
            Assert.Contains("--depth", output);
            Assert.Contains("--heading", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with unknown argument returns error.
    /// </summary>
    [Fact]
    public void Program_Main_UnknownArgument_ReturnsError()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            // Act
            var result = Program.Main(["--unknown"]);

            // Assert
            Assert.Equal(1, result);
            Assert.Contains("Unsupported argument", errWriter.ToString());
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Main with --validate flag runs self-validation.
    /// </summary>
    [Fact]
    public void Program_Main_ValidateFlag_RunsValidation()
    {
        // Act - run in silent mode to suppress all console output
        var result = Program.Main(["--validate", "--silent"]);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    ///     Test that Main processes a valid SARIF file successfully.
    /// </summary>
    [Fact]
    public void Program_Main_ValidSarifFile_ProcessesSuccessfully()
    {
        // Arrange
        var sarifFile = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.sarif");
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--sarif", sarifFile]);

            // Assert
            Assert.Equal(0, result);
            Assert.Contains("Tool: TestTool", outWriter.ToString());
            Assert.Contains("1.0.0", outWriter.ToString());
            Assert.Contains("Results: 1", outWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with --silent flag suppresses banner output.
    /// </summary>
    [Fact]
    public void Program_Main_SilentFlag_SuppressesBanner()
    {
        // Arrange
        var sarifFile = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.sarif");
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--silent", "--sarif", sarifFile]);

            // Assert
            Assert.Equal(0, result);
            var output = outWriter.ToString();
            Assert.DoesNotContain("SarifMark version", output);
            Assert.DoesNotContain("Copyright", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that enforce flag returns error exit code when issues are found.
    /// </summary>
    [Fact]
    public void Program_Main_EnforceFlagWithIssues_ReturnsError()
    {
        // Arrange
        var sarifFile = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.sarif");
        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var result = Program.Main(["--sarif", sarifFile, "--enforce"]);

            // Assert
            Assert.Equal(1, result);
            Assert.Contains("Issues found", errWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Main with report file creates the report.
    /// </summary>
    [Fact]
    public void Program_Main_ReportFile_CreatesReport()
    {
        // Arrange
        var sarifFile = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.sarif");
        var reportFile = Path.Combine(Path.GetTempPath(), $"test-report-{Guid.NewGuid()}.md");
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--sarif", sarifFile, "--report", reportFile]);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(reportFile), "Report file was not created");
        }
        finally
        {
            Console.SetOut(originalOut);
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        }
    }
}
