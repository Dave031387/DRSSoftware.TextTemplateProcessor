namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ControlItem"/> class saves the values of any options that were specified on a
/// segment header line in the text template file. This information is used to control the
/// formatting of the segment when it is being used to generate the output file.
/// </summary>
/// <remarks>
/// Each <see cref="ControlItem"/> object is associated with a single segment in the text template
/// file. This segment is called the associated segment.
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class ControlItem : IEquatable<ControlItem>
{
    /// <summary>
    /// Creates a new instance of the <see cref="ControlItem"/> class with default values.
    /// </summary>
    internal ControlItem()
    {
        FirstTimeIndent = 0;
        PadSegment = string.Empty;
        SegmentName = string.Empty;
        TabSize = 0;
        IsFirstTime = true;
    }

    /// <summary>
    /// Gets an integer value indicating how many tab stops the first line of the associated segment
    /// should be indented the first time the segment is processed.
    /// </summary>
    internal int FirstTimeIndent
    {
        get;
        init;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is the first time the associated segment is
    /// being processed.
    /// </summary>
    internal bool IsFirstTime
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the name of the pad segment that should be inserted ahead of the associated segment on
    /// the second and subsequent times the associated segment is processed.
    /// </summary>
    /// <remarks>
    /// This property will be an empty string if nothing should be inserted ahead of the associated segment.
    /// </remarks>
    internal string PadSegment
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the name of the segment that this control item object belongs to.
    /// </summary>
    internal string SegmentName
    {
        get;
        init;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the pad segment should be inserted the next
    /// time the associated segment is processed.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> only when a valid pad segment has been specified and the
    /// IsFirstTime property is false.
    /// </remarks>
    internal bool ShouldGeneratePadSegment => !(string.IsNullOrEmpty(PadSegment) || IsFirstTime);

    /// <summary>
    /// Gets the tab size for the associated segment.
    /// </summary>
    internal int TabSize
    {
        get;
        init;
    }

    /// <summary>
    /// Determines whether this <see cref="ControlItem"/> object is equivalent to another <see
    /// cref="ControlItem"/> object.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="ControlItem"/> object to be compared to this <see cref="ControlItem"/> object.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the other <see cref="ControlItem"/> object is not <see
    /// langword="null"/> and has identical property values as this <see cref="ControlItem"/>
    /// object; otherwise, returns <see langword="false"/>
    /// </returns>
    public bool Equals(ControlItem? other)
        => other is not null
        && other.SegmentName == SegmentName
        && other.FirstTimeIndent == FirstTimeIndent
        && other.IsFirstTime == IsFirstTime
        && other.PadSegment == PadSegment
        && other.TabSize == TabSize;

    /// <summary>
    /// Determines if the given object is equivalent to this <see cref="ControlItem"/> object.
    /// </summary>
    /// <param name="obj">
    /// The object to be compared with this <see cref="ControlItem"/> object.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given object is a <see cref="ControlItem"/> object and has
    /// identical property values as this <see cref="ControlItem"/> object; otherwise, returns <see langword="false"/>
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as ControlItem);

    /// <summary>
    /// Gets the hash code for this <see cref="ControlItem"/> object.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    public override int GetHashCode() => SegmentName.GetHashCode();
}