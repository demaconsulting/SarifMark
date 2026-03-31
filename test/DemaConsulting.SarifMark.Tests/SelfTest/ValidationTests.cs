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
///     Unit tests for the Validation class.
/// </summary>
[TestClass]
public class ValidationTests
{
    /// <summary>
    ///     Tests that passing a null context throws an ArgumentNullException.
    /// </summary>
    [TestMethod]
    public void Validation_Run_NullContext_ThrowsArgumentNullException()
    {
        // Arrange - null context is the condition under test
        Context? context = null;

        // Act & Assert - null context must immediately throw ArgumentNullException
        Assert.ThrowsExactly<ArgumentNullException>(() => Validation.Run(context!));
    }

    /// <summary>
    ///     Tests that running validation with a valid context writes a validation header to the log.
    /// </summary>
    [TestMethod]
    public void Validation_Run_ValidContext_PrintsValidationHeader()
    {
        // Arrange - silent context that writes all output to a temp log file
        var logFile = CreateTempFile(".log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                Validation.Run(context);
            }

            // Assert - log must contain the header table with system-information fields
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("SarifMark Version", logContent,
                "Log should contain the 'SarifMark Version' header row");
            Assert.Contains("Machine Name", logContent,
                "Log should contain the 'Machine Name' header row");
            Assert.Contains("OS Version", logContent,
                "Log should contain the 'OS Version' header row");
            Assert.Contains("DotNet Runtime", logContent,
                "Log should contain the 'DotNet Runtime' header row");
            Assert.Contains("Time Stamp", logContent,
                "Log should contain the 'Time Stamp' header row");
        }
        finally
        {
            SafeDeleteFile(logFile);
        }
    }

    /// <summary>
    ///     Tests that running validation executes all three internal tests and reports them as passed.
    /// </summary>
    [TestMethod]
    public void Validation_Run_ValidContext_RunsAllTests()
    {
        // Arrange - silent context that writes all output to a temp log file
        var logFile = CreateTempFile(".log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                Validation.Run(context);
            }

            // Assert - log must contain each test name with a passing indicator
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("SarifMark_SarifReading", logContent,
                "Log should contain the SarifMark_SarifReading test name");
            Assert.Contains("SarifMark_MarkdownReportGeneration", logContent,
                "Log should contain the SarifMark_MarkdownReportGeneration test name");
            Assert.Contains("SarifMark_Enforcement", logContent,
                "Log should contain the SarifMark_Enforcement test name");
            Assert.Contains("Passed", logContent,
                "Log should report at least one test as Passed");
        }
        finally
        {
            SafeDeleteFile(logFile);
        }
    }

    /// <summary>
    ///     Tests that running validation prints a summary with the correct totals.
    /// </summary>
    [TestMethod]
    public void Validation_Run_ValidContext_PrintsSummary()
    {
        // Arrange - silent context that writes all output to a temp log file
        var logFile = CreateTempFile(".log");
        try
        {
            using (var context = Context.Create(["--silent", "--log", logFile]))
            {
                // Act
                Validation.Run(context);
            }

            // Assert - log must contain the three-line summary with correct counts
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Total Tests: 3", logContent,
                "Summary should report exactly 3 total tests");
            Assert.Contains("Passed: 3", logContent,
                "Summary should report exactly 3 passed tests");
            Assert.Contains("Failed: 0", logContent,
                "Summary should report 0 failed tests");
        }
        finally
        {
            SafeDeleteFile(logFile);
        }
    }

    /// <summary>
    ///     Tests that when a .trx results file path is supplied the file is created and contains TRX content.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithTrxResultsFile_WritesResultsFile()
    {
        // Arrange - supply both a log file and a .trx results file via command-line args
        var logFile = CreateTempFile(".log");
        var trxFile = CreateTempFile(".trx");
        try
        {
            using (var context = Context.Create(
                ["--silent", "--log", logFile, "--results", trxFile]))
            {
                // Act
                Validation.Run(context);
            }

            // Assert - the .trx file must exist and contain TRX XML markers
            Assert.IsTrue(File.Exists(trxFile), "TRX results file should have been created");
            var trxContent = File.ReadAllText(trxFile);
            Assert.Contains("TestRun", trxContent,
                "TRX file should contain a 'TestRun' XML element");
            Assert.Contains("SarifMark Self-Validation", trxContent,
                "TRX file should contain the test suite name");
        }
        finally
        {
            SafeDeleteFile(logFile);
            SafeDeleteFile(trxFile);
        }
    }

    /// <summary>
    ///     Tests that when a results file path with an unsupported extension is supplied, an error is reported.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithUnsupportedResultsFileExtension_WritesError()
    {
        // Arrange - supply a .json results path (not a supported format)
        var logFile = CreateTempFile(".log");
        var jsonFile = CreateTempFile(".json");
        try
        {
            int exitCode;
            using (var context = Context.Create(
                ["--silent", "--log", logFile, "--results", jsonFile]))
            {
                // Act
                Validation.Run(context);
                exitCode = context.ExitCode;
            }

            // Assert - error must be reported
            Assert.AreEqual(1, exitCode,
                "Exit code should be 1 when an unsupported results file extension is used");
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Unsupported results file format", logContent,
                "Log should contain the unsupported format error message");
        }
        finally
        {
            SafeDeleteFile(logFile);
            SafeDeleteFile(jsonFile);
        }
    }

    /// <summary>
    ///     Tests that when a .xml results file path is supplied the file is created and contains JUnit XML content.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithXmlResultsFile_WritesResultsFile()
    {
        // Arrange - supply both a log file and a .xml results file via command-line args
        var logFile = CreateTempFile(".log");
        var xmlFile = CreateTempFile(".xml");
        try
        {
            using (var context = Context.Create(
                ["--silent", "--log", logFile, "--results", xmlFile]))
            {
                // Act
                Validation.Run(context);
            }

            // Assert - the .xml file must exist and contain JUnit XML markers
            Assert.IsTrue(File.Exists(xmlFile), "JUnit XML results file should have been created");
            var xmlContent = File.ReadAllText(xmlFile);
            Assert.Contains("testsuites", xmlContent,
                "JUnit XML file should contain a 'testsuites' root element");
            Assert.Contains("SarifMark Self-Validation", xmlContent,
                "JUnit XML file should contain the test suite name");
        }
        finally
        {
            SafeDeleteFile(logFile);
            SafeDeleteFile(xmlFile);
        }
    }

    /// <summary>
    ///     Creates a unique temporary file path with the specified extension.
    ///     The file itself is not created; only the path is returned.
    /// </summary>
    /// <param name="extension">File extension including the leading dot (e.g. ".log").</param>
    /// <returns>A unique absolute path suitable for a temporary file.</returns>
    private static string CreateTempFile(string extension) =>
        PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-{Guid.NewGuid()}{extension}");

    /// <summary>
    ///     Deletes a file if it exists, ignoring any errors.
    /// </summary>
    /// <param name="path">Path to the file to delete.</param>
    private static void SafeDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Ignore cleanup errors during test teardown
        }
    }
}
