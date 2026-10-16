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
        VerifyMocks();
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
    public void CreateTokenProcessorWithNullTokenParser_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ITokenParser? tokenParser = null;
        string expected = GetNullDependencyMessage(nameof(TokenProcessor), nameof(ITokenParser), nameof(tokenParser));

        // Act
        Action action = () => _ = new TokenProcessor(LocaterMock.Object, LoggerMock.Object, tokenParser!);

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
        VerifyMocks();
    }

    [Fact]
    public void ExtractTokensWhenTextTemplateLineContainsInvalidTokenDelimiters_ShouldReturnModifiedTextTemplateLine()
    {
        // Arrange
        InitializeMocks();
        string original = "Text containing invalid delimiters.";
        string expected = "Modified text template line.";
        TokenParserMock
            .Setup(x => x.InitializeParser(original, true))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupGet(x => x.IsTextModified)
            .Returns(true)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
        string expected = "Text template line with no tokens";
        TokenParserMock
            .Setup(x => x.InitializeParser(expected, true))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupGet(x => x.ValidTokenFound)
            .Returns(false)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
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
        TokenParserMock
            .Setup(x => x.InitializeParser(expectedText, true))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .SetupSequence(x => x.GetNextToken())
            .Returns(tokenInfo1A)
            .Returns(tokenInfo2)
            .Returns(tokenInfo1B)
            .Returns(TokenInfo.Empty);
        TokenParserMock
            .SetupSequence(x => x.ValidTokenFound)
            .Returns(true)
            .Returns(true)
            .Returns(true)
            .Returns(false);
        TokenParserMock
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
        InitializeMocks();
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryIsEmpty, locationName);
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryIsNull, locationName);
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
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
        VerifyMocks();
    }

    [Fact]
    public void LoadTokenValuesWhenTokenNameIsInvalid_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string tokenName = "1token";
        string locationName = "location";
        string message = GetMessage(MsgTokenDictionaryContainsInvalidTokenName, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, EmptyString }
        };
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
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
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
        string tokenName = "token";
        string originalValue = "value";
        string locationName = "location";
        string message = GetMessage(MsgTokenWithEmptyValue, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, EmptyString }
        };
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
        string tokenName = "token";
        string originalValue = "value";
        string locationName = "location";
        string message = GetMessage(MsgTokenWithNullValue, locationName, tokenName);
        Dictionary<string, string> dictionary = new()
        {
            { tokenName, null! }
        };
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
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
        InitializeMocks();
        string original = $"{DefaultTokenEscapeCharacter}{DefaultTokenEndDelimiter}text{DefaultTokenEscapeCharacter}{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter}text";
        string expected = $"{DefaultTokenEndDelimiter}text{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter}text";
        TokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
        string expected = "template text with no tokens";
        TokenParserMock
            .Setup(x => x.InitializeParser(expected, false))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(TokenInfo.Empty)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
        string tokenName = "token";
        string tokenString = $"{DefaultTokenStartDelimiter}{tokenName}{DefaultTokenEndDelimiter}";
        TokenInfo tokenInfo = new(tokenString, tokenName, SameCaseFlag);
        string locationName = "location";
        string message = GetMessage(MsgTokenNameNotFound, locationName, tokenName);
        string expected = $"text {tokenString} more text";
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TokenParserMock
            .Setup(x => x.InitializeParser(expected, false))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(tokenInfo)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
        string tokenName = "token";
        string tokenString = $"{DefaultTokenStartDelimiter}{tokenName}{DefaultTokenEndDelimiter}";
        TokenInfo tokenInfo = new(tokenString, tokenName, SameCaseFlag);
        string original = $"text {tokenString} more text";
        string expected = $"text  more text";
        string locationName = "location";
        string message = GetMessage(MsgTokenValueIsEmpty, locationName, tokenName);
        LocaterMock
            .SetupGet(x => x.CurrentLocationName)
            .Returns(locationName)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        TokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .Setup(x => x.GetNextToken())
            .Returns(tokenInfo)
            .Verifiable(Times.Once);
        TokenParserMock
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
        InitializeMocks();
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
        TokenParserMock
            .Setup(x => x.InitializeParser(original, false))
            .Verifiable(Times.Once);
        TokenParserMock
            .SetupSequence(x => x.EndOfText)
            .Returns(false)
            .Returns(false)
            .Returns(false)
            .Returns(true);
        TokenParserMock
            .SetupSequence(x => x.GetNextToken())
            .Returns(tokenInfo1A)
            .Returns(tokenInfo2)
            .Returns(tokenInfo1B);
        TokenParserMock
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
    public void ResetTokenDelimiters_ShouldSetDelimiterValuesBackToTheirDefaultValues()
    {
        // Arrange
        InitializeMocks();
        string altStartDelimiter = "{{";
        string altEndDelimiter = "}}";
        char altEscapeCharacter = '!';
        TokenProcessor tokenProcessor = GetTokenProcessor();
        TokenParserMock.Reset();
        TokenParserMock
            .Setup(x => x.SetTokenDelimiterValues(altStartDelimiter, altEndDelimiter, altEscapeCharacter))
            .Verifiable(Times.Once);
        TokenParserMock
            .Setup(x => x.SetTokenDelimiterValues(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, DefaultTokenEscapeCharacter))
            .Verifiable(Times.Once);
        tokenProcessor.SetTokenDelimiters(altStartDelimiter, altEndDelimiter, altEscapeCharacter);

        // Act
        tokenProcessor.ResetTokenDelimiters();

        // Assert
        tokenProcessor.TokenStart
            .Should()
            .Be(DefaultTokenStartDelimiter);
        tokenProcessor.TokenEnd
            .Should()
            .Be(DefaultTokenEndDelimiter);
        tokenProcessor.TokenEscape
            .Should()
            .Be(DefaultTokenEscapeCharacter);
        VerifyMocks();
    }

    [Fact]
    public void SetTokenDelimitersWhenAllDelimitersAreValid_ShouldSetDelimiterValues()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "{{";
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        TokenParserMock
            .Setup(x => x.SetTokenDelimiterValues(startDelimiter, endDelimiter, escapeCharacter))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        bool actual = tokenProcessor.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter);

        // Assert
        actual
            .Should()
            .BeTrue();
        tokenProcessor.TokenStart
            .Should()
            .Be(startDelimiter);
        tokenProcessor.TokenEnd
            .Should()
            .Be(endDelimiter);
        tokenProcessor.TokenEscape
            .Should()
            .Be(escapeCharacter);
        VerifyMocks();
    }

    [Fact]
    public void SetTokenDelimitersWhenEndDelimiterAndEscapeCharacterAreTheSame_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "{{";
        string endDelimiter = "!";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenEndAndTokenEscapeAreSame, endDelimiter, escapeCharacter.ToString());
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Fact]
    public void SetTokenDelimitersWhenEndDelimiterIsEmpty_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "{{";
        string endDelimiter = EmptyString;
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenEndDelimiterIsEmpty);
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Fact]
    public void SetTokenDelimitersWhenEndDelimiterIsNull_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "{{";
        string endDelimiter = null!;
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenEndDelimiterIsNull);
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Fact]
    public void SetTokenDelimitersWhenStartAndEndDelimitersAreEqual_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "}}";
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenStartAndTokenEndAreSame, startDelimiter, endDelimiter);
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Fact]
    public void SetTokenDelimitersWhenStartDelimiterAndEscapeCharacterAreTheSame_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "!";
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenStartAndTokenEscapeAreSame, startDelimiter, escapeCharacter.ToString());
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Theory]
    [InlineData(LowercaseFlag)]
    [InlineData(UppercaseFlag)]
    [InlineData(SameCaseFlag)]
    public void SetTokenDelimitersWhenStartDelimiterEndsInCaseFlagCharacter_ShouldLogWarningMessageAndSetDelimiterValues(char caseFlag)
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = "{{" + caseFlag;
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenStartDelimiterWarning);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        TokenParserMock
            .Setup(x => x.SetTokenDelimiterValues(startDelimiter, endDelimiter, escapeCharacter))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        bool actual = tokenProcessor.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter);

        // Assert
        actual
            .Should()
            .BeTrue();
        tokenProcessor.TokenStart
            .Should()
            .Be(startDelimiter);
        tokenProcessor.TokenEnd
            .Should()
            .Be(endDelimiter);
        tokenProcessor.TokenEscape
            .Should()
            .Be(escapeCharacter);
        VerifyMocks();
    }

    [Fact]
    public void SetTokenDelimitersWhenStartDelimiterIsEmpty_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = EmptyString;
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenStartDelimiterIsEmpty);
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    [Fact]
    public void SetTokenDelimitersWhenStartDelimiterIsNull_ShouldLogErrorMessage()
    {
        // Arrange
        InitializeMocks();
        string startDelimiter = null!;
        string endDelimiter = "}}";
        char escapeCharacter = '!';
        string message = GetMessage(MsgTokenStartDelimiterIsNull);
        AssertSetTokenDelimiterError(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    private void AssertSetTokenDelimiterError(string message, string startDelimiter, string endDelimiter, char escapeCharacter)
    {
        // Arrange
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        TokenProcessor tokenProcessor = GetTokenProcessor();

        // Act
        bool actual = tokenProcessor.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter);

        // Assert
        actual
            .Should()
            .BeFalse();
        tokenProcessor.TokenStart
            .Should()
            .Be(DefaultTokenStartDelimiter);
        tokenProcessor.TokenEnd
            .Should()
            .Be(DefaultTokenEndDelimiter);
        tokenProcessor.TokenEscape
            .Should()
            .Be(DefaultTokenEscapeCharacter);
        VerifyMocks();
    }

    private TokenProcessor GetTokenProcessor()
    {
        TokenParserMock
            .Setup(x => x.SetTokenDelimiterValues(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, DefaultTokenEscapeCharacter))
            .Verifiable(Times.Once);
        return new(LocaterMock.Object, LoggerMock.Object, TokenParserMock.Object);
    }

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