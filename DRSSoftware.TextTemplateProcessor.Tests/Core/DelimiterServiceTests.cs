namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class DelimiterServiceTests
{
    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateDelimiterServiceWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger? logger = null;
        string expected = GetNullDependencyMessage(nameof(DelimiterService), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new DelimiterService(logger!);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateDelimiterServiceWithValidDependency_ShouldInitializeStateCorrectly()
    {
        // Arrange
        InitializeMocks();

        // Act
        DelimiterService actual = GetDelimiterService();

        // Assert
        actual
            .Should()
            .NotBeNull();
        actual.EndDelimiter
            .Should()
            .Be(DefaultTokenEndDelimiter);
        actual.StartDelimiter
            .Should()
            .Be(DefaultTokenStartDelimiter);
        actual.DelimiterEscapeCharacter
            .Should()
            .Be(DefaultDelimiterEscapeCharacter);
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
        DelimiterService delimiterService = GetDelimiterService();
        delimiterService.SetTokenDelimiters(altStartDelimiter, altEndDelimiter, altEscapeCharacter);

        // Act
        delimiterService.ResetTokenDelimiters();

        // Assert
        delimiterService.StartDelimiter
            .Should()
            .Be(DefaultTokenStartDelimiter);
        delimiterService.EndDelimiter
            .Should()
            .Be(DefaultTokenEndDelimiter);
        delimiterService.DelimiterEscapeCharacter
            .Should()
            .Be(DefaultDelimiterEscapeCharacter);
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
        AssertSetTokenDelimiters(EmptyString, startDelimiter, endDelimiter, escapeCharacter, LogSeverity.Information);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter, LogSeverity.Warning);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
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
        AssertSetTokenDelimiters(message, startDelimiter, endDelimiter, escapeCharacter);
    }

    private void AssertSetTokenDelimiters(string message, string startDelimiter, string endDelimiter, char escapeCharacter, LogSeverity logSeverity = LogSeverity.Error)
    {
        // Arrange
        if (!string.IsNullOrEmpty(message))
        {
            LoggerMock
                .Setup(x => x.Log(logSeverity, message))
                .Verifiable(Times.Once);
        }

        DelimiterService delimiterService = GetDelimiterService();

        // Act
        bool actual = delimiterService.SetTokenDelimiters(startDelimiter, endDelimiter, escapeCharacter);

        // Assert
        if (logSeverity is LogSeverity.Error)
        {
            actual
                .Should()
                .BeFalse();
            delimiterService.StartDelimiter
                .Should()
                .Be(DefaultTokenStartDelimiter);
            delimiterService.EndDelimiter
                .Should()
                .Be(DefaultTokenEndDelimiter);
            delimiterService.DelimiterEscapeCharacter
                .Should()
                .Be(DefaultDelimiterEscapeCharacter);
        }
        else
        {
            actual
                .Should()
                .BeTrue();
            delimiterService.StartDelimiter
                .Should()
                .Be(startDelimiter);
            delimiterService.EndDelimiter
                .Should()
                .Be(endDelimiter);
            delimiterService.DelimiterEscapeCharacter
                .Should()
                .Be(escapeCharacter);
        }

        VerifyMocks();
    }

    private DelimiterService GetDelimiterService() => new(LoggerMock.Object);

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