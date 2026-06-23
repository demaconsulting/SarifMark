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
public class CliTests
{
    /// <summary>
    ///     Test that version flag sets the version flag in context.
    /// </summary>
    [Fact]
    public void Cli_Create_VersionFlag_SetsVersionFlag()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--version"]);

        // Assert
        Assert.True(context.Version);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that help flag sets the help flag in context.
    /// </summary>
    [Fact]
    public void Cli_Create_HelpFlag_SetsHelpFlag()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--help"]);

        // Assert
        Assert.True(context.Help);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that silent flag suppresses console output.
    /// </summary>
    [Fact]
    public void Cli_Create_SilentFlag_SuppressesOutput()
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
            Assert.Equal(0, context.ExitCode);
            Assert.Equal(string.Empty, output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that log file parameter writes output to file.
    /// </summary>
    [Fact]
    public void Cli_Create_LogFile_WritesOutputToFile()
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
            Assert.True(File.Exists(logFile), "Log file was not created");

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
    [Fact]
    public void Cli_Create_EnforceFlag_SetsEnforceFlag()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--enforce"]);

        // Assert
        Assert.True(context.Enforce);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that WriteError writes to stderr and sets exit code to one.
    /// </summary>
    [Fact]
    public void Cli_WriteError_WithMessage_SetsExitCodeToOne()
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
            Assert.Equal(1, context.ExitCode);
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
    [Fact]
    public void Cli_Create_UnknownArgument_ThrowsArgumentException()
    {
        // Arrange - No special setup needed

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--unknown-flag"]));

        // Assert
        Assert.Contains("unknown-flag", ex.Message);
    }

    /// <summary>
    ///     Test that validate flag sets the validate flag in context.
    /// </summary>
    [Fact]
    public void Cli_Create_ValidateFlag_SetsValidateFlag()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--validate"]);

        // Assert
        Assert.True(context.Validate);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that sarif parameter sets the SARIF file path in context.
    /// </summary>
    [Fact]
    public void Cli_Create_SarifParameter_SetsSarifFilePath()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--sarif", "analysis.sarif"]);

        // Assert
        Assert.Equal("analysis.sarif", context.SarifFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that report parameter sets the report file path in context.
    /// </summary>
    [Fact]
    public void Cli_Create_ReportParameter_SetsReportFilePath()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--report", "report.md"]);

        // Assert
        Assert.Equal("report.md", context.ReportFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that depth parameter sets the depth in context.
    /// </summary>
    [Fact]
    public void Cli_Create_DepthParameter_SetsDepth()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--depth", "3"]);

        // Assert
        Assert.Equal(3, context.Depth);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that legacy report-depth parameter sets the report depth in context.
    /// </summary>
    [Fact]
    public void Cli_Create_ReportDepthParameter_SetsReportDepth()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--report-depth", "3"]);

        // Assert
        Assert.Equal(3, context.Depth);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that heading parameter sets the custom heading in context.
    /// </summary>
    [Fact]
    public void Cli_Create_HeadingParameter_SetsCustomHeading()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--heading", "My Analysis"]);

        // Assert
        Assert.Equal("My Analysis", context.Heading);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that results parameter sets the results file path in context.
    /// </summary>
    [Fact]
    public void Cli_Create_ResultsParameter_SetsResultsFilePath()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--results", "results.trx"]);

        // Assert
        Assert.Equal("results.trx", context.ResultsFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the legacy --result alias sets the results file path in context.
    /// </summary>
    [Fact]
    public void Cli_Create_ResultLegacyAlias_SetsResultsFilePath()
    {
        // Arrange - No special setup needed

        // Act
        using var context = Context.Create(["--result", "results.trx"]);

        // Assert
        Assert.Equal("results.trx", context.ResultsFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that an invalid (non-integer) depth value throws ArgumentException.
    /// </summary>
    [Fact]
    public void Cli_Create_DepthInvalidValue_ThrowsArgumentException()
    {
        // Arrange - No special setup needed

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "abc"]));

        // Assert
        Assert.Contains("--depth requires an integer between 1 and 6", ex.Message);
    }

    /// <summary>
    ///     Test that the log file is written even when silent mode is not active (both console and log receive output).
    /// </summary>
    [Fact]
    public void Cli_Create_LogWithoutSilent_WritesToConsoleAndLogFile()
    {
        // Arrange
        var logFile = Path.Combine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            using (var context = Context.Create(["--log", logFile]))
            {
                context.WriteLine("Hello from both channels");
            }

            var consoleOutput = outWriter.ToString();
            var logContent = File.ReadAllText(logFile);

            // Assert
            Assert.Contains("Hello from both channels", consoleOutput);
            Assert.Contains("Hello from both channels", logContent);
        }
        finally
        {
            Console.SetOut(originalOut);
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that --depth 0 is rejected with an error.
    /// </summary>
    [Fact]
    public void Cli_Create_DepthZero_ThrowsArgumentException()
    {
        // Arrange
        // (no setup required)

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "0"]));

        // Assert
        Assert.Contains("--depth requires an integer between 1 and 6", ex.Message);
    }

    /// <summary>
    ///     Test that --depth -1 is rejected with an error.
    /// </summary>
    [Fact]
    public void Cli_Create_DepthNegative_ThrowsArgumentException()
    {
        // Arrange
        // (no setup required)

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "-1"]));

        // Assert
        Assert.Contains("--depth requires an integer between 1 and 6", ex.Message);
    }
}
