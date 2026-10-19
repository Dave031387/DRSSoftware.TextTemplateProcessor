namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// Defines the contract that a token parser service should satisfy.
/// </summary>
internal interface ITokenParser
{
    /// <summary>
    /// Gets a value indicating whether the parser has reached the end of the current text template line.
    /// </summary>
    bool EndOfText
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether the current template text line has been modified to escape
    /// any invalid token delimiters.
    /// </summary>
    bool IsTextModified
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether a valid token string has been found.
    /// </summary>
    bool ValidTokenFound
    {
        get;
    }

    /// <summary>
    /// In the unlikely event the last found token isn't able to be added to the token dictionary,
    /// this method provides a means of escaping the last found token.
    /// </summary>
    /// <remarks>
    /// Note that this method must be called before the <see cref="GetNextToken"/> method is called
    /// since it relies on the delimiter indexes still being set to the position of the last found token.
    /// </remarks>
    void EscapeLastFoundToken();

    /// <summary>
    /// Gets the modified template text line.
    /// </summary>
    /// <returns>
    /// The modified template text line with all invalid token delimiters escaped.
    /// </returns>
    string GetModifiedText();

    /// <summary>
    /// Extract and return the next valid token from the current template text line.
    /// </summary>
    /// <returns>
    /// A <see cref="TokenInfo"/> object that either contains the details for the next valid token,
    /// or contains an empty default value if no valid token was found.
    /// </returns>
    TokenInfo GetNextToken();

    /// <summary>
    /// Initializes the token parser and prepares it for parsing the next line from the text template.
    /// </summary>
    /// <remarks>
    /// When <paramref name="shouldValidateTokenDelimiters"/> is set to <see langword="true"/> all
    /// token delimiters on the current text template line will be validated and invalid delimiters
    /// will be escaped.
    /// </remarks>
    /// <param name="templateText">
    /// The next line of text to be parsed from the text template.
    /// </param>
    /// <param name="shouldValidateTokenDelimiters">
    /// A value indicating whether token delimiters on the current template text line should be validated.
    /// </param>
    void InitializeParser(string templateText, bool shouldValidateTokenDelimiters = false);
}