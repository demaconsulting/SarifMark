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

using System.Globalization;
using DemaConsulting.HelpCards.Catalog;
using DemaConsulting.HelpCards.Composition;
using DemaConsulting.HelpCards.Integration;
using DemaConsulting.HelpCards.Loading;

namespace DemaConsulting.SarifMark;

/// <summary>
///     Answers the <c>sarifmark help</c> command from the built-in help cards, before the
///     command-line options are parsed.
/// </summary>
/// <remarks>
///     <para>
///         <c>HelpCommand</c> is the only unit that references the HelpCards library. It wraps one
///         <see cref="HelpCli"/> and a <see cref="HelpSource"/> over the <c>Help</c> folder that the
///         HelpCards build targets embed in the tool assembly. Creating either object performs no
///         file or console I/O, so the static <see cref="Default"/> instance costs nothing at start-up.
///     </para>
///     <para>
///         The help catalog is built lazily: <see cref="TryRun"/> returns <see langword="false"/>
///         at once for every first argument other than <c>help</c> (compared without regard to case)
///         and builds the catalog only after the verb was recognized. A normal run of the tool
///         therefore never reads or parses a help card.
///     </para>
///     <para>
///         The command bypasses <see cref="Context"/>: <c>--silent</c> and <c>--log</c> do not apply to
///         it and no banner is printed. The verb is answered before the options are parsed, so no
///         context exists yet; parsing the options a second time just for help would duplicate
///         <see cref="Context.Create"/>. The command writes to the console (or to the writers supplied
///         to <see cref="Create"/>) and reports exit code 0 for success, 2 for a wrong help argument or
///         an unknown topic, and 70 for a defective card catalog.
///     </para>
///     <para>
///         An instance is immutable after creation and the helper keeps no state between calls, so an
///         instance can be shared.
///     </para>
/// </remarks>
internal sealed class HelpCommand
{
    /// <summary>
    ///     The name of the tool as the user types it; used in usage text, command examples, and tokens.
    /// </summary>
    internal const string ToolName = "sarifmark";

    /// <summary>
    ///     The closed tag vocabulary: every card carries one of these purpose tags, and no other tag.
    /// </summary>
    internal static readonly string[] PurposeTags = ["tutorial", "reference", "troubleshooting"];

    /// <summary>
    ///     Ids of the root cards, which are the topics listed by <c>sarifmark help</c>. A missing or
    ///     extra root card fails the catalog.
    /// </summary>
    internal static readonly string[] RootCardIds =
    [
        "getting-started",
        "command-line",
        "sarif-input",
        "reports",
        "quality-gates",
        "self-validation",
        "troubleshooting"
    ];

    /// <summary>
    ///     The cards embedded in the tool assembly. Creating the source reads nothing. Declared
    ///     before <see cref="Default"/> so that it is initialized first.
    /// </summary>
    private static readonly HelpSource Cards = HelpSource.FromAssembly(typeof(HelpCommand).Assembly);

    /// <summary>
    ///     The underlying command-line helper.
    /// </summary>
    private readonly HelpCli _cli;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HelpCommand"/> class.
    /// </summary>
    /// <param name="output">The writer for output, or <see langword="null"/> for the console output.</param>
    /// <param name="error">The writer for errors, or <see langword="null"/> for the console error.</param>
    /// <param name="configure">
    ///     The action that configures the catalog builder, or <see langword="null"/> to use
    ///     <see cref="Configure"/>. Supplied only by tests.
    /// </param>
    private HelpCommand(
        TextWriter? output,
        TextWriter? error,
        Action<HelpCatalogBuilder, CultureInfo?>? configure) =>
        _cli = HelpCli.Create(CreateOptions(output, error, configure));

    /// <summary>
    ///     Gets the instance used by the tool, which writes to the console at the time of the command.
    /// </summary>
    internal static HelpCommand Default { get; } = new(null, null, null);

    /// <summary>
    ///     Gets the sentence that points a reader of the usage text to <c>sarifmark help</c>.
    /// </summary>
    /// <remarks>
    ///     The sentence is a constant: reading it never builds the help catalog.
    /// </remarks>
    public string Footer => _cli.Footer;

    /// <summary>
    ///     Creates the options of the command-line helper. The tool name and the version are set here
    ///     and nowhere else: the helper gives them to the catalog builder itself.
    /// </summary>
    /// <param name="output">The writer for output, or <see langword="null"/> for the console output.</param>
    /// <param name="error">The writer for errors, or <see langword="null"/> for the console error.</param>
    /// <param name="configure">
    ///     Optional replacement for <see cref="Configure"/>, used by tests to observe when the catalog
    ///     is built.
    /// </param>
    /// <returns>
    ///     The options; <see cref="HelpCliOptions.GetCatalogConfiguration"/> gives the tests the
    ///     composition that the tool ships.
    /// </returns>
    internal static HelpCliOptions CreateOptions(
        TextWriter? output = null,
        TextWriter? error = null,
        Action<HelpCatalogBuilder, CultureInfo?>? configure = null) =>
        new()
        {
            ToolName = ToolName,
            Version = Program.Version,
            ConfigureCatalog = configure ?? ((builder, _) => Configure(builder)),
            UsageLines = ["sarifmark help [topic]", "sarifmark help --search <words>"],
            IndexPreamble = "Start with 'sarifmark help getting-started'.",
            Output = output,
            Error = error
        };

    /// <summary>
    ///     Creates a help command that writes to the given writers.
    /// </summary>
    /// <param name="output">The writer for output.</param>
    /// <param name="error">The writer for errors.</param>
    /// <param name="configure">
    ///     Optional replacement for <see cref="Configure"/>, used by tests to observe when the
    ///     catalog is built.
    /// </param>
    /// <returns>The new help command.</returns>
    internal static HelpCommand Create(
        TextWriter output,
        TextWriter error,
        Action<HelpCatalogBuilder, CultureInfo?>? configure = null) =>
        new(output, error, configure);

    /// <summary>
    ///     Adds the tool's own part of the catalog configuration to a builder: the embedded cards, the
    ///     closed tag vocabulary, and the expected set of topics. The tool name and the version are set
    ///     by the helper from <see cref="CreateOptions"/>.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    internal static void Configure(HelpCatalogBuilder builder) =>
        builder
            .AddSource(Cards)
            .TagVocabulary(PurposeTags, PurposeTags)
            .ExpectedRoots(RootCardIds);

    /// <summary>
    ///     Runs the help command when the arguments start with the <c>help</c> verb.
    /// </summary>
    /// <param name="args">The command-line arguments of the tool.</param>
    /// <param name="exitCode">Receives the exit code when the command was answered.</param>
    /// <returns>
    ///     <see langword="true"/> when the arguments started with the help verb and were answered;
    ///     <see langword="false"/> when the tool should continue with its own option parser. Returning
    ///     <see langword="false"/> never builds the catalog.
    /// </returns>
    public bool TryRun(string[] args, out int exitCode) => _cli.TryRun(args, out exitCode);
}
