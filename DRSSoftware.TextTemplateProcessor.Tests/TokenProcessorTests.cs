namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenProcessorTests
{
    private const char Space = ' ';

    private Mock<ILocater> LocaterMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<ITokenParser> TokenParserMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void ClearTokens_ShouldClearTokenDictionary()
    {
        // Arrange
        InitializeMocks();
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add("Token1", "Value1");
        tokenProcessor.TokenDictionary.Add("Token2", "Value2");

        // Act
        tokenProcessor.ClearTokens();

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateTokenProcessorWithNullLocater_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILocater? locater = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ILocater), nameof(locater));

        // Act
        Action action = () => _ = new TokenProcessor(locater!, LoggerMock.Object, TokenParserMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateTokenProcessorWithNullLogger_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILogger? logger = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new TokenProcessor(LocaterMock.Object, logger!, TokenParserMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateTokenProcessorWithValidDependencies_ShouldSucceedAndInitializeState()
    {
        // Arrange
        InitializeMocks();

        // Act
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Assert
        tokenProcessor
            .Should()
            .NotBeNull();
        tokenProcessor.TokenDictionary
            .Should()
            .NotBeNull()
            .And
            .BeEmpty();
        tokenProcessor.TokenStart
            .Should()
            .Be(DefaultTokenStartDelimiter);
        tokenProcessor.TokenEnd
            .Should()
            .Be(DefaultTokenEndDelimiter);
        tokenProcessor.TokenEscape
            .Should()
            .Be(DefaultTokenEscapeCharacter);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("text", "text", "text")]
    [InlineData("text", "text", EmptyString)]
    [InlineData("text", EmptyString, "text")]
    [InlineData("text", EmptyString, EmptyString)]
    [InlineData(EmptyString, "text", "text")]
    [InlineData(EmptyString, "text", EmptyString)]
    [InlineData(EmptyString, EmptyString, "text")]
    [InlineData(EmptyString, EmptyString, EmptyString)]
    public void ExtractTokensWhenTextContainsEscapedTokens_ShouldIgnoreEscapedTokens(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenProcessor tokenProcessor = GetTokenProcessor();
        string name1 = "Token1";
        string name2 = "Token2";
        string token1 = CreateToken(name1, Space, true);
        string token2 = CreateToken(name2, Space, true);
        string expectedText = $"{text1}{token1}{text2}{token2}{text3}";
        string actual = expectedText;

        // Act
        tokenProcessor.ExtractTokens(ref actual);

        // Assert
        actual
            .Should()
            .Be(expectedText);
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void ExtractTokensWhenTextContainsNoTokens_ShouldDoNothing()
    {
        // Arrange
        InitializeMocks();
        TokenProcessor tokenProcessor = GetTokenProcessor();
        string expected = "This is a sample text without any tokens.";
        string actual = expected;

        // Act
        tokenProcessor.ExtractTokens(ref actual);

        // Assert
        actual
            .Should()
            .Be(expected);
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("text", "text", "text")]
    [InlineData("text", "text", EmptyString)]
    [InlineData("text", EmptyString, "text")]
    [InlineData("text", EmptyString, EmptyString)]
    [InlineData(EmptyString, "text", "text")]
    [InlineData(EmptyString, "text", EmptyString)]
    [InlineData(EmptyString, EmptyString, "text")]
    [InlineData(EmptyString, EmptyString, EmptyString)]
    public void ExtractTokensWhenTextContainsTokens_ShouldExtractTokensAndAddThemToDictionary(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenProcessor tokenProcessor = GetTokenProcessor();
        string name1 = "Token1";
        string name2 = "Token2";
        string token1 = CreateToken(name1);
        string token2 = CreateToken(name2);
        string expectedText = $"{text1}{token1}{text2}{token2}{text3}";
        Dictionary<string, string> expectedTokens = new()
        {
            { name1, string.Empty },
            { name2, string.Empty }
        };
        string actual = expectedText;

        // Act
        tokenProcessor.ExtractTokens(ref actual);

        // Assert
        actual
            .Should()
            .Be(expectedText);
        tokenProcessor.TokenDictionary
            .Should()
            .HaveCount(expectedTokens.Count)
            .And
            .Contain(expectedTokens);
        MocksVerifyNoOtherCalls();
    }

    private static string CreateToken(string tokenName, char initialCaseFlag = Space, bool isEscaped = false)
    {
        return isEscaped
            ? initialCaseFlag == Space
                ? $"{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}"
            : initialCaseFlag == Space
                ? $"{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}";
    }

    private TokenProcessor GetTokenProcessor()
        => new(LocaterMock.Object, LoggerMock.Object, TokenParserMock.Object);

    private void InitializeMocks()
    {
        LocaterMock.Reset();
        LoggerMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        LocaterMock.VerifyNoOtherCalls();
        LoggerMock.VerifyNoOtherCalls();
        TokenParserMock.VerifyNoOtherCalls();
    }

    private void VerifyMocks()
    {
        if (LocaterMock.Setups.Any())
        {
            LocaterMock.VerifyAll();
        }

        if (LoggerMock.Setups.Any())
        {
            LoggerMock.VerifyAll();
        }

        if (TokenParserMock.Setups.Any())
        {
            TokenParserMock.VerifyAll();
        }

        MocksVerifyNoOtherCalls();
    }
}