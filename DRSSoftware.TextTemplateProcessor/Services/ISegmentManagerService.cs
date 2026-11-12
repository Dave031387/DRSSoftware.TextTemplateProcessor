namespace DRSSoftware.TextTemplateProcessor.Services;

/// <summary>
/// This interface defines the contract for a segment manager service where a segment is a logical
/// grouping of text lines in a text template file.
/// </summary>
internal interface ISegmentManagerService
{
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
    bool AddControlItem(ControlItem controlItem);

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
    bool AddTextItem(string segmentName, TextItem textItem);

    /// <summary>
    /// Clears the contents of the control items and text items dictionaries.
    /// </summary>
    void Clear();

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
    bool ContainsSegment(string segmentName);

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
    ControlItem? GetControlItem(string segmentName);

    /// <summary>
    /// Gets the list of text items belonging to the given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment whose text items are to be retrieved.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of text items belonging to the specified segment.
    /// </returns>
    IEnumerable<TextItem> GetTextItems(string segmentName);

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
    bool IsEmptySegment(string segmentName);

    /// <summary>
    /// Resets the IsFirstTime flag to <see langword="true"/> for all segments found in the control
    /// items dictionary.
    /// </summary>
    void ResetAllSegments();

    /// <summary>
    /// Sets the IsFirstTime flag to <see langword="true"/> for the control item belonging to the
    /// given segment name.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment whose control item is to be reset.
    /// </param>
    void ResetSegment(string segmentName);
}