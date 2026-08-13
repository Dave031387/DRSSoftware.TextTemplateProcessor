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
        string path = NextAbsoluteName;

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
        string path = NextAbsoluteName;
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
        DeleteTestFiles(path);
    }

    [Fact]
    public void ClearDirectoryWhenDirectoryExistsAndIsEmpty_ShouldNotThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string path = NextAbsoluteName;
        CreateTestFiles(path, true);

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
        DeleteTestFiles(path);
    }

    [Fact]
    public void CombinePathsUsingAbsoluteDirectoryPath_ShouldReturnFullFilePath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string absolutePath = NextAbsoluteName;
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
        string relativePath = NextRelativeName;
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
        AssertException<FileAndDirectoryServiceException>((Action)action, expectedMessage);
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
        AssertException<FileAndDirectoryServiceException>((Action)action, expectedMessage);
    }
}