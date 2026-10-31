namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class FileHeaderParserTests : MoqUnitTestBase
{
    [Fact]
    public void CreateFileHeaderParserWithNullDelimiterService_ShouldThrowException()
    {
        // Arrange
        IDelimiterService delimiterService = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(IDelimiterService), nameof(delimiterService));

        // Act
        Action action = () => _ = new FileHeaderParser(delimiterService, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        Action action = () => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, indentProcessor, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, GetMock<IIndentProcessor>().Object, logger, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullOptionParser_ShouldThrowException()
    {
        // Arrange
        IOptionParser optionParser = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(IOptionParser), nameof(optionParser));

        // Act
        Action action = () => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, optionParser);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithValidDependencies_ShouldSucceed()
    {
        // Act
        FileHeaderParser actual = GetFileHeaderParser();

        // Assert
        actual
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void ParseFileHeaderWithDelimiterEscapeLongerThanOneCharacter_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "xx";
        string fileHeaderText = $"{FileHeaderCode} {DelimiterEscapeOption}={invalidValue}";
        string message1 = GetMessage(MsgDelimiterEscapeMustBeSingleCharacter);
        string message2 = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(DelimiterEscapeOption, invalidValue, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message2))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithInvalidDelimiterEscapeOption_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "x";
        char delimiterEscape = invalidValue[0];
        string fileHeaderText = $"{FileHeaderCode} {DelimiterEscapeOption}={invalidValue}";
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(DelimiterEscapeOption, invalidValue, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, delimiterEscape))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithInvalidEndDelimiterOption_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "x";
        string fileHeaderText = $"{FileHeaderCode} {EndDelimiterOption}={invalidValue}";
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(EndDelimiterOption, invalidValue, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, invalidValue, DefaultDelimiterEscapeCharacter))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithInvalidOption_ShouldDoNothing()
    {
        // Arrange
        string optionName = "Invalid";
        string optionValue = "value";
        string fileHeaderText = $"{FileHeaderCode} {optionName}={optionValue}";
        SetupDelimiterServiceMock();
        SetupOptionParserMock(optionName, optionValue);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithInvalidStartDelimiterOption_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "x";
        string fileHeaderText = $"{FileHeaderCode} {StartDelimiterOption}={invalidValue}";
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(StartDelimiterOption, invalidValue, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(invalidValue, DefaultTokenEndDelimiter, DefaultDelimiterEscapeCharacter))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithInvalidTabSizeOption_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "invalid";
        int intValue = It.IsAny<int>();
        string fileHeaderText = $"{FileHeaderCode} {TabSizeOption}={invalidValue}";
        string message = GetMessage(MsgInvalidFileHeaderTabSizeOption, invalidValue);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(TabSizeOption, invalidValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(invalidValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithMultipleDelimiterEscapeOptions_ShouldUseTheFirstAndIgnoreRest()
    {
        // Arrange
        string value1 = "!";
        string value2 = "^";
        char delimiterEscape = value1[0];
        string fileHeaderText = $"{FileHeaderCode} {DelimiterEscapeOption}={value1} {DelimiterEscapeOption}={value2}";
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, DelimiterEscapeOption);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(DelimiterEscapeOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, delimiterEscape))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserMock(DelimiterEscapeOption, value2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithMultipleEndDelimiterOptions_ShouldUseTheFirstAndIgnoreRest()
    {
        // Arrange
        string value1 = "=)";
        string value2 = ")]";
        string fileHeaderText = $"{FileHeaderCode} {EndDelimiterOption}={value1} {EndDelimiterOption}={value2}";
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, EndDelimiterOption);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(EndDelimiterOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, value1, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserMock(EndDelimiterOption, value2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithMultipleStartDelimiterOptions_ShouldUseTheFirstAndIgnoreRest()
    {
        // Arrange
        string value1 = "(=";
        string value2 = "[(";
        string fileHeaderText = $"{FileHeaderCode} {StartDelimiterOption}={value1} {StartDelimiterOption}={value2}";
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, StartDelimiterOption);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(StartDelimiterOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(value1, DefaultTokenEndDelimiter, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserMock(StartDelimiterOption, value2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithMultipleTabSizeOptions_ShouldUseTheFirstAndIgnoreRest()
    {
        // Arrange
        int intValue1 = 3;
        int intValue2 = 5;
        string stringValue1 = intValue1.ToString();
        string stringValue2 = intValue2.ToString();
        string fileHeaderText = $"{FileHeaderCode} {TabSizeOption}={stringValue1} {TabSizeOption}={stringValue2}";
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, TabSizeOption);
        SetupDelimiterServiceMock();
        SetupOptionParserMock(TabSizeOption, stringValue1, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue1, out intValue1))
            .Returns(true)
            .Verifiable(Times.Once);
        GetMock<IIndentProcessor>()
            .Setup(x => x.SetTabSize(intValue1))
            .Verifiable(Times.Once);
        SetupOptionParserMock(TabSizeOption, stringValue2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithNoOptionsSpecified_ShouldLogWarningMessage()
    {
        // Arrange
        string fileHeaderText = $"{FileHeaderCode}    ";
        string message = GetMessage(MsgFileHeaderHasNoOptions);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, message))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithValidDelimiterEscapeOption_ShouldSetDelimiterValues()
    {
        // Arrange
        string optionValue = "!";
        char delimiterEscape = optionValue[0];
        string fileHeaderText = $"{FileHeaderCode} {DelimiterEscapeOption}={optionValue}";
        SetupDelimiterServiceMock();
        SetupOptionParserMock(DelimiterEscapeOption, optionValue, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, delimiterEscape))
            .Returns(true)
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithValidEndDelimiterOption_ShouldSetDelimiterValues()
    {
        // Arrange
        string endDelimiter = "=]";
        string fileHeaderText = $"{FileHeaderCode} {EndDelimiterOption}={endDelimiter}";
        SetupDelimiterServiceMock();
        SetupOptionParserMock(EndDelimiterOption, endDelimiter, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, endDelimiter, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithValidStartDelimiterOption_ShouldSetDelimiterValues()
    {
        // Arrange
        string startDelimiter = "[=";
        string fileHeaderText = $"{FileHeaderCode} {StartDelimiterOption}={startDelimiter}";
        SetupDelimiterServiceMock();
        SetupOptionParserMock(StartDelimiterOption, startDelimiter, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(startDelimiter, DefaultTokenEndDelimiter, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithValidTabSizeOption_ShouldSetTabSize()
    {
        // Arrange
        int intValue = 2;
        string stringValue = intValue.ToString();
        string fileHeaderText = $"{FileHeaderCode} {TabSizeOption}={stringValue}";
        SetupDelimiterServiceMock();
        SetupOptionParserMock(TabSizeOption, stringValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        GetMock<IIndentProcessor>()
            .Setup(x => x.SetTabSize(intValue))
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Parse(fileHeaderText);

        // Assert
        VerifyMocks();
    }

    private FileHeaderParser GetFileHeaderParser()
        => new(GetMock<IDelimiterService>().Object,
               GetMock<IIndentProcessor>().Object,
               GetMock<ILogger>().Object,
               GetMock<IOptionParser>().Object);

    private void SetupDelimiterServiceMock()
    {
        Mock<IDelimiterService> delimiterService = GetMock<IDelimiterService>();
        delimiterService
            .SetupGet(x => x.StartDelimiter)
            .Returns(DefaultTokenStartDelimiter)
            .Verifiable(Times.Once);
        delimiterService
            .SetupGet(x => x.EndDelimiter)
            .Returns(DefaultTokenEndDelimiter)
            .Verifiable(Times.Once);
        delimiterService
            .SetupGet(x => x.DelimiterEscapeCharacter)
            .Returns(DefaultDelimiterEscapeCharacter)
            .Verifiable(Times.Once);
    }

    private void SetupOptionParserMock(string optionName, string optionValue, bool isValid = false)
    {
        string optionString = $"{optionName}={optionValue}";
        Option expected = new();

        if (isValid)
        {
            expected.Name = optionName;
            expected.Value = optionValue;
        }
        else
        {
            expected.Name = EmptyString;
            expected.Value = EmptyString;
        }

        GetMock<IOptionParser>()
            .Setup(x => x.Parse(optionString, IsValidFileHeaderOption, FileHeaderOptionGroup, null))
            .Returns(expected)
            .Verifiable(Times.Once);
    }
}