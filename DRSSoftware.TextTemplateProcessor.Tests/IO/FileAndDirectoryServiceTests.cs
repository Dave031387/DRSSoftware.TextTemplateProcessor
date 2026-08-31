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
        string expected = $"{absolutePath}{Path.DirectorySeparatorChar}{fileName}";

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
        string expected = $"{relativePath}{Path.DirectorySeparatorChar}{fileName}";

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

    [Fact]
    public void CreateDirectoryWhenDirectoryPathAndRootDirectoryAreBothAbsolutePaths_ShouldReturnDirectoryPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextAbsoluteDirectoryPath;
        string rootDirectory = NextAbsoluteDirectoryPath;
        string expected = directoryPath;

        // Act
        string actual = service.CreateDirectory(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
        Directory.Exists(directoryPath)
            .Should()
            .BeTrue();
        Directory.Exists(rootDirectory)
            .Should()
            .BeFalse();

        // Cleanup
        DeleteTestDirectory(directoryPath);
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
    public void CreateDirectoryWhenDirectoryPathAndRootDirectoryAreBothRelativePaths_ShouldReturnCombinedPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextRelativeDirectoryPath;
        string rootDirectory = NextRelativeDirectoryPath;
        string absoluteRootDirectory = Path.Combine(CurrentDirectory, rootDirectory);
        string expected = Path.Combine(absoluteRootDirectory, directoryPath);

        // Act
        string actual = service.CreateDirectory(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
        Directory.Exists(expected)
            .Should()
            .BeTrue();

        // Cleanup
        DeleteTestDirectory(absoluteRootDirectory);
    }

    [Fact]
    public void CreateDirectoryWhenDirectoryPathIsEmptyAndRootDirectoryIsAbsolutePath_ShouldReturnRootDirectory()
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
    public void CreateDirectoryWhenDirectoryPathIsEmptyAndRootDirectoryIsRelativePath_ShouldReturnRootDirectory()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string rootDirectory = NextRelativeDirectoryPath;
        string expected = Path.Combine(CurrentDirectory, rootDirectory);

        // Act
        string actual = service.CreateDirectory(EmptyString, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
        Directory.Exists(expected)
            .Should()
            .BeTrue();

        // Cleanup
        DeleteTestDirectory(expected);
    }

    [Fact]
    public void CreateDirectoryWhenDirectoryPathIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expectedInnerMessage = FormatMessage(MsgUnableToGetFullPathString, NullStringValue);
        string expectedOuterMessage = FormatMessage(MsgUnableToCreateDirectory, NullStringValue);

        // Act
        void action() => service.CreateDirectory(null!);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void CreateDirectoryWhenDirectoryPathIsRelativeAndRootDirectoryIsNull_ShouldReturnFullDirectoryPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextRelativeDirectoryPath;
        string expected = Path.Combine(CurrentDirectory, directoryPath);

        // Act
        string actual = service.CreateDirectory(directoryPath);

        // Assert
        actual
            .Should()
            .Be(expected);
        Directory.Exists(expected)
            .Should()
            .BeTrue();

        // Cleanup
        DeleteTestDirectory(expected);
    }

    [Theory]
    [MemberData(nameof(TestData.PathStrings), MemberType = typeof(TestData))]
    public void GetDirectoryName_ShouldReturnDirectoryName(string? path, string expected, string _)
    {
        // Arrange
        FileAndDirectoryService service = new();

        // Act
        string actual = service.GetDirectoryName(path!);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [MemberData(nameof(TestData.PathStrings), MemberType = typeof(TestData))]
    public void GetFileName_ShouldReturnFileName(string? path, string _, string expected)
    {
        // Arrange
        FileAndDirectoryService service = new();

        // Act
        string actual = service.GetFileName(path!);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFullPathWhenDirectoryPathAndRootDirectoryAreBothAbsolutePaths_ShouldReturnDirectoryPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextAbsoluteDirectoryPath;
        string rootDirectory = NextAbsoluteDirectoryPath;
        string expected = directoryPath;

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFullPathWhenDirectoryPathAndRootDirectoryAreBothRelativePaths_ShouldReturnCombinedPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextRelativeDirectoryPath;
        string rootDirectory = NextRelativeDirectoryPath;
        string absoluteRootDirectory = Path.Combine(CurrentDirectory, rootDirectory);
        string expected = Path.Combine(absoluteRootDirectory, directoryPath);

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(EmptyString, null)]
    [InlineData(EmptyString, EmptyString)]
    [InlineData(EmptyString, Whitespace)]
    [InlineData(Whitespace, null)]
    [InlineData(Whitespace, EmptyString)]
    [InlineData(Whitespace, Whitespace)]
    public void GetFullPathWhenDirectoryPathAndRootDirectoryAreNullOrWhitespace_ShouldReturnCurrentWorkingDirectory(string directoryPath, string? rootDirectory)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expected = CurrentDirectory;

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void GetFullPathWhenDirectoryPathIsAbsolutePathAndRootDirectoryIsNullOrWhitespace_ShouldReturnDirectoryPath(string? rootDirectory)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextAbsoluteDirectoryPath;
        string expected = directoryPath;

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void GetFullPathWhenDirectoryPathIsEmptyOrWhitespaceAndRootDirectoryIsAbsolutePath_ShouldReturnRootDirectory(string directoryPath)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string rootDirectory = NextAbsoluteDirectoryPath;
        string expected = rootDirectory;

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFullPathWhenDirectoryPathIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expectedInnerMessage = MsgNullDirectoryPath;
        string expectedOuterMessage = FormatMessage(MsgUnableToGetFullPathString, NullStringValue);

        // Act
        void action() => service.GetFullPath(null!);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void GetFullPathWhenDirectoryPathIsRelativeAndRootDirectoryIsAbsolutePath_ShouldReturnCombinedPath()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextRelativeDirectoryPath;
        string rootDirectory = NextAbsoluteDirectoryPath;
        string expected = Path.Combine(rootDirectory, directoryPath);

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void GetFullPathWhenDirectoryPathIsRelativeAndRootDirectoryIsNullOrWhitespace_ShouldReturnFullDirectoryPath(string? rootDirectory)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextRelativeDirectoryPath;
        string expected = Path.Combine(CurrentDirectory, directoryPath);

        // Act
        string actual = service.GetFullPath(directoryPath, rootDirectory);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFullPathWhenFilePathIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expectedInnerMessage = MsgNullFilePath;
        string expectedOuterMessage = FormatMessage(MsgUnableToGetFullPathString, NullStringValue);

        // Act
        void action() => service.GetFullPath(null!, null, true);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void GetSolutionDirectory_ShouldReturnSolutionDirectory()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expected = SolutionDirectory;

        // Act
        string actual = service.GetSolutionDirectory();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void ReadTextFileWhenFileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string filePath = NextAbsoluteFilePath;
        string expectedInnerMessage = FormatMessage(MsgFileNotFound, filePath);
        string expectedOuterMessage = FormatMessage(MsgUnableToReadTextFile, filePath);

        // Act
        void action() => service.ReadTextFile(filePath);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void ReadTextFileWhenFileExists_ShouldReturnFileContents()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextAbsoluteDirectoryPath;
        string[] expected = SampleText;
        string filePath = CreateTestFile(directoryPath, SampleText);

        // Act
        IEnumerable<string> actual = service.ReadTextFile(filePath);

        // Assert
        actual
            .Should()
            .BeEqualTo(expected);

        // Cleanup
        DeleteTestDirectory(directoryPath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void ReadTextFileWhenFilePathIsNullOrWhitespace_ShouldThrowException(string? filePath)
    {
        // Arrange
        FileAndDirectoryService service = new();
        string pathString = filePath is null ? NullStringValue : filePath;
        string expectedInnerMessage = FormatMessage(MsgFileNotFound, pathString);
        string expectedOuterMessage = FormatMessage(MsgUnableToReadTextFile, pathString);

        // Act
        void action() => service.ReadTextFile(filePath!);

        // Assert
        AssertException<FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void WriteTextFileUsingValidPathAndInputText_ShouldWriteFileSuccessfully()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string directoryPath = NextAbsoluteDirectoryPath;
        string filePath = Path.Combine(directoryPath, NextFileName);
        string[] expected = SampleText;
        CreateTestDirectory(directoryPath);

        // Act
        service.WriteTextFile(filePath, SampleText);

        // Assert
        File.Exists(filePath)
            .Should()
            .BeTrue();
        string[] actual = File.ReadAllLines(filePath);
        actual
            .Should()
            .BeEqualTo(expected);

        // Cleanup
        DeleteTestDirectory(directoryPath);
    }

    [Fact]
    public void WriteTextFileWhenFilePathIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string expectedInnerMessage = FormatMessage(ArgumentNullMessage, "filePath");
        string expectedOuterMessage = FormatMessage(MsgUnableToWriteToTextFile, NullStringValue);

        // Act
        void action() => service.WriteTextFile(null!, SampleText);

        // Assert
        AssertException<ArgumentNullException, FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }

    [Fact]
    public void WriteTextFileWhenInputTextIsNull_ShouldThrowException()
    {
        // Arrange
        FileAndDirectoryService service = new();
        string filePath = NextAbsoluteFilePath;
        string expectedOuterMessage = FormatMessage(MsgUnableToWriteToTextFile, filePath);
        string expectedInnerMessage = FormatMessage(ArgumentNullMessage, "textLines");

        // Act
        void action() => service.WriteTextFile(filePath, null!);

        // Assert
        AssertException<ArgumentNullException, FileAndDirectoryServiceException>(action, expectedInnerMessage, expectedOuterMessage);
    }
}