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
    ///     Test that version flag outputs version information.
    /// </summary>
    [TestMethod]
    public void Cli_VersionFlag_OutputsVersion()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var exitCode = Program.Main(["--version"]);
            var output = outWriter.ToString();

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(output));
            Assert.DoesNotContain("Error", output);
            Assert.DoesNotContain("Copyright", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that help flag outputs usage information.
    /// </summary>
    [TestMethod]
    public void Cli_HelpFlag_OutputsUsageInformation()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var exitCode = Program.Main(["--help"]);
            var output = outWriter.ToString();

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.Contains("Usage: sarifmark", output);
            Assert.Contains("Options:", output);
            Assert.Contains("--version", output);
            Assert.Contains("--help", output);
            Assert.Contains("--sarif", output);
            Assert.MatchesRegex(@"--report(?!-)", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that silent flag suppresses console output.
    /// </summary>
    [TestMethod]
    public void Cli_SilentFlag_SuppressesOutput()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var exitCode = Program.Main(["--silent", "--sarif", sarifFile]);
            var output = outWriter.ToString() + errWriter.ToString();

            // Assert
            Assert.AreEqual(0, exitCode);
            Assert.DoesNotContain("SarifMark version", output);
            Assert.DoesNotContain("Copyright", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that log file parameter writes output to file.
    /// </summary>
    [TestMethod]
    public void Cli_LogFile_WritesOutputToFile()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var logFile = Path.Combine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");

        try
        {
            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);

                // Act
                var exitCode = Program.Main(["--log", logFile, "--sarif", sarifFile]);

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
                Console.SetOut(originalOut);
            }
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
    public void Cli_EnforceFlagWithIssues_ReturnsError()
    {
        // Arrange
        var sarifFile = Path.Combine(_testDataPath, "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var exitCode = Program.Main(["--sarif", sarifFile, "--enforce"]);
            var output = outWriter.ToString() + errWriter.ToString();

            // Assert
            Assert.AreEqual(1, exitCode);
            Assert.Contains("Issues found in SARIF file", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that unknown arguments are rejected with error.
    /// </summary>
    [TestMethod]
    public void Cli_UnknownArgument_ShowsError()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            // Act
            var exitCode = Program.Main(["--unknown-flag"]);
            var errorOutput = errWriter.ToString();

            // Assert
            Assert.AreEqual(1, exitCode);
            Assert.Contains("Unsupported argument", errorOutput);
            Assert.Contains("unknown-flag", errorOutput);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }
}
