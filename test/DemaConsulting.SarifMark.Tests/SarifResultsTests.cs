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
///     Unit tests for the SarifResults class.
/// </summary>
[TestClass]
public class SarifResultsTests
{
    /// <summary>
    ///     Test directory for temporary test files.
    /// </summary>
    private string? _testDirectory;

    /// <summary>
    ///     Initialize test resources before each test.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"SarifMarkTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>
    ///     Clean up test resources after each test.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        if (_testDirectory != null && Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is null.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_NullPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SarifResults.Read(null!));
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is empty.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_EmptyPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SarifResults.Read(string.Empty));
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is whitespace.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_WhitespacePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SarifResults.Read("   "));
    }

    /// <summary>
    ///     Test that Read throws FileNotFoundException when file does not exist.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException()
    {
        var filePath = Path.Combine(_testDirectory!, "nonexistent.sarif");
        Assert.Throws<FileNotFoundException>(() => SarifResults.Read(filePath));
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException for invalid JSON.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_InvalidJson_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "invalid.sarif");
        File.WriteAllText(filePath, "{ invalid json }");

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("Invalid JSON", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when version is missing.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingVersion_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-version.sarif");
        File.WriteAllText(filePath, """
            {
                "runs": []
            }
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("missing 'version'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when runs is missing.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingRuns_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-runs.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0"
            }
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("missing or invalid 'runs'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when runs array is empty.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "empty-runs.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": []
            }
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("'runs' array is empty", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when tool is missing.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingTool_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-tool.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {}
                ]
            }
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("missing 'tool'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when driver is missing.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingDriver_ThrowsInvalidOperationException()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-driver.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {}
                    }
                ]
            }
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));
        Assert.Contains("missing 'driver'", exception.Message);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with no results.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_NoResults_ReturnsValidResults()
    {
        var filePath = Path.Combine(_testDirectory!, "no-results.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual("1.0.0", results.ToolVersion);
        Assert.AreEqual(0, results.ResultCount);
        Assert.IsEmpty(results.Results);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with empty results array.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_EmptyResults_ReturnsValidResults()
    {
        var filePath = Path.Combine(_testDirectory!, "empty-results.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0"
                            }
                        },
                        "results": []
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual("1.0.0", results.ToolVersion);
        Assert.AreEqual(0, results.ResultCount);
        Assert.IsEmpty(results.Results);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with results.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_WithResults_ReturnsValidResults()
    {
        var filePath = Path.Combine(_testDirectory!, "with-results.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0"
                            }
                        },
                        "results": [
                            {
                                "ruleId": "TEST001",
                                "level": "error",
                                "message": {
                                    "text": "Error 1"
                                }
                            },
                            {
                                "ruleId": "TEST002",
                                "level": "warning",
                                "message": {
                                    "text": "Warning 1"
                                }
                            },
                            {
                                "ruleId": "TEST003",
                                "level": "note",
                                "message": {
                                    "text": "Note 1"
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual("1.0.0", results.ToolVersion);
        Assert.AreEqual(3, results.ResultCount);
        Assert.HasCount(3, results.Results);
        
        Assert.AreEqual("TEST001", results.Results[0].RuleId);
        Assert.AreEqual("error", results.Results[0].Level);
        Assert.AreEqual("Error 1", results.Results[0].Message);
        
        Assert.AreEqual("TEST002", results.Results[1].RuleId);
        Assert.AreEqual("warning", results.Results[1].Level);
        Assert.AreEqual("Warning 1", results.Results[1].Message);
        
        Assert.AreEqual("TEST003", results.Results[2].RuleId);
        Assert.AreEqual("note", results.Results[2].Level);
        Assert.AreEqual("Note 1", results.Results[2].Message);
    }

    /// <summary>
    ///     Test that Read handles missing tool name gracefully.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingToolName_UsesUnknown()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-tool-name.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "version": "1.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual("Unknown", results.ToolName);
        Assert.AreEqual("1.0.0", results.ToolVersion);
        Assert.AreEqual(0, results.ResultCount);
        Assert.IsEmpty(results.Results);
    }

    /// <summary>
    ///     Test that Read handles missing tool version gracefully.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_MissingToolVersion_UsesUnknown()
    {
        var filePath = Path.Combine(_testDirectory!, "missing-tool-version.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool"
                            }
                        }
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual("Unknown", results.ToolVersion);
        Assert.AreEqual(0, results.ResultCount);
        Assert.IsEmpty(results.Results);
    }

    /// <summary>
    ///     Test internal constructor for testing purposes.
    /// </summary>
    [TestMethod]
    public void SarifResults_InternalConstructor_CreatesValidInstance()
    {
        var resultList = new List<SarifResult>
        {
            new("RULE001", "error", "Error message", "file.cs", 10),
            new("RULE002", "warning", "Warning message", "file.cs", 20),
            new("RULE003", "note", "Note message", null, null),
            new("RULE004", "error", "Another error", "other.cs", 5),
            new("RULE005", "warning", "Another warning", "other.cs", 15)
        };

        var results = new SarifResults("TestTool", "1.0.0", resultList);

        Assert.AreEqual("TestTool", results.ToolName);
        Assert.AreEqual("1.0.0", results.ToolVersion);
        Assert.AreEqual(5, results.ResultCount);
        Assert.HasCount(5, results.Results);
        Assert.AreEqual("RULE001", results.Results[0].RuleId);
        Assert.AreEqual("error", results.Results[0].Level);
        Assert.AreEqual("Error message", results.Results[0].Message);
        Assert.AreEqual("file.cs", results.Results[0].Uri);
        Assert.AreEqual(10, results.Results[0].StartLine);
    }

    /// <summary>
    ///     Test that Read successfully parses result with location information.
    /// </summary>
    [TestMethod]
    public void SarifResults_Read_WithLocations_ReturnsResultsWithLocationData()
    {
        var filePath = Path.Combine(_testDirectory!, "with-locations.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0"
                            }
                        },
                        "results": [
                            {
                                "ruleId": "CA1001",
                                "level": "warning",
                                "message": {
                                    "text": "Types that own disposable fields should be disposable"
                                },
                                "locations": [
                                    {
                                        "physicalLocation": {
                                            "artifactLocation": {
                                                "uri": "src/MyClass.cs"
                                            },
                                            "region": {
                                                "startLine": 42
                                            }
                                        }
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
            """);

        var results = SarifResults.Read(filePath);

        Assert.AreEqual(1, results.ResultCount);
        Assert.AreEqual("CA1001", results.Results[0].RuleId);
        Assert.AreEqual("warning", results.Results[0].Level);
        Assert.AreEqual("Types that own disposable fields should be disposable", results.Results[0].Message);
        Assert.AreEqual("src/MyClass.cs", results.Results[0].Uri);
        Assert.AreEqual(42, results.Results[0].StartLine);
    }
}
