namespace DRSSoftware.TextTemplateProcessor.Core;

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
    bool SetTokenDelimiters(string startDelimiter, string endDelimiter, char delimiterEscapeCharacter);

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
    bool ValidateTokenDelimiterValues(string startDelimiter, string endDelimiter, char delimiterEscapeCharacter);
}