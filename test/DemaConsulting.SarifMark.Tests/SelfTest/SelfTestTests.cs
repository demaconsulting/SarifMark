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
///     Subsystem tests for the Self-Validation subsystem.
/// </summary>
public class SelfTestTests
{
    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [Fact]
    public void SelfTest_ValidateFlag_RunsSelfValidation()
    {
        // Arrange - silent context that captures all output to a temp log file
        var logFile = Path.Combine(Path.GetTempPath(), $"self-test-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--validate", "--silent", "--log", logFile]))
            {
                // Act
                Validation.Run(context);

                // Assert
                Assert.Equal(0, context.ExitCode);
            }

            var output = File.ReadAllText(logFile);
            Assert.Contains("Total Tests:", output);
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
    ///     Test that validate flag with TRX results parameter writes a TRX file.
    /// </summary>
    [Fact]
    public void SelfTest_ResultsFile_TrxPath_WritesTrxFile()
    {
        // Arrange
        var resultsFile = Path.Combine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.trx");

        try
        {
            // Act
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);
            Validation.Run(context);

            // Assert
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(resultsFile), "TRX results file was not created");

            var content = File.ReadAllText(resultsFile);
            Assert.Contains("<TestRun", content);
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that validate flag with JUnit XML results parameter writes a JUnit XML file.
    /// </summary>
    [Fact]
    public void SelfTest_ResultsFile_XmlPath_WritesJUnitFile()
    {
        // Arrange
        var resultsFile = Path.Combine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.xml");

        try
        {
            // Act
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);
            Validation.Run(context);

            // Assert
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(resultsFile), "JUnit XML results file was not created");

            var content = File.ReadAllText(resultsFile);
            Assert.Contains("<testsuite", content);
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that --depth affects the self-validation markdown report depth.
    /// </summary>
    [Fact]
    public void SelfTest_DepthParameter_AffectsSelfValidationReport()
    {
        // Arrange - silent context that captures all output to a temp log file
        var logFile = Path.Combine(Path.GetTempPath(), $"self-test-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--validate", "--silent", "--depth", "2", "--log", logFile]))
            {
                // Act - run validation with non-default depth of 2
                Validation.Run(context);

                // Assert - validation passes
                Assert.Equal(0, context.ExitCode);
            }

            // Assert - the depth-sensitive report generation test passes
            var output = File.ReadAllText(logFile);
            Assert.Contains("SarifMark_MarkdownReportGeneration - Passed", output);
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
    ///     Test that enforcement mode behavior is verified by the self-validation suite.
    /// </summary>
    [Fact]
    public void SelfTest_EnforcementTest_RunsWithinValidation()
    {
        // Arrange - silent context that captures all output to a temp log file
        var logFile = Path.Combine(Path.GetTempPath(), $"self-test-{Guid.NewGuid()}.log");
        try
        {
            using (var context = Context.Create(["--validate", "--silent", "--log", logFile]))
            {
                // Act - Run validation which internally exercises enforcement mode
                Validation.Run(context);

                // Assert - enforcement test within validation runs and passes
                Assert.Equal(0, context.ExitCode);
            }

            var output = File.ReadAllText(logFile);
            Assert.Contains("SarifMark_Enforcement - Passed", output);
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
    ///     Test that enforcement mode returns a non-zero exit code when issues are found.
    /// </summary>
    [Fact]
    public void SelfTest_EnforceFlag_WithIssues_ReturnsNonZeroExitCode()
    {
        // Arrange
        var sarifFile = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");
        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var exitCode = Program.Main(["--sarif", sarifFile, "--enforce", "--silent"]);

            // Assert
            Assert.Equal(1, exitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
