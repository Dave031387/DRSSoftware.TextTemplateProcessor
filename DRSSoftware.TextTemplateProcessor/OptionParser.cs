namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="OptionParser"/> class is used for parsing the options on the file header or
/// segment header lines of the text template file.
/// </summary>
internal class OptionParser : DependencyCheckerBase, IOptionParser
{
    /// <summary>
    /// Creates a new instance of the <see cref="OptionParser"/> class.
    /// </summary>
    /// <param name="logger">
    /// A reference to the logger service.
    /// </param>
    internal OptionParser(ILogger logger) => Logger = NullDependencyCheck(logger,
                                                                          nameof(OptionParser),
                                                                          nameof(ILogger),
                                                                          nameof(logger));

    /// <summary>
    /// A delegate that validates that the given <paramref name="optionName"/> is a valid option name.
    /// </summary>
    /// <param name="optionName">
    /// The option name to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="optionName"/> is a valid option name;
    /// otherwise <see langword="false"/>
    /// </returns>
    internal delegate bool OptionValidator(string optionName);

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get;
        init;
    }

    /// <summary>
    /// Parse the given option string and return the option name and value if they are valid.
    /// </summary>
    /// <param name="optionString">
    /// The option string to be parsed and validated.
    /// </param>
    /// <param name="validator">
    /// The validator delegate used to validate the option name contained with the option string.
    /// </param>
    /// <param name="optionGroupName">
    /// A name that gives the option group currently being parsed. Valid values are "file header"
    /// and "segment header".
    /// </param>
    /// <param name="identifier">
    /// An optional identifier name. (Currently used to pass in the segment name found on the
    /// segment header being parsed.)
    /// </param>
    /// <returns>
    /// An <see cref="Option"/> instance containing the option name and option value that was found
    /// in the given option string.
    /// </returns>
    public Option Parse(string optionString, OptionValidator validator, string optionGroupName, string? identifier = null)
    {
        Option option = new();
        int optionIndex;
        string identifierString = identifier is null
            ? optionGroupName
            : $"{identifier} {optionGroupName}";

        if (optionString.Contains(OptionValueSeparator))
        {
            optionIndex = optionString.IndexOf(OptionValueSeparator);
        }
        else
        {
            string message = GetMessage(MsgInvalidFormOfOption,
                                        identifierString,
                                        optionString);
            Logger.Log(LogSeverity.Error, message);
            return option;
        }

        if (optionIndex < 1)
        {
            string message = GetMessage(MsgOptionNameMustPrecedeEqualsSign,
                                        identifierString);
            Logger.Log(LogSeverity.Error, message);
            return option;
        }

        string optionName = optionString[..optionIndex].ToUpperInvariant();

        if (!validator(optionName))
        {
            string message = GetMessage(MsgUnknownHeaderOptionFound,
                                        identifierString,
                                        optionString);
            Logger.Log(LogSeverity.Error, message);
            return option;
        }

        optionIndex++;

        if (optionIndex == optionString.Length)
        {
            string message = GetMessage(MsgOptionValueMustFollowEqualsSign,
                                        identifierString,
                                        optionName);
            Logger.Log(LogSeverity.Error, message);
            return option;
        }

        option.Name = optionName;
        option.Value = optionString[optionIndex..];

        return option;
    }
}