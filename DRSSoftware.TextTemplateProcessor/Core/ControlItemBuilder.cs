namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ControlItemBuilder" /> class provides a way to create and configure instances of
/// the <see cref="ControlItem" /> class.
/// </summary>
internal class ControlItemBuilder : IControlItemBuilder
{
    /// <summary>
    /// Create a new instance of the <see cref="ControlItemBuilder" /> class with default values.
    /// </summary>
    internal ControlItemBuilder() => Initialize();

    /// <summary>
    /// Gets or sets an integer value indicating how many tab stops the first line of the associated
    /// segment should be indented the first time the segment is processed.
    /// </summary>
    public int FirstTimeIndent
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the name of the pad segment that should be inserted ahead of the associated
    /// segment on the second and subsequent times the associated segment is processed.
    /// </summary>
    /// <remarks>
    /// This property will be an empty string if nothing should be inserted ahead of the associated
    /// segment.
    /// </remarks>
    public string PadSegment
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the segment that this control item object belongs to.
    /// </summary>
    public string SegmentName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the tab size for the associated segment.
    /// </summary>
    public int TabSize
    {
        get;
        set;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ControlItem" /> class with the values from this
    /// builder.
    /// </summary>
    /// <returns>
    /// A new instance of the <see cref="ControlItem" /> class with the values from this builder.
    /// </returns>
    public ControlItem Build()
    {
        return new ControlItem
        {
            FirstTimeIndent = FirstTimeIndent,
            PadSegment = PadSegment,
            SegmentName = SegmentName,
            TabSize = TabSize,
            IsFirstTime = true
        };
    }

    /// <summary>
    /// Initializes this <see cref="ControlItemBuilder" /> instance.
    /// </summary>
    public void Initialize()
    {
        FirstTimeIndent = 0;
        PadSegment = string.Empty;
        SegmentName = string.Empty;
        TabSize = 0;
    }
}