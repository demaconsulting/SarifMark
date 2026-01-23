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

namespace DemaConsulting.SarifMark.Tests;

/// <summary>
///     Unit tests for the Program class.
/// </summary>
[TestClass]
public class ProgramTests
{
    /// <summary>
    ///     Test that Main with no arguments returns error due to missing sarif parameter.
    /// </summary>
    [TestMethod]
    public void Program_Main_NoArguments_ReturnsError()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            var result = InvokeMain([]);

            Assert.AreEqual(1, result);
            var output = outWriter.ToString();
            Assert.Contains("SarifMark version", output);
            Assert.Contains("--sarif parameter is required", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with --version flag displays version only.
    /// </summary>
    [TestMethod]
    public void Program_Main_VersionFlag_DisplaysVersionOnly()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            var result = InvokeMain(["--version"]);

            Assert.AreEqual(0, result);
            var output = outWriter.ToString();
            Assert.DoesNotContain("Copyright", output);
            Assert.DoesNotContain("SarifMark version", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with --help flag displays help with banner.
    /// </summary>
    [TestMethod]
    public void Program_Main_HelpFlag_DisplaysHelp()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            var result = InvokeMain(["--help"]);

            Assert.AreEqual(0, result);
            var output = outWriter.ToString();
            Assert.Contains("SarifMark version", output);
            Assert.Contains("Copyright", output);
            Assert.Contains("Usage:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Main with unknown argument returns error.
    /// </summary>
    [TestMethod]
    public void Program_Main_UnknownArgument_ReturnsError()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            var result = InvokeMain(["--unknown"]);

            Assert.AreEqual(1, result);
            Assert.Contains("Unsupported argument", outWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Invokes the Main method using reflection.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>The exit code returned by Main.</returns>
    private static int InvokeMain(string[] args)
    {
        var programType = typeof(Program).Assembly.GetType("DemaConsulting.SarifMark.Program");
        var mainMethod = programType?.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
        var result = mainMethod?.Invoke(null, [args]);
        return result is int exitCode ? exitCode : -1;
    }
}
