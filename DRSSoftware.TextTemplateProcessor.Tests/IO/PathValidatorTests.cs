using System.IO;
using static DRSSoftware.TextTemplateProcessor.TestShared.TestFileHelper;

namespace DRSSoftware.TextTemplateProcessor.IO;

[ExcludeFromCodeCoverage]
public class PathValidatorTests
{
    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenDirectoryPathIsEmptyOrWhitespace_ShouldReturnFullFilePath(string whitespace)
    {
        // Arrange
        string fileName = NextFileName;
        string filePath = $"{whitespace}{Path.DirectorySeparatorChar}{fileName}";
        string fullFilePath = Path.Combine(CurrentDirectory, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        fullFilePath);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenFileDirectoryPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRootPath}x{invalidChar}x{Path.DirectorySeparatorChar}{NextFileName}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidDirectoryCharacters);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenFileNameContainsInvalidFileNameCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{NextAbsoluteDirectoryPath}{Path.DirectorySeparatorChar}x{invalidChar}x.test";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidFileNameCharacters);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenFileNameIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Arrange
        string filePath = $"{NextAbsoluteDirectoryPath}{Path.DirectorySeparatorChar}{whitespace}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgMissingFileName);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenFilePathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        true,
                        false,
                        MsgFilePathIsEmptyOrWhitespace);
    }

    [Fact]
    public void ValidatePathWhenOkayIfAbsoluteDirectoryPathNotFound_ShouldReturnAbsoluteDirectoryPath()
    {
        // Arrange
        string absolutePath = NextAbsoluteDirectoryPath;

        // Act/Assert
        AssertValidCall(absolutePath,
                        false,
                        false,
                        absolutePath);
    }

    [Fact]
    public void ValidatePathWhenOkayIfAbsoluteFilePathNotFound_ShouldReturnAbsoluteFilePath()
    {
        // Arrange
        string filePath = NextAbsoluteFilePath;

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        filePath);
    }

    [Fact]
    public void ValidatePathWhenOkayIfRelativeDirectoryPathNotFound_ShouldReturnDirectoryFilePath()
    {
        // Arrange
        string relativePath = NextRelativeDirectoryPath;
        string fullFilePath = Path.Combine(CurrentDirectory, relativePath);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        false,
                        fullFilePath);
    }

    [Fact]
    public void ValidatePathWhenOkayIfRelativeFilePathNotFound_ShouldReturnFullFilePath()
    {
        // Arrange
        string filePath = NextRelativeFilePath;
        string fullFilePath = Path.Combine(CurrentDirectory, filePath);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        fullFilePath);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRootPath}x{invalidChar}x";

        // Act/Assert
        AssertException(filePath,
                        false,
                        false,
                        MsgInvalidDirectoryCharacters);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenPathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        false,
                        false,
                        MsgDirectoryPathIsEmptyOrWhitespace);
    }

    [Fact]
    public void ValidatePathWhenPathIsNull_ShouldThrowException()
    {
        // Act/Assert
        AssertException(null,
                        false,
                        false,
                        MsgNullDirectoryPath);
    }

    [Fact]
    public void ValidatePathWhenPathIsUncPath_ShouldThrowException()
    {
        // Arrange
        string filePath = $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}server{Path.DirectorySeparatorChar}share";
        string expectedMessage = FormatMessage(MsgUncPathIsNotSupported, filePath);

        // Act/Assert
        AssertException(filePath,
                        false,
                        false,
                        expectedMessage);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteDirectoryPathExists_ShouldReturnAbsoluteDirectoryPath()
    {
        // Arrange
        string absolutePath = NextAbsoluteDirectoryPath;
        CreateTestDirectory(absolutePath);

        // Act/Assert
        AssertValidCall(absolutePath,
                        false,
                        true,
                        absolutePath);

        // Cleanup
        DeleteTestDirectory(absolutePath);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string absolutePath = NextAbsoluteDirectoryPath;
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, absolutePath);

        // Act/Assert
        AssertException(absolutePath,
                        false,
                        true,
                        expectedMessage);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteFilePathExists_ShouldReturnAbsoluteFilePath()
    {
        // Arrange
        string absolutePath = NextAbsoluteDirectoryPath;
        string fileName = CreateTestFile(absolutePath);
        string filePath = Path.Combine(absolutePath, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        filePath);

        // Cleanup
        DeleteTestDirectory(absolutePath);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteFilePathNotFound_ShouldThrowException()
    {
        // Arrange
        string filePath = NextAbsoluteFilePath;
        string expectedMessage = FormatMessage(MsgFileNotFound, filePath);

        // Act/Assert
        AssertException(filePath,
                        true,
                        true,
                        expectedMessage);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeDirectoryPathExists_ShouldReturnFullDirectoryPath()
    {
        // Arrange
        string relativePath = NextRelativeDirectoryPath;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        CreateTestDirectory(fullDirectoryPath);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        true,
                        fullDirectoryPath);

        // Cleanup
        DeleteTestDirectory(fullDirectoryPath);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string relativePath = NextRelativeDirectoryPath;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, fullDirectoryPath);

        // Act/Assert
        AssertException(relativePath,
                        false,
                        true,
                        expectedMessage);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeFilePathExists_ShouldReturnFullFilePath()
    {
        // Arrange
        string relativePath = NextRelativeDirectoryPath;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string fileName = CreateTestFile(fullDirectoryPath);
        string filePath = Path.Combine(relativePath, fileName);
        string fullFilePath = Path.Combine(CurrentDirectory, filePath);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        fullFilePath);

        // Cleanup
        DeleteTestDirectory(fullDirectoryPath);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeFilePathNotFound_ShouldThrowException()
    {
        // Arrange
        string filePath = NextRelativeFilePath;
        string fullFilePath = $"{CurrentDirectory}{Path.DirectorySeparatorChar}{filePath}";
        string expectedMessage = FormatMessage(MsgFileNotFound, fullFilePath);

        // Act/Assert
        AssertException(filePath,
                        true,
                        true,
                        expectedMessage);
    }

    private static void AssertException(string? path,
                                        bool isFilePath,
                                        bool shouldExist,
                                        string expectedMessage)
    {
        // Arrange
        PathValidator pathValidator = new();

        // Act
        void action() => pathValidator.ValidatePath(path!, isFilePath, shouldExist);

        // Assert
        AssertException<PathValidatorException>(action, expectedMessage);
    }

    private static void AssertValidCall(string path,
                                        bool isFilePath,
                                        bool shouldExist,
                                        string expected)
    {
        // Arrange
        PathValidator pathValidator = new();

        // Act/Assert
        string actual = pathValidator.ValidatePath(path, isFilePath, shouldExist);
        actual
            .Should()
            .Be(expected);
    }
}