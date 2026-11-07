namespace DRSSoftware.TextTemplateProcessor.Services;

[ExcludeFromCodeCoverage]
public class GeneratedFileServiceTests : MoqUnitTestBase
{
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
    public void CreatedGeneratedFileServiceWithValidDependencies_ShouldSucceedAndInitializeState()
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
    }

    private GeneratedFileService GetGeneratedFileService()
        => new(GetMock<IFileAndDirectoryService>().Object,
               GetMock<ILogger>().Object,
               GetMock<IPathValidator>().Object);
}