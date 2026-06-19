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
public sealed class SarifResultsTests : IDisposable
{
    /// <summary>
    ///     Test directory for temporary test files.
    /// </summary>
    private readonly string _testDirectory;

    /// <summary>
    ///     Initialize test resources before each test.
    /// </summary>
    public SarifResultsTests()
    {
        _testDirectory = PathHelpers.SafePathCombine(Path.GetTempPath(), $"SarifMarkTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>Releases resources after each test.</summary>
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is null.
    /// </summary>
    [Fact]
    public void SarifResults_Read_NullPath_ThrowsArgumentException()
    {
        // Arrange
        // (no setup required)

        // Act
        var exception = Assert.Throws<ArgumentException>(() => SarifResults.Read(null!));

        // Assert
        Assert.NotNull(exception);
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is empty.
    /// </summary>
    [Fact]
    public void SarifResults_Read_EmptyPath_ThrowsArgumentException()
    {
        // Arrange
        // (no setup required)

        // Act
        var exception = Assert.Throws<ArgumentException>(() => SarifResults.Read(string.Empty));

        // Assert
        Assert.NotNull(exception);
    }

    /// <summary>
    ///     Test that Read throws ArgumentException when file path is whitespace.
    /// </summary>
    [Fact]
    public void SarifResults_Read_WhitespacePath_ThrowsArgumentException()
    {
        // Arrange
        // (no setup required)

        // Act
        var exception = Assert.Throws<ArgumentException>(() => SarifResults.Read("   "));

        // Assert
        Assert.NotNull(exception);
    }

    /// <summary>
    ///     Test that Read throws FileNotFoundException when file does not exist.
    /// </summary>
    [Fact]
    public void SarifResults_Read_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "nonexistent.sarif");

        // Act
        var exception = Assert.Throws<FileNotFoundException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.NotNull(exception);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException for invalid JSON.
    /// </summary>
    [Fact]
    public void SarifResults_Read_InvalidJson_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file with invalid JSON content
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "invalid.sarif");
        File.WriteAllText(filePath, "{ invalid json }");

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("Invalid JSON", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when version is missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingVersion_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file missing the required 'version' field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-version.sarif");
        File.WriteAllText(filePath, """
            {
                "runs": []
            }
            """);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("missing 'version'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when runs is missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingRuns_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file missing the required 'runs' array
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-runs.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0"
            }
            """);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("missing or invalid 'runs'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when runs array is empty.
    /// </summary>
    [Fact]
    public void SarifResults_Read_EmptyRuns_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file with an empty 'runs' array
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "empty-runs.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": []
            }
            """);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("'runs' array is empty", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when tool is missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingTool_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file with a run missing the 'tool' field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-tool.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {}
                ]
            }
            """);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("missing 'tool'", exception.Message);
    }

    /// <summary>
    ///     Test that Read throws InvalidOperationException when driver is missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingDriver_ThrowsInvalidOperationException()
    {
        // Arrange - SARIF file with a tool missing the 'driver' field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-driver.sarif");
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

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => SarifResults.Read(filePath));

        // Assert
        Assert.Contains("missing 'driver'", exception.Message);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with no results.
    /// </summary>
    [Fact]
    public void SarifResults_Read_NoResults_ReturnsValidResults()
    {
        // Arrange - SARIF file with no results array
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "no-results.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - returns valid results with empty collection
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with empty results array.
    /// </summary>
    [Fact]
    public void SarifResults_Read_EmptyResults_ReturnsValidResults()
    {
        // Arrange - SARIF file with an empty results array
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "empty-results.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - returns valid results with zero count
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read successfully reads SARIF file with results.
    /// </summary>
    [Fact]
    public void SarifResults_Read_WithResults_ReturnsValidResults()
    {
        // Arrange - SARIF file with three results of varying levels
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "with-results.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - returns all three results with correct properties
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(3, results.Runs[0].ResultCount);
        Assert.Equal(3, results.Runs[0].Results.Count);

        Assert.Equal("TEST001", results.Runs[0].Results[0].RuleId);
        Assert.Equal("error", results.Runs[0].Results[0].Level);
        Assert.Equal("Error 1", results.Runs[0].Results[0].Message);

        Assert.Equal("TEST002", results.Runs[0].Results[1].RuleId);
        Assert.Equal("warning", results.Runs[0].Results[1].Level);
        Assert.Equal("Warning 1", results.Runs[0].Results[1].Message);

        Assert.Equal("TEST003", results.Runs[0].Results[2].RuleId);
        Assert.Equal("note", results.Runs[0].Results[2].Level);
        Assert.Equal("Note 1", results.Runs[0].Results[2].Message);
    }

    /// <summary>
    ///     Test that Read handles missing tool name gracefully.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingToolName_UsesUnknown()
    {
        // Arrange - SARIF file with driver missing the 'name' field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-tool-name.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - tool name defaults to 'Unknown'
        Assert.Equal("Unknown", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read handles missing tool version gracefully.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MissingToolVersion_UsesUnknown()
    {
        // Arrange - SARIF file with driver missing the 'version' field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "missing-tool-version.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - tool version defaults to 'Unknown'
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("Unknown", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read uses semanticVersion field when version field is missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_SemanticVersionField_ReturnsSemanticVersion()
    {
        // Arrange - SARIF file with semanticVersion field and no version field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "semantic-version.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "CodeQL",
                                "semanticVersion": "2.15.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - semanticVersion is used as the tool version
        Assert.Equal("CodeQL", results.Runs[0].ToolName);
        Assert.Equal("2.15.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read uses dottedQuadFileVersion field when version and semanticVersion are missing.
    /// </summary>
    [Fact]
    public void SarifResults_Read_DottedQuadFileVersionField_ReturnsDottedQuadFileVersion()
    {
        // Arrange - SARIF file with only dottedQuadFileVersion field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "dotted-quad-version.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "dottedQuadFileVersion": "3.0.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - dottedQuadFileVersion is used as the tool version
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("3.0.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read prioritizes version field over semanticVersion.
    /// </summary>
    [Fact]
    public void SarifResults_Read_VersionAndSemanticVersion_PrioritizesVersion()
    {
        // Arrange - SARIF file with both version and semanticVersion fields
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "version-priority.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0",
                                "semanticVersion": "2.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - version field takes priority over semanticVersion
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read prioritizes semanticVersion field over dottedQuadFileVersion.
    /// </summary>
    [Fact]
    public void SarifResults_Read_SemanticAndDottedQuad_PrioritizesSemanticVersion()
    {
        // Arrange - SARIF file with semanticVersion and dottedQuadFileVersion fields
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "semantic-priority.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "semanticVersion": "2.0.0",
                                "dottedQuadFileVersion": "3.0.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - semanticVersion takes priority over dottedQuadFileVersion
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("2.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read handles all three version fields with correct priority.
    /// </summary>
    [Fact]
    public void SarifResults_Read_AllVersionFields_PrioritizesVersion()
    {
        // Arrange - SARIF file with all three version fields present
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "all-versions.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0",
                                "semanticVersion": "2.0.0",
                                "dottedQuadFileVersion": "3.0.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - version field takes highest priority
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read handles empty version field and falls back to semanticVersion.
    /// </summary>
    [Fact]
    public void SarifResults_Read_EmptyVersionField_FallsBackToSemanticVersion()
    {
        // Arrange - SARIF file with empty version field and a semanticVersion field
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "empty-version.sarif");
        File.WriteAllText(filePath, """
            {
                "version": "2.1.0",
                "runs": [
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "",
                                "semanticVersion": "2.0.0"
                            }
                        }
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - semanticVersion is used when version field is empty
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("2.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(0, results.Runs[0].ResultCount);
        Assert.Empty(results.Runs[0].Results);
    }

