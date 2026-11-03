namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TemplateFileServiceTests : MoqUnitTestBase
{
    [Fact]
    public void CreateTemplateFileServiceWhenFileAndDirectoryServiceIsNull_ShouldThrowException()
    {
        // Arrange
        IFileAndDirectoryService fileAndDirectoryService = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateFileService), nameof(IFileAndDirectoryService), nameof(fileAndDirectoryService));

        // Act
        void action() => _ = new TemplateFileService(fileAndDirectoryService, GetMock<ILogger>().Object, GetMock<IPathValidator>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateFileServiceWhenLoggerIsNull_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateFileService), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new TemplateFileService(GetMock<IFileAndDirectoryService>().Object, logger, GetMock<IPathValidator>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateFileServiceWhenPathValidatorIsNull_ShouldThrowException()
    {
        // Arrange
        IPathValidator pathValidator = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateFileService), nameof(IPathValidator), nameof(pathValidator));

        // Act
        void action() => _ = new TemplateFileService(GetMock<IFileAndDirectoryService>().Object, GetMock<ILogger>().Object, pathValidator);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateFileServiceWithValidDependencies_ShouldSucceedAndInitializeState()
    {
        // Act
        TemplateFileService templateFileService = GetTemplateFileService();
        List<string> textLines = GetTemplateLines(templateFileService);

        // Assert
        templateFileService
            .Should()
            .NotBeNull();
        templateFileService.TemplateFileName
            .Should()
            .BeEmpty();
        templateFileService.TemplateFilePath
            .Should()
            .BeEmpty();
        templateFileService.IsEmptyFile
            .Should()
            .BeTrue();
        textLines.Count
            .Should()
            .Be(0);
    }

    [Fact]
    public void ReadTemplateFileMoreThanOnce_ShouldLogWarningMessageAndReadFileOnlyOnce()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        List<string> expected = ["Line 1", "Line 2", "Line 3"];
        string message1 = GetMessage(MsgAttemptingToReadFile, fileName);
        string message2 = GetMessage(MsgFileSuccessfullyRead, expected.Count.ToString());
        string message3 = GetMessage(MsgFileAlreadyLoaded, filePath);
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, true))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.ReadTextFile(fullFilePath))
            .Returns(expected)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message3))
            .Verifiable(Times.Once);
        TemplateFileService templateFileService = GetTemplateFileService();
        templateFileService.ReadTemplateFile(filePath);

        // Act
        templateFileService.ReadTemplateFile(filePath);
        List<string> actual = GetTemplateLines(templateFileService);

        // Assert
        templateFileService.TemplateFileName
            .Should()
            .Be(fileName);
        templateFileService.TemplateFilePath
            .Should()
            .Be(fullFilePath);
        templateFileService.IsEmptyFile
            .Should()
            .BeFalse();
        actual.Count
            .Should()
            .Be(expected.Count);
        actual
            .Should()
            .BeEqualTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReadTemplateFileWhenFileAndDirectoryServiceThrowsExceptionReadingFile_ShouldLogErrorMessage()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        string message1 = GetMessage(MsgAttemptingToReadFile, fileName);
        string message2 = GetMessage(MsgTemplateFileCouldNotBeRead, GetMoqExceptionString<FileAndDirectoryServiceException>());
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, true))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.ReadTextFile(fullFilePath))
            .Throws<FileAndDirectoryServiceException>()
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message2))
            .Verifiable(Times.Once);
        TemplateFileService templateFileService = GetTemplateFileService();

        // Act
        templateFileService.ReadTemplateFile(filePath);
        List<string> textLines = GetTemplateLines(templateFileService);

        // Assert
        templateFileService.TemplateFileName
            .Should()
            .Be(fileName);
        templateFileService.TemplateFilePath
            .Should()
            .Be(fullFilePath);
        templateFileService.IsEmptyFile
            .Should()
            .BeTrue();
        textLines.Count
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void ReadTemplateFileWhenFileAndDirectoryServiceThrowsExceptionValidatingFilePath_ShouldLogErrorMessage()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string message = GetMessage(MsgUnableToSetTemplateFilePath, filePath, GetMoqExceptionString<FileAndDirectoryServiceException>());
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, true))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Throws<FileAndDirectoryServiceException>()
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TemplateFileService templateFileService = GetTemplateFileService();

        // Act
        templateFileService.ReadTemplateFile(filePath);

        // Assert
        templateFileService.TemplateFileName
            .Should()
            .BeEmpty();
        templateFileService.TemplateFilePath
            .Should()
            .Be(fullFilePath);
        templateFileService.IsEmptyFile
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ReadTemplateFileWhenNoExceptionsThrown_ShouldReadFileContents()
    {
        // Arrange
        string filePath = "filePath";
        string fullFilePath = "fullFilePath";
        string fileName = "fileName";
        List<string> expected = ["Line 1", "Line 2", "Line 3"];
        string message1 = GetMessage(MsgAttemptingToReadFile, fileName);
        string message2 = GetMessage(MsgFileSuccessfullyRead, expected.Count.ToString());
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, true))
            .Returns(fullFilePath)
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.GetFileName(fullFilePath))
            .Returns(fileName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<IFileAndDirectoryService>()
            .Setup(x => x.ReadTextFile(fullFilePath))
            .Returns(expected)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        TemplateFileService templateFileService = GetTemplateFileService();

        // Act
        templateFileService.ReadTemplateFile(filePath);
        List<string> actual = GetTemplateLines(templateFileService);

        // Assert
        templateFileService.TemplateFileName
            .Should()
            .Be(fileName);
        templateFileService.TemplateFilePath
            .Should()
            .Be(fullFilePath);
        templateFileService.IsEmptyFile
            .Should()
            .BeFalse();
        actual.Count
            .Should()
            .Be(expected.Count);
        actual
            .Should()
            .BeEqualTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReadTemplateFileWhenPathValidatorThrowsException_ShouldLogErrorMessage()
    {
        // Arrange
        string filePath = "filePath";
        string message = GetMessage(MsgUnableToSetTemplateFilePath, filePath, GetMoqExceptionString<PathValidatorException>());
        GetMock<IPathValidator>()
            .Setup(x => x.ValidatePath(filePath, true, true))
            .Throws<PathValidatorException>()
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TemplateFileService templateFileService = GetTemplateFileService();

        // Act
        templateFileService.ReadTemplateFile(filePath);

        // Assert
        templateFileService.TemplateFileName
            .Should()
            .BeEmpty();
        templateFileService.TemplateFilePath
            .Should()
            .BeEmpty();
        templateFileService.IsEmptyFile
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    private static List<string> GetTemplateLines(TemplateFileService templateFileService)
    {
        List<string> lines = [];

        foreach (string line in templateFileService)
        {
            lines.Add(line);
        }

        return lines;
    }

    private TemplateFileService GetTemplateFileService()
        => new(GetMock<IFileAndDirectoryService>().Object,
               GetMock<ILogger>().Object,
               GetMock<IPathValidator>().Object);
}