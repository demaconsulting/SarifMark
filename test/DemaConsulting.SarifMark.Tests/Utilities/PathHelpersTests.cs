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
///     Unit tests for the PathHelpers class.
/// </summary>
[TestClass]
public class PathHelpersTests
{
    /// <summary>
    ///     Test that SafePathCombine throws ArgumentNullException for null base path.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            PathHelpers.SafePathCombine(null!, "file.txt"));
        Assert.AreEqual("basePath", exception.ParamName);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentNullException for null relative path.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            PathHelpers.SafePathCombine("/home/user", null!));
        Assert.AreEqual("relativePath", exception.ParamName);
    }

    /// <summary>
    ///     Test that SafePathCombine successfully combines valid paths.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_ValidPaths_CombinesSuccessfully()
    {
        // Arrange
        var basePath = "/home/user";
        var relativePath = "documents/file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.AreEqual(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for path with parent directory traversal.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_PathWithParentDirectory_ThrowsArgumentException()
    {
        // Arrange
        var basePath = "/home/user";
        var relativePath = "../etc/passwd";

        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentException>(() =>
            PathHelpers.SafePathCombine(basePath, relativePath));
        Assert.Contains("Invalid path component", exception.Message);
        Assert.AreEqual("relativePath", exception.ParamName);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for path with double dots in middle.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_PathWithDoubleDots_ThrowsArgumentException()
    {
        // Arrange
        var basePath = "/home/user";
        var relativePath = "documents/../../../etc/passwd";

        // Act & Assert
        var exception = Assert.ThrowsExactly<ArgumentException>(() =>
            PathHelpers.SafePathCombine(basePath, relativePath));
        Assert.Contains("Invalid path component", exception.Message);
        Assert.AreEqual("relativePath", exception.ParamName);
    }

    /// <summary>
    ///     Test that SafePathCombine accepts a filename that contains ".." as an embedded substring.
    ///     The post-combine check uses <see cref="Path.GetRelativePath"/> which treats ".." as an
    ///     escape segment only when it is the full result or is followed by a directory separator,
    ///     so names like "v1..0.sarif" are accepted as valid in-base filenames.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_FilenameWithEmbeddedDots_CombinesSuccessfully()
    {
        // Arrange - "v1..0.sarif" contains ".." but is not a path traversal
        var basePath = "/home/user";
        var relativePath = "v1..0.sarif";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.AreEqual(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for absolute paths.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException()
    {
        // Arrange
        var unixBasePath = "/home/user";
        var unixRelativePath = "/etc/passwd";

        // Act & Assert
        var unixException = Assert.ThrowsExactly<ArgumentException>(() =>
            PathHelpers.SafePathCombine(unixBasePath, unixRelativePath));
        Assert.Contains("Invalid path component", unixException.Message);
        Assert.AreEqual("relativePath", unixException.ParamName);

        // Windows absolute paths are only rooted on Windows
        if (OperatingSystem.IsWindows())
        {
            // Arrange
            var windowsBasePath = "C:\\Users\\User";
            var windowsRelativePath = "C:\\Windows\\System32";

            // Act & Assert
            var windowsException = Assert.ThrowsExactly<ArgumentException>(() =>
                PathHelpers.SafePathCombine(windowsBasePath, windowsRelativePath));
            Assert.Contains("Invalid path component", windowsException.Message);
            Assert.AreEqual("relativePath", windowsException.ParamName);
        }
    }

    /// <summary>
    ///     Test that SafePathCombine accepts simple filename.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_SimpleFilename_CombinesSuccessfully()
    {
        // Arrange
        var basePath = "/home/user/documents";
        var relativePath = "file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.AreEqual(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine accepts path with subdirectories.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_PathWithSubdirectories_CombinesSuccessfully()
    {
        // Arrange
        var basePath = "/home/user";
        var relativePath = "documents/work/report.pdf";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.AreEqual(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine accepts GUID-based filename.
    /// </summary>
    [TestMethod]
    public void PathHelpers_SafePathCombine_GuidBasedFilename_CombinesSuccessfully()
    {
        // Arrange
        var basePath = Path.GetTempPath();
        var guid = Guid.NewGuid();
        var relativePath = $"test-{guid}.tmp";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.AreEqual(Path.Combine(basePath, relativePath), result);
    }
}
