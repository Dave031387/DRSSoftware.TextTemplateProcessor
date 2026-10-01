namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenParserTests
{
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
        MocksVerifyNoOtherCalls();
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
    }

    [Fact]
    public void EscapeLastFoundTokenWhenTokenWasFound_ShouldEscapeTheToken()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string prefixText = $"text before the token {DefaultTokenStartDelimiter} FirstToken {DefaultTokenEndDelimiter} ";
        string suffixText = " text after the token";
        string tokenName = "SecondToken";
        string originalText = $"{prefixText}{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}{suffixText}";
        string expected = $"{prefixText}{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEscapeCharacter}{DefaultTokenEndDelimiter}{suffixText}";
        tokenParser.InitializeParser(originalText, true);
        _ = tokenParser.GetNextToken();
        _ = tokenParser.GetNextToken();

        // Act
        tokenParser.EscapeLastFoundToken();

        // Assert
        tokenParser.GetModifiedText()
            .Should()
            .Be(expected);
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