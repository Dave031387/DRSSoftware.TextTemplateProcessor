namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenProcessorTests
{
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

    private TokenProcessor GetTokenProcessor()
        => new(LocaterMock.Object, LoggerMock.Object, TokenParserMock.Object);

    private void InitializeMocks()
    {
        LocaterMock.Reset();
        LoggerMock.Reset();
        TokenParserMock.Reset();
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