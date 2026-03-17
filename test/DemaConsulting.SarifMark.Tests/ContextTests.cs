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
///     Unit tests for the Context class.
/// </summary>
[TestClass]
public class ContextTests
{
    /// <summary>
    ///     Test creating a context with no arguments.
    /// </summary>
    [TestMethod]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Act
        using var context = Context.Create([]);

        // Assert
        Assert.IsFalse(context.Version);
        Assert.IsFalse(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that creating a context with the version flag sets the Version property to true.
    /// </summary>
    [TestMethod]
    public void Context_Create_VersionFlag_SetsVersionTrue()
    {
        // Act
        using var context = Context.Create(["--version"]);

        // Assert
        Assert.IsTrue(context.Version);
        Assert.IsFalse(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that creating a context with -v sets the Version property to true.
    /// </summary>
    [TestMethod]
    public void Context_Create_ShortVersionFlag_SetsVersionTrue()
    {
        // Act
        using var context = Context.Create(["-v"]);

        // Assert
        Assert.IsTrue(context.Version);
        Assert.IsFalse(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the help flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpFlag_SetsHelpTrue()
    {
        // Act
        using var context = Context.Create(["--help"]);

        // Assert
        Assert.IsFalse(context.Version);
        Assert.IsTrue(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that creating a context with -? sets the Help property to true.
    /// </summary>
    [TestMethod]
    public void Context_Create_QuestionMarkHelpFlag_SetsHelpTrue()
    {
        // Act
        using var context = Context.Create(["-?"]);

        // Assert
        Assert.IsTrue(context.Help);
    }

    /// <summary>
    ///     Test that creating a context with -h sets the Help property to true.
    /// </summary>
    [TestMethod]
    public void Context_Create_ShortHelpFlag_SetsHelpTrue()
    {
        // Act
        using var context = Context.Create(["-h"]);

        // Assert
        Assert.IsTrue(context.Help);
    }

    /// <summary>
    ///     Test creating a context with an unknown argument throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_UnknownArgument_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--unknown"]));
        Assert.Contains("Unsupported argument", exception.Message);
    }

    /// <summary>
    ///     Test WriteLine writes to console output.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_WritesToConsole()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            using var context = Context.Create([]);

            // Act
            context.WriteLine("Test message");

            // Assert
            Assert.Contains("Test message", outWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteError writes to console error stream and sets exit code.
    /// </summary>
    [TestMethod]
    public void Context_WriteError_WritesToErrorAndSetsExitCode()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            using var context = Context.Create([]);

            // Act
            context.WriteError("Error message");

            // Assert
            Assert.AreEqual(1, context.ExitCode);
            Assert.Contains("Error message", errWriter.ToString());
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test creating a context with --heading argument.
    /// </summary>
    [TestMethod]
    public void Context_Create_HeadingArgument_SetsHeading()
    {
        // Act
        using var context = Context.Create(["--heading", "My Custom Heading"]);

        // Assert
        Assert.AreEqual("My Custom Heading", context.Heading);
    }

    /// <summary>
    ///     Test creating a context with --heading but no value throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_HeadingWithoutValue_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--heading"]));
        Assert.Contains("--heading requires", exception.Message);
    }

    /// <summary>
    ///     Test that WriteLine in silent mode does not write to console.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_SilentMode_DoesNotWriteToConsole()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            using var context = Context.Create(["--silent"]);

            // Act
            context.WriteLine("Test message");

            // Assert
            Assert.IsEmpty(outWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that WriteError in silent mode does not write to console but sets exit code.
    /// </summary>
    [TestMethod]
    public void Context_WriteError_SilentMode_DoesNotWriteToConsoleButSetsExitCode()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            using var context = Context.Create(["--silent"]);

            // Act
            context.WriteError("Error message");

            // Assert
            Assert.AreEqual(1, context.ExitCode);
            Assert.IsEmpty(errWriter.ToString());
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that creating a context with a log file opens the file successfully.
    /// </summary>
    [TestMethod]
    public void Context_Create_LogFile_OpensFileSuccessfully()
    {
        // Arrange
        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");
        bool logFileExists = false;
        string? logContent = null;

        try
        {
            // Act - dispose the context so the log file is flushed and closed before reading
            using (var context = Context.Create(["--log", logFile]))
            {
                context.WriteLine("Test message");
            }

            // Assert - context is disposed; file handle is closed and safe to read
            logFileExists = File.Exists(logFile);
            if (logFileExists)
            {
                logContent = File.ReadAllText(logFile);
            }
        }
        finally
        {
            // Cleanup - delete the log file regardless of assertion outcomes
            try
            {
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                }
            }
            catch
            {
                // Ignore cleanup failures
            }
        }

        // Assert - Verify the log file was created and contains the message
        Assert.IsTrue(logFileExists, "Log file should have been created");
        Assert.IsNotNull(logContent, "Log file content should have been read");
        Assert.Contains("Test message", logContent);
    }

    /// <summary>
    ///     Test that creating a context with an invalid log file path throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_InvalidLogFilePath_ThrowsInvalidOperationException()
    {
        // Arrange - Use an invalid path (directory that doesn't exist)
        var invalidPath = PathHelpers.SafePathCombine("/nonexistent/directory", "test.log");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => Context.Create(["--log", invalidPath]));
        Assert.Contains("Failed to open log file", exception.Message);
    }

    /// <summary>
    ///     Test that Dispose properly cleans up resources.
    /// </summary>
    [TestMethod]
    public void Context_Dispose_ProperlyClosesLogFile()
    {
        // Arrange
        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");

        try
        {
            var context = Context.Create(["--log", logFile]);
            context.WriteLine("Before dispose");

            // Act
            context.Dispose();

            // Assert - Should be able to read the log file after disposal
            Assert.IsTrue(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Before dispose", logContent);
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
    ///     Test that ExitCode starts at 0 and changes to 1 after an error.
    /// </summary>
    [TestMethod]
    public void Context_ExitCode_StartsAtZero_ChangesToOneAfterError()
    {
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            using var context = Context.Create([]);

            // Act - Check initial exit code
            Assert.AreEqual(0, context.ExitCode);

            // Act - Write an error
            context.WriteError("Error message");

            // Assert - Exit code should be 1
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that creating a context with multiple flags sets all properties correctly.
    /// </summary>
    [TestMethod]
    public void Context_Create_MultipleFlags_SetsAllPropertiesCorrectly()
    {
        // Arrange & Act
        using var context = Context.Create(["--silent", "--enforce", "--sarif", "test.sarif"]);

        // Assert
        Assert.IsTrue(context.Silent);
        Assert.IsTrue(context.Enforce);
        Assert.AreEqual("test.sarif", context.SarifFile);
        Assert.IsFalse(context.Version);
        Assert.IsFalse(context.Help);
    }

    /// <summary>
    ///     Test that creating a context with --sarif parameter sets SarifFile property.
    /// </summary>
    [TestMethod]
    public void Context_Create_SarifParameter_SetsSarifFile()
    {
        // Act
        using var context = Context.Create(["--sarif", "input.sarif"]);

        // Assert
        Assert.AreEqual("input.sarif", context.SarifFile);
    }

    /// <summary>
    ///     Test that creating a context with --report parameter sets ReportFile property.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportParameter_SetsReportFile()
    {
        // Act
        using var context = Context.Create(["--report", "output.md"]);

        // Assert
        Assert.AreEqual("output.md", context.ReportFile);
    }

    /// <summary>
    ///     Test that creating a context with --report-depth parameter sets ReportDepth property.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthParameter_SetsReportDepth()
    {
        // Act
        using var context = Context.Create(["--report-depth", "3"]);

        // Assert
        Assert.AreEqual(3, context.ReportDepth);
    }

    /// <summary>
    ///     Test that creating a context with --report-depth but no value throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthWithoutValue_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth"]));
        Assert.Contains("--report-depth requires", exception.Message);
    }

    /// <summary>
    ///     Test that creating a context with --report-depth and invalid value throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthInvalidValue_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "invalid"]));
        Assert.Contains("--report-depth requires a positive integer", exception.Message);
    }

    /// <summary>
    ///     Test that creating a context with --report-depth and zero value throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthZero_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "0"]));
        Assert.Contains("--report-depth requires a positive integer", exception.Message);
    }

    /// <summary>
    ///     Test that creating a context with --validate flag sets Validate property.
    /// </summary>
    [TestMethod]
    public void Context_Create_ValidateFlag_SetsValidateTrue()
    {
        // Act
        using var context = Context.Create(["--validate"]);

        // Assert
        Assert.IsTrue(context.Validate);
    }

    /// <summary>
    ///     Test that creating a context with --enforce flag sets Enforce property.
    /// </summary>
    [TestMethod]
    public void Context_Create_EnforceFlag_SetsEnforceTrue()
    {
        // Act
        using var context = Context.Create(["--enforce"]);

        // Assert
        Assert.IsTrue(context.Enforce);
    }

    /// <summary>
    ///     Test that creating a context with --silent flag sets Silent property.
    /// </summary>
    [TestMethod]
    public void Context_Create_SilentFlag_SetsSilentTrue()
    {
        // Act
        using var context = Context.Create(["--silent"]);

        // Assert
        Assert.IsTrue(context.Silent);
    }

    /// <summary>
    ///     Test that creating a context with --results parameter sets ResultsFile property.
    /// </summary>
    [TestMethod]
    public void Context_Create_ResultsParameter_SetsResultsFile()
    {
        // Act
        using var context = Context.Create(["--results", "results.trx"]);

        // Assert
        Assert.AreEqual("results.trx", context.ResultsFile);
    }

    /// <summary>
    ///     Test that WriteLine writes to the log file when it is open.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_WithLogFile_WritesToLog()
    {
        // Arrange
        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                context.WriteLine("LogLine message");
            }

            // Assert - message must appear in the log file even though --silent suppresses console
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("LogLine message", logContent);
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
    ///     Test that WriteLine writes to the log file in silent mode.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_SilentModeWithLogFile_WritesToLog()
    {
        // Arrange
        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                context.WriteLine("Silent log message");
            }

            // Assert - message must appear in the log even in silent mode
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Silent log message", logContent);
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
    ///     Test that WriteError writes to the log file when it is open.
    /// </summary>
    [TestMethod]
    public void Context_WriteError_WithLogFile_WritesToLog()
    {
        // Arrange
        var logFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                context.WriteError("Error log message");
            }

            // Assert - error message must appear in the log file
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Error log message", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
