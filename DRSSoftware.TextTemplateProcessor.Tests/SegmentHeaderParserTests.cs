namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class SegmentHeaderParserTests : MoqUnitTestBase
{
    [Fact]
    public void CreateSegmentHeaderParserWithNullControlItemBuilder_ShouldThrowException()
    {
        // Arrange
        IControlItemBuilder controlItemBuilder = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IControlItemBuilder), nameof(controlItemBuilder));

        // Act
        Action action = () => _ = new SegmentHeaderParser(controlItemBuilder, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        Action action = () => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, indentProcessor, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        Action action = () => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, GetMock<IIndentProcessor>().Object, logger, GetMock<IOptionParser>().Object);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullOptionParser_ShouldThrowException()
    {
        // Arrange
        IOptionParser optionParser = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IOptionParser), nameof(optionParser));

        // Act
        Action action = () => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, optionParser);

        // Assert
        action
            .Should()
            .Throw<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithValidDependencies_ShouldCreateInstance()
    {
        // Act
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Assert
        parser
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void ParseSegmentHeaderWhenMultipleOptionsArePresent_ShouldParseAllOptions()
    {
        // Arrange
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
        GetMock<IIndentProcessor>()
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
        GetMock<ILogger>()
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
        GetMock<ILogger>()
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
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFirstTimeIndentIsInvalid,
                                            segmentName,
                                            stringValue);
        GetMock<ILogger>()
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

        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFirstTimeIndentSetToZero,
                                            segmentName);
        GetMock<ILogger>()
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
        GetMock<ILogger>()
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
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption,
                                            segmentName,
                                            FirstTimeIndentOption);
        GetMock<ILogger>()
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
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption,
                                            segmentName,
                                            PadSegmentNameOption);
        GetMock<ILogger>()
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
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption,
                                            segmentName,
                                            TabSizeOption);
        GetMock<ILogger>()
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
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        string expectedMessage = GetMessage(MsgInvalidSegmentHeaderTabSizeOption,
                                            segmentName,
                                            stringValue);
        GetMock<ILogger>()
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
        GetMock<IIndentProcessor>()
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
        GetMock<IIndentProcessor>()
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
        => new(GetMock<IControlItemBuilder>().Object,
               GetMock<IIndentProcessor>().Object,
               GetMock<ILogger>().Object,
               GetMock<IOptionParser>().Object);

    private void SetupControlItemBuilderMock(ControlItem controlItem)
    {
        Mock<IControlItemBuilder> controlItemBuilderMock = GetMock<IControlItemBuilder>();
        controlItemBuilderMock
            .Setup(static x => x.Initialize())
            .Verifiable(Times.Once);

        controlItemBuilderMock
            .Setup(static x => x.Build())
            .Returns(controlItem)
            .Verifiable(Times.Once);

        controlItemBuilderMock
            .SetupSet(x => x.SegmentName = controlItem.SegmentName)
            .Verifiable(Times.Once);

        if (controlItem.FirstTimeIndent is not 0)
        {
            controlItemBuilderMock
                .SetupSet(x => x.FirstTimeIndent = controlItem.FirstTimeIndent)
                .Verifiable(Times.Once);
        }

        if (controlItem.TabSize is not 0)
        {
            controlItemBuilderMock
                .SetupSet(x => x.TabSize = controlItem.TabSize)
                .Verifiable(Times.Once);
        }

        if (!string.IsNullOrEmpty(controlItem.PadSegment))
        {
            controlItemBuilderMock
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

        GetMock<IOptionParser>()
            .Setup(x => x.Parse(optionString, IsValidSegmentHeaderOption, SegmentHeaderOptionGroup, segmentName))
            .Returns(expected)
            .Verifiable(Times.Once);
    }
}