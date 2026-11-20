namespace DRSSoftware.TextTemplateProcessor.Services;

/// <summary>
/// The <see cref="SegmentManager"/> manages the segment control information and segment text lines
/// that are parsed from the text template file.
/// </summary>
internal class SegmentManager : DependencyCheckerBase, ISegmentManager
{
    /// <summary>
    /// A dictionary of control items. The key is the segment name that each control item pertains to.
    /// </summary>
    private readonly Dictionary<string, ControlItem> _controlItems = [];

    /// <summary>
    /// A dictionary of text item lists. The key is the segment name that each text item list
    /// pertains to.
    /// </summary>
    private readonly Dictionary<string, List<TextItem>> _textItems = [];

    /// <summary>
    /// Creates a new instance of the <see cref="SegmentManager"/> class.
    /// </summary>
    /// <param name="logger">
    /// A reference to the logger service.
    /// </param>
    internal SegmentManager(ILogger logger)
        => Logger = NullDependencyCheck(logger, nameof(logger));

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get;
        init;
    }

    /// <summary>
    /// Adds the given control item to the control items dictionary.
    /// </summary>
    /// <param name="controlItem">
    /// The control item to be added to the dictionary.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the control item is successfully added to the dictionary;
    /// otherwise, <see langword="false"/>
    /// </returns>
    public bool AddControlItem(ControlItem controlItem)
    {
        string message;

        try
        {
            ControlItem validatedControlItem = ValidateControlItem(controlItem);

            if (ContainsSegment(validatedControlItem.SegmentName))
            {
                message = GetMessage(MsgAttemptToAddDuplicateSegmentName, validatedControlItem.SegmentName);
                Logger.Log(LogSeverity.Error, message);
                return false;
            }

            _controlItems.Add(validatedControlItem.SegmentName, validatedControlItem);
            _textItems.Add(validatedControlItem.SegmentName, []);
            message = GetMessage(MsgSegmentHasBeenAdded, validatedControlItem.SegmentName);
            Logger.Log(LogSeverity.Information, message);
            return true;
        }
        catch (Exception ex)
        {
            string segmentName = controlItem is null || controlItem.SegmentName is null ? NullStringValue : controlItem.SegmentName;
            message = GetMessage(MsgUnableToAddSegmentToControlDictionary, segmentName, ex.Message);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }
    }

    /// <summary>
    /// Adds the given text item to the text items dictionary for the given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment to which the text item is to be added.
    /// </param>
    /// <param name="textItem">
    /// The text item that is to be added to the dictionary.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the text item is successfully added to the dictionary; otherwise,
    /// <see langword="false"/>
    /// </returns>
    public bool AddTextItem(string segmentName, TextItem textItem)
    {
        try
        {
            ValidateTextItem(segmentName, textItem);

            if (ContainsSegment(segmentName))
            {
                _textItems[segmentName].Add(textItem);
                return true;
            }
            else
            {
                string message = GetMessage(MsgAttemptToAddTextItemForNonExistingSegment, segmentName);
                Logger.Log(LogSeverity.Error, message);
                return false;
            }
        }
        catch (Exception ex)
        {
            string segment = segmentName is null ? NullStringValue : segmentName;
            string message = GetMessage(MsgUnableToAddTextItemForSegment, segment, ex.Message);
            Logger.Log(LogSeverity.Error, message);

            if (!string.IsNullOrWhiteSpace(segmentName))
            {
                message = GetMessage(MsgSegmentWillBeRemoved, segmentName);
                Logger.Log(LogSeverity.Information, message);
                _ = _textItems.Remove(segmentName);
                _ = _controlItems.Remove(segmentName);
            }

            return false;
        }
    }

    /// <summary>
    /// Clears the contents of the control items and text items dictionaries.
    /// </summary>
    public void Clear()
    {
        _textItems.Clear();
        _controlItems.Clear();
        string message = GetMessage(MsgAllSegmentsHaveBeenCleared);
        Logger.Log(LogSeverity.Information, OperationType.Reset, message);
    }

    /// <summary>
    /// Gets a value indicating whether or not the given segment name exists in the control items dictionary.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment to look for in the control items dictionary.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given segment exists in the control items dictionary;
    /// otherwise, <see langword="false"/>
    /// </returns>
    public bool ContainsSegment(string segmentName)
        => !string.IsNullOrWhiteSpace(segmentName) && _controlItems.ContainsKey(segmentName);

    /// <summary>
    /// Gets the control item for the given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment whose control item is to be retrieved.
    /// </param>
    /// <returns>
    /// The control item for the given segment name, or <see langword="null"/> if the segment can't
    /// be found.
    /// </returns>
    public ControlItem? GetControlItem(string segmentName)
    {
        string message;

        if (string.IsNullOrWhiteSpace(segmentName))
        {
            message = GetMessage(MsgAttemptToRetrieveControlItemForInvalidSegmentName);
        }
        else if (_controlItems.TryGetValue(segmentName, out ControlItem? controlItem))
        {
            return controlItem;
        }
        else
        {
            message = GetMessage(MsgAttemptToRetrieveControlItemForNonExistingSegment, segmentName);
        }

        Logger.Log(LogSeverity.Error, message);
        return null;
    }

    /// <summary>
    /// Gets the list of text items belonging to the given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment whose text items are to be retrieved.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of text items belonging to the specified segment, or an
    /// empty collection if the segment doesn't exist.
    /// </returns>
    public IEnumerable<TextItem> GetTextItems(string segmentName)
    {
        string message;

        if (string.IsNullOrWhiteSpace(segmentName))
        {
            message = GetMessage(MsgAttemptToRetrieveTextItemsForInvalidSegmentName);
        }
        else if (_textItems.TryGetValue(segmentName, out List<TextItem>? textItems))
        {
            return textItems;
        }
        else
        {
            message = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName);
        }

        Logger.Log(LogSeverity.Error, message);
        return [];
    }

    /// <summary>
    /// Gets a value indicating whether the given segment name has no associated text items.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment to be checked for text lines.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given segment name has no associated text items; otherwise,
    /// <see langword="false"/>
    /// </returns>
    public bool IsEmptySegment(string segmentName)
    {
        if (!string.IsNullOrWhiteSpace(segmentName))
        {
            if (_textItems.TryGetValue(segmentName, out List<TextItem>? textItem))
            {
                return textItem.Count is 0;
            }
        }

        return true;
    }

    /// <summary>
    /// Resets the IsFirstTime flag to <see langword="true"/> for all segments found in the control
    /// items dictionary.
    /// </summary>
    public void ResetAllSegments()
    {
        if (_controlItems.Count > 0)
        {
            foreach (ControlItem controlItem in _controlItems.Values)
            {
                controlItem.IsFirstTime = true;
            }

            string message = GetMessage(MsgAllSegmentsHaveBeenReset);
            Logger.Log(LogSeverity.Information, OperationType.Reset, message);
        }
    }

    /// <summary>
    /// Sets the IsFirstTime flag to <see langword="true"/> for the control item belonging to the
    /// given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment whose control item is to be reset.
    /// </param>
    public void ResetSegment(string segmentName)
    {
        if (!string.IsNullOrWhiteSpace(segmentName) && ContainsSegment(segmentName))
        {
            _controlItems[segmentName].IsFirstTime = true;
            string message = GetMessage(MsgSegmentHasBeenReset, segmentName);
            Logger.Log(LogSeverity.Information, OperationType.Reset, message);
        }
        else
        {
            string segment = segmentName is null ? NullStringValue : segmentName;
            string message = GetMessage(MsgAttemptToResetNonExistingSegment, segment);
            Logger.Log(LogSeverity.Error, OperationType.Reset, message);
        }
    }

    /// <summary>
    /// Validates the given <see cref="TextItem"/> object to ensure that it is not <see
    /// langword="null"/> and that its Text property is not <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// An exception will be thrown if the <see cref="TextItem"/> object fails validation, or if the
    /// given segment name is <see langword="null"/>, empty, or whitespace.
    /// </remarks>
    /// <param name="segmentName">
    /// The segment name that the given <see cref="TextItem"/> object belongs to.
    /// </param>
    /// <param name="textItem">
    /// The <see cref="TextItem"/> object to be validated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <see cref="TextItem"/> object is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the Text property of the <see cref="TextItem"/> object is <see langword="null"/>
    /// or if the segment name is <see langword="null"/>, empty, or whitespace.
    /// </exception>
    private static void ValidateTextItem(string? segmentName, TextItem? textItem)
    {
        ArgumentNullException.ThrowIfNull(textItem, nameof(textItem));

        if (textItem.Text is null)
        {
            string message = GetMessage(MsgTextItemTextIsNull);
            throw new ArgumentException(message);
        }

        if (string.IsNullOrWhiteSpace(segmentName))
        {
            string message = GetMessage(MsgTextItemSegmentNameInvalid);
            throw new ArgumentException(message);
        }
    }

    /// <summary>
    /// Validates the given <see cref="ControlItem"/> object to ensure that it is not <see
    /// langword="null"/> and its SegmentName property is not <see langword="null"/>, empty, or whitespace.
    /// </summary>
    /// <remarks>
    /// An exception will be thrown if the <see cref="ControlItem"/> object fails validation. Also,
    /// the PadSegment property will be set to an empty string if the specified pad segment hasn't
    /// already been added to the control item dictionary.
    /// </remarks>
    /// <param name="controlItem">
    /// The <see cref="ControlItem"/> object to be validated.
    /// </param>
    /// <returns>
    /// A validated <see cref="ControlItem"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <see cref="ControlItem"/> object is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the SegmentName property of the <see cref="ControlItem"/> object is <see
    /// langword="null"/>, empty, or whitespace.
    /// </exception>
    private ControlItem ValidateControlItem(ControlItem? controlItem)
    {
        ArgumentNullException.ThrowIfNull(controlItem, nameof(controlItem));

        if (string.IsNullOrWhiteSpace(controlItem.SegmentName))
        {
            string message = GetMessage(MsgControlItemSegmentNameNullOrWhitespace);
            throw new ArgumentException(message);
        }

        if (!string.IsNullOrWhiteSpace(controlItem.PadSegment))
        {
            if (!_controlItems.ContainsKey(controlItem.PadSegment))
            {
                string message = GetMessage(MsgPadSegmentMustBeDefinedEarlier, controlItem.SegmentName, controlItem.PadSegment);
                Logger.Log(LogSeverity.Error, message);
                return new()
                {
                    SegmentName = controlItem.SegmentName,
                    PadSegment = string.Empty,
                    FirstTimeIndent = controlItem.FirstTimeIndent,
                    TabSize = controlItem.TabSize,
                    IsFirstTime = controlItem.IsFirstTime
                };
            }
        }

        return controlItem;
    }
}