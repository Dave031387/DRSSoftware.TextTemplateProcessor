namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class SegmentHeaderParserTests
{
    private Mock<IControlItemBuilder> ControlItemBuilderMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<IIndentProcessor> IndentProcessorMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<ILogger> LoggerMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<IOptionParser> OptionParserMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateSegmentHeaderParserWithNullControlItemBuilder_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IControlItemBuilder controlItemBuilder = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IControlItemBuilder), nameof(controlItemBuilder));

        // Act
        Action action = () => _ = new SegmentHeaderParser(controlItemBuilder, IndentProcessorMock.Object, LoggerMock.Object, OptionParserMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        VerifyMocks();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, indentProcessor, LoggerMock.Object, OptionParserMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        VerifyMocks();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, IndentProcessorMock.Object, logger, OptionParserMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        VerifyMocks();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullOptionParser_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IOptionParser optionParser = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IOptionParser), nameof(optionParser));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, IndentProcessorMock.Object, LoggerMock.Object, optionParser!);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        VerifyMocks();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithValidDependencies_ShouldCreateInstance()
    {
        // Arrange
        InitializeMocks();

        // Act
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Assert
        parser
            .Should()
            .NotBeNull();
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenMultipleOptionsArePresent_ShouldParseAllOptions()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        int intValue = 2;
        string stringValue = intValue.ToString();
        string padSegment = "PadSegment";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={intValue}, {PadSegmentNameOption}={padSegment}";
        ControlItem expected = new()
        {
            FirstTimeIndent = intValue,
            IsFirstTime = true,
            PadSegment = padSegment,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue, true);
        SetupOptionParserMock(segmentName, PadSegmentNameOption, padSegment, true);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentNameIsInvalid_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string invalidSegmentName = "123ABC";
        string segmentHeaderLine = $"{SegmentHeaderCode} {invalidSegmentName}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        string expectedMessage = string.Format(MsgInvalidSegmentName,
                                               invalidSegmentName);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData($"{SegmentHeaderCode} ")]
    [InlineData($"{SegmentHeaderCode}  ")]
    public void ParseSegmentHeaderWhenSegmentNameIsMissing_ShouldLogError(string segmentHeaderLine)
    {
        // Arrange
        InitializeMocks();
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        string expectedMessage = GetMessage(MsgSegmentNameIsMissing);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentNameIsValid_ShouldNotLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(MinIndentValue - 1)]
    [InlineData(MaxIndentValue + 1)]
    public void ParseSegmentHeaderWithFirstTimeIndentOptionOutOfRange_ShouldLogError(int intValue)
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string stringValue = intValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue, true);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFirstTimeIndentIsInvalid,
                                            segmentName,
                                            stringValue);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithFirstTimeIndentOptionSetToZero_ShouldLogWarning()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        int intValue = 0;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue, true);

        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFirstTimeIndentSetToZero,
                                            segmentName);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidOption_ShouldIgnoreOption()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string optionName = "INVALID";
        string optionValue = "value";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {optionName}={optionValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, optionName, optionValue);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidPadSegmentOptionValue_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string invalidPadSegmentName = "_wrong";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {PadSegmentNameOption}={invalidPadSegmentName}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, PadSegmentNameOption, invalidPadSegmentName, true);
        string expectedMessage = GetMessage(MsgInvalidPadSegmentName,
                                            segmentName,
                                            invalidPadSegmentName);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithMultipleFirstTimeIndentOptions_ShouldSaveFirstValueAndLogWarning()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        int firstValue = MinIndentValue;
        string stringValue1 = firstValue.ToString();
        int secondValue = MaxIndentValue;
        string stringValue2 = secondValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue1} {FirstTimeIndentOption}={stringValue2}";
        ControlItem expected = new()
        {
            FirstTimeIndent = firstValue,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue1, true);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue2, true);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFoundDuplicateOptionNameOnHeaderLine,
                                            segmentName,
                                            FirstTimeIndentOption);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithMultiplePadOptions_ShouldSaveFirstValueAndLogWarning()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string firstValue = "PadSegment1";
        string secondValue = "PadSegment2";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {PadSegmentNameOption}={firstValue} {PadSegmentNameOption}={secondValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = firstValue,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, PadSegmentNameOption, firstValue, true);
        SetupOptionParserMock(segmentName, PadSegmentNameOption, secondValue, true);
        string expectedMessage = GetMessage(MsgFoundDuplicateOptionNameOnHeaderLine,
                                            segmentName,
                                            PadSegmentNameOption);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithMultipleTabOptions_ShouldSaveFirstValueAndLogWarning()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        int firstValue = MinTabSize;
        string stringValue1 = firstValue.ToString();
        int secondValue = MaxTabSize;
        string stringValue2 = secondValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {TabSizeOption}={stringValue1} {TabSizeOption}={stringValue2}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = firstValue
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, TabSizeOption, stringValue1, true);
        SetupOptionParserMock(segmentName, TabSizeOption, stringValue2, true);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFoundDuplicateOptionNameOnHeaderLine,
                                            segmentName,
                                            TabSizeOption);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(MinTabSize - 1)]
    [InlineData(MaxTabSize + 1)]
    public void ParseSegmentHeaderWithTabSizeOptionOutOfRange_ShouldLogError(int intValue)
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string stringValue = intValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {TabSizeOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, TabSizeOption, stringValue, true);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgInvalidTabSizeOption,
                                            segmentName,
                                            stringValue);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(MinIndentValue)]
    [InlineData(MaxIndentValue)]
    public void ParseSegmentHeaderWithValidFirstTimeIndentOption_ShouldNotLogError(int intValue)
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string stringValue = intValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = intValue,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, FirstTimeIndentOption, stringValue, true);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithValidPadSegmentOptionValue_ShouldNotLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string validPadSegmentName = "valid_pad_segment";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {PadSegmentNameOption}={validPadSegmentName}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = validPadSegmentName,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, PadSegmentNameOption, validPadSegmentName, true);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(MinTabSize)]
    [InlineData(MaxTabSize)]
    public void ParseSegmentHeaderWithValidTabSizeOption_ShouldNotLogError(int intValue)
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string stringValue = intValue.ToString();
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {TabSizeOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = intValue
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserMock(segmentName, TabSizeOption, stringValue, true);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    private SegmentHeaderParser GetSegmentHeaderParser()
        => new(ControlItemBuilderMock.Object, IndentProcessorMock.Object, LoggerMock.Object, OptionParserMock.Object);

    private void InitializeMocks()
    {
        ControlItemBuilderMock.Reset();
        IndentProcessorMock.Reset();
        LoggerMock.Reset();
        OptionParserMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        ControlItemBuilderMock.VerifyNoOtherCalls();
        IndentProcessorMock.VerifyNoOtherCalls();
        LoggerMock.VerifyNoOtherCalls();
        OptionParserMock.VerifyNoOtherCalls();
    }

    private void SetupControlItemBuilderMock(ControlItem controlItem)
    {
        ControlItemBuilderMock
            .Setup(static x => x.Initialize())
            .Verifiable(Times.Once);

        ControlItemBuilderMock
            .Setup(static x => x.Build())
            .Returns(controlItem)
            .Verifiable(Times.Once);

        ControlItemBuilderMock
            .SetupSet(x => x.SegmentName = controlItem.SegmentName)
            .Verifiable(Times.Once);

        if (controlItem.FirstTimeIndent is not 0)
        {
            ControlItemBuilderMock
                .SetupSet(x => x.FirstTimeIndent = controlItem.FirstTimeIndent)
                .Verifiable(Times.Once);
        }

        if (controlItem.TabSize is not 0)
        {
            ControlItemBuilderMock
                .SetupSet(x => x.TabSize = controlItem.TabSize)
                .Verifiable(Times.Once);
        }

        if (!string.IsNullOrEmpty(controlItem.PadSegment))
        {
            ControlItemBuilderMock
                .SetupSet(x => x.PadSegment = controlItem.PadSegment)
                .Verifiable(Times.Once);
        }
    }

    private void SetupOptionParserMock(string segmentName, string optionName, string optionValue, bool isValid = false)
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

        OptionParserMock
            .Setup(x => x.Parse(optionString, IsValidSegmentHeaderOption, SegmentHeaderOptionGroup, segmentName))
            .Returns(expected)
            .Verifiable(Times.Once);
    }

    private void VerifyMocks()
    {
        if (ControlItemBuilderMock.Setups.Any())
        {
            ControlItemBuilderMock.VerifyAll();
        }

        if (IndentProcessorMock.Setups.Any())
        {
            IndentProcessorMock.VerifyAll();
        }

        if (LoggerMock.Setups.Any())
        {
            LoggerMock.VerifyAll();
        }

        if (OptionParserMock.Setups.Any())
        {
            OptionParserMock.VerifyAll();
        }

        MocksVerifyNoOtherCalls();
    }
}