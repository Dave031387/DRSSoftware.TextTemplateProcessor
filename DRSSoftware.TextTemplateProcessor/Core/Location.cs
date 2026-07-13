namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This record represents a location in a text template file, consisting of a segment name and a
/// line number. It is used to identify the specific location of log messages or other relevant
/// information within the text template processing context.
/// </summary>
/// <param name="SegmentName">
/// The name of the segment in which the current location is found.
/// </param>
/// <param name="LineNumber">
/// The line number within the segment at which the current location is found.
/// </param>
internal record Location(string SegmentName, int LineNumber)
{
    /// <summary>
    /// Gets an empty <see cref="Location" /> instance, which has an empty segment name and a line
    /// number of 0.
    /// </summary>
    public static Location Empty => new(string.Empty, 0);

    /// <summary>
    /// Gets a value indicating whether or not this <see cref="Location" /> instance is empty.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the location is empty; otherwise, <see langword="false" />.
    /// </returns>
    public bool IsEmpty
        => SegmentName == Empty.SegmentName && LineNumber == Empty.LineNumber;

    /// <summary>
    /// Gets the string representation of the current location.
    /// </summary>
    /// <returns>
    /// The string representation of the current location, or an empty string if the location is
    /// empty.
    /// </returns>
    public override string ToString() => IsEmpty
        ? string.Empty
        : $"{SegmentName}[{LineNumber}]";
}