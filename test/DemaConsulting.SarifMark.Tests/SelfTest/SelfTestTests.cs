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
}
