namespace DRSSoftware.TextTemplateProcessor.Parsers;

[ExcludeFromCodeCoverage]
public class TextLineParserTests : MoqUnitTestBase
{
    [Fact]
    public void CreateTextLineParserWithNullTokenExtractor_ShouldThrowException()
    {
        // Arrange
        ITokenExtractor tokenExtractor = null!;
        string expected = GetNullDependencyMessage(nameof(TextLineParser), nameof(ITokenExtractor), nameof(tokenExtractor));

        // Act
        void action() => _ = new TextLineParser(tokenExtractor);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateTextLineParserWithValidDependencies_ShouldSucceed()
    {
        // Act
        TextLineParser actual = GetTextLineParser();

        // Assert
        actual
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void ParseTextLineHavingAbsoluteIndentButNoText_ShouldReturnEmptyTextWithCorrectIndent()
    {
        // Arrange
        int indent = 3;
        string textLine = $"{NormalIndentString}{AbsoluteIndexString}{indent} ";
        TextItem expected = new(indent, false, false, EmptyString);
        TextLineParser parser = GetTextLineParser();

        // Act
        TextItem actual = parser.Parse(textLine);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void ParseTextLineHavingNoControlCodeAndNoTokens_ShouldReturnUnchangedTextLineWithZeroIndent()
    {
        // Arrange
        string text = "some text";
        string textLine = $"{NoControlCode} {text}";
        TextItem expected = new(0, true, false, text);
        GetMock<ITokenExtractor>()
            .Setup(x => x.ExtractTokens(text))
            .Returns(text)
            .Verifiable(Times.Once);
        TextLineParser parser = GetTextLineParser();

        // Act
        TextItem actual = parser.Parse(textLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseTextLineHavingTokensAndNegativeOneTimeIndent_ShouldReturnUpdatedTextWithCorrectIndent()
    {
        // Arrange
        string text = "some text with tokens";
        string updatedText = "updated text";
        int indent = -2;
        string textLine = $"{OneTimeIndentString}{RelativeLeftIndentString}{-indent} {text}";
        TextItem expected = new(indent, true, true, updatedText);
        GetMock<ITokenExtractor>()
            .Setup(x => x.ExtractTokens(text))
            .Returns(updatedText)
            .Verifiable(Times.Once);
        TextLineParser parser = GetTextLineParser();

        // Act
        TextItem actual = parser.Parse(textLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    private TextLineParser GetTextLineParser()
        => new(GetMock<ITokenExtractor>().Object);
}