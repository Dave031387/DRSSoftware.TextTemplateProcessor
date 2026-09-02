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
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    /// <param name="locater">
    /// A reference to a locater object for keeping track of the current location being processed
    /// within a text template file.
    /// </param>
    /// <param name="indentProcessor">
    /// A reference to an indent processor object used for managing line indentation in the
    /// generated text file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are
    /// <see langword="null" />.
    /// </exception>
    internal SegmentHeaderParser(IIndentProcessor indentProcessor,
                                 ILocater locater,
                                 ILogger logger)
    {
        IndentProcessor = NullDependencyCheck(indentProcessor,
                                              nameof(SegmentHeaderParser),
                                              nameof(IIndentProcessor),
                                              nameof(indentProcessor));
        Locater = NullDependencyCheck(locater,
                                      nameof(SegmentHeaderParser),
                                      nameof(ILocater),
                                      nameof(locater));
        Logger = NullDependencyCheck(logger,
                                     nameof(SegmentHeaderParser),
                                     nameof(ILogger),
                                     nameof(logger));
    }

    /// <summary>
    /// Gets a reference to the indent processor service.
    /// </summary>
    private IIndentProcessor IndentProcessor
    {
        get; init;
    }

    /// <summary>
    /// Gets a reference to the locater service.
    /// </summary>
    private ILocater Locater
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
        ControlItem controlItem = new();

        Locater.CurrentLocationName = string.Empty;

        if (headerLine.Length < 5 || headerLine[4] == ' ')
        {
            Locater.CurrentLocationName = UnknownSegmentName;
            Logger.Log(LogSeverity.Error,
                       MsgSegmentNameIsMissing);
        }

        string[] args = headerLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrEmpty(Locater.CurrentLocationName))
        {
            string segmentName = args[1];

            if (IsValidName(segmentName))
            {
                Locater.CurrentLocationName = segmentName;
            }
            else
            {
                Locater.CurrentLocationName = UnknownSegmentName;
                Logger.Log(LogSeverity.Error,
                           MsgInvalidSegmentName,
                           segmentName);
            }
        }

        if (args.Length > 2)
        {
            ParseSegmentOptions(controlItem, args);
        }

        return controlItem;
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
                       Locater.CurrentLocationName,
                       arg);
            return result;
        }

        if (optionIndex < 1)
        {
            Logger.Log(LogSeverity.Error,
                       MsgOptionNameMustPrecedeEqualsSign,
                       Locater.CurrentLocationName);
            return result;
        }

        result.optionName = arg[..optionIndex].ToUpperInvariant();

        if (!IsValidSegmentHeaderOption(result.optionName))
        {
            Logger.Log(LogSeverity.Error,
                       MsgUnknownSegmentOptionFound,
                       Locater.CurrentLocationName,
                       arg);
            return result;
        }

        optionIndex++;

        if (optionIndex == arg.Length)
        {
            Logger.Log(LogSeverity.Error,
                       MsgOptionValueMustFollowEqualsSign,
                       Locater.CurrentLocationName,
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
    /// <param name="controlItem">
    /// The control item to set the options on.
    /// </param>
    /// <param name="args">
    /// The option arguments from the segment header line.
    /// </param>
    private void ParseSegmentOptions(ControlItem controlItem, string[] args)
    {
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
                           Locater.CurrentLocationName,
                           optionName);
                continue;
            }

            switch (optionName)
            {
                case FirstTimeIndentOption:
                    SetFirstTimeIndentOption(controlItem, optionValue);
                    firstTimeIndentOptionFound = true;
                    break;

                case PadSegmentNameOption:
                    SetPadSegmentOption(controlItem, optionValue);
                    padSegmentOptionFound = true;
                    break;

                case TabSizeOption:
                    SetTabSizeOption(controlItem, optionValue);
                    tabOptionFound = true;
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Validate the first time indent option and set the corresponding property on the given
    /// control item if the option value is valid.
    /// </summary>
    /// <param name="controlItem">
    /// The control item to set the option on.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetFirstTimeIndentOption(ControlItem controlItem, string optionValue)
    {
        if (IndentProcessor.IsValidIndentValue(optionValue, out int indentValue))
        {
            if (indentValue == 0)
            {
                Logger.Log(LogSeverity.Warning,
                           MsgFirstTimeIndentSetToZero,
                           Locater.CurrentLocationName);
            }

            controlItem.FirstTimeIndent = indentValue;
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgFirstTimeIndentIsInvalid,
                       Locater.CurrentLocationName,
                       optionValue);
        }
    }

    /// <summary>
    /// Validate the pad segment option and set the corresponding property on the given control item
    /// if the option value is valid.
    /// </summary>
    /// <param name="controlItem">
    /// The control item to set the option on.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetPadSegmentOption(ControlItem controlItem, string optionValue)
    {
        if (IsValidName(optionValue))
        {
            controlItem.PadSegment = optionValue;
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgInvalidPadSegmentName,
                       Locater.CurrentLocationName,
                       optionValue);
        }
    }

    /// <summary>
    /// Validate the tab size option and set the corresponding property on the given control item if
    /// the option value is valid.
    /// </summary>
    /// <param name="controlItem">
    /// The control item to set the option on.
    /// </param>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetTabSizeOption(ControlItem controlItem, string optionValue)
    {
        if (IndentProcessor.IsValidTabSizeValue(optionValue, out int tabValue))
        {
            controlItem.TabSize = tabValue;
        }
        else
        {
            Logger.Log(LogSeverity.Error,
                       MsgInvalidTabSizeOption,
                       Locater.CurrentLocationName);
        }
    }
}