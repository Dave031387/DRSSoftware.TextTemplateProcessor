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
    /// Loads the current values of the token start delimiter, token end delimiter, and the
    /// delimiter escape character.
    /// </summary>
    /// <param name="tokenStartDelimiter">
    /// The value of the token start delimiter.
    /// </param>
    /// <param name="tokenEndDelimiter">
    /// The value of the token end delimiter.
    /// </param>
    /// <param name="delimiterEscapeCharacter">
    /// The value of the delimiter escape character.
    /// </param>
    void InitializeDelimiters(string tokenStartDelimiter, string tokenEndDelimiter, char delimiterEscapeCharacter);

    /// <summary>
    /// Initializes the token parser and prepares it for parsing the next line from the text template.
    /// </summary>
    /// <remarks>
    /// When <paramref name="isInitialPass"/> is set to <see langword="true"/> it implies that we
    /// are preparing to extract all tokens from the template text. <br/> Otherwise, it is assumed
    /// that we will be replacing token strings in the template text with their corresponding
    /// substitution values.
    /// </remarks>
    /// <param name="templateText">
    /// The next line of text to be parsed from the text template.
    /// </param>
    /// <param name="isInitialPass">
    /// A value indicating whether this is the first time the given template text line is being parsed.
    /// </param>
    void InitializeParser(string templateText, bool isInitialPass = false);
}