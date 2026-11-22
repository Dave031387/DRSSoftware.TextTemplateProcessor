using System.Text;

namespace DRSSoftware.TextTemplateProcessor.Parsers;

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
        void action() => _ = new FileHeaderParser(delimiterService, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        void action() => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, indentProcessor, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, GetMock<IIndentProcessor>().Object, logger, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateFileHeaderParserWithNullOptionParser_ShouldThrowException()
    {
        // Arrange
        IOptionParser optionParser = null!;
        string expected = GetNullDependencyMessage(nameof(FileHeaderParser), nameof(IOptionParser), nameof(optionParser));

        // Act
        void action() => _ = new FileHeaderParser(GetMock<IDelimiterService>().Object, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, optionParser);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
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
    public void Initialize_ShouldInitializeDelimiterServiceAndIndentProcessor()
    {
        // Arrange
        GetMock<IDelimiterService>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        GetMock<IIndentProcessor>()
            .Setup(x => x.Initialize())
            .Verifiable(Times.Once);
        FileHeaderParser parser = GetFileHeaderParser();

        // Act
        parser.Initialize();

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ParseFileHeaderWithDelimiterEscapeLongerThanOneCharacter_ShouldLogErrorMessage()
    {
        // Arrange
        string invalidValue = "xx";
        string fileHeaderText = SetupOptionParserGet((DelimiterEscapeOption, invalidValue));
        string message1 = GetMessage(MsgDelimiterEscapeMustBeSingleCharacter);
        string message2 = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterService();
        SetupOptionParserParse(DelimiterEscapeOption, invalidValue, true);
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
        string fileHeaderText = SetupOptionParserGet((DelimiterEscapeOption, invalidValue));
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterService();
        SetupOptionParserParse(DelimiterEscapeOption, invalidValue, true);
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
        string fileHeaderText = SetupOptionParserGet((EndDelimiterOption, invalidValue));
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterService();
        SetupOptionParserParse(EndDelimiterOption, invalidValue, true);
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
        string fileHeaderText = SetupOptionParserGet((optionName, optionValue));
        SetupDelimiterService();
        SetupOptionParserParse(optionName, optionValue);
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
        string fileHeaderText = SetupOptionParserGet((StartDelimiterOption, invalidValue));
        string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
        SetupDelimiterService();
        SetupOptionParserParse(StartDelimiterOption, invalidValue, true);
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
        string fileHeaderText = SetupOptionParserGet((TabSizeOption, invalidValue));
        string message = GetMessage(MsgInvalidFileHeaderTabSizeOption, invalidValue);
        SetupDelimiterService();
        SetupOptionParserParse(TabSizeOption, invalidValue, true);
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
        string fileHeaderText = SetupOptionParserGet((DelimiterEscapeOption, value1), (DelimiterEscapeOption, value2));
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, DelimiterEscapeOption);
        SetupDelimiterService();
        SetupOptionParserParse(DelimiterEscapeOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, delimiterEscape))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(DelimiterEscapeOption, value2, true);
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
        string fileHeaderText = SetupOptionParserGet((EndDelimiterOption, value1), (EndDelimiterOption, value2));
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, EndDelimiterOption);
        SetupDelimiterService();
        SetupOptionParserParse(EndDelimiterOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(DefaultTokenStartDelimiter, value1, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(EndDelimiterOption, value2, true);
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
        string fileHeaderText = SetupOptionParserGet((StartDelimiterOption, value1), (StartDelimiterOption, value2));
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, StartDelimiterOption);
        SetupDelimiterService();
        SetupOptionParserParse(StartDelimiterOption, value1, true);
        GetMock<IDelimiterService>()
            .Setup(x => x.SetTokenDelimiters(value1, DefaultTokenEndDelimiter, DefaultDelimiterEscapeCharacter))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(StartDelimiterOption, value2, true);
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
        string fileHeaderText = SetupOptionParserGet((TabSizeOption, stringValue1), (TabSizeOption, stringValue2));
        string message = GetMessage(MsgFoundDuplicateFileHeaderOption, TabSizeOption);
        SetupDelimiterService();
        SetupOptionParserParse(TabSizeOption, stringValue1, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue1, out intValue1))
            .Returns(true)
            .Verifiable(Times.Once);
        GetMock<IIndentProcessor>()
            .Setup(x => x.SetTabSize(intValue1))
            .Verifiable(Times.Once);
        SetupOptionParserParse(TabSizeOption, stringValue2, true);
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
        string fileHeaderText = SetupOptionParserGet();
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
        string fileHeaderText = SetupOptionParserGet((DelimiterEscapeOption, optionValue));
        SetupDelimiterService();
        SetupOptionParserParse(DelimiterEscapeOption, optionValue, true);
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
        string fileHeaderText = SetupOptionParserGet((EndDelimiterOption, endDelimiter));
        SetupDelimiterService();
        SetupOptionParserParse(EndDelimiterOption, endDelimiter, true);
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
        string fileHeaderText = SetupOptionParserGet((StartDelimiterOption, startDelimiter));
        SetupDelimiterService();
        SetupOptionParserParse(StartDelimiterOption, startDelimiter, true);
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
        string fileHeaderText = SetupOptionParserGet((TabSizeOption, stringValue));
        SetupDelimiterService();
        SetupOptionParserParse(TabSizeOption, stringValue, true);
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

    private void SetupDelimiterService()
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

    private string SetupOptionParserGet(params (string optionName, string optionValue)[] options)
    {
        StringBuilder textBuffer = new($"{FileHeaderCode} ", 100);
        List<string> optionStrings = [];

        foreach ((string optionName, string optionValue) in options)
        {
            string optionString = $"{optionName}={optionValue}";

            if (optionStrings.Count is 0)
            {
                textBuffer.Append($"{optionString}");
            }
            else
            {
                textBuffer.Append($", {optionString}");
            }

            optionStrings.Add(optionString);
        }

        string fileHeaderLine = textBuffer.ToString();
        GetMock<IOptionParser>()
            .Setup(x => x.GetOptionStrings(fileHeaderLine))
            .Returns([.. optionStrings])
            .Verifiable(Times.Once);

        return fileHeaderLine;
    }

    private void SetupOptionParserParse(string optionName, string optionValue, bool isValid = false)
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