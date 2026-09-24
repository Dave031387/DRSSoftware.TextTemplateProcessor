namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// Defines operations for extracting tokens from text, maintaining a token dictionary, and
/// configuring token delimiters and the token escape character.
/// </summary>
internal interface ITokenExtractor
{
    /// <summary>
    /// Clears all tokens from the token dictionary.
    /// </summary>
    void ClearTokens();

    /// <summary>
    /// Extracts all valid tokens from the given line of text and adds any tokens found to the token dictionary.
    /// </summary>
    /// <param name="text">
    /// A line of text possibly containing one or more tokens.
    /// </param>
    /// <remarks>
    /// If the <paramref name="text"/> parameter contains any invalid tokens, the text will be
    /// modified to insert a token escape character ahead of the token start delimiter of each
    /// invalid token.
    /// </remarks>
    /// <returns>
    /// The given <paramref name="text"/> after being modified to escape any invalid token delimiters.
    /// </returns>
    string ExtractTokens(string text);

    /// <summary>
    /// Resets the token delimiters and token escape character to their default values.
    /// </summary>
    void ResetTokenDelimiters();

    /// <summary>
    /// Sets the token start and token end delimiters and the token escape character to the
    /// specified values.
    /// </summary>
    /// <param name="tokenStart">
    /// The new token start delimiter string.
    /// </param>
    /// <param name="tokenEnd">
    /// The new token end delimiter string.
    /// </param>
    /// <param name="tokenEscapeChar">
    /// The new token escape character.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the delimiter values were successfully changed. Otherwise,
    /// returns <see langword="false" />.
    /// </returns>
    bool SetTokenDelimiters(string tokenStart, string tokenEnd, char tokenEscapeChar);
}