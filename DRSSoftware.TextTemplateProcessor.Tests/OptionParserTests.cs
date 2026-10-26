namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class OptionParserTests : MoqUnitTestBase
{
    private const string SegmentIdentifierString = SegmentName + " " + SegmentHeaderOptionGroup;
    private const string SegmentName = "Segment1";

    [Fact]
    public void CreateOptionParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(OptionParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new OptionParser(logger);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateOptionParserWithValidDependencies_ShouldSucceed()
    {
        // Act
        OptionParser actual = GetOptionParser();

        // Assert
        actual
            .Should()
            .NotBeNull();
    }

    [Theory]
    [InlineData(EndDelimiterOption)]
    [InlineData(StartDelimiterOption)]
    [InlineData(DelimiterEscapeOption)]
    [InlineData(TabSizeOption)]
    public void ParseFileHeaderOptionWhenOptionNameAndValueAreValid_ShouldReturnOptionNameAndValue(string optionName)
    {
        // Arrange
        string optionValue = "value";
        string optionString = $"{optionName}={optionValue}";
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseFileHeaderOption(parser, optionString);

        // Assert
        actual.IsEmpty
            .Should()
            .BeFalse();
        actual.Name
            .Should()
            .Be(optionName);
        actual.Value
            .Should()
            .Be(optionValue);
    }

    [Fact]
    public void ParseFileHeaderOptionWhenOptionNameIsInvalid_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"{PadSegmentNameOption}=optionValue";
        string message = GetMessage(MsgUnknownHeaderOptionFound, FileHeaderOptionGroup, optionString);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseFileHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderOptionWhenOptionNameIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"=optionValue";
        string message = GetMessage(MsgOptionNameMustPrecedeEqualsSign, FileHeaderOptionGroup);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseFileHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderOptionWhenOptionValueIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionName = EndDelimiterOption;
        string optionString = $"{optionName}=";
        string message = GetMessage(MsgOptionValueMustFollowEqualsSign, FileHeaderOptionGroup, optionName);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseFileHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderOptionWhenValueSeparatorIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"{DelimiterEscapeOption}missingSeparator";
        string message = GetMessage(MsgInvalidFormOfOption, FileHeaderOptionGroup, optionString);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseFileHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Theory]
    [InlineData(FirstTimeIndentOption)]
    [InlineData(PadSegmentNameOption)]
    [InlineData(TabSizeOption)]
    public void ParseSegmentHeaderOptionWhenOptionNameAndValueAreValid_ShouldReturnOptionNameAndValue(string optionName)
    {
        // Arrange
        string optionValue = "value";
        string optionString = $"{optionName}={optionValue}";
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseSegmentHeaderOption(parser, optionString);

        // Assert
        actual.IsEmpty
            .Should()
            .BeFalse();
        actual.Name
            .Should()
            .Be(optionName);
        actual.Value
            .Should()
            .Be(optionValue);
    }

    [Fact]
    public void ParseSegmentHeaderOptionWhenOptionNameIsInvalid_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"{DelimiterEscapeOption}=optionValue";
        string message = GetMessage(MsgUnknownHeaderOptionFound, SegmentIdentifierString, optionString);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseSegmentHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderOptionWhenOptionNameIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"=optionValue";
        string message = GetMessage(MsgOptionNameMustPrecedeEqualsSign, SegmentIdentifierString);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseSegmentHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderOptionWhenOptionValueIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionName = FirstTimeIndentOption;
        string optionString = $"{optionName}=";
        string message = GetMessage(MsgOptionValueMustFollowEqualsSign, SegmentIdentifierString, optionName);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseSegmentHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderOptionWhenValueSeparatorIsMissing_ShouldLogErrorMessage()
    {
        // Arrange
        string optionString = $"{DelimiterEscapeOption}missingSeparator";
        string message = GetMessage(MsgInvalidFormOfOption, SegmentIdentifierString, optionString);
        OptionParser parser = GetOptionParser();

        // Act
        Option actual = ParseSegmentHeaderOption(parser, optionString, message);

        // Assert
        actual.IsEmpty
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    private OptionParser GetOptionParser() => new(GetMock<ILogger>().Object);

    private Option ParseFileHeaderOption(OptionParser parser, string optionString, string? message = null)
    {
        // Arrange
        if (message is not null)
        {
            GetMock<ILogger>()
                .Setup(x => x.Log(LogSeverity.Error, message))
                .Verifiable(Times.Once);
        }

        // Act
        return parser.Parse(optionString, IsValidFileHeaderOption, FileHeaderOptionGroup, null);
    }

    private Option ParseSegmentHeaderOption(OptionParser parser, string optionString, string? message = null)
    {
        // Arrange
        if (message is not null)
        {
            GetMock<ILogger>()
                .Setup(x => x.Log(LogSeverity.Error, message))
                .Verifiable(Times.Once);
        }

        // Act
        return parser.Parse(optionString, IsValidSegmentHeaderOption, SegmentHeaderOptionGroup, SegmentName);
    }
}