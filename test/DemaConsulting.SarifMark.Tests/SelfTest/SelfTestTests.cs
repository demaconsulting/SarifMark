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
///     Subsystem tests for the Self-Validation subsystem.
/// </summary>
[TestClass]
public class SelfTestTests
{
    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [TestMethod]
    public void SelfTest_ValidateFlag_RunsSelfValidation()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            using var context = Context.Create(["--validate"]);
            Validation.Run(context);
            var output = outWriter.ToString();

            // Assert
            Assert.AreEqual(0, context.ExitCode);
            Assert.Contains("Total Tests:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that validate flag with TRX results parameter writes a TRX file.
    /// </summary>
    [TestMethod]
    public void SelfTest_ResultsFile_WritesTrxFile()
    {
        // Arrange
        var resultsFile = Path.Combine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.trx");

        try
        {
            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);

                // Act
                using var context = Context.Create(["--validate", "--results", resultsFile]);
                Validation.Run(context);

                // Assert
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(File.Exists(resultsFile), "TRX results file was not created");

                var content = File.ReadAllText(resultsFile);
                Assert.Contains("<TestRun", content);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that validate flag with JUnit XML results parameter writes a JUnit XML file.
    /// </summary>
    [TestMethod]
    public void SelfTest_ResultsFile_WritesJUnitFile()
    {
        // Arrange
        var resultsFile = Path.Combine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.xml");

        try
        {
            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);

                // Act
                using var context = Context.Create(["--validate", "--results", resultsFile]);
                Validation.Run(context);

                // Assert
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(File.Exists(resultsFile), "JUnit XML results file was not created");

                var content = File.ReadAllText(resultsFile);
                Assert.Contains("<testsuite", content);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that enforcement mode behavior is verified by the self-validation suite.
    /// </summary>
    [TestMethod]
    public void SelfTest_EnforcementTest_RunsWithinValidation()
    {
        // Arrange
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run validation which internally exercises enforcement mode
            using var context = Context.Create(["--validate"]);
            Validation.Run(context);
            var output = outWriter.ToString();

            // Assert - enforcement test within validation passes
            Assert.AreEqual(0, context.ExitCode);
            Assert.Contains("SarifMark_Enforcement", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
