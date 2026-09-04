namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class SegmentHeaderParserTests
{
    private Mock<IControlItemBuilder> ControlItemBuilderMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<IIndentProcessor> IndentProcessorMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateSegmentHeaderParserWithNullControlItemBuilder_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IControlItemBuilder controlItemBuilder = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IControlItemBuilder), nameof(controlItemBuilder));

        // Act
        Action action = () => _ = new SegmentHeaderParser(controlItemBuilder, IndentProcessorMock.Object, LoggerMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, indentProcessor, LoggerMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, IndentProcessorMock.Object, logger);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithValidDependencies_ShouldCreateInstance()
    {
        // Arrange
        InitializeMocks();

        // Act
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Assert
        parser
            .Should()
            .NotBeNull();
        MocksVerifyNoOtherCalls();
    }

    private SegmentHeaderParser GetSegmentHeaderParser()
        => new(ControlItemBuilderMock.Object, IndentProcessorMock.Object, LoggerMock.Object);

    private void InitializeMocks()
    {
        ControlItemBuilderMock.Reset();
        IndentProcessorMock.Reset();
        LoggerMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        ControlItemBuilderMock.VerifyNoOtherCalls();
        IndentProcessorMock.VerifyNoOtherCalls();
        LoggerMock.VerifyNoOtherCalls();
    }

    private void VerifyMocks()
    {
        if (ControlItemBuilderMock.Setups.Any())
        {
            ControlItemBuilderMock.VerifyAll();
        }

        if (IndentProcessorMock.Setups.Any())
        {
            IndentProcessorMock.VerifyAll();
        }

        if (LoggerMock.Setups.Any())
        {
            LoggerMock.VerifyAll();
        }

        MocksVerifyNoOtherCalls();
    }
}