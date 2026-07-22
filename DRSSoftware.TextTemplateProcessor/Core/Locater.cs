namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="Locater" /> class is used for keeping track of the current location within a text
/// file that is being processed.
/// </summary>
/// <remarks>
/// The current location is defined by a location name identifying the relevant area of the file and
/// a line number identifying the line within that area. <br /> The location name will be either
/// "TemplateFile", "OutputFile", or a segment name within the template file.
/// </remarks>
internal class Locater : ILocater
{
    /// <summary>
    /// The default constructor initializes the <see cref="Locater" /> class with an empty location
    /// name and line number 0.
    /// </summary>
    public Locater()
    {
        CurrentLocationName = Location.Empty.LocationName;
        LineNumber = Location.Empty.LineNumber;
    }

    /// <summary>
    /// Gets or sets the name of the current location being processed in the text file.
    /// </summary>
    /// <remarks>
    /// Leading and trailing whitespace will be trimmed from the location name when it is set. If
    /// the location name is null or whitespace, it will be replaced with an empty
    /// <see langword="string" />.
    /// </remarks>
    public string CurrentLocationName
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                field = Location.Empty.LocationName;
            }
            else
            {
                field = value.Trim();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not the current location is empty, meaning that the
    /// current location name is null or whitespace and the line number is 0.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the current location is empty; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsEmpty => CurrentLocationName == Location.Empty.LocationName
        && LineNumber == Location.Empty.LineNumber;

    /// <summary>
    /// Gets or sets the current line number of the location that is being processed in the text
    /// file.
    /// </summary>
    public int LineNumber
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a <see cref="Location" /> record that has been initialized with the current location in
    /// the text file.
    /// </summary>
    public Location Location => new(CurrentLocationName, LineNumber);

    /// <summary>
    /// Resets the current location to an empty location.
    /// </summary>
    public void Reset()
    {
        CurrentLocationName = Location.Empty.LocationName;
        LineNumber = Location.Empty.LineNumber;
    }

    /// <summary>
    /// Generates a string representation of the <see cref="Locater" /> class object.
    /// </summary>
    /// <returns>
    /// A <see langword="string" /> containing the current location name and line number, or an
    /// empty string if the current location is empty.
    /// </returns>
    public override string ToString() => IsEmpty
        ? string.Empty
        : $"{CurrentLocationName}[{LineNumber}]";
}