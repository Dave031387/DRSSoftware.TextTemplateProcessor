namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// Defines the contract that a token parser service should satisfy.
/// </summary>
internal interface ITokenParser
{
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
    /// Initialize the token parser and prepare it for parsing the next line from the text template.
    /// </summary>
    /// <param name="templateText">
    /// The next line of text from the text template.
    /// </param>
    void InitializeParser(string templateText);
}