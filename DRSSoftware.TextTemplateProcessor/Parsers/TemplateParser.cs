namespace DRSSoftware.TextTemplateProcessor.Parsers;

/// <summary>
/// The <see cref="TemplateParser"/> class parses the entire contents of a text template file and
/// extracts all of the segments, text lines, and control information.
/// </summary>
internal class TemplateParser : DependencyCheckerBase, ITemplateParser
{
    /// <summary>
    /// Creates a new instance of the <see cref="TemplateParser"/> class.
    /// </summary>
    /// <param name="fileHeaderParser">
    /// A reference to the file header parser service.
    /// </param>
    /// <param name="locater">
    /// A reference to the locater service.
    /// </param>
    /// <param name="logger">
    /// A reference to the logger service.
    /// </param>
    /// <param name="segmentHeaderParser">
    /// A reference to the segment header parser service.
    /// </param>
    /// <param name="segmentManager">
    /// A reference to the segment manager service.
    /// </param>
    /// <param name="templateFileService">
    /// A reference to the template file service.
    /// </param>
    /// <param name="textLineParser">
    /// A reference to the text line parser service.
    /// </param>
    internal TemplateParser(IFileHeaderParser fileHeaderParser,
                            ILocater locater,
                            ILogger logger,
                            ISegmentHeaderParser segmentHeaderParser,
                            ISegmentManager segmentManager,
                            ITemplateFileService templateFileService,
                            ITextLineParser textLineParser)
    {
        FileHeaderParser = NullDependencyCheck(fileHeaderParser, nameof(fileHeaderParser));
        Locater = NullDependencyCheck(locater, nameof(locater));
        Logger = NullDependencyCheck(logger, nameof(logger));
        SegmentHeaderParser = NullDependencyCheck(segmentHeaderParser, nameof(segmentHeaderParser));
        SegmentManager = NullDependencyCheck(segmentManager, nameof(segmentManager));
        TemplateFileService = NullDependencyCheck(templateFileService, nameof(templateFileService));
        TextLineParser = NullDependencyCheck(textLineParser, nameof(textLineParser));
    }

    /// <summary>
    /// Gets a reference to the file header parser service.
    /// </summary>
    private IFileHeaderParser FileHeaderParser
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the locater service.
    /// </summary>
    private ILocater Locater
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

    /// <summary>
    /// Gets a reference to the segment header parser service.
    /// </summary>
    private ISegmentHeaderParser SegmentHeaderParser
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the segment manager service.
    /// </summary>
    private ISegmentManager SegmentManager
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the template file service.
    /// </summary>
    private ITemplateFileService TemplateFileService
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the text line parser service.
    /// </summary>
    private ITextLineParser TextLineParser
    {
        get;
        init;
    }

    public void Parse(string filePath)
    {
        try
        {
            Initialize();
            Locater.CurrentLocationName = TemplateFileLocationName;
            Locater.LineNumber = 0;
            Logger.PushOperationType(OperationType.Parsing);
            Initialize();
            TemplateFileService.ReadTemplateFile(filePath);

            if (TemplateFileService.IsEmptyFile)
            {
            }
        }
        catch (Exception ex)
        {
        }
        finally
        {
            Logger.PopOperationType();
            Locater.Reset();
        }
    }

    /// <summary>
    /// Initializes the state of this <see cref="TemplateParser"/> object before parsing a new text
    /// template file.
    /// </summary>
    internal void Initialize()
    {
        FileHeaderParser.Initialize();
        Locater.Initialize();
        SegmentManager.Initialize();
        TemplateFileService.Initialize();
        TextLineParser.Initialize();
    }

    private void ParseTemplateLine(string templateLine)
    {
    }
}