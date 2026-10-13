namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class TokenParserTests
{
    // NOTE: The unit tests for the GetNextToken method have names that start with
    // "GetNextToken_Case" followed by a 1 to 3-digit number. The digits are the numbers 1 through
    // 4, where 1 represents a token start delimiter, 2 is an escaped token start delimiter, 3 is a
    // token end delimiter, and 4 is an escaped token end delimiter. The order of the numbers
    // indicates the order that the delimiters are found in the template text string.
    private const string EndDelimiter = DefaultTokenEndDelimiter;

    private const string EscapedEndDelimiter = DefaultTokenEscapeString + DefaultTokenEndDelimiter;

    private const string EscapedStartDelimiter = DefaultTokenEscapeString + DefaultTokenStartDelimiter;

    private const string StartDelimiter = DefaultTokenStartDelimiter;

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
        string expected = $"{prefixText}{escapedTokenString}";
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

    [Fact]
    public void GetNextToken_Case0_ShouldReturnEmptyTokenInfoAndUnchangedText()
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

    [Theory]
    [MemberData(nameof(TestData.StringValues2), MemberType = typeof(TestData))]
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case113_ShouldEscapeStartDelimitersAndEndDelimiter(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{StartDelimiter}{EscapedStartDelimiter}{EndDelimiter}{text3}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{EscapedStartDelimiter}{EscapedEndDelimiter}{text3}";
        string message1 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgMissingTokenStartDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Exactly(2));
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case114_ShouldEscapeStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{StartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
        string message = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Exactly(2));
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case123_ShouldEscapeStartDelimiterAndEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EndDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
        string message1 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgMissingTokenStartDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case124_ShouldEscapeStartDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case131WithInvalidTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenName = "1,Invalid Name";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{tokenName}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{StartDelimiter}";
        string expected = $"{text1}{escapedTokenString}{text4}{EscapedStartDelimiter}";
        string message1 = GetMessage(MsgTokenHasInvalidName, tokenName);
        string message2 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case131WithMissingTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{StartDelimiter}";
        string expected = $"{text1}{escapedTokenString}{text4}{EscapedStartDelimiter}";
        string message1 = GetMessage(MsgMissingTokenName);
        string message2 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case131WithValidToken_ShouldReturnValidTokenInfo(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        char expectedCase = caseFlag == Space ? SameCaseFlag : caseFlag;
        string tokenName = "Token1";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{StartDelimiter}";
        string expected = $"{text1}{tokenString}";
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        actual
            .Should()
            .Be(expected);
        AssertTokenInfo(tokenInfo, tokenString, tokenName, expectedCase);
        VerifyState(tokenParser, false, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case132WithInvalidTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenName = "1,Invalid Name";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{tokenName}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{EscapedStartDelimiter}";
        string expected = $"{text1}{escapedTokenString}{text4}{EscapedStartDelimiter}";
        string message = GetMessage(MsgTokenHasInvalidName, tokenName);
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case132WithMissingTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{EscapedStartDelimiter}";
        string expected = $"{text1}{escapedTokenString}{text4}{EscapedStartDelimiter}";
        string message = GetMessage(MsgMissingTokenName);
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case132WithValidToken_ShouldReturnValidTokenInfo(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        char expectedCase = caseFlag == Space ? SameCaseFlag : caseFlag;
        string tokenName = "Token1";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}{EscapedStartDelimiter}";
        string expected = $"{text1}{tokenString}";
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        actual
            .Should()
            .Be(expected);
        AssertTokenInfo(tokenInfo, tokenString, tokenName, expectedCase);
        VerifyState(tokenParser, false, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case13WithInvalidTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenName = "1,Invalid Name";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{tokenName}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}";
        string expected = $"{text1}{escapedTokenString}{text4}";
        string message = GetMessage(MsgTokenHasInvalidName, tokenName);
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case13WithMissingTokenName_ShouldEscapeTokenDelimiters(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{text3}{EndDelimiter}";
        string escapedTokenString = $"{EscapedStartDelimiter}{caseString}{text2}{text3}{EscapedEndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}";
        string expected = $"{text1}{escapedTokenString}{text4}";
        string message = GetMessage(MsgMissingTokenName);
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
    [MemberData(nameof(TestData.StringValues4WithCaseFlag), MemberType = typeof(TestData))]
    public void GetNextToken_Case13WithValidToken_ShouldReturnValidTokenInfo(char caseFlag, string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string caseString = caseFlag == Space ? string.Empty : $"{caseFlag}";
        char expectedCase = caseFlag == Space ? SameCaseFlag : caseFlag;
        string tokenName = "Token1";
        string tokenString = $"{StartDelimiter}{caseString}{text2}{tokenName}{text3}{EndDelimiter}";
        string originalText = $"{text1}{tokenString}{text4}";
        string expected = $"{text1}{tokenString}";
        bool endOfText = string.IsNullOrEmpty(text4);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        actual
            .Should()
            .Be(expected);
        AssertTokenInfo(tokenInfo, tokenString, tokenName, expectedCase);
        VerifyState(tokenParser, endOfText, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case14_ShouldEscapeStartDelimiter(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedEndDelimiter}{text3}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case141_ShouldEscapeStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedEndDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Exactly(2));
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case142_ShouldEscapeStartDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{StartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues2), MemberType = typeof(TestData))]
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case21_ShouldEscapeAllStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{StartDelimiter}{text3}{StartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case213_ShouldEscapeStartDelimiterAndEndDelimiter(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{StartDelimiter}{EscapedStartDelimiter}{EndDelimiter}{text3}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{EscapedStartDelimiter}{EscapedEndDelimiter}{text3}";
        string message1 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgMissingTokenStartDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case214_ShouldEscapeStartDelimiter(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{StartDelimiter}{text3}{EscapedEndDelimiter}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}";
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case22_ShouldReturnUnchangedText(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case223_ShouldEscapeEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EndDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case224_ShouldReturnUnchangedText(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case23_ShouldEscapeAllEndDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{EndDelimiter}{text3}{EndDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedEndDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case231_ShouldEscapeStartAndEndDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{EndDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message1 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case232_ShouldEscapeEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{EndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case24_ShouldReturnUnchangedText(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case241_ShouldEscapeStartDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case242_ShouldReturnUnchangedText(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedStartDelimiter}{text2}{EscapedEndDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case31_ShouldEscapeStartAndEndDelimiters(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{StartDelimiter}{text3}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}";
        string message1 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case311_ShouldEscapeStartDelimitersAndEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{StartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message1 = GetMessage(MsgMissingTokenEndDelimiter);
        string message2 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case312_ShouldEscapeStartDelimiterAndEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{StartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message1 = GetMessage(MsgMissingTokenEndDelimiter);
        string message2 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case32_ShouldEscapeAllStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EndDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedEndDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case321_ShouldEscapeStartDelimitersAndEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{EscapedStartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message1 = GetMessage(MsgMissingTokenEndDelimiter);
        string message2 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case322_ShouldEscapeEndDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string message = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
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
    [MemberData(nameof(TestData.StringValues2), MemberType = typeof(TestData))]
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

    [Theory]
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case41_ShouldEscapeStartDelimiter(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedEndDelimiter}{text2}{StartDelimiter}{text3}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case411_ShouldEscapeStartDelimiters(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedEndDelimiter}{text2}{StartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case412_ShouldEscapeStartDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedEndDelimiter}{text2}{StartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues3), MemberType = typeof(TestData))]
    public void GetNextToken_Case42_ShouldReturnUnchangedText(string text1, string text2, string text3)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case421_ShouldEscapeStartDelimiter(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string originalText = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{StartDelimiter}{text4}";
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    [MemberData(nameof(TestData.StringValues4), MemberType = typeof(TestData))]
    public void GetNextToken_Case422_ShouldReturnUnchangedText(string text1, string text2, string text3, string text4)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string expected = $"{text1}{EscapedEndDelimiter}{text2}{EscapedStartDelimiter}{text3}{EscapedStartDelimiter}{text4}";
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
    public void GetNextToken_ShouldAdvanceSearchIndexPastFirstFoundStartDelimiter()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        char caseFlag1 = LowercaseFlag;
        char caseFlag2 = UppercaseFlag;
        string tokenName1 = "Token1";
        string tokenName2 = "Token2";
        string tokenString1 = $"{StartDelimiter}{caseFlag1}{tokenName1}{EndDelimiter}";
        string tokenString2 = $"{StartDelimiter}{caseFlag2} {tokenName2} {EndDelimiter}";
        string templateText = $" {EscapedStartDelimiter} text {tokenString1} text {StartDelimiter}{tokenString2}";
        tokenParser.InitializeParser(templateText);

        // Act
        TokenInfo tokenInfo1 = tokenParser.GetNextToken();
        TokenInfo tokenInfo2 = tokenParser.GetNextToken();

        // Assert
        tokenParser.GetModifiedText()
            .Should()
            .BeEmpty();
        AssertTokenInfo(tokenInfo1, tokenString1, tokenName1, caseFlag1);
        AssertTokenInfo(tokenInfo2, tokenString2, tokenName2, caseFlag2);
        VerifyState(tokenParser, true, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(EscapedStartDelimiter, EscapedStartDelimiter, EndDelimiter)]
    [InlineData(StartDelimiter, EscapedStartDelimiter, EndDelimiter)]
    [InlineData(EscapedStartDelimiter, StartDelimiter, EscapedEndDelimiter)]
    [InlineData(StartDelimiter, StartDelimiter, EscapedEndDelimiter)]
    public void GetNextToken_ShouldAdvanceSearchIndexPastSecondFoundToken(string firstStartDelimiter, string secondStartDelimiter, string endDelimiter)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string tokenName = "Token1";
        string tokenString = $"{StartDelimiter}  {tokenName}  {EndDelimiter}";
        string templateText = $"text {firstStartDelimiter} text {secondStartDelimiter} text {endDelimiter} text {tokenString} text";
        tokenParser.InitializeParser(templateText);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();

        // Assert
        tokenParser.GetModifiedText()
            .Should()
            .BeEmpty();
        AssertTokenInfo(tokenInfo, tokenString, tokenName, SameCaseFlag);
        VerifyState(tokenParser, false, false, true);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void GetNextToken_ShouldAdvanceSearchIndexPastSecondFoundTokenWhenOnlyEscapedStartTokensAreFound()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string templateText = $"text {EscapedStartDelimiter} text {EscapedStartDelimiter} text {StartDelimiter} text";
        tokenParser.InitializeParser(templateText);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();

        // Assert
        tokenInfo
            .Should()
            .Be(TokenInfo.Empty);
        tokenParser.GetModifiedText()
            .Should()
            .BeEmpty();
        VerifyState(tokenParser, true, false, false);
        MocksVerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(EmptyString, EmptyString, EmptyString)]
    [InlineData(EscapedStartDelimiter, EmptyString, EmptyString)]
    [InlineData(StartDelimiter, EmptyString, EmptyString)]
    [InlineData(EscapedStartDelimiter, StartDelimiter, EmptyString)]
    [InlineData(StartDelimiter, EscapedStartDelimiter, EmptyString)]
    [InlineData(StartDelimiter, StartDelimiter, EmptyString)]
    [InlineData(EndDelimiter, EmptyString, EmptyString)]
    [InlineData(EscapedStartDelimiter, EndDelimiter, EmptyString)]
    [InlineData(EndDelimiter, EscapedStartDelimiter, EmptyString)]
    public void GetNextToken_ShouldAdvanceSearchIndexToEndOfTextLine(string firstStartDelimiter, string secondDelimiter, string thirdDelimiter)
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string templateText = $"text {firstStartDelimiter} text {secondDelimiter} text {thirdDelimiter} text";
        tokenParser.InitializeParser(templateText);

        // Act
        TokenInfo tokenInfo = tokenParser.GetNextToken();

        // Assert
        tokenInfo
            .Should()
            .Be(TokenInfo.Empty);
        tokenParser.GetModifiedText()
            .Should()
            .BeEmpty();
        VerifyState(tokenParser, true, false, false);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void GetNextTokenWithAlternateDelimiterValues_ShouldReturnFoundTokenAndEscapeInvalidDelimiters()
    {
        // Arrange
        InitializeMocks();
        TokenParser tokenParser = GetTokenParser();
        string startDelimiter = "<{{";
        string endDelimiter = "}>";
        char escapeCharacter = '!';
        tokenParser.SetTokenDelimiterValues(startDelimiter, endDelimiter, escapeCharacter);
        string tokenName = "Token1";
        string tokenString = $"{startDelimiter}{tokenName}{endDelimiter}";
        string originalText = $"text {startDelimiter} text {tokenString} text {endDelimiter} {escapeCharacter}{startDelimiter} text {startDelimiter} text {startDelimiter} text";
        string expected = $"text {escapeCharacter}{startDelimiter} text {tokenString} text {escapeCharacter}{endDelimiter} {escapeCharacter}{startDelimiter} text {escapeCharacter}{startDelimiter} text {escapeCharacter}{startDelimiter} text";
        string message1 = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
        string message2 = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
        string message3 = GetMessage(MsgMissingTokenEndDelimiter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message2))
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, message3))
            .Verifiable(Times.Once);
        tokenParser.InitializeParser(originalText, true);

        // Act
        TokenInfo tokenInfo1 = tokenParser.GetNextToken();
        TokenInfo tokenInfo2 = tokenParser.GetNextToken();
        string actual = tokenParser.GetModifiedText();

        // Assert
        actual
            .Should()
            .Be(expected);
        AssertTokenInfo(tokenInfo1, tokenString, tokenName, SameCaseFlag);
        tokenInfo2
            .Should()
            .Be(TokenInfo.Empty);
        VerifyState(tokenParser, true, true, false);
        VerifyMocks();
    }

    private static void AssertTokenInfo(TokenInfo tokenInfo, string tokenString, string tokenName, char caseFlag)
    {
        tokenInfo.TokenString
            .Should()
            .Be(tokenString);
        tokenInfo.TokenName
            .Should()
            .Be(tokenName);
        tokenInfo.CaseFlag
            .Should()
            .Be(caseFlag);
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