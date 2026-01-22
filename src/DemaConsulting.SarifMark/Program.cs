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

namespace DemaConsulting.SarifMark;

/// <summary>
///     Main program entry point for the SarifMark tool.
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Gets the version string for the SarifMark tool.
    /// </summary>
    private static string Version => typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0";

    /// <summary>
    ///     Main entry point for the SarifMark tool.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code: 0 for success, non-zero for failure.</returns>
    private static int Main(string[] args)
    {
        try
        {
            using var context = Context.Create(args);

            if (context.Version)
            {
                Console.WriteLine($"SarifMark version {Version}");
                return 0;
            }

            if (context.Help || context.HasErrors)
            {
                PrintHelp();
                return context.HasErrors ? 1 : 0;
            }

            context.WriteLine("SarifMark tool - SARIF report generation");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    ///     Prints the help message.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine("SarifMark - Tool to create SARIF reports from various static analysis tools");
        Console.WriteLine();
        Console.WriteLine("Usage: sarifmark [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --version       Display version information");
        Console.WriteLine("  --help          Display this help message");
        Console.WriteLine();
    }
}
