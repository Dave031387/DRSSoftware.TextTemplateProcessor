using System.Diagnostics.CodeAnalysis;

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
        Initialize();
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not a file header has been found in the text
    /// template file.
    /// </summary>
    private bool FileHeaderFound
    {
        get;
        set;
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
    /// Gets or sets the last segment name that was found in the text template file.
    /// </summary>
    private string LastSegmentName
    {
        get;
        set;
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
    /// Gets or sets a value indicating whether any text lines have been found in the text template file.
    /// </summary>
    private bool TextLineFound
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a reference to the text line parser service.
    /// </summary>
    private ITextLineParser TextLineParser
    {
        get;
        init;
    }

    /// <summary>
    /// Parses the text template file found at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">
    /// The file path of the text template file that is to be parsed.
    /// </param>
    public void Parse(string filePath)
    {
        try
        {
            Initialize();
            Locater.CurrentLocationName = TemplateFileLocationName;
            Locater.LineNumber = 0;
            Logger.PushOperationType(OperationType.Parsing);
            TemplateFileService.ReadTemplateFile(filePath);
            string message = GetMessage(MsgStartingToParseFile, TemplateFileService.TemplateFileName);
            Logger.Log(LogSeverity.Information, message);
            ParseTemplateFile();
            message = GetMessage(MsgParsingCompletedSuccessfully);
            Logger.Log(LogSeverity.Information, message);
        }
        catch (Exception ex)
        {
            string message = GetMessage(MsgUnexpectedExceptionWhileParsing, filePath, ex.Message);
            Logger.Log(LogSeverity.Error, message);
        }
        finally
        {
            Logger.PopOperationType();
            Locater.Reset();
        }
    }

    private void CheckForEmptySegment()
    {
        if (!string.IsNullOrEmpty(LastSegmentName))
        {
            if (SegmentManager.IsEmptySegment(LastSegmentName))
            {
            }
        }
    }

    private void EndOfFileCheck()
    {
        if (string.IsNullOrEmpty(LastSegmentName))
        {
        }
        else
        {
            CheckForEmptySegment();
        }
    }

    /// <summary>
    /// Initializes the state of this <see cref="TemplateParser"/> object before parsing a new text
    /// template file.
    /// </summary>
    [MemberNotNull(nameof(LastSegmentName))]
    internal void Initialize()
    {
        FileHeaderParser.Initialize();
        Locater.Initialize();
        SegmentManager.Initialize();
        TemplateFileService.Initialize();
        TextLineParser.Initialize();
        FileHeaderFound = false;
        LastSegmentName = string.Empty;
        TextLineFound = false;
    }

    private bool IsValidTemplateLine(string templateLine)
    {
        bool isValid = true;

        if (!HasValidControlCode(templateLine))
        {
            string message = GetMessage(MsgInvalidControlCode, templateLine);
            Logger.Log(LogSeverity.Error, message);
            isValid = false;
        }

        if (templateLine.Length < ControlCodeLength + 1 || templateLine[ControlCodeLength] is not ' ')
        {
            string message = GetMessage(MsgFourthCharacterMustBeBlank,
                                        templateLine,
                                        MinTemplateLineLength);
            Logger.Log(LogSeverity.Error, message);
            isValid = false;
        }

        return isValid;
    }

    private void ParseFileHeader(string templateLine)
    {
        if (FileHeaderFound)
        {
            string message = GetMessage(MsgMoreThanOneFileHeader);
            Logger.Log(LogSeverity.Error, message);
            return;
        }

        FileHeaderFound = true;

        if (!string.IsNullOrEmpty(LastSegmentName) || TextLineFound)
        {
            string message = GetMessage(MsgFileHeaderMustComeFirst);
            Logger.Log(LogSeverity.Error, message);
            return;
        }

        FileHeaderParser.Parse(templateLine);
    }

    private void ParseSegmentHeader(string templateLine)
    {
        CheckForEmptySegment();
    }

    private void ParseTemplateFile()
    {
        if (TemplateFileService.IsEmptyFile)
        {
            string message = GetMessage(MsgTemplateFileIsEmpty, TemplateFileService.TemplateFileName);
            Logger.Log(LogSeverity.Error, message);
            return;
        }

        foreach (string templateLine in TemplateFileService)
        {
            ParseTemplateLine(templateLine);
        }

        EndOfFileCheck();
    }

    private void ParseTemplateLine(string templateLine)
    {
        Locater.LineNumber++;

        if (!IsValidTemplateLine(templateLine))
        {
            return;
        }

        if (IsTextLine(templateLine))
        {
            ParseTextLine(templateLine);
        }
        else if (IsSegmentHeader(templateLine))
        {
            ParseSegmentHeader(templateLine);
        }
        else if (IsFileHeader(templateLine))
        {
            ParseFileHeader(templateLine);
        }

        // NOTE: Comment lines are ignored and discarded.
    }

    private void ParseTextLine(string templateLine)
    {
    }
}