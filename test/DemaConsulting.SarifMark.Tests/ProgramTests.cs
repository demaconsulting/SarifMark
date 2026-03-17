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
        // Arrange
        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            // Act
            var result = Program.Main([]);

            // Assert
            Assert.AreEqual(1, result);
            Assert.Contains("SarifMark version", outWriter.ToString());
            Assert.Contains("--sarif parameter is required", errWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Main with --version flag displays version only.
    /// </summary>
    [TestMethod]
    public void Program_Main_VersionFlag_DisplaysVersionOnly()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--version"]);

            // Assert
            Assert.AreEqual(0, result);
            var output = outWriter.ToString().Trim();
            Assert.MatchesRegex(@"^\d+\.\d+\.\d+(?:-[\w.-]+)?(?:\+[\w.-]+)?$", output);
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
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            var result = Program.Main(["--help"]);

            // Assert
            Assert.AreEqual(0, result);
            var output = outWriter.ToString();
            Assert.Contains("SarifMark version", output);
            Assert.Contains("Copyright", output);
            Assert.Contains("Usage:", output);
            Assert.Contains("--version", output);
            Assert.Contains("--help", output);
            Assert.Contains("--silent", output);
            Assert.Contains("--validate", output);
            Assert.Contains("--results", output);
            Assert.Contains("--enforce", output);
            Assert.Contains("--log", output);
            Assert.Contains("--sarif", output);
            Assert.MatchesRegex(@"--report(?!-)", output);
            Assert.Contains("--report-depth", output);
            Assert.Contains("--heading", output);
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
        // Arrange
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            // Act
            var result = Program.Main(["--unknown"]);

            // Assert
            Assert.AreEqual(1, result);
            Assert.Contains("Unsupported argument", errWriter.ToString());
        }
        finally
        {
            Console.SetError(originalError);
        }
    }
}
