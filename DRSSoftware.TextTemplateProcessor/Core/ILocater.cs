namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ILocater" /> interface defines the contract for a class that is used to keep
/// track of the current location within a text template file.
/// </summary>
internal interface ILocater
{
    /// <summary>
    /// Gets or sets the name of the current segment being processed in the text template file.
    /// </summary>
    /// <remarks>
    /// Leading and trailing whitespace will be trimmed from the segment name when it is set. If the
    /// segment name is null or whitespace, it will be replaced with an empty
    /// <see langword="string" />.
    /// </remarks>
    string CurrentSegment
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether or not the current location is empty, meaning that the
    /// current segment name is null or whitespace and the line number is 0.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the current location is empty; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsEmpty
    {
        get;
    }

    /// <summary>
    /// Gets or sets the current line number of the segment that is being processed in the text
    /// template file.
    /// </summary>
    int LineNumber
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a <see cref="Location" /> record that has been initialized with the current location in
    /// the text template file.
    /// </summary>
    public Location Location
    {
        get;
    }

    /// <summary>
    /// Resets the current location to an empty location.
    /// </summary>
    void Reset();

    /// <summary>
    /// Generates a string representation of the <see cref="Locater" /> class object.
    /// </summary>
    /// <returns>
    /// A <see langword="string" /> containing the current segment name and line number, or an empty
    /// string if the current location is empty.
    /// </returns>
    string ToString();
}