namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// The <see cref="TextLineParser"/> class manages the parsing of a text line from the text template
/// file, including calling the <see cref="TokenParser"/> class to extract all tokens on the text line.
/// </summary>
internal class TextLineParser : DependencyCheckerBase, ITextLineParser
{
    /// <summary>
    /// Creates a new instance of the <see cref="TextLineParser"/> class.
    /// </summary>
    /// <param name="tokenExtractor">
    /// A reference to the token extractor service.
    /// </param>
    internal TextLineParser(ITokenExtractor tokenExtractor)
        => TokenExtractor = NullDependencyCheck(tokenExtractor, nameof(tokenExtractor));

    /// <summary>
    /// Gets a reference to the token extractor service.
    /// </summary>
    private ITokenExtractor TokenExtractor
    {
        get;
        init;
    }

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
    public TextItem Parse(string textLine)
    {
        int indent = 0;
        string text;
        bool isRelative = true;
        bool isOneTime = false;

        if (!HasNoControlCode(textLine))
        {
            string indentString = textLine[1..ControlCodeLength].Replace(AbsoluteIndexChar, RelativeRightIndentChar);
            _ = int.TryParse(indentString, out indent);
            isRelative = HasRelativeIndent(textLine);
            isOneTime = HasOneTimeIndent(textLine);
        }

        text = textLine.Length < ControlCodeLength + 2
            ? string.Empty
            : TokenExtractor.ExtractTokens(textLine[(ControlCodeLength + 1)..]);

        return new(indent, isRelative, isOneTime, text);
    }
}