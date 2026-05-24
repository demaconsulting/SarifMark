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
    ///     Log file stream writer (if logging is enabled).
    /// </summary>
    private StreamWriter? _logWriter;

    /// <summary>
    ///     Indicates whether errors have been reported.
    /// </summary>
    private bool _hasErrors;

    /// <summary>
    ///     Gets a value indicating whether the version flag was specified.
    /// </summary>
    public bool Version { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the help flag was specified.
    /// </summary>
    public bool Help { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the silent flag was specified.
    /// </summary>
    public bool Silent { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the validate flag was specified.
    /// </summary>
    public bool Validate { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the enforce flag was specified.
    /// </summary>
    public bool Enforce { get; private init; }

    /// <summary>
    ///     Gets the SARIF file path.
    /// </summary>
    public string? SarifFile { get; private init; }

    /// <summary>
    ///     Gets the report file path.
    /// </summary>
    public string? ReportFile { get; private init; }

    /// <summary>
    ///     Gets the markdown depth.
    /// </summary>
    public int Depth { get; private init; } = 1;

    /// <summary>
    ///     Gets the custom heading for the report.
    /// </summary>
    public string? Heading { get; private init; }

    /// <summary>
    ///     Gets the validation results file path.
    /// </summary>
    public string? ResultsFile { get; private init; }

    /// <summary>
    ///     Gets the proposed exit code for the application (0 for success, 1 for errors).
    /// </summary>
    public int ExitCode => _hasErrors ? 1 : 0;

    /// <summary>
    ///     Private constructor - use Create factory method instead.
    /// </summary>
    private Context()
    {
    }

    /// <summary>
    ///     Creates a Context instance from command-line arguments.
    /// </summary>
    /// <remarks>
    ///     A factory method is used instead of a public constructor because all settable
    ///     properties on <see cref="Context"/> use <see langword="private init"/> accessors
    ///     that can only be assigned during object-initializer syntax. The factory method
    ///     delegates the actual parsing to the private <see cref="ArgumentParser"/> helper,
    ///     validates and transforms the parsed values, and then constructs the immutable
    ///     <see cref="Context"/> record in a single initializer block. This pattern keeps
    ///     the public surface of <see cref="Context"/> read-only while still allowing
    ///     thorough validation before any property is set.
    /// </remarks>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A new Context instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arguments are invalid.</exception>
    public static Context Create(string[] args)
    {
        // Validate input
        ArgumentNullException.ThrowIfNull(args);

        var parser = new ArgumentParser();
        parser.ParseArguments(args);

        var result = new Context
        {
            Version = parser.Version,
            Help = parser.Help,
            Silent = parser.Silent,
            Validate = parser.Validate,
            Enforce = parser.Enforce,
            SarifFile = parser.SarifFile,
            ReportFile = parser.ReportFile,
            Depth = parser.Depth,
            Heading = parser.Heading,
            ResultsFile = parser.ResultsFile
        };

        // Open log file if specified
        if (parser.LogFile != null)
        {
            result.OpenLogFile(parser.LogFile);
        }

        return result;
    }

    /// <summary>
    ///     Opens the log file for writing.
    /// </summary>
    /// <param name="logFile">Log file path.</param>
    /// <exception cref="InvalidOperationException">Thrown when the log file at <paramref name="logFile"/> cannot be opened for writing.</exception>
    private void OpenLogFile(string logFile)
    {
        try
        {
            // Open with AutoFlush enabled so log entries are immediately written to disk
            // even if the application terminates unexpectedly before Dispose is called
            _logWriter = new StreamWriter(logFile, append: false) { AutoFlush = true };
        }
        // Catch all exceptions to wrap them with context about which file failed to open.
        // This is intentional to provide better error messages to users regardless of the
        // underlying exception type (IOException, UnauthorizedAccessException, etc.)
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to open log file '{logFile}': {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Helper class for parsing command-line arguments.
    /// </summary>
    private sealed class ArgumentParser
    {
        /// <summary>
        ///     Gets a value indicating whether the version flag was specified.
        /// </summary>
        public bool Version { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the help flag was specified.
        /// </summary>
        public bool Help { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the silent flag was specified.
        /// </summary>
        public bool Silent { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the validate flag was specified.
        /// </summary>
        public bool Validate { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the enforce flag was specified.
        /// </summary>
        public bool Enforce { get; private set; }

        /// <summary>
        ///     Gets the SARIF file path.
        /// </summary>
        public string? SarifFile { get; private set; }

        /// <summary>
        ///     Gets the report file path.
        /// </summary>
        public string? ReportFile { get; private set; }

        /// <summary>
        ///     Gets the markdown depth.
        /// </summary>
        public int Depth { get; private set; } = 1;

        /// <summary>
        ///     Gets the custom heading for the report.
        /// </summary>
        public string? Heading { get; private set; }

        /// <summary>
        ///     Gets the log file path.
        /// </summary>
        public string? LogFile { get; private set; }

        /// <summary>
        ///     Gets the validation results file path.
        /// </summary>
        public string? ResultsFile { get; private set; }

        /// <summary>
        ///     Parses command-line arguments.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <exception cref="ArgumentException">Thrown when an unsupported argument is encountered, or a value-bearing argument is missing its value, or a depth argument is not a positive integer.</exception>
        public void ParseArguments(string[] args)
        {
            // Validate input
            ArgumentNullException.ThrowIfNull(args);

            int i = 0;
            while (i < args.Length)
            {
                var arg = args[i++];
                i = ParseArgument(arg, args, i);
            }
        }

        /// <summary>
        ///     Parses a single argument.
        /// </summary>
        /// <param name="arg">Argument to parse.</param>
        /// <param name="args">All arguments.</param>
        /// <param name="index">Current index.</param>
        /// <returns>Updated index.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="arg"/> is not a recognized argument, or a value-bearing argument is the last token without a following value, or a depth value is not a positive integer.</exception>
        private int ParseArgument(string arg, string[] args, int index)
        {
            switch (arg)
            {
                case "-v":
                case "--version":
                    Version = true;
                    return index;

                case "-?":
                case "-h":
                case "--help":
                    Help = true;
                    return index;

                case "--silent":
                    Silent = true;
                    return index;

                case "--validate":
                    Validate = true;
                    return index;

                case "--enforce":
                    Enforce = true;
                    return index;

                case "--log":
                    LogFile = GetRequiredStringArgument(arg, args, index, "a filename argument");
                    return index + 1;

                case "--sarif":
                    SarifFile = GetRequiredStringArgument(arg, args, index, "a filename argument");
                    return index + 1;

                case "--report":
                    ReportFile = GetRequiredStringArgument(arg, args, index, "a filename argument");
                    return index + 1;

                case "--depth":
                case "--report-depth": // Legacy alias for --depth
                    Depth = GetRequiredIntArgument(arg, args, index);
                    return index + 1;

                case "--heading":
                    Heading = GetRequiredStringArgument(arg, args, index, "a heading text argument");
                    return index + 1;

                case "--result":     // Legacy alias for --results to preserve backwards compatibility
                case "--results":
                    ResultsFile = GetRequiredStringArgument(arg, args, index, "a results filename argument");
                    return index + 1;

                default:
                    throw new ArgumentException($"Unsupported argument '{arg}'", nameof(args));
            }
        }

        /// <summary>
        ///     Gets a required string argument value.
        /// </summary>
        /// <param name="arg">Argument name.</param>
        /// <param name="args">All arguments.</param>
        /// <param name="index">Current index.</param>
        /// <param name="description">Description of what's required.</param>
        /// <returns>The argument value.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="arg"/> is the last token in the argument list and has no following value.</exception>
        private static string GetRequiredStringArgument(string arg, string[] args, int index, string description)
        {
            if (index >= args.Length)
            {
                throw new ArgumentException($"{arg} requires {description}", nameof(args));
            }

            return args[index];
        }

        /// <summary>
        ///     Gets a required positive integer argument value.
        /// </summary>
        /// <param name="arg">Argument name.</param>
        /// <param name="args">All arguments.</param>
        /// <param name="index">Current index.</param>
        /// <returns>The argument value.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="arg"/> is the last token in the argument list, or its value is not a positive integer.</exception>
        private static int GetRequiredIntArgument(string arg, string[] args, int index)
        {
            if (index >= args.Length)
            {
                throw new ArgumentException($"{arg} requires a depth argument", nameof(args));
            }

            if (!int.TryParse(args[index], out var value) || value < 1)
            {
                throw new ArgumentException($"{arg} requires a positive integer", nameof(args));
            }

            return value;
        }
    }

    /// <summary>
    ///     Writes a line of output to the console and log file (if logging is enabled).
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void WriteLine(string message)
    {
        // Write to console unless silent mode is enabled
        if (!Silent)
        {
            Console.WriteLine(message);
        }

        // Write to log file if logging is enabled
        _logWriter?.WriteLine(message);
    }

    /// <summary>
    ///     Writes an error message to the error console and log file (if logging is enabled).
    /// </summary>
    /// <param name="message">The error message to write.</param>
    /// <remarks>
    ///     This method sets <see cref="ExitCode"/> to 1 on the first call, and this change is permanent
    ///     for the lifetime of the <see cref="Context"/> instance. Additionally, this method is not safe
    ///     for concurrent use because it temporarily modifies <see cref="Console.ForegroundColor"/>.
    /// </remarks>
    public void WriteError(string message)
    {
        // Mark that we have encountered errors
        _hasErrors = true;

        // Write to error console unless silent mode is enabled
        if (!Silent)
        {
            var previousColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
            }
            finally
            {
                Console.ForegroundColor = previousColor;
            }
        }

        // Write to log file if logging is enabled
        _logWriter?.WriteLine(message);
    }

    /// <summary>
    ///     Disposes resources used by the Context.
    /// </summary>
    public void Dispose()
    {
        // Close and dispose the log file writer if it exists
        _logWriter?.Dispose();
        _logWriter = null;
    }
}
