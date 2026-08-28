using System.IO;
using static DRSSoftware.TextTemplateProcessor.TestShared.TestFileHelper;

namespace DRSSoftware.TextTemplateProcessor.IO;

[ExcludeFromCodeCoverage]
public class PathValidatorTests
{
    [Fact]
    public void ValidateDirectoryPathWhenOkayIfAbsoluteDirectoryPathNotFound_ShouldReturnAbsoluteDirectoryPath()
    {
        // Arrange
        string absolutePath = NextAbsoluteDirectoryPath;

        // Act/Assert
        AssertValidCall(absolutePath,
                        false,
                        false,
                        absolutePath);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidateDirectoryPathWhenPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
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
    public void ValidateDirectoryPathWhenPathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        false,
                        false,
                        MsgDirectoryPathIsEmptyOrWhitespace);
    }

    [Fact]
    public void ValidateDirectoryPathWhenPathIsNull_ShouldThrowException()
    {
        // Act/Assert
        AssertException(null,
                        false,
                        false,
                        MsgNullDirectoryPath);
    }

    [Fact]
    public void ValidateDirectoryPathWhenPathIsSingleCharacter_ShouldReturnDirectoryPath()
    {
        // Arrange
        string relativePath = "a";
        string fullFilePath = Path.Combine(CurrentDirectory, relativePath);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        false,
                        fullFilePath);
    }

    [Fact]
    public void ValidateDirectoryPathWhenPathIsUncPath_ShouldThrowException()
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
    public void ValidateDirectoryPathWhenRequiredAbsoluteDirectoryPathExists_ShouldReturnAbsoluteDirectoryPath()
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
    public void ValidateDirectoryPathWhenRequiredAbsoluteDirectoryPathNotFound_ShouldThrowException()
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
    public void ValidateDirectoryPathWhenRequiredRelativeDirectoryPathExists_ShouldReturnFullDirectoryPath()
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
    public void ValidateDirectoryPathWhenRequiredRelativeDirectoryPathNotFound_ShouldThrowException()
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

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidateFilePathWhenDirectoryPartContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRootPath}x{invalidChar}x{Path.DirectorySeparatorChar}{NextFileName}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidDirectoryCharacters);
    }

    [Fact]
    public void ValidateFilePathWhenDirectoryPartIsMissing_ShouldReturnFullFilePath()
    {
        // Arrange
        string fileName = NextFileName;
        string fullFilePath = Path.Combine(CurrentDirectory, fileName);

        // Act/Assert
        AssertValidCall(fileName,
                        true,
                        false,
                        fullFilePath);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFilePathWhenDirectoryPathIsEmptyOrWhitespace_ShouldReturnFullFilePath(string whitespace)
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
    [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
    public void ValidateFilePathWhenFileNameContainsInvalidFileNameCharacters_ShouldThrowException(string invalidChar)
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
    public void ValidateFilePathWhenFileNameIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Arrange
        string filePath = $"{NextAbsoluteDirectoryPath}{Path.DirectorySeparatorChar}{whitespace}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgMissingFileName);
    }

    [Fact]
    public void ValidateFilePathWhenOkayIfAbsoluteFilePathNotFound_ShouldReturnAbsoluteFilePath()
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
    public void ValidateFilePathWhenOkayIfRelativeFilePathNotFound_ShouldReturnFullFilePath()
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
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFilePathWhenPathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        true,
                        false,
                        MsgFilePathIsEmptyOrWhitespace);
    }

    [Fact]
    public void ValidateFilePathWhenPathIsNull_ShouldThrowException()
    {
        // Act/Assert
        AssertException(null,
                        true,
                        false,
                        MsgNullFilePath);
    }

    [Fact]
    public void ValidateFilePathWhenRequiredAbsoluteFilePathExists_ShouldReturnAbsoluteFilePath()
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
    public void ValidateFilePathWhenRequiredAbsoluteFilePathNotFound_ShouldThrowException()
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
    public void ValidateFilePathWhenRequiredRelativeFilePathExists_ShouldReturnFullFilePath()
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
    public void ValidateFilePathWhenRequiredRelativeFilePathNotFound_ShouldThrowException()
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

        // Act
        string actual = pathValidator.ValidatePath(path, isFilePath, shouldExist);

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}