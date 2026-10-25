namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="SegmentHeaderParser"/> class is used for parsing segment headers in a text
/// template file to extract the segment names and control options.
/// </summary>
internal class SegmentHeaderParser : DependencyCheckerBase, ISegmentHeaderParser
{
    /// <summary>
    /// Constructor that creates an instance of the <see cref="SegmentHeaderParser"/> class and
    /// initializes dependencies.
    /// </summary>
    /// <param name="controlItemBuilder">
    /// A reference to a control item builder object used for creating control item objects.
    /// </param>
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    /// <param name="optionParser">
    /// A reference to the option parser service.
    /// </param>
    /// <param name="indentProcessor">
    /// A reference to an indent processor object used for managing line indentation in the
    /// generated text file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are <see langword="null"/>.
    /// </exception>
    internal SegmentHeaderParser(IControlItemBuilder controlItemBuilder,
                                 IIndentProcessor indentProcessor,
                                 ILogger logger,
                                 IOptionParser optionParser)
    {
        ControlItemBuilder = NullDependencyCheck(controlItemBuilder,
                                                 nameof(SegmentHeaderParser),
                                                 nameof(IControlItemBuilder),
                                                 nameof(controlItemBuilder));
        IndentProcessor = NullDependencyCheck(indentProcessor,
                                              nameof(SegmentHeaderParser),
                                              nameof(IIndentProcessor),
                                              nameof(indentProcessor));
        Logger = NullDependencyCheck(logger,
                                     nameof(SegmentHeaderParser),
                                     nameof(ILogger),
                                     nameof(logger));
        OptionParser = NullDependencyCheck(optionParser,
                                           nameof(SegmentHeaderParser),
                                           nameof(IOptionParser),
                                           nameof(optionParser));
    }

    /// <summary>
    /// Gets a reference to the control item builder service.
    /// </summary>
    private IControlItemBuilder ControlItemBuilder
    {
        get; init;
    }

