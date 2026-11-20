namespace DRSSoftware.TextTemplateProcessor.Parsers;

internal class TemplateParser : DependencyCheckerBase, ITemplateParser
{
    internal TemplateParser(IFileHeaderParser fileHeaderParser,
                            ILogger logger,
                            ISegmentHeaderParser segmentHeaderParser,
                            ISegmentManager segmentManager,
                            ITemplateFileService templateFileService,
                            ITextLineParser textLineParser)
    {
        FileHeaderParser = NullDependencyCheck(fileHeaderParser, nameof(fileHeaderParser));
        Logger = NullDependencyCheck(logger, nameof(logger));
        SegmentHeaderParser = NullDependencyCheck(segmentHeaderParser, nameof(segmentHeaderParser));
        SegmentManager = NullDependencyCheck(segmentManager, nameof(segmentManager));
        TemplateFileService = NullDependencyCheck(templateFileService, nameof(templateFileService));
        TextLineParser = NullDependencyCheck(textLineParser, nameof(textLineParser));
    }

    private IFileHeaderParser FileHeaderParser
    {
        get;
        init;
    }

    private ILogger Logger
    {
        get;
        init;
    }

    private ISegmentHeaderParser SegmentHeaderParser
    {
        get;
        init;
    }

    private ISegmentManager SegmentManager
    {
        get;
        init;
    }

    private ITemplateFileService TemplateFileService
    {
        get;
        init;
    }

    private ITextLineParser TextLineParser
    {
        get;
        init;
    }
}