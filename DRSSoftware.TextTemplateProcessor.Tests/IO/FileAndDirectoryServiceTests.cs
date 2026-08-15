using System.IO;
using static DRSSoftware.TextTemplateProcessor.TestShared.TestFileHelper;

namespace DRSSoftware.TextTemplateProcessor.IO;

[ExcludeFromCodeCoverage]
public class FileAndDirectoryServiceTests
{
    [Fact]
    public void ClearDirectoryWhenDirectoryDoesNotExist_ShouldNotThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string path = NextAbsoluteDirectoryPath;

        // Act
        Action action = () => service.ClearDirectory(path);

        // Assert
        action
            .Should()
            .NotThrow();
        Directory.Exists(path)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ClearDirectoryWhenDirectoryExistsAndContainsFiles_ShouldDeleteFiles()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string path = NextAbsoluteDirectoryPath;
        CreateTestFiles(path, 3);

        // Act
        service.ClearDirectory(path);

        // Assert
        Directory.Exists(path)
            .Should()
            .BeTrue();
        Directory.GetFiles(path)
            .Should()
            .BeEmpty();

        // Cleanup
        DeleteTestDirectory(path);
    }

    [Fact]
    public void ClearDirectoryWhenDirectoryExistsAndIsEmpty_ShouldNotThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string path = NextAbsoluteDirectoryPath;
        CreateTestDirectory(path);

        // Act
        Action action = () => service.ClearDirectory(TemplateDirectoryPath);

        // Assert
        action
            .Should()
            .NotThrow();
        Directory.Exists(path)
            .Should()
            .BeTrue();

        // Cleanup
        DeleteTestDirectory(path);
    }

    [Fact]
    public void CombinePathsUsingAbsoluteDirectoryPath_ShouldReturnFullFilePath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string absolutePath = NextAbsoluteDirectoryPath;
        string fileName = NextFileName;
        string expected = $@"{absolutePath}{Path.DirectorySeparatorChar}{fileName}";

        // Act
        string actual = service.CombinePaths(absolutePath, fileName);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void CombinePathsUsingRelativeDirectoryPath_ShouldReturnRelativeFilePath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string relativePath = NextRelativeDirectoryPath;
        string fileName = NextFileName;
        string expected = $@"{relativePath}{Path.DirectorySeparatorChar}{fileName}";

        // Act
        string actual = service.CombinePaths(relativePath, fileName);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void CombinePathsWhenFirstArgumentIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string fileName = NextFileName;
        string expectedMessage = MsgCombinePathsArgument1IsNull;

        // Act
        void action() => service.CombinePaths(null!, fileName);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedMessage);
    }

    [Fact]
    public void CombinePathsWhenSecondArgumentIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string fileName = NextFileName;
        string expectedMessage = MsgCombinePathsArgument2IsNull;

        // Act
        void action() => service.CombinePaths(fileName, null!);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedMessage);
    }

    [Theory]
    [InlineData(EmptyString, EmptyString)]
    [InlineData(Whitespace, Whitespace)]
    [InlineData(EmptyString, Whitespace)]
    [InlineData(Whitespace, EmptyString)]
    public void CreateDirectoryWhenDirectoryPathAndRootDirectoryAreBothEmptyOrWhitespace_ShouldReturnCurrentWorkingDirectory(string directoryPath, string rootDirectory)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expected = CurrentDirectory;

        // Act
        string actual = service.CreateDirectory(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void CreateDirectoryWhenDirectoryPathIsEmptyAndRootDirectoryIsNotEmpty_ShouldReturnRootDirectory()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string rootDirectory = NextAbsoluteDirectoryPath;
        string expected = rootDirectory;

        // Act
        string actual = service.CreateDirectory(EmptyString, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
        Directory.Exists(rootDirectory)
            .Should()
            .BeTrue();

        // Cleanup
        DeleteTestDirectory(rootDirectory);
    }

    [Fact]
    public void CreateDirectoryWhenDirectoryPathIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expectedInnerMessage = MsgNullDirectoryPath;
        string expectedOuterMessage = FormatMessage(MsgUnableToCreateDirectory, string.Empty);

        // Act
        void action() => service.CreateDirectory(null!);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }
}