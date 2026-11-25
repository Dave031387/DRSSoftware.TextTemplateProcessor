namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// The interface defines the contract for a text template file parser that parses the entire
/// contents of a text template file and extracts segments, text lines, and control information.
/// </summary>
internal interface ITemplateParser
{
    /// <summary>
    /// Parses the text template file found at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">
    /// The file path of the text template file that is to be parsed.
    /// </param>
    void Parse(string filePath);
}