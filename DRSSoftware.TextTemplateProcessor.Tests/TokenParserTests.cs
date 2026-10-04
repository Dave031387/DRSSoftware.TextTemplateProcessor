namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenParserTests
{
    // NOTE: The unit tests for the GetNextToken method have names that start with
    // "GetNextToken_Case" followed by a 1 to 3-digit number. The digits are the numbers 1 through
    // 4, where 1 represents a token start delimiter, 2 is an escaped token start delimiter, 3 is a
    // token end delimiter, and 4 is an escaped token end delimiter. The order of the numbers
    // indicates the order that the delimiters are found in the template text string.
    private static string EndDelimiter => DefaultTokenEndDelimiter;

    private static string EscapedEndDelimiter => $"{DefaultTokenEscapeCharacter}{DefaultTokenEndDelimiter}";
    private static string EscapedStartDelimiter => $"{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter}";
    private static string StartDelimiter => DefaultTokenStartDelimiter;

    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateTokenParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger? logger = null;
        string expected = GetNullDependencyMessage(nameof(TokenParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new TokenParser(logger!);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateTokenParserWithValidDependency_ShouldInitializeStateCorrectly()
    {
        // Arrange
        InitializeMocks();

        // Act
        TokenParser actual = GetTokenParser();

        // Assert
        actual
            .Should()
            .NotBeNull();
        actual.DelimiterEscapeCharacter
            .Should()
            .Be(DefaultTokenEscapeCharacter);
        actual.TokenEndDelimiter
            .Should()
            .NotBeNull()
            .And
            .Be(DefaultTokenEndDelimiter);
        actual.TokenStartDelimiter
            .Should()
            .NotBeNull()
            .And
            .Be(DefaultTokenStartDelimiter);
        actual.GetModifiedText()
            .Should()
            .NotBeNull()
            .And
            .BeEmpty();
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void EscapeLastFoundTokenWhenNoTokenFound_ShouldDoNothing()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = "text with no token strings";
        tokenParser.InitializeParser(expected, true);
        _ = tokenParser.GetNextToken();

        // Act
        tokenParser.EscapeLastFoundToken();

        // Assert
        tokenParser.GetModifiedText()
            .Should()
            .Be(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void EscapeLastFoundTokenWhenTokenWasFound_ShouldEscapeTheToken()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string prefixText = $"text before the token {StartDelimiter} FirstToken {EndDelimiter} ";
        string suffixText = " text after the token";
        string tokenName = "SecondToken";
        string tokenString = $"{StartDelimiter} {tokenName} {EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter} {tokenName} {EscapedEndDelimiter}";
        string originalText = $"{prefixText}{tokenString}{suffixText}";
        string expected = $"{prefixText}{escapedTokenString}{suffixText}";
        tokenParser.InitializeParser(originalText, true);
        _ = tokenParser.GetNextToken();
        _ = tokenParser.GetNextToken();

        // Act
        tokenParser.EscapeLastFoundToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        actual
            .Should()
            .Be(expected);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString)]
    [InlineData("text1", EmptyString)]
    [InlineData(EmptyString, "text1")]
    [InlineData("text1", "text2")]
    public void GetNextToken_Case1_ShouldEscapeStartDelimiter(string text1, string text2)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}";
        string message = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, true, false);
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString, EmptyString, EmptyString)]
    [InlineData("text1", EmptyString, EmptyString, EmptyString)]
    [InlineData(EmptyString, "text1", EmptyString, EmptyString)]
    [InlineData(EmptyString, EmptyString, "text1", EmptyString)]
    [InlineData(EmptyString, EmptyString, EmptyString, "text1")]
    [InlineData("text1", "text2", "text3", "text4")]
    public void GetNextToken_Case11_ShouldEscapeAllStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{StartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message = GetMessage(MsgMissingTokenEndDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, true, false);
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString, EmptyString, EmptyString)]
    [InlineData("text1", EmptyString, EmptyString, EmptyString)]
    [InlineData(EmptyString, "text1", EmptyString, EmptyString)]
    [InlineData(EmptyString, EmptyString, "text1", EmptyString)]
    [InlineData(EmptyString, EmptyString, EmptyString, "text1")]
    [InlineData("text1", "text2", "text3", "text4")]
    public void GetNextToken_Case12_ShouldEscapeAllStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedStartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message = GetMessage(MsgMissingTokenEndDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, true, false);
        VerifyMocks();
    }

    [Theory]
    [InlineData(Space, EmptyString, EmptyString, EmptyString, EmptyString)]
    [InlineData(SameCaseFlag, "text1", " ", EmptyString, "text2")]
    [InlineData(UppercaseFlag, EmptyString, EmptyString, " ", EmptyString)]
    [InlineData(LowercaseFlag, "text1", " ", " ", EmptyString)]
    [InlineData(Space, EmptyString, EmptyString, " ", "text1")]
    [InlineData(UppercaseFlag, EmptyString, " ", " ", EmptyString)]
    [InlineData(LowercaseFlag, "text1", EmptyString, EmptyString, "text2")]
    public void GetNextToken_Case13WithValidToken_ShouldReturnValidTokenInfo(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        char expectedCase = caseFlag == Space ? SameCaseFlag : caseFlag;
        string tokenName = "Token1";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{EndDelimiter}more text";
        string expected = $"{text1}{tokenString}";
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.TokenName
            .Should()
            .Be(tokenName);
        tokenInfo.TokenString
            .Should()
            .Be(tokenString);
        tokenInfo.Case
            .Should()
            .Be(expectedCase);
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, false, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString)]
    [InlineData("text1", EmptyString)]
    [InlineData(EmptyString, "text1")]
    [InlineData("text1", "text2")]
    public void GetNextToken_Case2_ShouldReturnUnchangedText(string text1, string text2)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedStartDelimiter}{text2}";
        tokenParser.InitializeParser(expected, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, false, false);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString, EmptyString)]
    [InlineData("text1", EmptyString, EmptyString)]
    [InlineData(EmptyString, "text1", EmptyString)]
    [InlineData("text1", "text2", EmptyString)]
    [InlineData(EmptyString, EmptyString, "text1")]
    [InlineData("text1", EmptyString, "text2")]
    [InlineData(EmptyString, "text1", "text2")]
    [InlineData("text1", "text2", "text3")]
    public void GetNextToken_Case3_ShouldEscapeAllEndDelimiters(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{EndDelimiter}{text3}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedEndDelimiter}{text3}";
        string message = GetMessage(MsgMissingTokenStartDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, true, false);
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString)]
    [InlineData("text1", EmptyString)]
    [InlineData(EmptyString, "text1")]
    [InlineData("text1", "text2")]
    public void GetNextToken_Case4_ShouldReturnUnchangedText(string text1, string text2)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedEndDelimiter}{text2}";
        tokenParser.InitializeParser(expected, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, false, false);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void GetNextTokenWhenTextContainsNoDelimiters_ShouldReturnEmptyTokenInfoAndUnchangedText()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = "string with no delimiters";
        tokenParser.InitializeParser(expected, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        tokenInfo.IsEmpty
            .Should()
            .BeTrue();
        actual
            .Should()
            .Be(expected);
        VerifyState(tokenParser, true, false, false);
        MocksVerifyNoOtherCalls();
    }

    private static void VerifyState(TokenParser tokenParser, bool endOfText, bool isTextModified, bool validTokenFound)
    {
        tokenParser.EndOfText
            .Should()
            .Be(endOfText);
        tokenParser.IsTextModified
            .Should()
            .Be(isTextModified);
        tokenParser.ValidTokenFound
            .Should()
            .Be(validTokenFound);
    }

    private TokenParser GetTokenParser() => new(LoggerMock.Object);

    private void InitializeMocks() => LoggerMock.Reset();

    private void MocksVerifyNoOtherCalls() => LoggerMock.VerifyNoOtherCalls();

    private void VerifyMocks()
    {
        if (LoggerMock.Setups.Any())
        {
            LoggerMock.VerifyAll();
        }

        MocksVerifyNoOtherCalls();
    }
}