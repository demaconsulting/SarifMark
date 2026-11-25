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

using DemaConsulting.HelpCards.Testing;

namespace DemaConsulting.SarifMark.Tests;

/// <summary>
///     Unit tests for the HelpCommand class: the answers to the help verb, the deferred creation of
///     the catalog, and the validity of the embedded help cards.
/// </summary>
public class HelpCommandTests
{
    /// <summary>
    ///     Test that the help verb alone prints the topic index and exits with code 0.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero()
    {
        // Arrange: nothing to arrange - the bare help verb is the input

        // Act: run the help verb
        Run(["help"], out var answered, out var exitCode, out var output, out _, out _);

        // Assert: the command answered, succeeded, and listed every root topic with its command
        Assert.True(answered);
        Assert.Equal(0, exitCode);
        foreach (var topic in HelpCommand.RootCardIds)
        {
            Assert.Contains($"sarifmark help {topic}", output);
        }
    }

    /// <summary>
    ///     Test that a card path prints the card with its examples.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_CardPath_PrintsCardWithExamples()
    {
        // Arrange: nothing to arrange - the card path is the input

        // Act: print a card by its path
        Run(["help", "self-validation"], out var answered, out var exitCode, out var output, out _, out _);

        // Assert: the card text and its examples are printed
        Assert.True(answered);
        Assert.Equal(0, exitCode);
        Assert.Contains("# Self-validation", output);
        Assert.Contains("## Examples", output);
        Assert.Contains("Also write the results as a TRX file.", output);
    }

    /// <summary>
    ///     Test that an unknown topic exits with code 2 and prints a hint to the error writer.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_UnknownTopic_ReturnsTwo()
    {
        // Arrange: nothing to arrange - an unknown topic is the input

        // Act: ask for a card that does not exist
        Run(["help", "no-such-topic"], out var answered, out var exitCode, out var output, out var error, out _);

        // Assert: the command answered with the usage-error code and a hint on the error writer
        Assert.True(answered);
        Assert.Equal(2, exitCode);
        Assert.Equal(string.Empty, output);
        Assert.Contains("no-such-topic", error);
        Assert.Contains("sarifmark help", error);
    }

    /// <summary>
    ///     Test that --search lists the cards that match the search words.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_SearchOption_ListsMatchingCards()
    {
        // Arrange: nothing to arrange - the search words are the input

        // Act: search for a word in the title of a topic
        Run(["help", "--search", "validation"], out var answered, out var exitCode, out var output, out _, out _);

        // Assert: the self-validation card is found
        Assert.True(answered);
        Assert.Equal(0, exitCode);
        Assert.Contains("sarifmark help self-validation", output);
    }

    /// <summary>
    ///     Test that --json writes a versioned JSON document.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_JsonOption_WritesJsonDocument()
    {
        // Arrange: nothing to arrange - the card path and --json are the input

        // Act: print a card and the index as JSON
        Run(["help", "command-line", "--json"], out var answered, out var exitCode, out var card, out _, out _);
        Run(["help", "--json"], out _, out _, out var index, out _, out _);

        // Assert: the card is a JSON document of schema version 1 that names the card
        Assert.True(answered);
        Assert.Equal(0, exitCode);
        Assert.StartsWith("{", card);
        Assert.Contains("\"helpSchemaVersion\":1", card);
        Assert.Contains("\"id\":\"command-line\"", card);

        // Assert: the index carries the tool name and the version set once on the helper options
        Assert.Contains($"\"tool\":\"{HelpCommand.ToolName}\"", index);
        Assert.Contains($"\"version\":\"{Program.Version}\"", index);
    }

    /// <summary>
    ///     Test that arguments other than the help verb are left alone and never build the catalog.
    /// </summary>
    [Fact]
    public void HelpCommand_TryRun_NonHelpArguments_ReturnsFalseWithoutBuildingCatalog()
    {
        // Arrange: nothing to arrange - the arguments are the input

        // Act: run the arguments of ordinary runs, and one help command for comparison
        Run(["--sarif", "results.sarif"], out var sarifAnswered, out _, out var sarifOutput, out var sarifError, out var sarifCalls);
        Run(["--help"], out var helpFlagAnswered, out _, out _, out _, out var helpFlagCalls);
        Run([], out var emptyAnswered, out _, out _, out _, out var emptyCalls);
        Run(["help"], out var helpAnswered, out _, out _, out _, out var helpCalls);

        // Assert: ordinary runs are not answered, write nothing, and never build the catalog
        Assert.False(sarifAnswered);
        Assert.Equal(string.Empty, sarifOutput);
        Assert.Equal(string.Empty, sarifError);
        Assert.Equal(0, sarifCalls);
        Assert.False(helpFlagAnswered);
        Assert.Equal(0, helpFlagCalls);
        Assert.False(emptyAnswered);
        Assert.Equal(0, emptyCalls);

        // Assert: the help verb builds the catalog exactly once
        Assert.True(helpAnswered);
        Assert.Equal(1, helpCalls);
    }

    /// <summary>
    ///     Test that the shipped catalog passes validation with warnings treated as errors; the
    ///     validation covers the expected topics, the purpose tags, and the command examples.
    /// </summary>
    [Fact]
    public void HelpCommand_CreateOptions_ShippedCatalog_PassesValidation()
    {
        // Arrange: validate exactly the composition the tool ships, with warnings as errors
        var configuration = HelpCommand.CreateOptions().GetCatalogConfiguration();
        var options = new HelpValidationOptions { WarningsAreErrors = true };

        // Act: validate the catalog; the validator throws an exception that lists every defect
        var exception = Record.Exception(() => HelpCardValidator.AssertValid(configuration, options));

        // Assert: no defect was found
        Assert.Null(exception);
    }

    /// <summary>
    ///     Runs a help command with captured writers and a counting catalog configuration.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <param name="answered">Receives whether the help command answered the arguments.</param>
    /// <param name="exitCode">Receives the exit code of the help command.</param>
    /// <param name="output">Receives the text written to the output writer.</param>
    /// <param name="error">Receives the text written to the error writer.</param>
    /// <param name="catalogBuilds">Receives the number of times the catalog was built.</param>
    private static void Run(
        string[] args,
        out bool answered,
        out int exitCode,
        out string output,
        out string error,
        out int catalogBuilds)
    {
        using var outputWriter = new StringWriter();
        using var errorWriter = new StringWriter();
        var builds = 0;
        var command = HelpCommand.Create(outputWriter, errorWriter, (builder, _) =>
        {
            builds++;
            HelpCommand.Configure(builder);
        });

        answered = command.TryRun(args, out exitCode);
        output = outputWriter.ToString();
        error = errorWriter.ToString();
        catalogBuilds = builds;
    }
}
