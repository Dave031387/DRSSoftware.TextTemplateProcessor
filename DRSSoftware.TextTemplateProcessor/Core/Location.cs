namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This record represents a location within a file, consisting of a location name and a line
/// number.
/// </summary>
/// <remarks>
/// The location name identifies the area and the line number identifies the specific line within
/// that area.
/// </remarks>
/// <param name="LocationName">
/// The name of the location. This will be either "TemplateFile", "OutputFile", or a segment name.
/// </param>
/// <param name="LineNumber">
/// The line number within the area identified by the location name.
/// </param>
internal record Location(string LocationName, int LineNumber)
{
    /// <summary>
    /// Gets an empty <see cref="Location" /> instance, which has an empty location name and a line
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
        => LocationName == Empty.LocationName && LineNumber == Empty.LineNumber;

    /// <summary>
    /// Gets the string representation of the current location.
    /// </summary>
    /// <returns>
    /// The string representation of the current location, or an empty string if the location is
    /// empty.
    /// </returns>
    public override string ToString() => IsEmpty
        ? string.Empty
        : $"{LocationName}[{LineNumber}]";
}