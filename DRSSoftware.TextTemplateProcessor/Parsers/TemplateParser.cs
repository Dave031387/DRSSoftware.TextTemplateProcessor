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
    /// Gets or sets a value indicating whether or not the current segment header being parsed in
    /// the template file is valid or not.
    /// </summary>
    private bool IsValidSegmentHeader
    {
        get;
        set;
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
            message = GetMessage(MsgParsingCompletedSuccessfully, TemplateFileService.TemplateFileName);
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
        IsValidSegmentHeader = false;
        LastSegmentName = string.Empty;
        TextLineFound = false;
    }

    /// <summary>
    /// Checks to see if any valid text lines were parsed for the last segment and logs an error if
    /// none were found.
    /// </summary>
    private void CheckForEmptySegment()
    {
        if (!(string.IsNullOrEmpty(LastSegmentName) || LastSegmentName is UnknownSegmentName || IsValidSegmentHeader))
        {
            if (SegmentManager.IsEmptySegment(LastSegmentName))
            {
                string message = GetMessage(MsgNoTextLinesFollowingSegmentHeader, LastSegmentName);
                Logger.Log(LogSeverity.Warning, message);
            }
        }
    }

    /// <summary>
    /// Checks to see if at least one valid segment header has been parsed and if the last segment
    /// has at least one valid text line. An error is logged if either condition is false.
    /// </summary>
    private void EndOfFileCheck()
    {
        if (string.IsNullOrEmpty(LastSegmentName))
        {
            string message = GetMessage(MsgNoValidSegmentsFound);
            Logger.Log(LogSeverity.Warning, message);
        }
        else
        {
            CheckForEmptySegment();
        }
    }

    /// <summary>
    /// Checks to see if the given line from the text template file begins with a valid control code
    /// and is at least the minimum length.
    /// </summary>
    /// <param name="templateLine">
    /// The line to be checked from the text template file.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given template line is formatted correctly; otherwise, <see langword="false"/>
    /// </returns>
    private bool IsValidTemplateLine(string templateLine)
    {
        bool isValid = true;

        if (!HasValidControlCode(templateLine))
        {
            string message = GetMessage(MsgInvalidControlCode, templateLine);
            Logger.Log(LogSeverity.Error, message);
            isValid = false;
        }

        if (templateLine.Length < MinTemplateLineLength || templateLine[ControlCodeLength] is not ' ')
        {
            string message = GetMessage(MsgFourthCharacterMustBeBlank,
                                        templateLine,
                                        MinTemplateLineLength);
            Logger.Log(LogSeverity.Error, message);
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Parses the given file header line from the text template file.
    /// </summary>
    /// <param name="templateLine">
    /// The template file header line to be parsed.
    /// </param>
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

    /// <summary>
    /// Parses the given segment header line from the text template file.
    /// </summary>
    /// <param name="templateLine">
    /// The segment header line to be parsed.
    /// </param>
    private void ParseSegmentHeader(string templateLine)
    {
        CheckForEmptySegment();
        ControlItem controlItem = SegmentHeaderParser.Parse(templateLine);
        LastSegmentName = controlItem.SegmentName;
        IsValidSegmentHeader = LastSegmentName is not UnknownSegmentName && SegmentManager.AddControlItem(controlItem);
    }

    /// <summary>
    /// Parses the entire contents of the text template file.
    /// </summary>
    /// <remarks>
    /// An error is logged and no further processing is performed if the text template file is found
    /// to be empty.
    /// </remarks>
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

    /// <summary>
    /// Parses the next line from the text template file.
    /// </summary>
    /// <param name="templateLine">
    /// The text template line to be parsed.
    /// </param>
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

    /// <summary>
    /// Parses the given segment text line from the text template file.
    /// </summary>
    /// <param name="templateLine">
    /// The text line to be parsed.
    /// </param>
    private void ParseTextLine(string templateLine)
    {
        TextLineFound = true;

        if (string.IsNullOrEmpty(LastSegmentName))
        {
            string message = GetMessage(MsgMissingSegmentHeaderForText);
            Logger.Log(LogSeverity.Error, message);
        }
        else if (IsValidSegmentHeader)
        {
            TextItem textItem = TextLineParser.Parse(templateLine);

            if (!SegmentManager.AddTextItem(LastSegmentName, textItem))
            {
                string message = GetMessage(MsgTextLineDiscardedDueToError);
                Logger.Log(LogSeverity.Error, message);
            }
        }
        else
        {
            string message = GetMessage(MsgTextLineHasInvalidSegmentHeader);
            Logger.Log(LogSeverity.Error, message);
        }
    }
}