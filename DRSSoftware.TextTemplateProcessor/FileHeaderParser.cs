namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="FileHeaderParser"/> class is used for parsing the file header line of a text
/// template file to extract the file header options.
/// </summary>
internal class FileHeaderParser : DependencyCheckerBase, IFileHeaderParser
{
    /// <summary>
    /// Creates a new instance of the <see cref="FileHeaderParser"/> class.
    /// </summary>
    /// <param name="delimiterService">
    /// A reference to the token delimiter service.
    /// </param>
    /// <param name="indentProcessor">
    /// A reference to the indent processor service.
    /// </param>
    /// <param name="logger">
    /// A reference to the logger service.
    /// </param>
    internal FileHeaderParser(IDelimiterService delimiterService, IIndentProcessor indentProcessor, ILogger logger)
    {
        DelimiterService = NullDependencyCheck(delimiterService,
                                               nameof(FileHeaderParser),
                                               nameof(IDelimiterService),
                                               nameof(delimiterService));
        IndentProcessor = NullDependencyCheck(indentProcessor,
                                              nameof(FileHeaderParser),
                                              nameof(IIndentProcessor),
                                              nameof(indentProcessor));
        Logger = NullDependencyCheck(logger,
                                     nameof(FileHeaderParser),
                                     nameof(ILogger),
                                     nameof(logger));
    }

    /// <summary>
    /// Gets a reference to the token delimiter service.
    /// </summary>
    private IDelimiterService DelimiterService
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the indent processor service.
    /// </summary>
    private IIndentProcessor IndentProcessor
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get;
        init;
    }

    public void ParseFileHeader(string headerLine)
    {
        string[] args = headerLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        bool tabOptionFound = false;
        bool startDelimiterOptionFound = false;
        bool endDelimiterOptionFound = false;
        bool delimiterEscapeOptionFound = false;

        for (int i = 0; i < args.Length; i++)
        {

        }
    }
}