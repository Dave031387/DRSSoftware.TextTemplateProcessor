namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenProcessorTests : MoqUnitTestBase
{
    [Fact]
    public void ClearTokens_ShouldClearTokenDictionary()
    {
        // Arrange
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add("Token1", "Value1");
        tokenProcessor.TokenDictionary.Add("Token2", "Value2");

        // Act
        tokenProcessor.ClearTokens();

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void CreateTokenProcessorWithNullDelimiterService_ShouldThrowException()
    {
        // Arrange
        IDelimiterService? delimiterService = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(IDelimiterService), nameof(delimiterService));

        // Act
        Action action = () => _ = new TokenProcessor(GetMock<ILocater>().Object, GetMock<ILogger>().Object, delimiterService!, GetMock<ITokenParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateTokenProcessorWithNullLocater_ShouldThrowException()
    {
        // Arrange
        ILocater? locater = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ILocater), nameof(locater));

        // Act
        Action action = () => _ = new TokenProcessor(locater!, GetMock<ILogger>().Object, GetMock<IDelimiterService>().Object, GetMock<ITokenParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateTokenProcessorWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger? logger = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new TokenProcessor(GetMock<ILocater>().Object, logger!, GetMock<IDelimiterService>().Object, GetMock<ITokenParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateTokenProcessorWithNullTokenParser_ShouldThrowException()
    {
        // Arrange
        ITokenParser? tokenParser = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ITokenParser), nameof(tokenParser));

        // Act
        Action action = () => _ = new TokenProcessor(GetMock<ILocater>().Object, GetMock<ILogger>().Object, GetMock<IDelimiterService>().Object, tokenParser!);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateTokenProcessorWithValidDependencies_ShouldSucceedAndInitializeState()
    {
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
    }

    [Fact]
    public void ExtractTokensWhenTextTemplateLineContainsInvalidTokenDelimiters_ShouldReturnModifiedTextTemplateLine()
    {
        // Arrange
        string original = "Text containing invalid delimiters.";
        string expected = "Modified text template line.";
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(original, true))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.IsTextModified)
            .Returns(true)
            .Verifiable(Times.Once);
        tokenParserMock
            .Setup(x => x.GetModifiedText())
            .Returns(expected)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ExtractTokens(original);

        // Assert
        actual
            .Should()
            .Be(expected);
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void ExtractTokensWhenTextTemplateLineContainsNoTokens_ShouldReturnUnmodifiedTextTemplateLine()
    {
        // Arrange
        string expected = "Text template line with no tokens";
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(expected, true))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.IsTextModified)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ExtractTokens(expected);

        // Assert
        actual
            .Should()
            .Be(expected);
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void ExtractTokensWhenTextTemplateLineContainsValidTokens_ShouldAddTokenNamesToTokenDictionary()
    {
        // Arrange
        string expectedText = "Text template line containing three tokens";
        string tokenName1 = "TokenName1";
        string tokenName2 = "TokenName2";
        TokenInfo tokenInfo1A = new("TokenString1A", tokenName1, SameCaseFlag);
        TokenInfo tokenInfo1B = new("TokenString1B", tokenName1, UppercaseFlag);
        TokenInfo tokenInfo2 = new("TokenString2", tokenName2, LowercaseFlag);
        Dictionary<string, string> expectedDictionary = new()
        {
            { tokenName1, EmptyString },
            { tokenName2, EmptyString }
        };
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(expectedText, true))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .SetupSequence(x => x.GetNextToken())
            .Returns(tokenInfo1A)
            .Returns(tokenInfo2)
            .Returns(tokenInfo1B)
            .Returns(TokenInfo.Empty);
        tokenParserMock
            .SetupSequence(x => x.ValidTokenFound)
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(false);
        tokenParserMock
            .SetupGet(x => x.IsTextModified)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ExtractTokens(expectedText);

        // Assert
        actual
            .Should()
            .Be(expectedText);
        tokenProcessor.TokenDictionary
            .Should()
            .Contain(expectedDictionary);
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesUsingEmptyDictionary_ShouldLogWarningMessage()
    {
        // Arrange
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryIsEmpty, locationName);
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        tokenProcessor.LoadTokenValues([]);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesUsingNullDictionary_ShouldLogErrorMessage()
    {
        // Arrange
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryIsNull, locationName);
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        tokenProcessor.LoadTokenValues(null!);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesWhenAllTokensAreValid_ShouldLoadTokenValues()
    {
        // Arrange
        string tokenName1 = "token1";
        string tokenName2 = "token2";
        string tokenName3 = "token3";
        string newValue1 = "value1";
        string newValue3 = "value3";
        Dictionary<string, string> dictionary = new()
        {
            { tokenName1, newValue1 },
            { tokenName3, newValue3 }
        };
        Dictionary<string, string> expected = new()
        {
            { tokenName1, newValue1 },
            { tokenName2, EmptyString },
            { tokenName3, newValue3 }
        };
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(tokenName1, EmptyString);
        tokenProcessor.TokenDictionary.Add(tokenName2, EmptyString);
        tokenProcessor.TokenDictionary.Add(tokenName3, EmptyString);

        // Act
        tokenProcessor.LoadTokenValues(dictionary);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .Contain(expected);
    }

    [Fact]
    public void LoadTokenValuesWhenTokenNameIsInvalid_ShouldLogErrorMessage()
    {
        // Arrange
        string tokenName = "1token";
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryContainsInvalidTokenName, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, EmptyString }
        };
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        tokenProcessor.LoadTokenValues(dictionary);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesWhenTokenNameIsUnknown_ShouldLogWarningMessage()
    {
        // Arrange
        string unknownTokenName = "unknownToken";
        string unknownTokenValue = "unknown value";
        string knownTokenName = "knownToken";
        string knownTokenValue = "known value";
        string locationName = "location";
        string message = GetMessage(MsgUnknownTokenName, locationName, unknownTokenName);
        Dictionary<string, string> dictionary = new()
        {
            { unknownTokenName, unknownTokenValue }
        };
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(knownTokenName, knownTokenValue);

        // Act
        tokenProcessor.LoadTokenValues(dictionary);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .Contain(knownTokenName, knownTokenValue);
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesWhenTokenValueIsEmpty_ShouldLogWarningMessage()
    {
        // Arrange
        string tokenName = "token";
        string originalValue = "value";
        string locationName = "location";
        string message = GetMessage(MsgTokenWithEmptyValue, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, EmptyString }
        };
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(tokenName, originalValue);

        // Act
        tokenProcessor.LoadTokenValues(dictionary);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .Contain(tokenName, EmptyString);
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesWhenTokenValueIsNull_ShouldLogErrorMessage()
    {
        // Arrange
        string tokenName = "token";
        string originalValue = "value";
        string locationName = "location";
        string message = GetMessage(MsgTokenWithNullValue, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, null! }
        };
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(tokenName, originalValue);

        // Act
        tokenProcessor.LoadTokenValues(dictionary);

        // Assert
        tokenProcessor.TokenDictionary
            .Should()
            .Contain(tokenName, EmptyString);
        VerifyMocks();
    }

    [Fact]
    public void ReplaceTokens_ShouldRemoveEscapeCharacterFromEscapedDelimiters()
    {
        // Arrange
        string original = $"{DefaultDelimiterEscapeCharacter}{DefaultTokenEndDelimiter}text{DefaultDelimiterEscapeCharacter}{DefaultDelimiterEscapeCharacter}{DefaultTokenStartDelimiter}text";
        string expected = $"{DefaultTokenEndDelimiter}text{DefaultDelimiterEscapeCharacter}{DefaultTokenStartDelimiter}text";
        SetupDelimiterServiceMock();
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ReplaceTokens(original);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReplaceTokensWhenTextContainsNoTokens_ShouldReturnUnmodifiedText()
    {
        // Arrange
        string expected = "template text with no tokens";
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        SetupDelimiterServiceMock();
        tokenParserMock
            .Setup(x => x.InitializeParser(expected, false))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ReplaceTokens(expected);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReplaceTokensWhenTokenNotFoundInTokenDictionary_ShouldLogErrorMessage()
    {
        // Arrange
        string tokenName = "token";
        string tokenString = $"{DefaultTokenStartDelimiter}{tokenName}{DefaultTokenEndDelimiter}";
        TokenInfo tokenInfo = new(tokenString, tokenName, SameCaseFlag);
        string locationName = "location";
        string message = GetMessage(MsgTokenNameNotFound, locationName, tokenName);
        string expected = $"text {tokenString} more text";
        SetupDelimiterServiceMock();
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(expected, false))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(tokenInfo)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(true)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        string actual = tokenProcessor.ReplaceTokens(expected);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReplaceTokensWhenTokenValueIsEmpty_ShouldLogWarningMessage()
    {
        // Arrange
        string tokenName = "token";
        string tokenString = $"{DefaultTokenStartDelimiter}{tokenName}{DefaultTokenEndDelimiter}";
        TokenInfo tokenInfo = new(tokenString, tokenName, SameCaseFlag);
        string original = $"text {tokenString} more text";
        string expected = $"text  more text";
        string locationName = "location";
        string message = GetMessage(MsgTokenValueIsEmpty, locationName, tokenName);
        SetupDelimiterServiceMock();
        GetMock<ILocater>()
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(tokenInfo)
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(true)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(tokenName, EmptyString);

        // Act
        string actual = tokenProcessor.ReplaceTokens(original);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ReplaceTokenValuesWhenTokenValuesAreNotEmpty_ShouldReplaceTokensWithValues()
    {
        // Arrange
        string tokenName1 = "token1";
        string tokenName2 = "token2";
        string tokenString1A = $"{DefaultTokenStartDelimiter}{UppercaseFlag}{tokenName1}{DefaultTokenEndDelimiter}";
        string tokenString1B = $"{DefaultTokenStartDelimiter} {tokenName1}  {DefaultTokenEndDelimiter}";
        string tokenString2 = $"{DefaultTokenStartDelimiter}{LowercaseFlag} {tokenName2}{DefaultTokenEndDelimiter}";
        string tokenValue1 = "value1";
        string tokenValue2 = "X";
        string tokenValue1Upper = "Value1";
        string tokenValue2Lower = "x";
        TokenInfo tokenInfo1A = new(tokenString1A, tokenName1, UppercaseFlag);
        TokenInfo tokenInfo1B = new(tokenString1B, tokenName1, SameCaseFlag);
        TokenInfo tokenInfo2 = new(tokenString2, tokenName2, LowercaseFlag);
        string original = $"text{tokenString1A}text {tokenString2} text {tokenString1B} text";
        string expected = $"text{tokenValue1Upper}text {tokenValue2Lower} text {tokenValue1} text";
        SetupDelimiterServiceMock();
        Mock<ITokenParser> tokenParserMock = GetMock<ITokenParser>();
        tokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        tokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(true);
        tokenParserMock
            .SetupSequence(x => x.GetNextToken())
            .Returns(tokenInfo1A)
            .Returns(tokenInfo2)
            .Returns(tokenInfo1B);
        tokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(true)
            .Verifiable(Times.Exactly(3));
        TokenProcessor tokenProcessor = GetTokenProcessor();
        tokenProcessor.TokenDictionary.Add(tokenName1, tokenValue1);
        tokenProcessor.TokenDictionary.Add(tokenName2, tokenValue2);

        // Act
        string actual = tokenProcessor.ReplaceTokens(original);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ResetTokenDelimiters_ShouldCallDelimiterServiceMethod()
    {
        // Arrange
        GetMock<IDelimiterService>()
            .Setup(x => x.ResetTokenDelimiters())
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        tokenProcessor.ResetTokenDelimiters();

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void SetTokenDelimiters_ShouldCallDelimiterServiceMethod()
    {
        // Arrange
        string startDelimiter = "{{";
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        bool actual = tokenProcessor.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter);

        // Assert
        actual
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    private TokenProcessor GetTokenProcessor()
        => new(GetMock<ILocater>().Object,
               GetMock<ILogger>().Object,
               GetMock<IDelimiterService>().Object,
               GetMock<ITokenParser>().Object);

    private void SetupDelimiterServiceMock()
    {
        Mock<IDelimiterService> delimiterServiceMock = GetMock<IDelimiterService>();
        delimiterServiceMock
            .SetupGet(x => x.StartDelimiter)
            .Returns(DefaultTokenStartDelimiter)
            .Verifiable(Times.AtLeastOnce);
        delimiterServiceMock
            .SetupGet(x => x.EndDelimiter)
            .Returns(DefaultTokenEndDelimiter)
            .Verifiable(Times.AtLeastOnce);
        delimiterServiceMock
            .SetupGet(x => x.DelimiterEscapeCharacter)
            .Returns(DefaultDelimiterEscapeCharacter)
            .Verifiable(Times.AtLeastOnce);
    }
}