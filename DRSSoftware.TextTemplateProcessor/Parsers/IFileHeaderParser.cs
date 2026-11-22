namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// The <see cref="IFileHeaderParser"/> interface defines the contract for parsing the file header
/// of a text template file.
/// </summary>
internal interface IFileHeaderParser
{
    /// <summary>
    /// Initializes the state of the <see cref="IDelimiterService"/> and <see
    /// cref="IIndentProcessor"/> objects used by this <see cref="IFileHeaderParser"/> object.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Parses the file header line from the text template file and handles any file options that
    /// are found.
    /// </summary>
    /// <param name="headerLine">
    /// The file header line from the text template file.
    /// </param>
    void Parse(string headerLine);
}