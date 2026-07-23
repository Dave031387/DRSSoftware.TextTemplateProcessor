namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ControlItem" /> class saves the values of any options that were specified on a
/// segment header line in the text template file. This information is used to control the
/// formatting of the segment when it is being used to generate the output file.
/// </summary>
/// <remarks>
/// Each <see cref="ControlItem" /> object is associated with a single segment in the text template
/// file. This segment is called the associated segment.
/// </remarks>
internal class ControlItem
{
    internal ControlItem(int firstTimeIndent, string padSegment, int tabSize)
    {
        FirstTimeIndent = firstTimeIndent;
        PadSegment = string.IsNullOrWhiteSpace(padSegment) ? string.Empty : padSegment;
        TabSize = tabSize;
        IsFirstTime = true;
    }

    /// <summary>
    /// Gets an integer value indicating how many tab stops the first line of the associated segment
    /// should be indented the first time the segment is processed.
    /// </summary>
    internal int FirstTimeIndent
    {
        get; init;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is the first time the associated segment is
    /// being processed.
    /// </summary>
    internal bool IsFirstTime
    {
        get; set;
    }

    /// <summary>
    /// Gets the name of the pad segment that should be inserted ahead of the associated segment on
    /// the second and subsequent times the associated segment is processed.
    /// </summary>
    /// <remarks>
    /// This property will be an empty string if nothing should be inserted ahead of the associated
    /// segment.
    /// </remarks>
    internal string PadSegment
    {
        get; init;
    }

    /// <summary>
    /// Gets a value indicating whether or not the pad segment should be inserted the next time the
    /// associated segment is processed.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true" /> only when a valid pad segment has been specified and the
    /// IsFirstTime property is false.
    /// </remarks>
    internal bool ShouldGeneratePadSegment => !(string.IsNullOrEmpty(PadSegment) || IsFirstTime);

    /// <summary>
    /// Gets or sets the tab size for the associated segment.
    /// </summary>
    internal int TabSize
    {
        get; init;
    }
}