    /// <summary>
    /// Gets a reference to the indent processor service.
    /// </summary>
    private IIndentProcessor IndentProcessor
    {
        get; init;
    }

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get; init;
    }

    /// <summary>
    /// Gets a reference to the option parser service.
    /// </summary>
    private IOptionParser OptionParser
    {
        get;
        init;
    }

    /// <summary>
    /// Parses a segment header line from a text template file and extracts the segment name and
    /// control information.
    /// </summary>
    /// <param name="headerLine">
    /// A segment header line from a text template file.
    /// </param>
    /// <returns>
    /// A <see cref="ControlItem"/> object containing the segment name and control information.
    /// </returns>
    public ControlItem Parse(string headerLine)
    {
        string[] args;
        ControlItemBuilder.Initialize();

        if (headerLine.Length < ControlCodeLength + 2 || headerLine[ControlCodeLength + 1] is ' ')
        {
            string message = GetMessage(MsgSegmentNameIsMissing);
            Logger.Log(LogSeverity.Error, message);
            string newHeaderLine = $"{SegmentHeaderCode} {UnknownSegmentName} {headerLine[3..].Trim()}";
            args = newHeaderLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            args = headerLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        string segmentName = args[1];

        if (IsValidName(segmentName))
        {
            ControlItemBuilder.SegmentName = segmentName;
        }
        else
        {
            ControlItemBuilder.SegmentName = UnknownSegmentName;
            string message = GetMessage(MsgInvalidSegmentName,
                                        segmentName);
            Logger.Log(LogSeverity.Error, message);
        }

        if (args.Length > 2)
        {
            ParseSegmentOptions(segmentName, args);
        }

        return ControlItemBuilder.Build();
    }

    /// <summary>
    /// Parse the segment options from a segment header line and set the corresponding properties on
    /// the <see cref="ControlItemBuilder"/> class object.
    /// </summary>
    /// <remarks>
    /// Because the entire segment header line is parsed, the first argument found will be the
    /// segment header code and the second will be the segment header name. The first segment header
    /// option will therefore be the third argument.
    /// </remarks>
    /// <param name="segmentName">
    /// The name of the segment where the option <paramref name="args"/> are defined.
    /// </param>
    /// <param name="args">
    /// The option arguments from the segment header line.
    /// </param>
    private void ParseSegmentOptions(string segmentName, string[] args)
    {
        bool firstTimeIndentOptionFound = false;
        bool padSegmentOptionFound = false;
        bool tabOptionFound = false;

        for (int i = 2; i < args.Length; i++)
        {
            Option option = OptionParser.Parse(args[i], IsValidSegmentHeaderOption, SegmentHeaderOptionGroup, segmentName);

            if (option.IsEmpty)
            {
                continue;
            }

            if ((option.Name is FirstTimeIndentOption && firstTimeIndentOptionFound)
                || (option.Name is PadSegmentNameOption && padSegmentOptionFound)
                || (option.Name is TabSizeOption && tabOptionFound))
            {
                string message = GetMessage(MsgFoundDuplicateSegmentHeaderOption,
                                            segmentName,
                                            option.Name);
                Logger.Log(LogSeverity.Warning, message);
                continue;
            }

            switch (option.Name)
            {
                case FirstTimeIndentOption:
                    SetFirstTimeIndentOption(segmentName, option.Value);
                    firstTimeIndentOptionFound = true;
                    break;

                case PadSegmentNameOption:
                    SetPadSegmentOption(segmentName, option.Value);
                    padSegmentOptionFound = true;
                    break;

                case TabSizeOption:
                    SetTabSizeOption(segmentName, option.Value);
                    tabOptionFound = true;
                    break;

                default:
                    // This case should never be reached because the option name is validated by the
                    // OptionParser class.
                    break;
            }
        }
    }

    /// <summary>
    /// Validate the first time indent option and set the corresponding property on the <see
    /// cref="ControlItemBuilder"/> class object if the option value is valid.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment where the first time indent is being set.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetFirstTimeIndentOption(string segmentName, string optionValue)
    {
        if (IndentProcessor.IsValidIndentValue(optionValue, out int indentValue))
        {
            if (indentValue == 0)
            {
                string message = GetMessage(MsgFirstTimeIndentSetToZero,
                                            segmentName);
                Logger.Log(LogSeverity.Warning, message);
            }
            else
            {
                ControlItemBuilder.FirstTimeIndent = indentValue;
            }
        }
        else
        {
            string message = GetMessage(MsgFirstTimeIndentIsInvalid,
                                        segmentName,
                                        optionValue);
            Logger.Log(LogSeverity.Error, message);
        }
    }

    /// <summary>
    /// Validate the pad segment option and set the corresponding property on the <see
    /// cref="ControlItemBuilder"/> class object if the option value is valid.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment where the pas segment is being set.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetPadSegmentOption(string segmentName, string optionValue)
    {
        if (IsValidName(optionValue))
        {
            ControlItemBuilder.PadSegment = optionValue;
        }
        else
        {
            string message = GetMessage(MsgInvalidPadSegmentName,
                                        segmentName,
                                        optionValue);
            Logger.Log(LogSeverity.Error, message);
        }
    }

    /// <summary>
    /// Validate the tab size option and set the corresponding property on the <see
    /// cref="ControlItemBuilder"/> class object if the option value is valid.
    /// </summary>
    /// <param name="segmentName">
    /// The name of the segment where the tab value is being set.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetTabSizeOption(string segmentName, string optionValue)
    {
        if (IndentProcessor.IsValidTabSizeValue(optionValue, out int tabValue))
        {
            ControlItemBuilder.TabSize = tabValue;
        }
        else
        {
            string message = GetMessage(MsgInvalidSegmentHeaderTabSizeOption,
                                        segmentName,
                                        optionValue);
            Logger.Log(LogSeverity.Error, message);
        }
    }
}