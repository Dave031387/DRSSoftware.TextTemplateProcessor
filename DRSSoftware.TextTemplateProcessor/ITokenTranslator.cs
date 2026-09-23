namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// Defines operations for replacing tokens in text with their corresponding substitution values.
/// </summary>
internal interface ITokenTranslator
{
    /// <summary>
    /// This method is used to load token substitution values into the Token Dictionary for the
    /// given token names.
    /// </summary>
    /// <param name="tokenValues">
    /// A dictionary of key/value pairs where the key is the token name and the value is the
    /// substitution value to be assigned to that token.
    /// </param>
    /// <remarks>
    /// The token names in the <paramref name="tokenValues" /> dictionary passed into this method
    /// must already exist in the Token Dictionary. Any token names not found will be ignored.
    /// </remarks>
    void LoadTokenValues(Dictionary<string, string> tokenValues);

    /// <summary>
    /// Replace tokens in the given text line with their corresponding substitution values.
    /// </summary>
    /// <param name="text">
    /// A text string that may contain one or more tokens.
    /// </param>
    /// <returns>
    /// The original <paramref name="text" /> string with all tokens replaced by their substitution
    /// values.
    /// </returns>
    /// <remarks>
    /// The token escape character will be removed from all escaped tokens in the
    /// <paramref name="text" /> string and those tokens will be output without any substitution.
    /// </remarks>
    string ReplaceTokens(string text);
}