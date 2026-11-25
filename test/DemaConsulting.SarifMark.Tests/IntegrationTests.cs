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

using DemaConsulting.HelpCards.Catalog;
using DemaConsulting.HelpCards.Testing;

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
    ///     Test that validate mode writes a results file when the results parameter is provided.
    /// </summary>
    [Fact]
    public void SarifMark_ValidateResultsParameter_WritesResultsFile()
    {
        // Arrange
        var resultsFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-validate-results-{Guid.NewGuid()}.trx");

        try
        {
            // Act
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--validate",
                "--results", resultsFile);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(resultsFile), "Results file was not created");
            Assert.Contains("<TestRun", File.ReadAllText(resultsFile));
        }
        finally
        {
            // Clean up the temporary results file
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
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

    /// <summary>
    ///     Test that a SARIF file with no issues generates a valid report.
    /// </summary>
    [Fact]
    public void SarifMark_ValidSarif_NoIssues_GeneratesReport()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "no-issues.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-no-issues-report-{Guid.NewGuid()}.md");

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
            Assert.Contains("**Tool:**", reportContent);
            Assert.Contains("Found no issues", reportContent);
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
    ///     Test that the --exclude flag filters matching findings from the generated report while
    ///     retaining non-matching findings, end-to-end through the compiled binary.
    /// </summary>
    [Fact]
    public void SarifMark_ExcludeFlag_FiltersMatchingFindings()
    {
        // Arrange
        var sarifFile = PathHelpers.SafePathCombine(_testDataPath, "multi-result.sarif");
        Assert.True(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        var reportFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-exclude-report-{Guid.NewGuid()}.md");

        try
        {
            // Act - multi-result.sarif's two findings are located at file:///path/to/first.cs
            // and file:///path/to/second.cs; exclude only the first.
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                _dllPath,
                "--sarif", sarifFile,
                "--exclude", "**/first.cs",
                "--report", reportFile);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.DoesNotContain("first.cs", reportContent);
            Assert.Contains("second.cs", reportContent);
            Assert.Contains("Found 1 issue", reportContent);
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
    ///     Test that the help verb lists the help topics.
    /// </summary>
    [Fact]
    public void SarifMark_HelpTopics_HelpVerb_ListsTopics()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: run the tool with the help verb
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "help");

        // Assert: the topics are listed with their commands, without the banner
        Assert.Equal(0, exitCode);
        Assert.Contains("sarifmark help getting-started", output);
        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that a help topic prints the card with its examples, and that one example prints alone.
    /// </summary>
    [Fact]
    public void SarifMark_HelpTopics_CardPath_PrintsCardWithExamples()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: print a card, and one of its examples
        var cardExitCode = Runner.Run(
            out var card,
            "dotnet",
            _dllPath,
            "help",
            "reports");
        var exampleExitCode = Runner.Run(
            out var example,
            "dotnet",
            _dllPath,
            "help",
            "getting-started",
            "--example",
            "version");

        // Assert: the card prints with its examples, and the example prints only its command
        Assert.Equal(0, cardExitCode);
        Assert.Contains("# Markdown reports", card);
        Assert.Contains("## Examples", card);
        Assert.Equal(0, exampleExitCode);
        Assert.Equal("sarifmark --version", example.Trim());
    }

    /// <summary>
    ///     Test that help search finds topics and that help writes JSON on request.
    /// </summary>
    [Fact]
    public void SarifMark_HelpTopics_SearchAndJson_FindsCardsAndWritesJson()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: search the help, and print the topic index as JSON
        var searchExitCode = Runner.Run(
            out var search,
            "dotnet",
            _dllPath,
            "help",
            "--search",
            "exclude");
        var jsonExitCode = Runner.Run(
            out var json,
            "dotnet",
            _dllPath,
            "help",
            "--json");

        // Assert: the search finds the quality-gates topic and the JSON is a versioned document
        Assert.Equal(0, searchExitCode);
        Assert.Contains("sarifmark help quality-gates", search);
        Assert.Equal(0, jsonExitCode);
        Assert.StartsWith("{", json);
        Assert.Contains("\"helpSchemaVersion\":1", json);
    }

    /// <summary>
    ///     Test that an unknown help topic returns the usage-error exit code.
    /// </summary>
    [Fact]
    public void SarifMark_HelpTopics_UnknownTopic_ReturnsUsageError()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: ask for a topic that does not exist
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "help",
            "no-such-topic");

        // Assert: exit code 2 and a message that names the topic
        Assert.Equal(2, exitCode);
        Assert.Contains("no-such-topic", output);
    }

    /// <summary>
    ///     Test that every command example in the help cards gives the result that its card states.
    /// </summary>
    [Fact]
    public void SarifMark_HelpExamples_AllCommandExamples_GiveStatedResults()
    {
        // Arrange: the catalog the tool ships, and the SARIF files that the examples read
        var catalog = HelpCatalog.Create(HelpCommand.CreateOptions().GetCatalogConfiguration());
        var exampleDataPath = PathHelpers.SafePathCombine(_testDataPath, "HelpExamples");

        // Act: run every command example against the built tool, in the scratch folder where the runner
        // saved the card's source examples, after copying the example SARIF files there; the runner
        // compares the exit codes and the 'expect' fragments
        var report = HelpExampleRunner.Run(catalog, invocation =>
        {
            StageHelpExampleData(exampleDataPath, invocation.WorkingDirectory);
            var exitCode = Runner.RunInDirectory(
                out var output,
                invocation.WorkingDirectory,
                "dotnet",
                [_dllPath, .. invocation.Arguments]);
            return new HelpExampleOutcome(exitCode, output);
        });

        // Assert: the cards hold command examples, and every one gave the result its card states
        Assert.True(report.CommandsRun > 0, "The help cards contain no command examples.");
        report.AssertSucceeded();
    }

    /// <summary>
    ///     Copies the input files that the help card examples name into the scratch folder of a card.
    /// </summary>
    /// <remarks>
    ///     The examples read SARIF files that an analyzer would produce, so the files are test data of
    ///     this project and not part of the cards. They are copied before every command so that a
    ///     command that overwrites a file cannot affect the next one.
    /// </remarks>
    /// <param name="sourceDirectory">The folder that holds the example input files.</param>
    /// <param name="workingDirectory">The scratch folder in which the example command runs.</param>
    private static void StageHelpExampleData(string sourceDirectory, string workingDirectory)
    {
        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            File.Copy(file, PathHelpers.SafePathCombine(workingDirectory, Path.GetFileName(file)), overwrite: true);
        }
    }
}

