namespace DRSSoftware.TextTemplateProcessor.Services;

internal interface IDelimiterService
{
    /// <summary>
    /// Gets the character currently being used as the delimiter escape character.
    /// </summary>
    /// <remarks>
    /// The escape character is used to indicate that a token start delimiter or token end delimiter
    /// should be treated as an ordinary string of text rather than the start or end of a token.
    /// </remarks>
    char DelimiterEscapeCharacter
    {
        get;
    }

    /// <summary>
    /// Gets the string that is currently being used to delimit the end of a token.
    /// </summary>
    string EndDelimiter
    {
        get;
    }

    /// <summary>
    /// Gets the string that is currently being used to delimit the start of a token.
    /// </summary>
    string StartDelimiter
    {
        get;
    }

    /// <summary>
    /// Resets the token delimiters and delimiter escape character to their default values.
    /// </summary>
    void ResetTokenDelimiters();

    /// <summary>
    /// Sets the token start and token end delimiters and the delimiter escape character to the
    /// specified values.
    /// </summary>
    /// <remarks>
    /// The delimiter values will be validated before making any changes. No changes will be made if
    /// the validation fails.
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
    bool SetTokenDelimiters(string startDelimiter, string endDelimiter, char delimiterEscapeCharacter);
}