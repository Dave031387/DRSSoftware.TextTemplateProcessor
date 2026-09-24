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
    /// <param name="templateText">
    /// The next line of text to be parsed from the text template.
    /// </param>
    /// <param name="isInitialPass">
    /// A value indicating whether this is the first time the given template text line is being parsed.
    /// </param>
    void InitializeParser(string templateText, bool isInitialPass = false);
}