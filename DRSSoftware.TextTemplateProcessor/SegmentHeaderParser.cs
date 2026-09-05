namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="SegmentHeaderParser" /> class is used for parsing segment headers in a text
/// template file to extract the segment names and control options.
/// </summary>
internal class SegmentHeaderParser : DependencyCheckerBase, ISegmentHeaderParser
{
    /// <summary>
    /// Constructor that creates an instance of the <see cref="SegmentHeaderParser" /> class and
    /// initializes dependencies.
    /// </summary>
    /// <param name="controlItemBuilder">
    /// A reference to a control item builder object used for creating control item objects.
    /// </param>
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    /// <param name="indentProcessor">
    /// A reference to an indent processor object used for managing line indentation in the
    /// generated text file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are
    /// <see langword="null" />.
    /// </exception>
    internal SegmentHeaderParser(IControlItemBuilder controlItemBuilder, IIndentProcessor indentProcessor,
                                 ILogger logger)
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
    /// This method parses a segment header line from a text template file and extracts the segment
    /// name and control information.
    /// </summary>
    /// <param name="headerLine">
    /// A segment header line from a text template file.
    /// </param>
    /// <returns>
    /// A <see cref="ControlItem" /> object containing the segment name and control information.
    /// </returns>
    public ControlItem ParseSegmentHeader(string headerLine)
    {
        ControlItemBuilder.Initialize();

        if (headerLine.Length < 5 || headerLine[4] == ' ')
        {
            ControlItemBuilder.SegmentName = UnknownSegmentName;
            Logger.Log(LogSeverity.Error,
                       MsgSegmentNameIsMissing);
        }

        string[] args = headerLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrEmpty(ControlItemBuilder.SegmentName))
        {
            string segmentName = args[1];

            if (IsValidName(segmentName))
            {
                ControlItemBuilder.SegmentName = segmentName;
            }
            else
            {
                ControlItemBuilder.SegmentName = UnknownSegmentName;
                Logger.Log(LogSeverity.Error,
                           MsgInvalidSegmentName,
                           segmentName);
            }
        }

        if (args.Length > 2)
        {
            ParseSegmentOptions(args);
        }

        return ControlItemBuilder.Build();
    }

    /// <summary>
    /// Parse a single segment option from a segment header line and return the option name and
    /// value as a tuple.
    /// </summary>
    /// <param name="arg">
    /// The segment option to parse.
    /// </param>
    /// <returns>
    /// A tuple containing the option name and value.
    /// </returns>
    private (string optionName, string optionValue) ParseSegmentOption(string arg)
    {
        string segmentName = ControlItemBuilder.SegmentName;
        (string optionName, string optionValue) result = (string.Empty, string.Empty);

        int optionIndex;

        if (arg.Contains(OptionValueSeparator))
        {
            optionIndex = arg.IndexOf(OptionValueSeparator);
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgInvalidFormOfOption,
                       segmentName,
                       arg);
            return result;
        }

        if (optionIndex < 1)
        {
            Logger.Log(LogSeverity.Error,
                       MsgOptionNameMustPrecedeEqualsSign,
                       segmentName);
            return result;
        }

        result.optionName = arg[..optionIndex].ToUpperInvariant();

        if (!IsValidSegmentHeaderOption(result.optionName))
        {
            Logger.Log(LogSeverity.Error,
                       MsgUnknownSegmentOptionFound,
                       segmentName,
                       arg);
            return result;
        }

        optionIndex++;

        if (optionIndex == arg.Length)
        {
            Logger.Log(LogSeverity.Error,
                       MsgOptionValueMustFollowEqualsSign,
                       segmentName,
                       result.optionName);
            return result;
        }

        result.optionValue = arg[optionIndex..];

        return result;
    }

    /// <summary>
    /// Parse the segment options from a segment header line and set the corresponding properties on
    /// the given control item.
    /// </summary>
    /// <param name="args">
    /// The option arguments from the segment header line.
    /// </param>
    private void ParseSegmentOptions(string[] args)
    {
        string segmentName = ControlItemBuilder.SegmentName;
        bool firstTimeIndentOptionFound = false;
        bool padSegmentOptionFound = false;
        bool tabOptionFound = false;

        for (int i = 2; i < args.Length; i++)
        {
            (string optionName, string optionValue) = ParseSegmentOption(args[i]);

            if (string.IsNullOrEmpty(optionValue))
            {
                continue;
            }

            if ((optionName == FirstTimeIndentOption && firstTimeIndentOptionFound)
                || (optionName == PadSegmentNameOption && padSegmentOptionFound)
                || (optionName == TabSizeOption && tabOptionFound))
            {
                Logger.Log(LogSeverity.Warning,
                           MsgFoundDuplicateOptionNameOnHeaderLine,
                           segmentName,
                           optionName);
                continue;
            }

            switch (optionName)
            {
                case FirstTimeIndentOption:
                    SetFirstTimeIndentOption(optionValue);
                    firstTimeIndentOptionFound = true;
                    break;

                case PadSegmentNameOption:
                    SetPadSegmentOption(optionValue);
                    padSegmentOptionFound = true;
                    break;

                case TabSizeOption:
                    SetTabSizeOption(optionValue);
                    tabOptionFound = true;
                    break;

                default:
                    // This case should never be reached because the option name is validated in the
                    // ParseSegmentOption method.
                    break;
            }
        }
    }

    /// <summary>
    /// Validate the first time indent option and set the corresponding property on the given
    /// control item if the option value is valid.
    /// </summary>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetFirstTimeIndentOption(string optionValue)
    {
        if (IndentProcessor.IsValidIndentValue(optionValue, out int indentValue))
        {
            if (indentValue == 0)
            {
                Logger.Log(LogSeverity.Warning,
                           MsgFirstTimeIndentSetToZero,
                           ControlItemBuilder.SegmentName);
            }
            else
            {
                ControlItemBuilder.FirstTimeIndent = indentValue;
            }
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgFirstTimeIndentIsInvalid,
                       ControlItemBuilder.SegmentName,
                       optionValue);
        }
    }

    /// <summary>
    /// Validate the pad segment option and set the corresponding property on the given control item
    /// if the option value is valid.
    /// </summary>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetPadSegmentOption(string optionValue)
    {
        if (IsValidName(optionValue))
        {
            ControlItemBuilder.PadSegment = optionValue;
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgInvalidPadSegmentName,
                       ControlItemBuilder.SegmentName,
                       optionValue);
        }
    }

    /// <summary>
    /// Validate the tab size option and set the corresponding property on the given control item if
    /// the option value is valid.
    /// </summary>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetTabSizeOption(string optionValue)
    {
        if (IndentProcessor.IsValidTabSizeValue(optionValue, out int tabValue))
        {
            ControlItemBuilder.TabSize = tabValue;
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgInvalidTabSizeOption,
                       ControlItemBuilder.SegmentName,
                       optionValue);
        }
    }
}