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

    [Fact]
    public void CreateSegmentHeaderParserWithNullControlItemBuilder_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IControlItemBuilder controlItemBuilder = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IControlItemBuilder), nameof(controlItemBuilder));

        // Act
        Action action = () => _ = new SegmentHeaderParser(controlItemBuilder, IndentProcessorMock.Object, LoggerMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, indentProcessor, LoggerMock.Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new SegmentHeaderParser(ControlItemBuilderMock.Object, IndentProcessorMock.Object, logger);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
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
        MocksVerifyNoOtherCalls();
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
            PadSegment = string.Empty,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error, MsgInvalidSegmentName, invalidSegmentName))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        LoggerMock
            .Setup(static x => x.Log(LogSeverity.Error, MsgSegmentNameIsMissing))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentOptionIsMissingSeparator_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgInvalidFormOfOption,
                              segmentName,
                              FirstTimeIndentOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentOptionNameIsMissing_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} =1";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgOptionNameMustPrecedeEqualsSign,
                              segmentName))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentOptionValueIsMissing_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string segmentOption = FirstTimeIndentOption;
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {segmentOption}=";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgOptionValueMustFollowEqualsSign,
                              segmentName,
                              segmentOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgFirstTimeIndentIsInvalid,
                              segmentName,
                              stringValue))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
        string stringValue = "0";
        int intValue = 0;
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning,
                              MsgFirstTimeIndentSetToZero,
                              segmentName))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgInvalidPadSegmentName,
                              segmentName,
                              invalidPadSegmentName))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidSegmentOption_ShouldLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        string invalidOption = "BAD=2";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {invalidOption}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgUnknownSegmentOptionFound,
                              segmentName,
                              invalidOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
        string stringValue = firstValue.ToString();
        int secondValue = MaxIndentValue;
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={stringValue} {FirstTimeIndentOption}={secondValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = firstValue,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning,
                              MsgFoundDuplicateOptionNameOnHeaderLine,
                              segmentName,
                              FirstTimeIndentOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning,
                              MsgFoundDuplicateOptionNameOnHeaderLine,
                              segmentName,
                              PadSegmentNameOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
        string stringValue = firstValue.ToString();
        int secondValue = MaxTabSize;
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {TabSizeOption}={stringValue} {TabSizeOption}={secondValue}";
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = firstValue
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Warning,
                              MsgFoundDuplicateOptionNameOnHeaderLine,
                              segmentName,
                              TabSizeOption))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithMultipleSegmentOptions_ShouldParseAllOptionsAndNotLogError()
    {
        // Arrange
        InitializeMocks();
        string segmentName = "ABC123";
        int firstTimeIndentValue = 2;
        int tabSizeValue = 4;
        string padSegmentName = "pad_segment";
        string segmentHeaderLine = $"{SegmentHeaderCode} {segmentName} {FirstTimeIndentOption}={firstTimeIndentValue} {TabSizeOption}={tabSizeValue} {PadSegmentNameOption}={padSegmentName}";
        ControlItem expected = new()
        {
            FirstTimeIndent = firstTimeIndentValue,
            IsFirstTime = true,
            PadSegment = padSegmentName,
            SegmentName = segmentName,
            TabSize = tabSizeValue
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(firstTimeIndentValue.ToString(), out firstTimeIndentValue))
            .Returns(true)
            .Verifiable(Times.Once);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(tabSizeValue.ToString(), out tabSizeValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        LoggerMock
            .Setup(x => x.Log(LogSeverity.Error,
                              MsgInvalidTabSizeOption,
                              segmentName,
                              stringValue))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = 0
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

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
            PadSegment = string.Empty,
            SegmentName = segmentName,
            TabSize = intValue
        };
        int counter = 0;
        int Counter() => ++counter;
        SetupControlItemBuilderMock(expected, Counter);
        IndentProcessorMock
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.ParseSegmentHeader(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected);
        VerifyMocks();
    }

    private SegmentHeaderParser GetSegmentHeaderParser()
        => new(ControlItemBuilderMock.Object, IndentProcessorMock.Object, LoggerMock.Object);

    private void InitializeMocks()
    {
        ControlItemBuilderMock.Reset();
        IndentProcessorMock.Reset();
        LoggerMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        ControlItemBuilderMock.VerifyNoOtherCalls();
        IndentProcessorMock.VerifyNoOtherCalls();
        LoggerMock.VerifyNoOtherCalls();
    }

    private void SetupControlItemBuilderMock(ControlItem controlItem)
    {
        SetupControlItemBuilderMockCommon(controlItem);

        if (!string.IsNullOrEmpty(controlItem.SegmentName))
        {
            ControlItemBuilderMock
                .SetupSet(x => x.SegmentName = controlItem.SegmentName)
                .Verifiable(Times.Once);
            ControlItemBuilderMock
                .SetupGet(static x => x.SegmentName)
                .Returns(controlItem.SegmentName)
                .Verifiable(Times.AtLeastOnce);
        }
    }

    private void SetupControlItemBuilderMock(ControlItem controlItem, Func<int> counter)
    {
        SetupControlItemBuilderMockCommon(controlItem);

        ControlItemBuilderMock
            .SetupSet(x => x.SegmentName = controlItem.SegmentName)
            .Verifiable(Times.Once);
        ControlItemBuilderMock
            .SetupGet(static x => x.SegmentName)
            .Returns(() => counter() == 1 ? string.Empty : controlItem.SegmentName)
            .Verifiable(Times.AtLeastOnce);
    }

    private void SetupControlItemBuilderMockCommon(ControlItem controlItem)
    {
        ControlItemBuilderMock
            .Setup(static x => x.Initialize())
            .Verifiable(Times.Once);

        ControlItemBuilderMock
            .Setup(static x => x.Build())
            .Returns(controlItem)
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

        MocksVerifyNoOtherCalls();
    }
}