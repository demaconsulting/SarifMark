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
///     Context class that handles command-line arguments and program output.
/// </summary>
internal sealed class Context : IDisposable
{
    /// <summary>
    ///     Gets a value indicating whether the version flag was specified.
    /// </summary>
    public bool Version { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the help flag was specified.
    /// </summary>
    public bool Help { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether errors have been reported.
    /// </summary>
    public bool HasErrors { get; private set; }

    /// <summary>
    ///     Creates a new context from command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A new context instance.</returns>
    public static Context Create(string[] args)
    {
        var context = new Context();

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--version":
                case "-v":
                    context = context with { Version = true };
                    break;

                case "--help":
                case "-h":
                case "-?":
                    context = context with { Help = true };
                    break;

                default:
                    Console.Error.WriteLine($"Unknown argument: {arg}");
                    context.HasErrors = true;
                    break;
            }
        }

        return context;
    }

    /// <summary>
    ///     Writes a line to the console output.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    ///     Writes an error message to the console error stream.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    public void WriteError(string message)
    {
        Console.Error.WriteLine(message);
        HasErrors = true;
    }

    /// <summary>
    ///     Disposes the context and releases any resources.
    /// </summary>
    public void Dispose()
    {
        // No resources to dispose at this time
    }
}
