namespace DRSSoftware.TextTemplateProcessor.Services;

[ExcludeFromCodeCoverage]
public class SegmentManagerTests : MoqUnitTestBase
{
    [Fact]
    public void AddControlItemWhenNoErrors_ShouldAddControlItemAndReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddControlItem(controlItem);

        // Assert
        actual
            .Should()
            .BeTrue();
        manager.ContainsSegment(segmentName)
            .Should()
            .BeTrue();
        manager.IsEmptySegment(segmentName)
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void AddControlItemWhenPadSegmentIsDefinedEarlier_ShouldAddControlItemAndReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        string padSegment = "padSegment";
        ControlItem controlItem1 = new()
        {
            SegmentName = padSegment
        };
        ControlItem controlItem2 = new()
        {
            SegmentName = segmentName,
            PadSegment = padSegment
        };
        ControlItem expected = new()
        {
            SegmentName = segmentName,
            PadSegment = padSegment
        };
        string message1 = GetMessage(MsgSegmentHasBeenAdded, padSegment);
        string message2 = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem1);

        // Act
        bool actual = manager.AddControlItem(controlItem2);

        // Assert
        actual
            .Should()
            .BeTrue();
        manager.ContainsSegment(segmentName)
            .Should()
            .BeTrue();
        manager.IsEmptySegment(segmentName)
            .Should()
            .BeTrue();
        manager.GetControlItem(segmentName)
            .Should()
            .NotBeNull()
            .And
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void AddControlItemWhenPadSegmentNotDefinedEarlier_ShouldLogErrorAndChangePadSegmentToEmptyString()
    {
        // Arrange
        string segmentName = "segment1";
        string padSegment = "padSegment";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName,
            PadSegment = padSegment
        };
        ControlItem expected = new()
        {
            SegmentName = segmentName,
            PadSegment = EmptyString
        };
        string message1 = GetMessage(MsgPadSegmentMustBeDefinedEarlier, segmentName, padSegment);
        string message2 = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddControlItem(controlItem);

        // Assert
        actual
            .Should()
            .BeTrue();
        manager.ContainsSegment(segmentName)
            .Should()
            .BeTrue();
        manager.IsEmptySegment(segmentName)
            .Should()
            .BeTrue();
        manager.GetControlItem(segmentName)
            .Should()
            .NotBeNull()
            .And
            .Be(expected);
        VerifyMocks();
    }

    [Fact]
    public void AddControlItemWhenSegmentAlreadyExists_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem1 = new()
        {
            SegmentName = segmentName
        };
        ControlItem controlItem2 = new()
        {
            SegmentName = segmentName,
            FirstTimeIndent = 2
        };
        string message1 = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        string message2 = GetMessage(MsgAttemptToAddDuplicateSegmentName, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message2))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem1);

        // Act
        bool actual = manager.AddControlItem(controlItem2);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void AddControlItemWhenSegmentNameIsEmptyOrWhitespace_ShouldLogErrorMessageAndReturnFalse(string segmentName)
    {
        // Arrange
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgUnableToAddSegmentToControlDictionary,
                                    segmentName,
                                    MsgControlItemSegmentNameNullOrWhitespace);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddControlItem(controlItem);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void AddControlItemWhenSegmentNameIsNull_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string segmentName = null!;
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgUnableToAddSegmentToControlDictionary,
                                    NullStringValue,
                                    MsgControlItemSegmentNameNullOrWhitespace);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddControlItem(controlItem);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void AddTextItemWhenSegmentIsDefined_ShouldAddTextItemAndReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        TextItem textItem = new(2, false, true, "text");
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);

        // Act
        bool actual = manager.AddTextItem(segmentName, textItem);

        // Assert
        actual
            .Should()
            .BeTrue();
        manager.IsEmptySegment(segmentName)
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void AddTextItemWhenSegmentNameIsEmptyOrWhitespace_ShouldLogErrorMessageAndReturnFalse(string segmentName)
    {
        // Arrange
        TextItem textItem = new(0, true, false, "text");
        string message = GetMessage(MsgUnableToAddTextItemForSegment,
                                    segmentName,
                                    MsgTextItemSegmentNameInvalid);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddTextItem(segmentName, textItem);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void AddTextItemWhenSegmentNameIsNull_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string segmentName = null!;
        TextItem textItem = new(0, true, false, "text");
        string message = GetMessage(MsgUnableToAddTextItemForSegment,
                                    NullStringValue,
                                    MsgTextItemSegmentNameInvalid);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddTextItem(segmentName, textItem);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void AddTextItemWhenSegmentNotDefined_ShouldLogErrorMessageAndReturnFalse()
    {
        // Arrange
        string segmentName = "segment1";
        TextItem textItem = new(0, true, false, "text");
        string message = GetMessage(MsgAttemptToAddTextItemForNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.AddTextItem(segmentName, textItem);

        // Assert
        actual
            .Should()
            .BeFalse();
        manager.ContainsSegment(segmentName)
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void AddTextItemWhenTextIsNull_ShouldLogErrorMessageAndRemoveSegmentFromDictionaryAndReturnFalse()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        TextItem textItem1 = new(3, true, true, "text");
        TextItem textItem2 = new(0, true, false, null!);
        string message1 = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        string message2 = GetMessage(MsgUnableToAddTextItemForSegment,
                                     segmentName,
                                     MsgTextItemTextIsNull);
        string message3 = GetMessage(MsgSegmentWillBeRemoved, segmentName);
        string message4 = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message2))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message3))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message4))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);
        _ = manager.AddTextItem(segmentName, textItem1);

        // Act
        bool actual = manager.AddTextItem(segmentName, textItem2);
        IEnumerable<TextItem> textItems = manager.GetTextItems(segmentName);

        // Assert
        actual
            .Should()
            .BeFalse();
        manager.ContainsSegment(segmentName)
            .Should()
            .BeFalse();
        textItems
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void ContainsSegmentWhenSegmentNameExistsInDictionary_ShouldReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);

        // Act
        bool actual = manager.ContainsSegment(segmentName);

        // Assert
        actual
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    [InlineData("segment1")]
    public void ContainsSegmentWhenSegmentNameIsEmptyWhitespaceOrNotFound_ShouldReturnFalse(string segmentName)
    {
        // Arrange
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.ContainsSegment(segmentName);

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ContainsSegmentWhenSegmentNameIsNull_ShouldReturnFalse()
    {
        // Arrange
        string segmentName = null!;
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.ContainsSegment(segmentName);

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CreateSegmentManagerWithNullLogger_ShouldThrowException()
    {
        // Arrange
        ILogger logger = null!;
        string expected = GetNullDependencyMessage(nameof(SegmentManager), nameof(ILogger), nameof(logger));

        // Act
        void action() => _ = new SegmentManager(logger);

        // Assert
        AssertException<ArgumentNullException>(action, expected);
    }

    [Fact]
    public void CreateSegmentManagerWithValidDependencies_ShouldSucceed()
    {
        // Act
        SegmentManager actual = GetSegmentManager();

        // Assert
        actual
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void GetControlItemWhenSegmentNameExists_ShouldReturnControlItem()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem newControlItem = new()
        {
            SegmentName = segmentName
        };
        ControlItem expected = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(newControlItem);

        // Act
        ControlItem? actual = manager.GetControlItem(segmentName);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .Be(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void GetControlItemWhenSegmentNameIsEmptyOrWhitespace_ShouldLogErrorMessageAndReturnNull(string segmentName)
    {
        // Arrange
        string message = GetMessage(MsgAttemptToRetrieveControlItemForInvalidSegmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        ControlItem? controlItem = manager.GetControlItem(segmentName);

        // Assert
        controlItem
            .Should()
            .BeNull();
        VerifyMocks();
    }

    [Fact]
    public void GetControlItemWhenSegmentNameIsNull_ShouldLogErrorMessageAndReturnNull()
    {
        // Arrange
        string segmentName = null!;
        string message = GetMessage(MsgAttemptToRetrieveControlItemForInvalidSegmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        ControlItem? controlItem = manager.GetControlItem(segmentName);

        // Assert
        controlItem
            .Should()
            .BeNull();
        VerifyMocks();
    }

    [Fact]
    public void GetControlItemWhenSegmentNameNotFound_ShouldLogErrorMessageAndReturnNull()
    {
        // Arrange
        string segmentName = "segment1";
        string message = GetMessage(MsgAttemptToRetrieveControlItemForNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        ControlItem? controlItem = manager.GetControlItem(segmentName);

        // Assert
        controlItem
            .Should()
            .BeNull();
        VerifyMocks();
    }

    [Fact]
    public void GetTextItemsWhenSegmentDoesNotExist_ShouldLogErrorMessageAndReturnEmptyCollection()
    {
        // Arrange
        string segmentName = "segment1";
        string message = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        IEnumerable<TextItem> actual = manager.GetTextItems(segmentName);

        // Assert
        actual
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void GetTextItemsWhenSegmentExists_ShouldReturnTextItems()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        TextItem textItem1 = new(0, true, false, "text1");
        TextItem textItem2 = new(1, true, false, "text2");
        List<TextItem> expected = [textItem1, textItem2];
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);
        _ = manager.AddTextItem(segmentName, textItem1);
        _ = manager.AddTextItem(segmentName, textItem2);

        // Act
        IEnumerable<TextItem> actual = manager.GetTextItems(segmentName);

        // Assert
        actual
            .Should()
            .BeEqualTo(expected);
        VerifyMocks();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void GetTextItemsWhenSegmentNameIsNullEmptyOrWhitespace_ShouldLogErrorMessageAndReturnEmptyCollection(string? segmentName)
    {
        // Arrange
        string message = GetMessage(MsgAttemptToRetrieveTextItemsForInvalidSegmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        IEnumerable<TextItem> actual = manager.GetTextItems(segmentName!);

        // Assert
        actual
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void Initialize_ShouldClearAllSegmentsFromTheControlItemsAndTextItemsDictionaries()
    {
        // Arrange
        string segmentName1 = "segment1";
        string segmentName2 = "segment2";
        ControlItem controlItem1 = new()
        {
            SegmentName = segmentName1
        };
        ControlItem controlItem2 = new()
        {
            SegmentName = segmentName2
        };
        TextItem textItem1 = new(6, false, false, "text1");
        TextItem textItem2 = new(4, true, false, "text2");
        string message1 = GetMessage(MsgSegmentHasBeenAdded, segmentName1);
        string message2 = GetMessage(MsgSegmentHasBeenAdded, segmentName2);
        string message3 = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName1);
        string message4 = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName2);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message3))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, message4))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem1);
        _ = manager.AddControlItem(controlItem2);
        _ = manager.AddTextItem(segmentName1, textItem1);
        _ = manager.AddTextItem(segmentName2, textItem2);

        // Act
        manager.Initialize();
        IEnumerable<TextItem> textItems1 = manager.GetTextItems(segmentName1);
        IEnumerable<TextItem> textItems2 = manager.GetTextItems(segmentName2);

        // Assert
        manager.ContainsSegment(segmentName1)
            .Should()
            .BeFalse();
        manager.ContainsSegment(segmentName2)
            .Should()
            .BeFalse();
        textItems1
            .Should()
            .BeEmpty();
        textItems2
            .Should()
            .BeEmpty();
        VerifyMocks();
    }

    [Fact]
    public void IsEmptySegmentWhenSegmentHasNoTextItems_ShouldReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);

        // Act
        bool actual = manager.IsEmptySegment(segmentName);

        // Assert
        actual
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void IsEmptySegmentWhenSegmentHasTextItems_ShouldReturnFalse()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName
        };
        TextItem textItem = new(-1, true, false, "text");
        string message = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);
        _ = manager.AddTextItem(segmentName, textItem);

        // Act
        bool actual = manager.IsEmptySegment(segmentName);

        // Assert
        actual
            .Should()
            .BeFalse();
        VerifyMocks();
    }

    [Fact]
    public void IsEmptySegmentWhenSegmentIsNotDefined_ShouldReturnTrue()
    {
        // Arrange
        string segmentName = "segment1";
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.IsEmptySegment(segmentName);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void IsEmptySegmentWhenSegmentNameIsNullEmptyOrWhitespace_ShouldReturnTrue(string? segmentName)
    {
        // Arrange
        SegmentManager manager = GetSegmentManager();

        // Act
        bool actual = manager.IsEmptySegment(segmentName!);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ResetAllSegmentsWhenNoSegmentsDefined_ShouldDoNothing()
    {
        // Arrange
        SegmentManager manager = GetSegmentManager();

        // Act
        manager.ResetAllSegments();

        // Assert
        GetMock<ILogger>()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public void ResetAllSegmentsWhenSegmentsAreDefined_ShouldResetAllSegmentsAndLogMessage()
    {
        // Arrange
        string segmentName1 = "segment1";
        string segmentName2 = "segment2";
        ControlItem controlItem1 = new()
        {
            SegmentName = segmentName1,
            IsFirstTime = false
        };
        ControlItem controlItem2 = new()
        {
            SegmentName = segmentName2,
            IsFirstTime = false
        };
        string message1 = GetMessage(MsgSegmentHasBeenAdded, segmentName1);
        string message2 = GetMessage(MsgSegmentHasBeenAdded, segmentName2);
        string message3 = GetMessage(MsgAllSegmentsHaveBeenReset);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message2))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Reset, message3))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem1);
        _ = manager.AddControlItem(controlItem2);

        // Act
        manager.ResetAllSegments();

        // Assert
        manager.GetControlItem(segmentName1)!.IsFirstTime
            .Should()
            .BeTrue();
        manager.GetControlItem(segmentName2)!.IsFirstTime
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Fact]
    public void ResetSegmentWhenSegmentDoesNotExist_ShouldLogErrorMessage()
    {
        // Arrange
        string segmentName = "segment1";
        string message = GetMessage(MsgAttemptToResetNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, OperationType.Reset, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        manager.ResetSegment(segmentName);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ResetSegmentWhenSegmentExists_ShouldResetSegmentAndLogMessage()
    {
        // Arrange
        string segmentName = "segment1";
        ControlItem controlItem = new()
        {
            SegmentName = segmentName,
            IsFirstTime = false
        };
        string message1 = GetMessage(MsgSegmentHasBeenAdded, segmentName);
        string message2 = GetMessage(MsgSegmentHasBeenReset, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, message1))
            .Verifiable(Times.Once);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Information, OperationType.Reset, message2))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();
        _ = manager.AddControlItem(controlItem);

        // Act
        manager.ResetSegment(segmentName);

        // Assert
        manager.GetControlItem(segmentName)!.IsFirstTime
            .Should()
            .BeTrue();
        VerifyMocks();
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(Whitespace)]
    public void ResetSegmentWhenSegmentNameIsEmptyOrWhitespace_ShouldLogErrorMessage(string segmentName)
    {
        // Arrange
        string message = GetMessage(MsgAttemptToResetNonExistingSegment, segmentName);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, OperationType.Reset, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        manager.ResetSegment(segmentName);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void ResetSegmentWhenSegmentNameIsNull_ShouldLogErrorMessage()
    {
        // Arrange
        string segmentName = null!;
        string message = GetMessage(MsgAttemptToResetNonExistingSegment, NullStringValue);
        GetMock<ILogger>()
            .Setup(x => x.Log(LogSeverity.Error, OperationType.Reset, message))
            .Verifiable(Times.Once);
        SegmentManager manager = GetSegmentManager();

        // Act
        manager.ResetSegment(segmentName);

        // Assert
        VerifyMocks();
    }

    private SegmentManager GetSegmentManager() => new(GetMock<ILogger>().Object);
}