namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// Defines the interface for a builder that creates instances of the <see cref="ControlItem" />
/// class.
/// </summary>
internal interface IControlItemBuilder
{
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
    }

    /// <summary>
    /// Gets or sets the name of the segment that this control item object belongs to.
    /// </summary>
    public string SegmentName
    {
        get;
        set;
    }

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
    ControlItem Build();

    /// <summary>
    /// Initializes the <see cref="ControlItemBuilder" /> instance.
    /// </summary>
    public void Initialize();
}