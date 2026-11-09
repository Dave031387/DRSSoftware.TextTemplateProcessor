namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// This interface defines the contract for a text line parser service used for parsing text lines
/// in a text template file.
/// </summary>
internal interface ITextLineParser
{
    /// <summary>
    /// Parses the given <paramref name="textLine"/>, extracts any tokens on that text line, and
    /// creates a new <see cref="TextItem"/> object representing that text line.
    /// </summary>
    /// <remarks>
    /// It is assumed that the given <paramref name="textLine"/> begins with no control code or a
    /// valid indent control code in the first few character positions of the text line.
    /// </remarks>
    /// <param name="textLine">
    /// A text line string from the text template file.
    /// </param>
    /// <returns>
    /// A <see cref="TextItem"/> object containing the indent control information (determined from
    /// the indent control code) and the text portion of the text line (the portion of the text line
    /// following the indent control code).
    /// </returns>
    TextItem Parse(string textLine);
}