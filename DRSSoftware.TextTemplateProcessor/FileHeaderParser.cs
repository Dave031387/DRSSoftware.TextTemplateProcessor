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
    /// <param name="optionParser">
    /// A reference to the option parser service.
    /// </param>
    internal FileHeaderParser(IDelimiterService delimiterService, IIndentProcessor indentProcessor, ILogger logger, IOptionParser optionParser)
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
        OptionParser = NullDependencyCheck(optionParser,
                                           nameof(FileHeaderParser),
                                           nameof(IOptionParser),
                                           nameof(optionParser));
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

    /// <summary>
    /// Gets a reference to the option parser service.
    /// </summary>
    private IOptionParser OptionParser
    {
        get;
        init;
    }

    /// <summary>
    /// Parses the file header line from the text template file and handles any file options that
    /// are found.
    /// </summary>
    /// <param name="headerLine">
    /// The file header line from the text template file.
    /// </param>
    public void Parse(string headerLine)
    {
        string[] args = headerLine.Split(OptionSeparatorChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (args.Length > 1)
        {
            ParseFileOptions(args);
        }
        else
        {
            string message = GetMessage(MsgFileHeaderHasNoOptions);
            Logger.Log(LogSeverity.Warning, message);
        }
    }

    /// <summary>
    /// Parses the options on the given file header line and sets the corresponding properties if
    /// the values are valid.
    /// </summary>
    /// <remarks>
    /// Because the entire file header line is parsed, the first argument found will be the file
    /// header code. Therefore, the first file header option will be the second argument found.
    /// </remarks>
    /// <param name="args">
    /// The option arguments from the file header line.
    /// </param>
    private void ParseFileOptions(string[] args)
    {
        bool tabOptionFound = false;
        bool startDelimiterOptionFound = false;
        bool endDelimiterOptionFound = false;
        bool delimiterEscapeOptionFound = false;
        string startDelimiterValue = DelimiterService.StartDelimiter;
        string endDelimiterValue = DelimiterService.EndDelimiter;
        string delimiterEscapeValue = DelimiterService.DelimiterEscapeCharacter.ToString();
        ;

        for (int i = 1; i < args.Length; i++)
        {
            Option option = OptionParser.Parse(args[i], IsValidFileHeaderOption, FileHeaderOptionGroup);

            if (option.IsEmpty)
            {
                continue;
            }

            if ((option.Name is TabSizeOption && tabOptionFound)
                || (option.Name is EndDelimiterOption && endDelimiterOptionFound)
                || (option.Name is StartDelimiterOption && startDelimiterOptionFound)
                || (option.Name is DelimiterEscapeOption && delimiterEscapeOptionFound))
            {
                string message = GetMessage(MsgFoundDuplicateFileHeaderOption,
                                            option.Name);
                Logger.Log(LogSeverity.Warning, message);
                continue;
            }

            switch (option.Name)
            {
                case TabSizeOption:
                    SetTabSizeOption(option.Value);
                    tabOptionFound = true;
                    break;

                case EndDelimiterOption:
                    endDelimiterValue = option.Value;
                    endDelimiterOptionFound = true;
                    break;

                case StartDelimiterOption:
                    startDelimiterValue = option.Value;
                    startDelimiterOptionFound = true;
                    break;

                case DelimiterEscapeOption:
                    delimiterEscapeValue = option.Value;
                    delimiterEscapeOptionFound = true;
                    break;

                default:
                    // This case should never be reached because the option name is validated by the
                    // OptionParser class.
                    break;
            }
        }

        if (startDelimiterOptionFound || endDelimiterOptionFound || delimiterEscapeOptionFound)
        {
            SetTokenDelimiterValues(startDelimiterValue, endDelimiterValue, delimiterEscapeValue);
        }
    }

    /// <summary>
    /// Validates the tab size option value and sets the tab size to that value if it is valid.
    /// </summary>
    /// <param name="optionValue">
    /// The value of the option to validate.
    /// </param>
    private void SetTabSizeOption(string optionValue)
    {
        if (IndentProcessor.IsValidTabSizeValue(optionValue, out int tabValue))
        {
            IndentProcessor.SetTabSize(tabValue);
        }
        else
        {
            string message = GetMessage(MsgInvalidFileHeaderTabSizeOption,
                                        optionValue);
            Logger.Log(LogSeverity.Error, message);
        }
    }

    /// <summary>
    /// Validates the token delimiter values that were entered on the file header line and sets the
    /// token delimiters to those values if they are valid.
    /// </summary>
    /// <param name="startDelimiterValue">
    /// The new token start delimiter value.
    /// </param>
    /// <param name="endDelimiterValue">
    /// The new token end delimiter value.
    /// </param>
    /// <param name="delimiterEscapeValue">
    /// The new delimiter escape value.
    /// </param>
    private void SetTokenDelimiterValues(string startDelimiterValue, string endDelimiterValue, string delimiterEscapeValue)
    {
        bool isValid = false;

        if (delimiterEscapeValue.Length > 1)
        {
            string msg = GetMessage(MsgDelimiterEscapeMustBeSingleCharacter);
            Logger.Log(LogSeverity.Error, msg);
        }
        else
        {
            isValid = DelimiterService.SetTokenDelimiters(startDelimiterValue, endDelimiterValue, delimiterEscapeValue[0]);
        }

        if (!isValid)
        {
            string message = GetMessage(MsgInvalidTokenDelimitersSpecifiedOnFileHeader);
            Logger.Log(LogSeverity.Error, message);
        }
    }
}