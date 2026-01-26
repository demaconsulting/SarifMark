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

using System.Runtime.InteropServices;
using DemaConsulting.TestResults.IO;

namespace DemaConsulting.SarifMark;

/// <summary>
///     Provides self-validation functionality for the SarifMark tool.
/// </summary>
internal static class Validation
{
    /// <summary>
    ///     Runs self-validation tests and optionally writes results to a file.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Print validation header
        PrintValidationHeader(context);

        // Create test results collection
        var testResults = new DemaConsulting.TestResults.TestResults
        {
            Name = "SarifMark Self-Validation"
        };

        // Run core functionality tests
        RunSarifReadingTest(context, testResults);
        RunMarkdownReportGenerationTest(context, testResults);
        RunEnforcementTest(context, testResults);

        // Calculate totals
        var totalTests = testResults.Results.Count;
        var passedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Passed);
        var failedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Failed);

        // Print summary
        context.WriteLine("");
        context.WriteLine($"Total Tests: {totalTests}");
        context.WriteLine($"Passed: {passedTests}");
        if (failedTests > 0)
        {
            context.WriteError($"Failed: {failedTests}");
        }
        else
        {
            context.WriteLine($"Failed: {failedTests}");
        }

        // Write results file if requested
        if (context.ResultsFile != null)
        {
            WriteResultsFile(context, testResults);
        }
    }

    /// <summary>
    ///     Prints the validation header with system information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintValidationHeader(Context context)
    {
        context.WriteLine("# DEMA Consulting SarifMark");
        context.WriteLine("");
        context.WriteLine("| Information         | Value                                              |");
        context.WriteLine("| :------------------ | :------------------------------------------------- |");
        context.WriteLine($"| SarifMark Version   | {Program.Version,-50} |");
        context.WriteLine($"| Machine Name        | {Environment.MachineName,-50} |");
        context.WriteLine($"| OS Version          | {RuntimeInformation.OSDescription,-50} |");
        context.WriteLine($"| DotNet Runtime      | {RuntimeInformation.FrameworkDescription,-50} |");
        context.WriteLine($"| Time Stamp          | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC{"",-29} |");
        context.WriteLine("");
    }

    /// <summary>
    ///     Runs a test for SARIF file reading functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunSarifReadingTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        RunValidationTest(
            context,
            testResults,
            "SarifMark_SarifReading",
            "SARIF File Reading Test",
            null,
            (logContent, _) =>
            {
                if (logContent.Contains("Tool: MockTool 1.0.0") &&
                    logContent.Contains("Results: 2"))
                {
                    return null;
                }

                return "Expected tool information not found in log";
            });
    }

    /// <summary>
    ///     Runs a test for markdown report generation functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunMarkdownReportGenerationTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        RunValidationTest(
            context,
            testResults,
            "SarifMark_MarkdownReportGeneration",
            "Markdown Report Generation Test",
            "sarif-report.md",
            (logContent, reportContent) =>
            {
                if (reportContent == null)
                {
                    return "Report file not created";
                }

                if (reportContent.Contains("MockTool Analysis") &&
                    reportContent.Contains("Found 2 results"))
                {
                    return null;
                }

                return "Report file missing expected content";
            });
    }

    /// <summary>
    ///     Runs a test for enforcement functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunEnforcementTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("SarifMark_Enforcement");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "enforcement.log");
            var sarifFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test.sarif");

            // Create mock SARIF file
            CreateMockSarifFile(sarifFile);

            // Build command line arguments
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--sarif", sarifFile,
                "--enforce"
            };

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args]))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution failed as expected (should return non-zero when issues found)
            if (exitCode != 0)
            {
                // Read log content
                var logContent = File.ReadAllText(logFile);

                if (logContent.Contains("Error: Issues found in SARIF file"))
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ Enforcement Test - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = "Expected error message not found";
                    context.WriteError($"✗ Enforcement Test - FAILED: Expected error message not found");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = "Program should have exited with non-zero code";
                context.WriteError($"✗ Enforcement Test - FAILED: Program should have exited with non-zero code");
            }
        }
        // Catch all exceptions as this is a test framework - any exception should be recorded as a test failure.
        // This is intentional to ensure robust test execution and reporting regardless of exception type.
        catch (Exception ex)
        {
            HandleTestException(test, context, "Enforcement Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a validation test with common test execution logic.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="testName">The name of the test.</param>
    /// <param name="displayName">The display name for console output.</param>
    /// <param name="reportFileName">Optional report file name to generate.</param>
    /// <param name="validator">Function to validate test results. Returns null on success or error message on failure.</param>
    private static void RunValidationTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        string testName,
        string displayName,
        string? reportFileName,
        Func<string, string?, string?> validator)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult(testName);

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, $"{testName}.log");
            var sarifFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test.sarif");
            var reportFile = reportFileName != null ? PathHelpers.SafePathCombine(tempDir.DirectoryPath, reportFileName) : null;

            // Create mock SARIF file
            CreateMockSarifFile(sarifFile);

            // Build command line arguments
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--sarif", sarifFile
            };

            if (reportFile != null)
            {
                args.Add("--report");
                args.Add(reportFile);
            }

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args]))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution succeeded
            if (exitCode == 0)
            {
                // Read log and report contents
                var logContent = File.ReadAllText(logFile);
                var reportContent = reportFile != null && File.Exists(reportFile)
                    ? File.ReadAllText(reportFile)
                    : null;

                // Validate the results
                var errorMessage = validator(logContent, reportContent);

                if (errorMessage == null)
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ {displayName} - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = errorMessage;
                    context.WriteError($"✗ {displayName} - FAILED: {errorMessage}");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = $"Program exited with code {exitCode}";
                context.WriteError($"✗ {displayName} - FAILED: Exit code {exitCode}");
            }
        }
        // Catch all exceptions as this is a test framework - any exception should be recorded as a test failure.
        // This is intentional to ensure robust test execution and reporting regardless of exception type.
        catch (Exception ex)
        {
            HandleTestException(test, context, displayName, ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Creates a mock SARIF file for validation testing.
    /// </summary>
    /// <param name="filePath">Path where the SARIF file should be created.</param>
    private static void CreateMockSarifFile(string filePath)
    {
        var sarifContent = """
            {
              "version": "2.1.0",
              "$schema": "https://raw.githubusercontent.com/oasis-tcs/sarif-spec/master/Schemata/sarif-schema-2.1.0.json",
              "runs": [
                {
                  "tool": {
                    "driver": {
                      "name": "MockTool",
                      "version": "1.0.0"
                    }
                  },
                  "results": [
                    {
                      "ruleId": "TEST001",
                      "level": "warning",
                      "message": {
                        "text": "Test issue 1"
                      },
                      "locations": [
                        {
                          "physicalLocation": {
                            "artifactLocation": {
                              "uri": "src/Program.cs"
                            },
                            "region": {
                              "startLine": 42
                            }
                          }
                        }
                      ]
                    },
                    {
                      "ruleId": "TEST002",
                      "level": "error",
                      "message": {
                        "text": "Test issue 2"
                      },
                      "locations": [
                        {
                          "physicalLocation": {
                            "artifactLocation": {
                              "uri": "src/Helper.cs"
                            },
                            "region": {
                              "startLine": 15
                            }
                          }
                        }
                      ]
                    }
                  ]
                }
              ]
            }
            """;

        File.WriteAllText(filePath, sarifContent);
    }

    /// <summary>
    ///     Writes test results to a file in TRX or JUnit format.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results to write.</param>
    private static void WriteResultsFile(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        if (context.ResultsFile == null)
        {
            return;
        }

        try
        {
            var extension = Path.GetExtension(context.ResultsFile).ToLowerInvariant();
            string content;

            if (extension == ".trx")
            {
                content = TrxSerializer.Serialize(testResults);
            }
            else if (extension == ".xml")
            {
                // Assume JUnit format for .xml extension
                content = JUnitSerializer.Serialize(testResults);
            }
            else
            {
                context.WriteError($"Error: Unsupported results file format '{extension}'. Use .trx or .xml extension.");
                return;
            }

            File.WriteAllText(context.ResultsFile, content);
            context.WriteLine($"Results written to {context.ResultsFile}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            context.WriteError($"Error: Failed to write results file: {ex.Message}");
        }
    }

    /// <summary>
    ///     Creates a new test result object with common properties.
    /// </summary>
    /// <param name="testName">The name of the test.</param>
    /// <returns>A new test result object.</returns>
    private static DemaConsulting.TestResults.TestResult CreateTestResult(string testName)
    {
        return new DemaConsulting.TestResults.TestResult
        {
            Name = testName,
            ClassName = "Validation",
            CodeBase = "SarifMark"
        };
    }

    /// <summary>
    ///     Finalizes a test result by setting its duration and adding it to the collection.
    /// </summary>
    /// <param name="test">The test result to finalize.</param>
    /// <param name="startTime">The start time of the test.</param>
    /// <param name="testResults">The test results collection to add to.</param>
    private static void FinalizeTestResult(
        DemaConsulting.TestResults.TestResult test,
        DateTime startTime,
        DemaConsulting.TestResults.TestResults testResults)
    {
        test.Duration = DateTime.UtcNow - startTime;
        testResults.Results.Add(test);
    }

    /// <summary>
    ///     Handles test exceptions by setting failure information and logging the error.
    /// </summary>
    /// <param name="test">The test result to update.</param>
    /// <param name="context">The context for output.</param>
    /// <param name="testName">The name of the test for error messages.</param>
    /// <param name="ex">The exception that occurred.</param>
    private static void HandleTestException(
        DemaConsulting.TestResults.TestResult test,
        Context context,
        string testName,
        Exception ex)
    {
        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
        test.ErrorMessage = $"Exception: {ex.Message}";
        context.WriteError($"✗ {testName} - FAILED: {ex.Message}");
    }

    /// <summary>
    ///     Represents a temporary directory that is automatically deleted when disposed.
    /// </summary>
    private sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>
        ///     Gets the path to the temporary directory.
        /// </summary>
        public string DirectoryPath { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TemporaryDirectory"/> class.
        /// </summary>
        public TemporaryDirectory()
        {
            DirectoryPath = PathHelpers.SafePathCombine(Path.GetTempPath(), $"sarifmark_validation_{Guid.NewGuid()}");

            try
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                throw new InvalidOperationException($"Failed to create temporary directory: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Deletes the temporary directory and all its contents.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Ignore cleanup errors during disposal
            }
        }
    }
}
