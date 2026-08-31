namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class IndentProcessorTests
{
    private const string SegmentName = "Segment1";

    private Mock<ILocater> LocaterMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateIndentProcessorWithNullLocater_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILocater locater = null!;
        string expected = GetNullDependencyMessage(nameof(IndentProcessor), nameof(ILocater), nameof(locater));

        // Act
        Action action = () => _ = new IndentProcessor(locater, LoggerMock.Object);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateIndentProcessorWithNullLogger_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(IndentProcessor), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new IndentProcessor(LocaterMock.Object, logger);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateIndentProcessorWithValidDependencies_ShouldSucceedAndInitializeProperties()
    {
        // Arrange
        InitializeMocks();

        // Act
        IndentProcessor indentProcessor = GetIndentProcessor();

        // Assert
        indentProcessor
            .Should()
            .NotBeNull();
        indentProcessor.CurrentIndent
            .Should()
            .Be(0);
        indentProcessor.TabSize
            .Should()
            .Be(DefaultTabSize);
        MocksVerifyNoOtherCalls();
    }

    // Case 01 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value < 0 /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case01()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = -2;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue,
                                MsgLeftIndentHasBeenTruncated);
    }

    // Case 02 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value < 0 /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case02()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = -2;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue,
                                MsgLeftIndentHasBeenTruncated);
    }

    // Case 03 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value = 0 /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case03()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 04 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value = 0 /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case04()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 05 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value > 0 /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case05()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentIndent = initialIndent * DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 06 / firstTimeOffset = 0 / isRelative = true / indent < 0 / calculated value > 0 /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case06()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 07 / firstTimeOffset = 0 / isRelative = true / indent = 0 / calculated value n/a /
    // isOneTime = n/a
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetFirstTimeIndent_Case07(bool isOneTime)
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 08 / firstTimeOffset = 0 / isRelative = true / indent > 0 / calculated value > 0 /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case08()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = 1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = (initialIndent + textIndent) * DefaultTabSize;
        int expectedCurrentIndent = DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 09 / firstTimeOffset = 0 / isRelative = true / indent > 0 / calculated value > 0 /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case09()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = 1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = (initialIndent + textIndent) * DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 10 / firstTimeOffset = 0 / isRelative = false / indent < 0 / calculated value n/a /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case10()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = initialIndent * DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue,
                                MsgLeftIndentHasBeenTruncated);
    }

    // Case 11 / firstTimeOffset = 0 / isRelative = false / indent < 0 / calculated value n/a /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case11()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue,
                                MsgLeftIndentHasBeenTruncated);
    }

    // Case 12 / firstTimeOffset = 0 / isRelative = false / indent = 0 / calculated value n/a /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case12()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = initialIndent * DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 13 / firstTimeOffset = 0 / isRelative = false / indent = 0 / calculated value n/a /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case13()
    {
        int firstTimeOffset = 0;
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 14 / firstTimeOffset = 0 / isRelative = false / indent > 0 / calculated value n/a /
    // isOneTime = true
    [Fact]
    public void GetFirstTimeIndent_Case14()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = 1;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentIndent = initialIndent * DefaultTabSize;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 15 / firstTimeOffset = 0 / isRelative = false / indent > 0 / calculated value n/a /
    // isOneTime = false
    [Fact]
    public void GetFirstTimeIndent_Case15()
    {
        int firstTimeOffset = 0;
        int initialIndent = 2;
        int textIndent = 1;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 16 / firstTimeOffset < 0 / isRelative = n/a / indent n/a / calculated value < 0 /
    // isOneTime = n/a
    [Theory]
    [InlineData(false, false, 2)]
    [InlineData(true, false, 3)]
    [InlineData(false, true, 4)]
    [InlineData(true, true, 5)]
    public void GetFirstTimeIndent_Case16(bool isRelative, bool isOneTime, int textIndent)
    {
        int firstTimeOffset = -2;
        int initialIndent = 1;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue,
                                MsgFirstTimeIndentHasBeenTruncated);
    }

    // Case 17 / firstTimeOffset < 0 / isRelative = n/a / indent n/a / calculated value = 0 /
    // isOneTime = n/a
    [Theory]
    [InlineData(false, false, 2)]
    [InlineData(true, false, 3)]
    [InlineData(false, true, 4)]
    [InlineData(true, true, 5)]
    public void GetFirstTimeIndent_Case17(bool isRelative, bool isOneTime, int textIndent)
    {
        int firstTimeOffset = -1;
        int initialIndent = 1;
        int expectedReturnValue = 0;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 18 / firstTimeOffset < 0 / isRelative = n/a / indent n/a / calculated value > 0 /
    // isOneTime = n/a
    [Theory]
    [InlineData(false, false, 2)]
    [InlineData(true, false, 3)]
    [InlineData(false, true, 4)]
    [InlineData(true, true, 5)]
    public void GetFirstTimeIndent_Case18(bool isRelative, bool isOneTime, int textIndent)
    {
        int firstTimeOffset = -1;
        int initialIndent = 2;
        int expectedReturnValue = (initialIndent + firstTimeOffset) * DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case 19 / firstTimeOffset > 0 / isRelative = n/a / indent n/a / calculated value > 0 /
    // isOneTime = n/a
    [Theory]
    [InlineData(false, false, 2)]
    [InlineData(true, false, 3)]
    [InlineData(false, true, 4)]
    [InlineData(true, true, 5)]
    public void GetFirstTimeIndent_Case19(bool isRelative, bool isOneTime, int textIndent)
    {
        int firstTimeOffset = 1;
        int initialIndent = 2;
        int expectedReturnValue = (initialIndent + firstTimeOffset) * DefaultTabSize;
        int expectedCurrentIndent = expectedReturnValue;

        Test_GetFirstTimeIndent(firstTimeOffset,
                                initialIndent,
                                textIndent,
                                isRelative,
                                isOneTime,
                                expectedCurrentIndent,
                                expectedReturnValue);
    }

    // Case01 / indent < 0 / isRelative = true / isOneTime = true / calculated value < 0
    [Fact]
    public void GetIndent_Case01()
    {
        int initialIndent = 1;
        int textIndent = -2;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentValue = DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue,
                       MsgLeftIndentHasBeenTruncated);
    }

    // Case02 / indent < 0 / isRelative = true / isOneTime = false / calculated value < 0
    [Fact]
    public void GetIndent_Case02()
    {
        int initialIndent = 1;
        int textIndent = -2;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue,
                       MsgLeftIndentHasBeenTruncated);
    }

    // Case03 / indent < 0 / isRelative = true / isOneTime = true / calculated value = 0
    [Fact]
    public void GetIndent_Case03()
    {
        int initialIndent = 1;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentValue = DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case04 / indent < 0 / isRelative = true / isOneTime = false / calculated value = 0
    [Fact]
    public void GetIndent_Case04()
    {
        int initialIndent = 1;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case05 / indent < 0 / isRelative = true / isOneTime = true / calculated value > 0
    [Fact]
    public void GetIndent_Case05()
    {
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentValue = initialIndent * DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case06 / indent < 0 / isRelative = true / isOneTime = false / calculated value > 0
    [Fact]
    public void GetIndent_Case06()
    {
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case07 / indent = 0 / isRelative = true / isOneTime = n/a / calculated value n/a
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetIndent_Case07(bool isOneTime)
    {
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case08 / indent > 0 / isRelative = true / isOneTime = true / calculated value n/a
    [Fact]
    public void GetIndent_Case08()
    {
        int initialIndent = 1;
        int textIndent = 1;
        bool isRelative = true;
        bool isOneTime = true;
        int expectedReturnValue = (initialIndent + textIndent) * DefaultTabSize;
        int expectedCurrentValue = DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case09 / indent > 0 / isRelative = true / isOneTime = false / calculated value n/a
    [Fact]
    public void GetIndent_Case09()
    {
        int initialIndent = 1;
        int textIndent = 1;
        bool isRelative = true;
        bool isOneTime = false;
        int expectedReturnValue = (initialIndent + textIndent) * DefaultTabSize;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case10 / indent < 0 / isRelative = false / isOneTime = true / calculated value n/a
    [Fact]
    public void GetIndent_Case10()
    {
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentValue = initialIndent * DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue,
                       MsgLeftIndentHasBeenTruncated);
    }

    // Case11 / indent < 0 / isRelative = false / isOneTime = false / calculated value n/a
    [Fact]
    public void GetIndent_Case11()
    {
        int initialIndent = 2;
        int textIndent = -1;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue,
                       MsgLeftIndentHasBeenTruncated);
    }

    // Case12 / indent = 0 / isRelative = false / isOneTime = true / calculated value n/a
    [Fact]
    public void GetIndent_Case12()
    {
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = 0;
        int expectedCurrentValue = DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case13 / indent = 0 / isRelative = false / isOneTime = false / calculated value n/a
    [Fact]
    public void GetIndent_Case13()
    {
        int initialIndent = 1;
        int textIndent = 0;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = 0;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case14 / indent > 0 / isRelative = false / isOneTime = true / calculated value n/a
    [Fact]
    public void GetIndent_Case14()
    {
        int initialIndent = 2;
        int textIndent = 1;
        bool isRelative = false;
        bool isOneTime = true;
        int expectedReturnValue = DefaultTabSize;
        int expectedCurrentValue = initialIndent * DefaultTabSize;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    // Case15 / indent > 0 / isRelative = false / isOneTime = false / calculated value n/a
    [Fact]
    public void GetIndent_Case15()
    {
        int initialIndent = 2;
        int textIndent = 1;
        bool isRelative = false;
        bool isOneTime = false;
        int expectedReturnValue = textIndent * DefaultTabSize;
        int expectedCurrentValue = expectedReturnValue;

        Test_GetIndent(initialIndent,
                       textIndent,
                       isRelative,
                       isOneTime,
                       expectedCurrentValue,
                       expectedReturnValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1x")]
    public void IsValidIndentValue_ShouldLogMessageAndReturnsFalseWhenNumberStringIsInvalid(string? numberString)
    {
        // Arrange
        InitializeMocks();
        LoggerMock
            .Setup(logger => logger.Log(LogSeverity.Error, MsgIndentValueMustBeValidNumber, numberString))
            .Verifiable(Times.Once);
        IndentProcessor processor = GetIndentProcessor();

        // Act
        bool isValid = processor.IsValidIndentValue(numberString!, out int actual);

        // Assert
        isValid
            .Should()
            .BeFalse();
        actual
            .Should()
            .Be(0);
        VerifyMocks();
    }

    private static void SetCurrentIndent(IndentProcessor processor, int indent)
    {
        TextItem textItem = new(indent,
                                false,
                                false,
                                EmptyString);
        processor.GetIndent(textItem);
    }

    private IndentProcessor GetIndentProcessor()
        => new(LocaterMock.Object, LoggerMock.Object);

    private void InitializeMocks()
    {
        LocaterMock.Reset();
        LoggerMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        LocaterMock.VerifyNoOtherCalls();
        LoggerMock.VerifyNoOtherCalls();
    }

    private void Test_GetFirstTimeIndent(int firstTimeOffset,
                                         int initialIndent,
                                         int textIndent,
                                         bool isRelative,
                                         bool isOneTime,
                                         int expectedCurrentIndent,
                                         int expectedReturnValue,
                                         string? message = null)
    {
        // Arrange
        InitializeMocks();

        if (message is not null)
        {
            LocaterMock
                .Setup(locater => locater.CurrentLocationName)
                .Returns(SegmentName)
                .Verifiable(Times.Once);
            LoggerMock
                .Setup(logger => logger.Log(LogSeverity.Warning, message, SegmentName))
                .Verifiable(Times.Once);
        }

        IndentProcessor indentProcessor = GetIndentProcessor();
        SetCurrentIndent(indentProcessor, initialIndent);
        TextItem textItem = new(textIndent, isRelative, isOneTime, EmptyString);

        // Act
        int actualReturnValue = indentProcessor.GetFirstTimeIndent(firstTimeOffset, textItem);

        // Assert
        actualReturnValue
            .Should()
            .Be(expectedReturnValue);
        indentProcessor.CurrentIndent
            .Should()
            .Be(expectedCurrentIndent);
        VerifyMocks();
    }

    private void Test_GetIndent(int initialIndent,
                                int textIndent,
                                bool isRelative,
                                bool isOneTime,
                                int expectedCurrentIndent,
                                int expectedReturnValue,
                                string? message = null)
    {
        // Arrange
        InitializeMocks();

        if (message is not null)
        {
            LocaterMock
                .Setup(locater => locater.CurrentLocationName)
                .Returns(SegmentName)
                .Verifiable(Times.Once);
            LoggerMock
                .Setup(logger => logger.Log(LogSeverity.Warning, message, SegmentName))
                .Verifiable(Times.Once);
        }

        IndentProcessor indentProcessor = GetIndentProcessor();
        SetCurrentIndent(indentProcessor, initialIndent);
        TextItem textItem = new(textIndent, isRelative, isOneTime, EmptyString);

        // Act
        int actualReturnValue = indentProcessor.GetIndent(textItem);

        // Assert
        actualReturnValue
            .Should()
            .Be(expectedReturnValue);
        indentProcessor.CurrentIndent
            .Should()
            .Be(expectedCurrentIndent);
        VerifyMocks();
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

        MocksVerifyNoOtherCalls();
    }
}