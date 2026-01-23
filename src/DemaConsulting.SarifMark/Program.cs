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

using System.Reflection;

namespace DemaConsulting.SarifMark;

/// <summary>
///     Main program entry point for the SarifMark tool.
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Gets the application version string.
    /// </summary>
    public static string Version
    {
        get
        {
            var assembly = typeof(Program).Assembly;
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                   ?? assembly.GetName().Version?.ToString()
                   ?? "0.0.0";
        }
    }

    /// <summary>
    ///     Main entry point for the SarifMark tool.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code: 0 for success, non-zero for failure.</returns>
    private static int Main(string[] args)
    {
        try
        {
            // Create context from arguments
            using var context = Context.Create(args);

            // Run the program logic
            Run(context);

            // Return the exit code from the context
            return context.ExitCode;
        }
        catch (ArgumentException ex)
        {
            // Print expected argument exceptions and return error code
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (InvalidOperationException ex)
        {
            // Print expected operation exceptions and return error code
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            // Print unexpected exceptions and re-throw to generate event logs
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Runs the program logic based on the provided context.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Priority 1: Version query
        if (context.Version)
        {
            Console.WriteLine(Version);
            return;
        }

        // Print application banner
        PrintBanner(context);

        // Priority 2: Help
        if (context.Help)
        {
            PrintHelp(context);
            return;
        }

        // Priority 3: Self-Validation
        if (context.Validate)
        {
            Validation.Run(context);
            return;
        }

        // Priority 4: SARIF analysis processing
        ProcessSarifAnalysis(context);
    }

    /// <summary>
    ///     Prints the application banner.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintBanner(Context context)
    {
        context.WriteLine($"SarifMark version {Version}");
        context.WriteLine("Copyright (c) DEMA Consulting");
        context.WriteLine("");
    }

    /// <summary>
    ///     Prints usage information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintHelp(Context context)
    {
        context.WriteLine("Usage: sarifmark [options]");
        context.WriteLine("");
        context.WriteLine("Options:");
        context.WriteLine("  -v, --version              Display version information");
        context.WriteLine("  -?, -h, --help             Display this help message");
        context.WriteLine("  --silent                   Suppress console output");
        context.WriteLine("  --validate                 Run self-validation");
        context.WriteLine("  --results <file>           Write validation results to file (.trx or .xml)");
        context.WriteLine("  --enforce                  Return non-zero exit code if issues found");
        context.WriteLine("  --log <file>               Write output to log file");
        context.WriteLine("  --sarif <file>             SARIF file to process");
        context.WriteLine("  --report <file>            Export analysis results to markdown file");
        context.WriteLine("  --report-depth <depth>     Markdown header depth for report (default: 1)");
        context.WriteLine("  --heading <text>           Custom heading for report (default: [ToolName] Analysis)");
    }

    /// <summary>
    ///     Processes SARIF analysis results and generates reports as requested.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void ProcessSarifAnalysis(Context context)
    {
        // Validate required parameters
        if (string.IsNullOrWhiteSpace(context.SarifFile))
        {
            context.WriteError("Error: --sarif parameter is required");
            return;
        }

        context.WriteLine($"SARIF File: {context.SarifFile}");

        // Read SARIF results
        context.WriteLine("Reading SARIF file...");
        SarifResults sarifResults;
        try
        {
            sarifResults = SarifResults.Read(context.SarifFile);
            context.WriteLine($"Tool: {sarifResults.ToolName} {sarifResults.ToolVersion}");
            context.WriteLine($"Results: {sarifResults.ResultCount}");
        }
        catch (FileNotFoundException ex)
        {
            context.WriteError($"Error: {ex.Message}");
            return;
        }
        catch (InvalidOperationException ex)
        {
            context.WriteError($"Error: Failed to read SARIF file: {ex.Message}");
            return;
        }

        // Check enforcement if requested
        if (context.Enforce && sarifResults.ResultCount > 0)
        {
            context.WriteError("Error: Issues found in SARIF file");
        }

        // Export report if requested
        if (context.ReportFile != null)
        {
            context.WriteLine($"Writing report to {context.ReportFile}...");
            try
            {
                var markdown = sarifResults.ToMarkdown(context.ReportDepth, context.Heading);
                File.WriteAllText(context.ReportFile, markdown);
                context.WriteLine("Report generated successfully.");
            }
            catch (Exception ex)
            {
                context.WriteError($"Error: Failed to write report: {ex.Message}");
            }
        }
    }
}
