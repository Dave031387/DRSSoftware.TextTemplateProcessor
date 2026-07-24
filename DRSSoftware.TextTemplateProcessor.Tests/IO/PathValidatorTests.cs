namespace DRSSoftware.TextTemplateProcessor.IO;

using System.IO;
using static DRSSoftware.TextTemplateProcessor.TestShared.TestFileHelper;

[ExcludeFromCodeCoverage]
public class PathValidatorTests
{
    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenDirectoryPathIsEmptyOrWhitespace_ShouldReturnFullFilePath(string whitespace)
    {
        // Arrange
        string fileName = NextFileName;
        string filePath = $"{whitespace}{Path.DirectorySeparatorChar}{fileName}";
        string fullFilePath = Path.Combine(CurrentDirectory, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        fullFilePath,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenFileDirectoryPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRoot}{Path.DirectorySeparatorChar}x{invalidChar}x{Path.DirectorySeparatorChar}{NextFileName}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidDirectoryCharacters,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenFileNameContainsInvalidFileNameCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{NextAbsoluteName}{Path.DirectorySeparatorChar}x{invalidChar}x.test";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidFileNameCharacters,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenFileNameIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Arrange
        string filePath = $"{NextAbsoluteName}{Path.DirectorySeparatorChar}{whitespace}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgMissingFileName,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenFilePathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        true,
                        false,
                        MsgFilePathIsEmptyOrWhitespace,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenOkayIfAbsoluteDirectoryPathNotFound_ShouldReturnAbsoluteDirectoryPath()
    {
        // Arrange
        string absolutePath = NextAbsoluteName;

        // Act/Assert
        AssertValidCall(absolutePath,
                        false,
                        false,
                        absolutePath,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenOkayIfAbsoluteFilePathNotFound_ShouldReturnAbsoluteFilePath()
    {
        // Arrange
        string filePath = NextAbsoluteFilePath;

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        filePath,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenOkayIfRelativeDirectoryPathNotFound_ShouldReturnDirectoryFilePath()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullFilePath = Path.Combine(CurrentDirectory, relativePath);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        false,
                        fullFilePath,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenOkayIfRelativeFilePathNotFound_ShouldReturnFullFilePath()
    {
        // Arrange
        string filePath = NextRelativeFilePath;
        string fullFilePath = Path.Combine(CurrentDirectory, filePath);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        fullFilePath,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRoot}{Path.DirectorySeparatorChar}x{invalidChar}x";

        // Act/Assert
        AssertException(filePath,
                        false,
                        false,
                        MsgInvalidDirectoryCharacters,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidateFullPathWhenPathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        false,
                        false,
                        MsgDirectoryPathIsEmptyOrWhitespace,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenPathIsNull_ShouldThrowException()
    {
        // Act/Assert
        AssertException(null,
                        false,
                        false,
                        MsgNullDirectoryPath,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredAbsoluteDirectoryPathExists_ShouldReturnAbsoluteDirectoryPath()
    {
        // Arrange
        string absolutePath = NextAbsoluteName;
        CreateTestFiles(absolutePath, true);

        // Act/Assert
        AssertValidCall(absolutePath,
                        false,
                        true,
                        absolutePath,
                        true);

        // Cleanup
        DeleteTestFiles(absolutePath);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredAbsoluteDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string absolutePath = NextAbsoluteName;
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, absolutePath);

        // Act/Assert
        AssertException(absolutePath,
                        false,
                        true,
                        expectedMessage,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredAbsoluteFilePathExists_ShouldReturnAbsoluteFilePath()
    {
        // Arrange
        string absolutePath = NextAbsoluteName;
        string fileName = CreateTestFiles(absolutePath);
        string filePath = Path.Combine(absolutePath, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        filePath,
                        true);

        // Cleanup
        DeleteTestFiles(absolutePath);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredAbsoluteFilePathNotFound_ShouldThrowException()
    {
        // Arrange
        string filePath = NextAbsoluteFilePath;
        string expectedMessage = FormatMessage(MsgFileNotFound, filePath);

        // Act/Assert
        AssertException(filePath,
                        true,
                        true,
                        expectedMessage,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredRelativeDirectoryPathExists_ShouldReturnFullDirectoryPath()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        CreateTestFiles(fullDirectoryPath, true);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        true,
                        fullDirectoryPath,
                        true);

        // Cleanup
        DeleteTestFiles(fullDirectoryPath);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredRelativeDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, fullDirectoryPath);

        // Act/Assert
        AssertException(relativePath,
                        false,
                        true,
                        expectedMessage,
                        true);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredRelativeFilePathExists_ShouldReturnFullFilePath()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string fileName = CreateTestFiles(fullDirectoryPath);
        string filePath = Path.Combine(relativePath, fileName);
        string fullFilePath = Path.Combine(CurrentDirectory, filePath);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        fullFilePath,
                        true);

        // Cleanup
        DeleteTestFiles(fullDirectoryPath);
    }

    [Fact]
    public void ValidateFullPathWhenRequiredRelativeFilePathNotFound_ShouldThrowException()
    {
        // Arrange
        string filePath = NextRelativeFilePath;
        string fullFilePath = $"{CurrentDirectory}{Path.DirectorySeparatorChar}{filePath}";
        string expectedMessage = FormatMessage(MsgFileNotFound, fullFilePath);

        // Act/Assert
        AssertException(filePath,
                        true,
                        true,
                        expectedMessage,
                        true);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenDirectoryPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRoot}{Path.DirectorySeparatorChar}x{invalidChar}x{Path.DirectorySeparatorChar}{NextFileName}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidDirectoryCharacters,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenDirectoryPathIsEmptyOrWhitespace_ShouldNotThrowException(string whitespace)
    {
        // Arrange
        string filePath = $"{whitespace}{Path.DirectorySeparatorChar}{NextFileName}";

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        false,
                        string.Empty,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenFileNameContainsInvalidFileNameCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{NextAbsoluteName}{Path.DirectorySeparatorChar}x{invalidChar}x.test";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgInvalidFileNameCharacters,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenFileNameIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Arrange
        string filePath = $"{NextAbsoluteName}{Path.DirectorySeparatorChar}{whitespace}";

        // Act/Assert
        AssertException(filePath,
                        true,
                        false,
                        MsgMissingFileName,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenFilePathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        true,
                        false,
                        MsgFilePathIsEmptyOrWhitespace,
                        false);
    }

    [Fact]
    public void ValidatePathWhenOkayIfAbsoluteDirectoryPathNotFound_ShouldNotThrowException()
    {
        // Act/Assert
        AssertValidCall(NextAbsoluteName,
                        false,
                        false,
                        string.Empty,
                        false);
    }

    [Fact]
    public void ValidatePathWhenOkayIfAbsoluteFilePathNotFound_ShouldNotThrowException()
    {
        // Act/Assert
        AssertValidCall(NextAbsoluteFilePath,
                        true,
                        false,
                        string.Empty,
                        false);
    }

    [Fact]
    public void ValidatePathWhenOkayIfRelativeDirectoryPathNotFound_ShouldNotThrowException()
    {
        // Act/Assert
        AssertValidCall(NextRelativeName,
                        false,
                        false,
                        string.Empty,
                        false);
    }

    [Fact]
    public void ValidatePathWhenOkayIfRelativeFilePathNotFound_ShouldNotThrowException()
    {
        // Act/Assert
        AssertValidCall(NextRelativeFilePath,
                        true,
                        false,
                        string.Empty,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
    public void ValidatePathWhenPathContainsInvalidPathCharacters_ShouldThrowException(string invalidChar)
    {
        // Arrange
        string filePath = $"{VolumeRoot}{Path.DirectorySeparatorChar}x{invalidChar}x";

        // Act/Assert
        AssertException(filePath,
                        false,
                        false,
                        MsgInvalidDirectoryCharacters,
                        false);
    }

    [Theory]
    [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
    public void ValidatePathWhenPathIsEmptyOrWhitespace_ShouldThrowException(string whitespace)
    {
        // Act/Assert
        AssertException(whitespace,
                        false,
                        false,
                        MsgDirectoryPathIsEmptyOrWhitespace,
                        false);
    }

    [Fact]
    public void ValidatePathWhenPathIsNull_ShouldThrowException()
    {
        // Act/Assert
        AssertException(null,
                        false,
                        false,
                        MsgNullDirectoryPath,
                        false);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteDirectoryPathExists_ShouldNotThrowException()
    {
        // Arrange
        string path = NextAbsoluteName;
        CreateTestFiles(path, true);

        // Act/Assert
        AssertValidCall(path,
                        false,
                        true,
                        string.Empty,
                        false);

        // Cleanup
        DeleteTestFiles(path);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string path = NextAbsoluteName;
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, path);

        // Act/Assert
        AssertException(path,
                        false,
                        true,
                        expectedMessage,
                        false);
    }

    [Fact]
    public void ValidatePathWhenRequiredAbsoluteFilePathExists_ShouldNotThrowException()
    {
        // Arrange
        string absolutePath = NextAbsoluteName;
        string fileName = CreateTestFiles(absolutePath);
        string filePath = Path.Combine(absolutePath, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        string.Empty,
                        false);

        // Cleanup
        DeleteTestFiles(absolutePath);
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
                        expectedMessage,
                        false);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeDirectoryPathExists_ShouldNotThrowException()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        CreateTestFiles(fullDirectoryPath, true);

        // Act/Assert
        AssertValidCall(relativePath,
                        false,
                        true,
                        string.Empty,
                        false);

        // Cleanup
        DeleteTestFiles(fullDirectoryPath);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeDirectoryPathNotFound_ShouldThrowException()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string expectedMessage = FormatMessage(MsgDirectoryNotFound, fullDirectoryPath);

        // Act/Assert
        AssertException(relativePath,
                        false,
                        true,
                        expectedMessage,
                        false);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeFilePathExists_ShouldNotThrowException()
    {
        // Arrange
        string relativePath = NextRelativeName;
        string fullDirectoryPath = Path.Combine(CurrentDirectory, relativePath);
        string fileName = CreateTestFiles(fullDirectoryPath);
        string filePath = Path.Combine(relativePath, fileName);

        // Act/Assert
        AssertValidCall(filePath,
                        true,
                        true,
                        string.Empty,
                        false);

        // Cleanup
        DeleteTestFiles(fullDirectoryPath);
    }

    [Fact]
    public void ValidatePathWhenRequiredRelativeFilePathNotFound_ShouldThrowException()
    {
        // Arrange
        string filePath = NextRelativeFilePath;
        string fullFilePath = Path.Combine(CurrentDirectory, filePath);
        string expectedMessage = FormatMessage(MsgFileNotFound, fullFilePath);

        // Act/Assert
        AssertException(filePath,
                        true,
                        true,
                        expectedMessage,
                        false);
    }

    private static void AssertException(string? path,
                                        bool isFilePath,
                                        bool shouldExist,
                                        string expectedMessage,
                                        bool validateFullPath)
    {
        // Arrange
        PathValidator pathValidator = new();
        Action action = validateFullPath
            ? (() => { pathValidator.ValidateFullPath(path!, isFilePath, shouldExist); })
            : (() => { pathValidator.ValidatePath(path!, isFilePath, shouldExist); });

        // Act/Assert
        AssertException<PathValidatorException>(action, expectedMessage);
    }

    private static void AssertValidCall(string path,
                                        bool isFilePath,
                                        bool shouldExist,
                                        string expected,
                                        bool validateFullPath)
    {
        // Arrange
        PathValidator pathValidator = new();

        // Act/Assert
        if (validateFullPath)
        {
            string actual = pathValidator.ValidateFullPath(path, isFilePath, shouldExist);
            actual
                .Should()
                .Be(expected);
        }
        else
        {
            Action action = () => { pathValidator.ValidatePath(path, isFilePath, shouldExist); };
            action
                .Should()
                .NotThrow();
        }
    }
}