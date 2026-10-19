using System.Diagnostics.CodeAnalysis;

namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="DelimiterService"/> class is used to maintain the token start and end delimiter
/// values and the delimiter escape character that are referenced by other classes in the Text
/// Template Processor application.
/// </summary>
internal class DelimiterService : DependencyCheckerBase, IDelimiterService
{
    /// <summary>
    /// Creates a new instance of the <see cref="DelimiterService"/> class.
    /// </summary>
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    internal DelimiterService(ILogger logger)
    {
        Logger = NullDependencyCheck(logger,
                                     nameof(DelimiterService),
                                     nameof(ILogger),
                                     nameof(logger));
        ResetTokenDelimiters();
    }

    /// <summary>
    /// Gets the character currently being used as the delimiter escape character.
    /// </summary>
    /// <remarks>
    /// The escape character is used to indicate that a token start delimiter or token end delimiter
    /// should be treated as an ordinary string of text rather than the start or end of a token.
    /// </remarks>
    public char DelimiterEscapeCharacter
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the string that is currently being used to delimit the end of a token.
    /// </summary>
    public string EndDelimiter
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the string that is currently being used to delimit the start of a token.
    /// </summary>
    public string StartDelimiter
    {
        get;
        private set;
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
    /// Resets the token delimiters and delimiter escape character to their default values.
    /// </summary>
    [MemberNotNull(nameof(EndDelimiter), nameof(StartDelimiter))]
    public void ResetTokenDelimiters()
    {
        DelimiterEscapeCharacter = DefaultDelimiterEscapeCharacter;
        EndDelimiter = DefaultTokenEndDelimiter;
        StartDelimiter = DefaultTokenStartDelimiter;
    }

    /// <summary>
    /// Sets the token start and token end delimiters and the delimiter escape character to the
    /// specified values.
    /// </summary>
    /// <remarks>
    /// The <see cref="ValidateTokenDelimiterValues(string, string, char)"/> method will be called
    /// before updating the delimiter values. No changes will be made if the validation fails.
    /// </remarks>
    /// <param name="startDelimiter">
    /// The new token start delimiter string.
    /// </param>
    /// <param name="endDelimiter">
    /// The new token end delimiter string.
    /// </param>
    /// <param name="delimiterEscapeCharacter">
    /// The new delimiter escape character.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the delimiter values were successfully changed. Otherwise, returns
    /// <see langword="false"/>.
    /// </returns>
    public bool SetTokenDelimiters(string startDelimiter, string endDelimiter, char delimiterEscapeCharacter)
    {
        if (ValidateTokenDelimiterValues(startDelimiter, endDelimiter, delimiterEscapeCharacter))
        {
            DelimiterEscapeCharacter = delimiterEscapeCharacter;
            EndDelimiter = endDelimiter;
            StartDelimiter = startDelimiter;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Validate the given token start and token end delimiters and delimiter escape character
    /// values to ensure they are valid and consistent.
    /// </summary>
    /// <param name="startDelimiter">
    /// The value that will be assigned to the token start delimiter.
    /// </param>
    /// <param name="endDelimiter">
    /// The value that will be assigned to the token end delimiter.
    /// </param>
    /// <param name="delimiterEscapeCharacter">
    /// The value that will be assigned to the delimiter escape character.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if all given delimiter values are valid and consistent; otherwise
    /// <see langword="false"/>
    /// </returns>
    public bool ValidateTokenDelimiterValues(string startDelimiter, string endDelimiter, char delimiterEscapeCharacter)
    {
        if (startDelimiter is null)
        {
            string message = GetMessage(MsgTokenStartDelimiterIsNull);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (endDelimiter is null)
        {
            string message = GetMessage(MsgTokenEndDelimiterIsNull);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (string.IsNullOrWhiteSpace(startDelimiter))
        {
            string message = GetMessage(MsgTokenStartDelimiterIsEmpty);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (string.IsNullOrWhiteSpace(endDelimiter))
        {
            string message = GetMessage(MsgTokenEndDelimiterIsEmpty);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (startDelimiter == endDelimiter)
        {
            string message = GetMessage(MsgTokenStartAndTokenEndAreSame,
                                        startDelimiter,
                                        endDelimiter);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (startDelimiter == delimiterEscapeCharacter.ToString())
        {
            string message = GetMessage(MsgTokenStartAndTokenEscapeAreSame,
                                        startDelimiter,
                                        delimiterEscapeCharacter.ToString());
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (endDelimiter == delimiterEscapeCharacter.ToString())
        {
            string message = GetMessage(MsgTokenEndAndTokenEscapeAreSame,
                                        endDelimiter,
                                        delimiterEscapeCharacter.ToString());
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (startDelimiter[^1] is LowercaseFlag or UppercaseFlag or SameCaseFlag)
        {
            string message = GetMessage(MsgTokenStartDelimiterWarning);
            Logger.Log(LogSeverity.Warning, message);
        }

        return true;
    }
}