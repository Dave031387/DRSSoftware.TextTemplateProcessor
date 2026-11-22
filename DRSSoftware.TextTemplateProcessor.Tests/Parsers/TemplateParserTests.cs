namespace DRSSoftware.TextTemplateProcessor.Parsers;

[ExcludeFromCodeCoverage]
public class TemplateParserTests : MoqUnitTestBase
{
    [Fact]
    public void CreateTemplateParserWithNullFileHeaderParser_ShouldThrowException()
    {
        // Arrange
        IFileHeaderParser fileHeaderParser = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(IFileHeaderParser), nameof(fileHeaderParser));

        // Act
        void action() =>_ = new TemplateParser(fileHeaderParser,
                                               GetMock<ILocater>().Object,
                                               GetMock<ILogger>().Object,
                                               GetMock<ISegmentHeaderParser>().Object,
                                               GetMock<ISegmentManager>().Object,
                                               GetMock<ITemplateFileService>().Object,
                                               GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullLocater_ShouldThrowException()
    {
        // Arrange
        ILocater locater = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ILocater), nameof(locater));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                locater,
                                                GetMock<ILogger>().Object,
                                                GetMock<ISegmentHeaderParser>().Object,
                                                GetMock<ISegmentManager>().Object,
                                                GetMock<ITemplateFileService>().Object,
                                                GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                GetMock<ILocater>().Object,
                                                logger,
                                                GetMock<ISegmentHeaderParser>().Object,
                                                GetMock<ISegmentManager>().Object,
                                                GetMock<ITemplateFileService>().Object,
                                                GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullSegmentHeaderParser_ShouldThrowException()
    {
        // Arrange
        ISegmentHeaderParser segmentHeaderParser = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ISegmentHeaderParser), nameof(segmentHeaderParser));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                GetMock<ILocater>().Object,
                                                GetMock<ILogger>().Object,
                                                segmentHeaderParser,
                                                GetMock<ISegmentManager>().Object,
                                                GetMock<ITemplateFileService>().Object,
                                                GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullSegmentManager_ShouldThrowException()
    {
        // Arrange
        ISegmentManager segmentManager = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ISegmentManager), nameof(segmentManager));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                GetMock<ILocater>().Object,
                                                GetMock<ILogger>().Object,
                                                GetMock<ISegmentHeaderParser>().Object,
                                                segmentManager,
                                                GetMock<ITemplateFileService>().Object,
                                                GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullTemplateFileService_ShouldThrowException()
    {
        // Arrange
        ITemplateFileService templateFileService = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ITemplateFileService), nameof(templateFileService));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                GetMock<ILocater>().Object,
                                                GetMock<ILogger>().Object,
                                                GetMock<ISegmentHeaderParser>().Object,
                                                GetMock<ISegmentManager>().Object,
                                                templateFileService,
                                                GetMock<ITextLineParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithNullTextLineParser_ShouldThrowException()
    {
        // Arrange
        ITextLineParser textLineParser = null!;
        string expected = GetNullDependencyMessage(nameof(TemplateParser), nameof(ITextLineParser), nameof(textLineParser));

        // Act
        void action() => _ = new TemplateParser(GetMock<IFileHeaderParser>().Object,
                                                GetMock<ILocater>().Object,
                                                GetMock<ILogger>().Object,
                                                GetMock<ISegmentHeaderParser>().Object,
                                                GetMock<ISegmentManager>().Object,
                                                GetMock<ITemplateFileService>().Object,
                                                textLineParser);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTemplateParserWithValidDependencies_ShouldSucceedAndInitializeState()
    {
        // Act
        TemplateParser parser = GetTemplateParser();

        // Assert
        parser
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Initialize_ShouldInitializeDependencies()
    {
        // Arrange
        SetupInitialize();
        TemplateParser parser = GetTemplateParser();

        // Act
        parser.Initialize();

        // Assert
        VerifyMocks();
    }

    private TemplateParser GetTemplateParser() => new(GetMock<IFileHeaderParser>().Object,
                                                      GetMock<ILocater>().Object,
                                                      GetMock<ILogger>().Object,
                                                      GetMock<ISegmentHeaderParser>().Object,
                                                      GetMock<ISegmentManager>().Object,
                                                      GetMock<ITemplateFileService>().Object,
                                                      GetMock<ITextLineParser>().Object);

    private void SetupInitialize()
    {
        GetMock<IFileHeaderParser>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        GetMock<ILocater>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        GetMock<ISegmentManager>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        GetMock<ITemplateFileService>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        GetMock<ITextLineParser>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
    }
}