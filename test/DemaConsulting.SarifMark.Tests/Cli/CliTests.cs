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
    /// <summary>
    ///     Test that version flag sets the version flag in context.
    /// </summary>
    [TestMethod]
    public void Cli_VersionFlag_SetsVersionFlag()
    {
        // Act
        using var context = Context.Create(["--version"]);

        // Assert
        Assert.IsTrue(context.Version);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that help flag sets the help flag in context.
    /// </summary>
    [TestMethod]
    public void Cli_HelpFlag_SetsHelpFlag()
    {
        // Act
        using var context = Context.Create(["--help"]);

        // Assert
        Assert.IsTrue(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that silent flag suppresses console output.
    /// </summary>
    [TestMethod]
    public void Cli_SilentFlag_SuppressesOutput()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            using var context = Context.Create(["--silent"]);
            context.WriteLine("SarifMark version 1.0");
            context.WriteLine("Copyright");
            var output = outWriter.ToString();

            // Assert
            Assert.AreEqual(0, context.ExitCode);
            Assert.AreEqual(string.Empty, output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that log file parameter writes output to file.
    /// </summary>
    [TestMethod]
    public void Cli_LogFile_WritesOutputToFile()
    {
        // Arrange
        var logFile = Path.Combine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");

        try
        {
            // Act
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                context.WriteLine("SarifMark version 1.0");
                context.WriteLine("SARIF File: test.sarif");
                context.WriteLine("Tool: TestTool");
            }

            // Assert
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
    ///     Test that enforce flag sets the enforce flag in context.
    /// </summary>
    [TestMethod]
    public void Cli_EnforceFlag_SetsEnforceFlag()
    {
        // Act
        using var context = Context.Create(["--enforce"]);

        // Assert
        Assert.IsTrue(context.Enforce);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that WriteError writes to stderr and sets exit code to one.
    /// </summary>
    [TestMethod]
    public void Cli_WriteError_SetsExitCodeToOne()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            // Act
            using var context = Context.Create([]);
            context.WriteError("Test error message");
            var output = errWriter.ToString();

            // Assert
            Assert.AreEqual(1, context.ExitCode);
            Assert.Contains("Test error message", output);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that unknown arguments are rejected by throwing ArgumentException.
    /// </summary>
    [TestMethod]
    public void Cli_UnknownArgument_ThrowsArgumentException()
    {
        // Arrange - No special setup needed

        // Act
        var ex = Assert.ThrowsExactly<ArgumentException>(() => Context.Create(["--unknown-flag"]));

        // Assert
        Assert.Contains("unknown-flag", ex.Message);
    }

    /// <summary>
    ///     Test that validate flag sets the validate flag in context.
    /// </summary>
    [TestMethod]
    public void Cli_ValidateFlag_SetsValidateFlag()
    {
        // Act
        using var context = Context.Create(["--validate"]);

        // Assert
        Assert.IsTrue(context.Validate);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that sarif parameter sets the SARIF file path in context.
    /// </summary>
    [TestMethod]
    public void Cli_SarifParameter_SetsSarifFilePath()
    {
        // Act
        using var context = Context.Create(["--sarif", "analysis.sarif"]);

        // Assert
        Assert.AreEqual("analysis.sarif", context.SarifFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that report parameter sets the report file path in context.
    /// </summary>
    [TestMethod]
    public void Cli_ReportParameter_SetsReportFilePath()
    {
        // Act
        using var context = Context.Create(["--report", "report.md"]);

        // Assert
        Assert.AreEqual("report.md", context.ReportFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that depth parameter sets the report depth in context.
    /// </summary>
    [TestMethod]
    public void Cli_DepthParameter_SetsReportDepth()
    {
        // Act
        using var context = Context.Create(["--depth", "3"]);

        // Assert
        Assert.AreEqual(3, context.ReportDepth);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that legacy report-depth parameter sets the report depth in context.
    /// </summary>
    [TestMethod]
    public void Cli_ReportDepthParameter_SetsReportDepth()
    {
        // Act
        using var context = Context.Create(["--report-depth", "3"]);

        // Assert
        Assert.AreEqual(3, context.ReportDepth);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that heading parameter sets the custom heading in context.
    /// </summary>
    [TestMethod]
    public void Cli_HeadingParameter_SetsCustomHeading()
    {
        // Act
        using var context = Context.Create(["--heading", "My Analysis"]);

        // Assert
        Assert.AreEqual("My Analysis", context.Heading);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that results parameter sets the results file path in context.
    /// </summary>
    [TestMethod]
    public void Cli_ResultsParameter_SetsResultsFilePath()
    {
        // Act
        using var context = Context.Create(["--results", "results.trx"]);

        // Assert
        Assert.AreEqual("results.trx", context.ResultsFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the legacy --result alias sets the results file path in context.
    /// </summary>
    [TestMethod]
    public void Cli_ResultLegacyAlias_SetsResultsFilePath()
    {
        // Act
        using var context = Context.Create(["--result", "results.trx"]);

        // Assert
        Assert.AreEqual("results.trx", context.ResultsFile);
        Assert.AreEqual(0, context.ExitCode);
    }
}
