namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="Locater" /> class is used for keeping track of the current location within a text
/// template file. The current location is defined by the current segment being processed and the
/// line number within that segment.
/// </summary>
internal class Locater : ILocater
{
    /// <summary>
    /// The default constructor initializes the <see cref="Locater" /> class with an empty segment
    /// name and line number 0.
    /// </summary>
    public Locater()
    {
        CurrentSegment = Location.Empty.SegmentName;
        LineNumber = Location.Empty.LineNumber;
    }

    /// <summary>
    /// Gets or sets the name of the current segment being processed in the text template file.
    /// </summary>
    /// <remarks>
    /// Leading and trailing whitespace will be trimmed from the segment name when it is set. If the
    /// segment name is null or whitespace, it will be replaced with an empty
    /// <see langword="string" />.
    /// </remarks>
    public string CurrentSegment
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                field = Location.Empty.SegmentName;
            }
            else
            {
                field = value.Trim();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not the current location is empty, meaning that the
    /// current segment name is null or whitespace and the line number is 0.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the current location is empty; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsEmpty => CurrentSegment == Location.Empty.SegmentName
        && LineNumber == Location.Empty.LineNumber;

    /// <summary>
    /// Gets or sets the current line number of the segment that is being processed in the text
    /// template file.
    /// </summary>
    public int LineNumber
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a <see cref="Location" /> record that has been initialized with the current location in
    /// the text template file.
    /// </summary>
    public Location Location => new(CurrentSegment, LineNumber);

    /// <summary>
    /// Resets the current location to an empty location.
    /// </summary>
    public void Reset()
    {
        CurrentSegment = Location.Empty.SegmentName;
        LineNumber = Location.Empty.LineNumber;
    }

    /// <summary>
    /// Generates a string representation of the <see cref="Locater" /> class object.
    /// </summary>
    /// <returns>
    /// A <see langword="string" /> containing the current segment name and line number, or an empty
    /// string if the current location is empty.
    /// </returns>
    public override string ToString() => IsEmpty
        ? string.Empty
        : $"{CurrentSegment}[{LineNumber}]";
}