namespace DRSSoftware.TextTemplateProcessor.Services;

[ExcludeFromCodeCoverage]
public class GeneratedFileServiceTests : MoqUnitTestBase
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AddText_ShouldAddTextToGeneratedText(int expected)
    {
        // Arrange
        GeneratedFileService generatedFileService = GetGeneratedFileService();

        // Act
        for (int i = 0; i < expected; i++)
        {
            string text = $"text {i}";
            generatedFileService.AddText(text);
        }

        // Assert
        generatedFileService.IsEmptyFile
            .Should()
            .BeFalse();
        generatedFileService.LineCount
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Clear_ShouldClearGeneratedTextLinesAndResetOutputFilePathInformation()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        string directoryPath = "directoryPath";
        string text = "text";
        int lineCount = 2;
        List<string> textLines = [text, text];
        string message1 = GetMessage(MsgWritingGeneratedTextFile, filePath);
        string message2 = GetMessage(MsgOutputFileSuccessfullyCreated, lineCount.ToString());
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message1))
            .Verifiable(Times.Once);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, false))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetDirectoryName(fullFilePath))
            .Returns(directoryPath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.FileExists(fullFilePath))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.CreateDirectory(directoryPath, null))
            .Returns(It.IsAny<string>())
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.WriteTextFile(fullFilePath, textLines))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message2))
            .Verifiable(Times.Once);
        GeneratedFileService generatedFileService = GetGeneratedFileService();
        generatedFileService.AddText(text);
        generatedFileService.AddText(text);
        _ = generatedFileService.WriteGeneratedText(filePath);

        // Act
        generatedFileService.Clear();

        // Assert
        generatedFileService.OutputDirectoryPath
            .Should()
            .BeEmpty();
        generatedFileService.OutputFileName
            .Should()
            .BeEmpty();
        generatedFileService.OutputFilePath
            .Should()
            .BeEmpty();
        generatedFileService.IsEmptyFile
            .Should()
            .BeTrue();
        generatedFileService.LineCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void CreateGeneratedFileServiceWhenFileAndDirectoryServiceIsNull_ShouldThrowException()
    {
        // Arrange
        IFileAndDirectoryService fileAndDirectoryService = null!;
        string expected = GetNullDependencyMessage(nameof(GeneratedFileService), nameof(IFileAndDirectoryService), nameof(fileAndDirectoryService));

        // Act
        void action() => _ = new GeneratedFileService(fileAndDirectoryService, GetMock<ILogger>().Object, GetMock<IPathValidator>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateGeneratedFileServiceWhenLoggerIsNull_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(GeneratedFileService), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new GeneratedFileService(GetMock<IFileAndDirectoryService>().Object, logger, GetMock<IPathValidator>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateGeneratedFileServiceWhenPathValidatorIsNull_ShouldThrowException()
    {
        // Arrange
        IPathValidator pathValidator = null!;
        string expected = GetNullDependencyMessage(nameof(GeneratedFileService), nameof(IPathValidator), nameof(pathValidator));

        // Act
        void action() => _ = new GeneratedFileService(GetMock<IFileAndDirectoryService>().Object, GetMock<ILogger>().Object, pathValidator);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateGeneratedFileServiceWithValidDependencies_ShouldSucceedAndInitializeState()
    {
        // Act
        GeneratedFileService generatedFileService = GetGeneratedFileService();

        // Assert
        generatedFileService
            .Should()
            .NotBeNull();
        generatedFileService.OutputDirectoryPath
            .Should()
            .BeEmpty();
        generatedFileService.OutputFileName
            .Should()
            .BeEmpty();
        generatedFileService.OutputFilePath
            .Should()
            .BeEmpty();
        generatedFileService.IsEmptyFile
            .Should()
            .BeTrue();
        generatedFileService.LineCount
            .Should()
            .Be(0);
    }

    [Fact]
    public void WriteGeneratedTextWhenFilePathIsInvalid_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string invalidFilePath = "invalid";
        string message1 = GetMessage(MsgWritingGeneratedTextFile, invalidFilePath);
        string message2 = GetMessage(MsgUnableToCreateOutputFile, GetMoqExceptionString<PathValidatorException>());
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message1))
            .Verifiable(Times.Once);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(invalidFilePath, true, false))
            .Throws<PathValidatorException>()
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, OperationType.Writing, message2))
            .Verifiable(Times.Once);
        GeneratedFileService generatedFileService = GetGeneratedFileService();

        // Act
        bool actual = generatedFileService.WriteGeneratedText(invalidFilePath);

        // Assert
        actual
            .Should()
            .BeFalse();
        generatedFileService.OutputDirectoryPath
            .Should()
            .BeEmpty();
        generatedFileService.OutputFileName
            .Should()
            .BeEmpty();
        generatedFileService.OutputFilePath
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void WriteGeneratedTextWhenGeneratedTextIsEmpty_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        string directoryPath = "directoryPath";
        string message1 = GetMessage(MsgWritingGeneratedTextFile, filePath);
        string message2 = GetMessage(MsgGeneratedTextIsEmpty);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message1))
            .Verifiable(Times.Once);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, false))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetDirectoryName(fullFilePath))
            .Returns(directoryPath)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, OperationType.Writing, message2))
            .Verifiable(Times.Once);
        GeneratedFileService generatedFileService = GetGeneratedFileService();

        // Act
        bool actual = generatedFileService.WriteGeneratedText(filePath);

        // Assert
        actual
            .Should()
            .BeFalse();
        generatedFileService.OutputDirectoryPath
            .Should()
            .Be(directoryPath);
        generatedFileService.OutputFileName
            .Should()
            .Be(fileName);
        generatedFileService.OutputFilePath
            .Should()
            .Be(fullFilePath);
        VerifyMocks();
    }

    [Fact]
    public void WriteGeneratedTextWhenOutputFileAlreadyExists_ShouldLogWarningAndWriteFileAndReturnTrue()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        string directoryPath = "directoryPath";
        string text = "text";
        int lineCount = 2;
        List<string> textLines = [text, text];
        string message1 = GetMessage(MsgWritingGeneratedTextFile, filePath);
        string message2 = GetMessage(MsgOutputFileWillBeOverwritten);
        string message3 = GetMessage(MsgOutputFileSuccessfullyCreated, lineCount.ToString());
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message1))
            .Verifiable(Times.Once);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, false))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetDirectoryName(fullFilePath))
            .Returns(directoryPath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.FileExists(fullFilePath))
            .Returns(true)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, OperationType.Writing, message2))
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.WriteTextFile(fullFilePath, textLines))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message3))
            .Verifiable(Times.Once);
        GeneratedFileService generatedFileService = GetGeneratedFileService();
        generatedFileService.AddText(text);
        generatedFileService.AddText(text);

        // Act
        bool actual = generatedFileService.WriteGeneratedText(filePath);

        // Assert
        actual
            .Should()
            .BeTrue();
        generatedFileService.OutputDirectoryPath
            .Should()
            .Be(directoryPath);
        generatedFileService.OutputFileName
            .Should()
            .Be(fileName);
        generatedFileService.OutputFilePath
            .Should()
            .Be(fullFilePath);
        generatedFileService.LineCount
            .Should()
            .Be(lineCount);
        VerifyMocks();
    }

    [Fact]
    public void WriteGeneratedTextWhenOutputFileDoesNotExist_ShouldCreateDirectoryAndWriteFileAndReturnTrue()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        string directoryPath = "directoryPath";
        string text = "text";
        int lineCount = 2;
        List<string> textLines = [text, text];
        string message1 = GetMessage(MsgWritingGeneratedTextFile, filePath);
        string message2 = GetMessage(MsgOutputFileSuccessfullyCreated, lineCount.ToString());
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message1))
            .Verifiable(Times.Once);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, false))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetDirectoryName(fullFilePath))
            .Returns(directoryPath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.FileExists(fullFilePath))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.CreateDirectory(directoryPath, null))
            .Returns(It.IsAny<string>())
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.WriteTextFile(fullFilePath, textLines))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Writing, message2))
            .Verifiable(Times.Once);
        GeneratedFileService generatedFileService = GetGeneratedFileService();
        generatedFileService.AddText(text);
        generatedFileService.AddText(text);

        // Act
        bool actual = generatedFileService.WriteGeneratedText(filePath);

        // Assert
        actual
            .Should()
            .BeTrue();
        generatedFileService.OutputDirectoryPath
            .Should()
            .Be(directoryPath);
        generatedFileService.OutputFileName
            .Should()
            .Be(fileName);
        generatedFileService.OutputFilePath
            .Should()
            .Be(fullFilePath);
        generatedFileService.LineCount
            .Should()
            .Be(lineCount);
        VerifyMocks();
    }

    private GeneratedFileService GetGeneratedFileService()
        => new(GetMock<IFileAndDirectoryService>().Object,
               GetMock<ILogger>().Object,
               GetMock<IPathValidator>().Object);
}