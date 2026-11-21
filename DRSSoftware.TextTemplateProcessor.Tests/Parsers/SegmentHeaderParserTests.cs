using System.Text;

namespace DRSSoftware.TextTemplateProcessor.Parsers;

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
        void action() => _ = new SegmentHeaderParser(controlItemBuilder, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullIndentProcessor_ShouldThrowException()
    {
        // Arrange
        IIndentProcessor indentProcessor = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IIndentProcessor), nameof(indentProcessor));

        // Act
        void action() => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, indentProcessor, GetMock<ILogger>().Object, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, GetMock<IIndentProcessor>().Object, logger, GetMock<IOptionParser>().Object);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateSegmentHeaderParserWithNullOptionParser_ShouldThrowException()
    {
        // Arrange
        IOptionParser optionParser = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentHeaderParser), nameof(IOptionParser), nameof(optionParser));

        // Act
        void action() => _ = new SegmentHeaderParser(GetMock<IControlItemBuilder>().Object, GetMock<IIndentProcessor>().Object, GetMock<ILogger>().Object, optionParser);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
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
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (FirstTimeIndentOption, stringValue), (PadSegmentNameOption, padSegment));
        ControlItem expected = new()
        {
            FirstTimeIndent = intValue,
            IsFirstTime = true,
            PadSegment = padSegment,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, padSegment, true);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenPadSegmentMatchesSegmentName_ShouldLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (PadSegmentNameOption, segmentName));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgPadSegmentNameSameAsSegmentName, segmentName);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, segmentName, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentNameIsInvalid_ShouldLogError()
    {
        // Arrange
        string invalidSegmentName = "123ABC";
        string segmentHeaderLine = SetupOptionParserGet(invalidSegmentName);
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        string expectedMessage = string.Format(MsgInvalidSegmentName,
                                               invalidSegmentName);
        SetupControlItemBuilderMock(expected);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData($"{SegmentHeaderCode} ")]
    [InlineData($"{SegmentHeaderCode}  ")]
    public void ParseSegmentHeaderWhenSegmentNameIsMissing_ShouldLogError(string segmentHeaderLine)
    {
        // Arrange
        string[] optionStrings = [];
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = UnknownSegmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgSegmentNameIsMissing);
        GetMock<IOptionParser>()
            .Setup(x => x.GetOptionStrings(segmentHeaderLine))
            .Returns(optionStrings)
            .Verifiable(Times.Once);
        SetupControlItemBuilderMock(expected);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWhenSegmentNameIsValid_ShouldNotLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        string segmentHeaderLine = SetupOptionParserGet(segmentName);
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
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithFirstTimeIndentOptionSetToZero_ShouldLogWarning()
    {
        // Arrange
        string segmentName = "ABC123";
        int intValue = 0;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (FirstTimeIndentOption, stringValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgFirstTimeIndentSetToZero, segmentName);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(true)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidFirstTimeIndentOption_ShouldLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        int intValue = 99;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (FirstTimeIndentOption, stringValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgFirstTimeIndentIsInvalid, segmentName, stringValue);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidOption_ShouldIgnoreOption()
    {
        // Arrange
        string segmentName = "ABC123";
        string optionName = "INVALID";
        string optionValue = "value";
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (optionName, optionValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, optionName, optionValue);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidPadSegmentOptionValue_ShouldLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        string invalidPadSegmentName = "_wrong";
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (PadSegmentNameOption, invalidPadSegmentName));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgInvalidPadSegmentName, segmentName, invalidPadSegmentName);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, invalidPadSegmentName, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithInvalidTabSizeOption_ShouldLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        int intValue = 99;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (TabSizeOption, stringValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgInvalidSegmentHeaderTabSizeOption, segmentName, stringValue);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, TabSizeOption, stringValue, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue, out intValue))
            .Returns(false)
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
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
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (FirstTimeIndentOption, stringValue1), (FirstTimeIndentOption, stringValue2));
        ControlItem expected = new()
        {
            FirstTimeIndent = firstValue,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption, segmentName, FirstTimeIndentOption);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue1, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidIndentValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithMultiplePadOptions_ShouldSaveFirstValueAndLogWarning()
    {
        // Arrange
        string segmentName = "ABC123";
        string firstValue = "PadSegment1";
        string secondValue = "PadSegment2";
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (PadSegmentNameOption, firstValue), (PadSegmentNameOption, secondValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = firstValue,
            SegmentName = segmentName,
            TabSize = 0
        };
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption, segmentName, PadSegmentNameOption);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, firstValue, true);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, secondValue, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
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
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (TabSizeOption, stringValue1), (TabSizeOption, stringValue2));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = firstValue
        };
        string expectedMessage = GetMessage(MsgFoundDuplicateSegmentHeaderOption, segmentName, TabSizeOption);
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, TabSizeOption, stringValue1, true);
        GetMock<IIndentProcessor>()
            .Setup(x => x.IsValidTabSizeValue(stringValue1, out firstValue))
            .Returns(true)
            .Verifiable(Times.Once);
        SetupOptionParserParse(segmentName, TabSizeOption, stringValue2, true);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Warning, expectedMessage))
            .Verifiable(Times.Once);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithValidFirstTimeIndentOption_ShouldNotLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        int intValue = 3;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (FirstTimeIndentOption, stringValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = intValue,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, FirstTimeIndentOption, stringValue, true);
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
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithValidPadSegmentOptionValue_ShouldNotLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        string validPadSegmentName = "valid_pad_segment";
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (PadSegmentNameOption, validPadSegmentName));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = validPadSegmentName,
            SegmentName = segmentName,
            TabSize = 0
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, PadSegmentNameOption, validPadSegmentName, true);
        SegmentHeaderParser parser = GetSegmentHeaderParser();

        // Act
        ControlItem actual = parser.Parse(segmentHeaderLine);

        // Assert
        actual
            .Should()
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void ParseSegmentHeaderWithValidTabSizeOption_ShouldNotLogError()
    {
        // Arrange
        string segmentName = "ABC123";
        int intValue = 3;
        string stringValue = intValue.ToString();
        string segmentHeaderLine = SetupOptionParserGet(segmentName, (TabSizeOption, stringValue));
        ControlItem expected = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = EmptyString,
            SegmentName = segmentName,
            TabSize = intValue
        };
        SetupControlItemBuilderMock(expected);
        SetupOptionParserParse(segmentName, TabSizeOption, stringValue, true);
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
            .Be(expected);
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

    private string SetupOptionParserGet(string segmentName, params (string optionName, string optionValue)[] options)
    {
        StringBuilder textBuffer = new($"{SegmentHeaderCode} {segmentName}", 100);
        List<string> optionStrings = [segmentName];

        foreach ((string optionName, string optionValue) in options)
        {
            string optionString = $"{optionName}={optionValue}";

            if (optionStrings.Count is 0)
            {
                textBuffer.Append($" {optionString}");
            }
            else
            {
                textBuffer.Append($", {optionString}");
            }

            optionStrings.Add(optionString);
        }

        string segmentHeaderLine = textBuffer.ToString();
        GetMock<IOptionParser>()
            .Setup(x => x.GetOptionStrings(segmentHeaderLine))
            .Returns([.. optionStrings])
            .Verifiable(Times.Once);

        return segmentHeaderLine;
    }

    private void SetupOptionParserParse(string segmentName, string optionName, string optionValue, bool isValid = false)
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