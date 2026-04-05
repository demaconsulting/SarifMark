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
    private string _dllPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating the SarifMark DLL.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        var baseDir = AppContext.BaseDirectory;
        _dllPath = PathHelpers.SafePathCombine(baseDir, "DemaConsulting.SarifMark.dll");

        Assert.IsTrue(File.Exists(_dllPath), $"Could not find SarifMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [TestMethod]
    public void SelfTest_ValidateFlag_RunsSelfValidation()
    {
        // Arrange - No special setup needed

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Assert
        Assert.AreEqual(0, exitCode);
        Assert.Contains("SarifMark version", output);
        Assert.Contains("Total Tests:", output);
    }

    /// <summary>
    ///     Test that validate flag with TRX results parameter writes a TRX file.
    /// </summary>
    [TestMethod]
    public void SelfTest_ResultsFile_WritesTrxFile()
    {
        // Arrange
        var resultsFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.trx");

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
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(resultsFile), "TRX results file was not created");

            var content = File.ReadAllText(resultsFile);
            Assert.Contains("<TestRun", content);
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
    ///     Test that enforce flag returns non-zero exit code when issues are found.
    /// </summary>
    [TestMethod]
    public void SelfTest_EnforceFlag_ReturnsNonZeroOnIssues()
    {
        // Arrange
        var baseDir = AppContext.BaseDirectory;
        var sarifFile = PathHelpers.SafePathCombine(
            PathHelpers.SafePathCombine(baseDir, "TestData"),
            "sample.sarif");
        Assert.IsTrue(File.Exists(sarifFile), $"Test SARIF file not found at {sarifFile}");

        // Act
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--sarif", sarifFile,
            "--enforce");

        // Assert
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Issues found in SARIF file", output);
    }

    /// <summary>
    ///     Test that validate flag with JUnit XML results parameter writes a JUnit XML file.
    /// </summary>
    [TestMethod]
    public void SelfTest_ResultsFile_WritesJUnitFile()
    {
        // Arrange
        var resultsFile = PathHelpers.SafePathCombine(Path.GetTempPath(), $"test-results-{Guid.NewGuid()}.xml");

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
            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(File.Exists(resultsFile), "JUnit XML results file was not created");

            var content = File.ReadAllText(resultsFile);
            Assert.Contains("<testsuite", content);
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }
}
