namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// The interface defines the contract for a text template file parser that parses the entire
/// contents of a text template file and extracts segments, text lines, and control information.
/// </summary>
internal interface ITemplateParser
{
    void Parse(string filePath);
}