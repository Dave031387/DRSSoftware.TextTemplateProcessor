namespace DRSSoftware.TextTemplateProcessor.Services;

/// <summary>
/// The <see cref="SegmentManagerService"/> manages the segment control information and segment text
/// lines that are parsed from the text template file.
/// </summary>
internal class SegmentManagerService : DependencyCheckerBase, ISegmentManagerService
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
    /// Creates a new instance of the <see cref="SegmentManagerService"/> class.
    /// </summary>
    /// <param name="logger">
    ///A reference to the logger service.
    /// </param>
    internal SegmentManagerService(ILogger logger)
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
            if (ContainsSegment(controlItem.SegmentName))
            {
                message = GetMessage(MsgAttemptToAddDuplicateSegmentName, controlItem.SegmentName);
                Logger.Log(LogSeverity.Error, message);
                return false;
            }

            _controlItems.Add(controlItem.SegmentName, controlItem);
            _textItems.Add(controlItem.SegmentName, []);
            message = GetMessage(MsgSegmentHasBeenAdded, controlItem.SegmentName);
            Logger.Log(LogSeverity.Information, message);
            return true;
        }
        catch (Exception ex)
        {
            message = GetMessage(MsgUnableToAddSegmentToControlDictionary, controlItem.SegmentName, ex.Message);
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
        if (ContainsSegment(segmentName))
        {
            try
            {
                _textItems[segmentName].Add(textItem);
                return true;
            }
            catch (Exception ex)
            {
                string message = GetMessage(MsgUnableToAddTextItemForSegment, segmentName, ex.Message);
                Logger.Log(LogSeverity.Error, message);
                message = GetMessage(MsgSegmentWillBeRemoved, segmentName);
                Logger.Log(LogSeverity.Information, message);
                _ = _textItems.Remove(segmentName);
                _ = _controlItems.Remove(segmentName);
                return false;
            }
        }
        else
        {
            string message = GetMessage(MsgAttemptToAddTextItemForNonExistingSegment, segmentName);
            Logger.Log(LogSeverity.Error, message);
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
    public bool ContainsSegment(string segmentName) => _controlItems.ContainsKey(segmentName);

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
        if (_controlItems.TryGetValue(segmentName, out ControlItem? controlItem))
        {
            return controlItem;
        }

        string message = GetMessage(MsgAttemptToRetrieveControlItemForNonExistingSegment, segmentName);
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
    /// An <see cref="IEnumerable{T}"/> of text items belonging to the specified segment.
    /// </returns>
    public IEnumerable<TextItem> GetTextItems(string segmentName)
    {
        if (_textItems.TryGetValue(segmentName, out List<TextItem>? textItems))
        {
            return textItems;
        }

        string message = GetMessage(MsgAttemptToRetrieveTextItemsForNonExistingSegment, segmentName);
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
        if (ContainsSegment(segmentName))
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
        foreach (ControlItem controlItem in _controlItems.Values)
        {
            controlItem.IsFirstTime = true;
        }

        string message = GetMessage(MsgAllSegmentsHaveBeenReset);
        Logger.Log(LogSeverity.Information, OperationType.Reset, message);
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
        if (ContainsSegment(segmentName))
        {
            _controlItems[segmentName].IsFirstTime = true;
            string message = GetMessage(MsgSegmentHasBeenReset, segmentName);
            Logger.Log(LogSeverity.Information, OperationType.Reset, message);
        }
        else
        {
            string message = GetMessage(MsgAttemptToResetNonExistingSegment, segmentName);
            Logger.Log(LogSeverity.Error, OperationType.Reset, message);
        }
    }
}