    /// <summary>
    ///     Test that Read successfully parses result with location information.
    /// </summary>
    [Fact]
    public void SarifResults_Read_WithLocations_ReturnsResultsWithLocationData()
    {
        // Arrange - SARIF file with a result that includes location information
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "with-locations.sarif");
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

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - result contains the expected location information
        Assert.Equal(1, results.Runs[0].ResultCount);
        Assert.Equal("CA1001", results.Runs[0].Results[0].RuleId);
        Assert.Equal("warning", results.Runs[0].Results[0].Level);
        Assert.Equal("Types that own disposable fields should be disposable", results.Runs[0].Results[0].Message);
        Assert.Equal("src/MyClass.cs", results.Runs[0].Results[0].Uri);
        Assert.Equal(42, results.Runs[0].Results[0].StartLine);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth 1 produces correct output.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_Depth1_ProducesCorrectOutput()
    {
        // Arrange
        var resultList = new List<SarifFinding>
        {
            new("CA1001", "warning", "Types that own disposable fields should be disposable", "src/MyClass.cs", 42),
            new("CA2000", "error", "Dispose objects before losing scope", "src/Program.cs", 15)
        };

        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.NotNull(markdown);
        Assert.Contains("# TestTool Analysis", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
        Assert.Contains("## Issues", markdown);
        Assert.Contains("Found 2 issues", markdown);
        Assert.Contains("src/MyClass.cs(42): warning [CA1001] Types that own disposable fields should be disposable", markdown);
        Assert.Contains("src/Program.cs(15): error [CA2000] Dispose objects before losing scope", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth 3 uses correct heading levels.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_Depth3_UsesCorrectHeadingLevels()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(3);

        // Assert
        Assert.Contains("### TestTool Analysis", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
        Assert.Contains("#### Issues", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with no results shows correct message.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_NoResults_ShowsFoundNoResults()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("Found no issues", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with one result uses singular form.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_OneResult_UsesSingularForm()
    {
        // Arrange
        var resultList = new List<SarifFinding>
        {
            new("CA1001", "warning", "Test warning", "src/Test.cs", 10)
        };

        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("Found 1 issue", markdown);
        Assert.DoesNotContain("Found 1 issues", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth less than 1 throws exception.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => results.ToMarkdown(0));

        // Assert
        Assert.Contains("Depth must be between 1 and 6", exception.Message);
    }

    /// <summary>
    ///     Test that ToMarkdown with depth greater than 6 throws exception.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => results.ToMarkdown(7));

        // Assert
        Assert.Contains("Depth must be between 1 and 6", exception.Message);
    }

    /// <summary>
    ///     Test that ToMarkdown with maximum depth of 6 produces correct output.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_Depth6_ProducesCorrectOutput()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(6);

        // Assert
        Assert.Contains("###### TestTool Analysis", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
        Assert.Contains("###### Issues", markdown); // Capped at 6
    }

    /// <summary>
    ///     Test that ToMarkdown handles result without location information.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_ResultWithoutLocation_ShowsNoLocation()
    {
        // Arrange
        var resultList = new List<SarifFinding>
        {
            new("RULE001", "error", "Error without location", null, null)
        };

        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("(no location): error [RULE001] Error without location", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown handles result with URI but no line number.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_ResultWithUriNoLine_ShowsUriOnly()
    {
        // Arrange
        var resultList = new List<SarifFinding>
        {
            new("RULE002", "warning", "Warning with URI only", "src/File.cs", null)
        };

        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("src/File.cs: warning [RULE002] Warning with URI only", markdown);
        Assert.DoesNotContain("src/File.cs(", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with custom heading uses the provided heading.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_CustomHeading_UsesProvidedHeading()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(1, "My Custom Analysis Report");

        // Assert
        Assert.Contains("# My Custom Analysis Report", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
        Assert.DoesNotContain("# TestTool Analysis", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown with null heading uses default heading.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_NullHeading_UsesDefaultHeading()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(1, null);

        // Assert
        Assert.Contains("# TestTool Analysis", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown without heading parameter uses default heading.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_NoHeadingParameter_UsesDefaultHeading()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("# TestTool Analysis", markdown);
        Assert.Contains("**Tool:** TestTool 1.0.0", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown enforces line breaks between results.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_MultipleResults_EnforcesLineBreaks()
    {
        // Arrange
        var resultList = new List<SarifFinding>
        {
            new("CA1001", "warning", "First issue", "src/MyClass.cs", 42),
            new("CA2000", "error", "Second issue", "src/Program.cs", 15),
            new("CA3001", "note", "Third issue", "src/Helper.cs", 7)
        };

        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert - Each result line should end with two spaces for hard line break
        Assert.Contains($"src/MyClass.cs(42): warning [CA1001] First issue  {Environment.NewLine}", markdown);
        Assert.Contains($"src/Program.cs(15): error [CA2000] Second issue  {Environment.NewLine}", markdown);
        Assert.Contains($"src/Helper.cs(7): note [CA3001] Third issue  {Environment.NewLine}", markdown);
    }

    /// <summary>
    ///     Test that Read excludes suppressed results from the results collection.
    /// </summary>
    [Fact]
    public void SarifResults_Read_WithSuppressedResults_ExcludesSuppressedResults()
    {
        // Arrange
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "with-suppressions.sarif");
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
                                "level": "warning",
                                "message": {
                                    "text": "Unsuppressed warning"
                                }
                            },
                            {
                                "ruleId": "TEST002",
                                "level": "warning",
                                "message": {
                                    "text": "Suppressed warning"
                                },
                                "suppressions": [
                                    {
                                        "kind": "inSource",
                                        "justification": "This is intentional"
                                    }
                                ]
                            },
                            {
                                "ruleId": "TEST003",
                                "level": "error",
                                "message": {
                                    "text": "Another unsuppressed error"
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert
        Assert.Equal("TestTool", results.Runs[0].ToolName);
        Assert.Equal("1.0.0", results.Runs[0].ToolVersion);
        Assert.Equal(2, results.Runs[0].ResultCount);
        Assert.Equal(2, results.Runs[0].Results.Count);

        Assert.Equal("TEST001", results.Runs[0].Results[0].RuleId);
        Assert.Equal("Unsuppressed warning", results.Runs[0].Results[0].Message);

        Assert.Equal("TEST003", results.Runs[0].Results[1].RuleId);
        Assert.Equal("Another unsuppressed error", results.Runs[0].Results[1].Message);
    }

    /// <summary>
    ///     Test that Read does not suppress results with an empty suppressions array.
    /// </summary>
    [Fact]
    public void SarifResults_Read_EmptySuppressions_DoesNotExcludeResult()
    {
        // Arrange - SARIF file with a result that has an empty suppressions array
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "empty-suppressions.sarif");
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
                                "level": "warning",
                                "message": {
                                    "text": "Unsuppressed result with empty suppressions"
                                },
                                "suppressions": []
                            }
                        ]
                    }
                ]
            }
            """);

        // Act - read the SARIF file
        var results = SarifResults.Read(filePath);

        // Assert - result with empty suppressions array is not excluded
        Assert.Equal(1, results.Runs[0].ResultCount);
        Assert.Equal("TEST001", results.Runs[0].Results[0].RuleId);
        Assert.Equal("Unsuppressed result with empty suppressions", results.Runs[0].Results[0].Message);
    }

    /// <summary>
    ///     Test that Read returns zero file count when no artifacts array is present.
    /// </summary>
    [Fact]
    public void SarifResults_Read_NoArtifacts_ReturnsZeroFileCount()
    {
        // Arrange
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "no-artifacts.sarif");
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

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - no artifacts array means zero file count
        Assert.Equal(0, results.Runs[0].FileCount);
    }

    /// <summary>
    ///     Test that Read sums artifacts across all runs.
    /// </summary>
    [Fact]
    public void SarifResults_Read_WithArtifacts_ReturnsFileCount()
    {
        // Arrange
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "with-artifacts.sarif");
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
                        "artifacts": [
                            { "location": { "uri": "src/File1.cs" } },
                            { "location": { "uri": "src/File2.cs" } },
                            { "location": { "uri": "src/File3.cs" } }
                        ]
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - file count equals the number of artifacts
        Assert.Equal(3, results.Runs[0].FileCount);
    }

    /// <summary>
    ///     Test that Read extracts independent file counts for each run.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MultipleRuns_EachRunHasOwnFileCount()
    {
        // Arrange
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "multi-run-artifacts.sarif");
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
                        "artifacts": [
                            { "location": { "uri": "src/File1.cs" } },
                            { "location": { "uri": "src/File2.cs" } }
                        ]
                    },
                    {
                        "tool": {
                            "driver": {
                                "name": "TestTool",
                                "version": "1.0.0"
                            }
                        },
                        "artifacts": [
                            { "location": { "uri": "src/File3.cs" } }
                        ]
                    }
                ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert - each run has its own independent file count
        Assert.Equal(2, results.Runs[0].FileCount);
        Assert.Equal(1, results.Runs[1].FileCount);
    }

    /// <summary>
    ///     Test that ToMarkdown includes the file count in the header.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_ShowsFileCount()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [], 5)]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("**Files:** 5", markdown);
    }

    /// <summary>
    ///     Test that ToMarkdown shows zero file count when no files were analyzed.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_ZeroFileCount_ShowsZero()
    {
        // Arrange
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Act
        var markdown = results.ToMarkdown(1);

        // Assert
        Assert.Contains("**Files:** 0", markdown);
    }

    /// <summary>
    ///     Test that the internal constructor stores the runs collection and exposes HasIssues.
    /// </summary>
    [Fact]
    public void SarifResults_InternalConstructor_ExposesRunsAndHasIssues()
    {
        // Arrange
        var run = new SarifRun("TestTool", "1.0.0", []);

        // Act
        var results = new SarifResults([run]);

        // Assert
        Assert.Single(results.Runs);
        Assert.Same(run, results.Runs[0]);
        Assert.False(results.HasIssues);
    }

    /// <summary>
    ///     Test that Runs property returns a single run for a single-run SarifResults.
    /// </summary>
    [Fact]
    public void SarifResults_Runs_SingleRun_ReturnsSingleRun()
    {
        // Arrange & Act
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Assert
        Assert.Single(results.Runs);
        Assert.Equal("TestTool", results.Runs[0].ToolName);
    }

    /// <summary>
    ///     Test that HasIssues returns false when there are no issues.
    /// </summary>
    [Fact]
    public void SarifResults_HasIssues_NoIssues_ReturnsFalse()
    {
        // Arrange & Act
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", [])]);

        // Assert
        Assert.False(results.HasIssues);
    }

    /// <summary>
    ///     Test that HasIssues returns true when there are issues.
    /// </summary>
    [Fact]
    public void SarifResults_HasIssues_WithIssues_ReturnsTrue()
    {
        // Arrange
        var resultList = new List<SarifFinding> { new SarifFinding("R1", "warning", "msg", null, null) };

        // Act
        var results = new SarifResults([new SarifRun("TestTool", "1.0.0", resultList)]);

        // Assert
        Assert.True(results.HasIssues);
    }

    /// <summary>
    ///     Test that HasIssues returns true when any run has issues.
    /// </summary>
    [Fact]
    public void SarifResults_HasIssues_AnyRunHasIssues_ReturnsTrue()
    {
        // Arrange
        var run1 = new SarifRun("Tool1", "1.0", [], 0);
        var run2Results = new List<SarifFinding> { new SarifFinding("R1", "warning", "msg", null, null) };
        var run2 = new SarifRun("Tool2", "2.0", run2Results, 0);

        // Act
        var results = new SarifResults(new List<SarifRun> { run1, run2 });

        // Assert
        Assert.True(results.HasIssues);
    }

    /// <summary>
    ///     Test that Read parses all runs in a multi-run SARIF file.
    /// </summary>
    [Fact]
    public void SarifResults_Read_MultipleRuns_ReturnsAllRuns()
    {
        // Arrange - inline multi-run SARIF: Tool1 (1 result, 1 artifact) and Tool2 (0 results, 2 artifacts)
        var filePath = PathHelpers.SafePathCombine(_testDirectory, "multi-run.sarif");
        File.WriteAllText(filePath, """
            {
              "version": "2.1.0",
              "runs": [
                {
                  "tool": { "driver": { "name": "Tool1", "version": "1.0.0" } },
                  "results": [
                    {
                      "ruleId": "RULE001",
                      "level": "warning",
                      "message": { "text": "A warning from Tool1" },
                      "locations": [
                        {
                          "physicalLocation": {
                            "artifactLocation": { "uri": "file1.cs" },
                            "region": { "startLine": 10 }
                          }
                        }
                      ]
                    }
                  ],
                  "artifacts": [ { "location": { "uri": "file1.cs" } } ]
                },
                {
                  "tool": { "driver": { "name": "Tool2", "version": "2.0.0" } },
                  "results": [],
                  "artifacts": [
                    { "location": { "uri": "file2.cs" } },
                    { "location": { "uri": "file3.cs" } }
                  ]
                }
              ]
            }
            """);

        // Act
        var results = SarifResults.Read(filePath);

        // Assert
        Assert.Equal(2, results.Runs.Count);
        Assert.Equal("Tool1", results.Runs[0].ToolName);
        Assert.Equal("Tool2", results.Runs[1].ToolName);
    }

    /// <summary>
    ///     Test that ToMarkdown for multi-run files includes run indices.
    /// </summary>
    [Fact]
    public void SarifResults_ToMarkdown_MultipleRuns_IncludesRunIndices()
    {
        // Arrange
        var run1 = new SarifRun("Tool1", "1.0", [], 0);
        var run2 = new SarifRun("Tool2", "2.0", [], 0);
        var results = new SarifResults(new List<SarifRun> { run1, run2 });

        // Act
        var md = results.ToMarkdown(1);

        // Assert
        Assert.Contains("(#1)", md);
        Assert.Contains("(#2)", md);
    }

}

