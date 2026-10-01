using System.Text;

namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="TokenProcessor"/> class is responsible for processing tokens in text templates.
/// It provides methods to extract tokens from text, load token values, replace tokens with their
/// corresponding values, and manage token delimiters and escape characters.
/// </summary>
internal class TokenProcessor : DependencyCheckerBase, ITokenExtractor, ITokenTranslator
{
    /// <summary>
    /// A constructor that creates an instance of the <see cref="TokenProcessor"/> class and
    /// initializes the dependencies.
    /// </summary>
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    /// <param name="tokenParser">
    /// A reference to a token parser object used for parsing tokens in a text string.
    /// </param>
    /// <param name="locater">
    /// A reference to a locater object for keeping track of the current location in a text template file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are <see langword="null"/>.
    /// </exception>
    internal TokenProcessor(ILocater locater, ILogger logger, ITokenParser tokenParser)
    {
        Locater = NullDependencyCheck(locater,
                                      nameof(TokenProcessor),
                                      nameof(ILocater),
                                      nameof(locater));
        Logger = NullDependencyCheck(logger,
                                     nameof(TokenProcessor),
                                     nameof(ILogger),
                                     nameof(logger));
        TokenParser = NullDependencyCheck(tokenParser,
                                          nameof(TokenProcessor),
                                          nameof(ITokenParser),
                                          nameof(tokenParser));
        ModifiedText = new(200, 1000);
    }

    /// <summary>
    /// Gets a dictionary of token names and their corresponding substitution values.
    /// </summary>
    internal Dictionary<string, string> TokenDictionary { get; } = [];

    /// <summary>
    /// Gets the string that is currently being used to denote the end of a token.
    /// </summary>
    internal string TokenEnd { get; private set; } = DefaultTokenEndDelimiter;

    /// <summary>
    /// Gets the character currently being used as the token escape character.
    /// </summary>
    /// <remarks>
    /// The escape character is used to indicate that a token start delimiter or token end delimiter
    /// should be treated as an ordinary string of text rather than the start or end of a token.
    /// </remarks>
    internal char TokenEscape { get; private set; } = DefaultTokenEscapeCharacter;

    /// <summary>
    /// Gets the string that is currently being used to denote the start of a token.
    /// </summary>
    internal string TokenStart { get; private set; } = DefaultTokenStartDelimiter;

    /// <summary>
    /// Gets a reference to the locater service.
    /// </summary>
    private ILocater Locater
    {
        get; init;
    }

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get; init;
    }

    /// <summary>
    /// Gets the <see cref="StringBuilder"/> object used for editing the template text with the
    /// substituted token values.
    /// </summary>
    private StringBuilder ModifiedText
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the token parser service.
    /// </summary>
    private ITokenParser TokenParser
    {
        get; init;
    }

    /// <summary>
    /// Clears all tokens from the token dictionary.
    /// </summary>
    public void ClearTokens() => TokenDictionary.Clear();

    /// <summary>
    /// Extracts all valid tokens from the given line of text and adds any tokens found to the token dictionary.
    /// </summary>
    /// <param name="text">
    /// A line of text possibly containing one or more tokens.
    /// </param>
    /// <remarks>
    /// If the <paramref name="text"/> parameter contains any invalid tokens, the text will be
    /// modified to insert token escape characters ahead of the token start and end delimiters of
    /// each invalid token.
    /// </remarks>
    /// <returns>
    /// The given <paramref name="text"/> after being modified to escape any invalid token delimiters.
    /// </returns>
    public string ExtractTokens(string text)
    {
        TokenParser.InitializeParser(text, true);

        while (!TokenParser.EndOfText)
        {
            string tokenName = TokenParser.GetNextToken().TokenName;

            if (TokenParser.ValidTokenFound && !TokenDictionary.ContainsKey(tokenName))
            {
                if (!TokenDictionary.TryAdd(tokenName, string.Empty))
                {
                    if (!TokenDictionary.ContainsKey(tokenName))
                    {
                        TokenParser.EscapeLastFoundToken();
                        string message = GetMessage(MsgUnableToAddTokenToDictionary, tokenName);
                        Logger.Log(LogSeverity.Error, message);
                    }
                }
            }
        }

        return TokenParser.IsTextModified ? TokenParser.GetModifiedText() : text;
    }

    /// <summary>
    /// Loads token substitution values into the Token Dictionary for the given token names.
    /// </summary>
    /// <param name="tokenValues">
    /// A dictionary of key/value pairs where the key is the token name and the value is the
    /// substitution value to be assigned to that token.
    /// </param>
    /// <remarks>
    /// The token names in the <paramref name="tokenValues"/> dictionary passed into this method
    /// must already exist in the Token Dictionary. Any token names not found will be ignored.
    /// </remarks>
    public void LoadTokenValues(Dictionary<string, string> tokenValues)
    {
        if (tokenValues is null)
        {
            string message = GetMessage(MsgTokenDictionaryIsNull,
                                        Locater.CurrentLocationName);
            Logger.Log(LogSeverity.Error, message);
            return;
        }

        if (tokenValues.Count is 0)
        {
            string message = GetMessage(MsgTokenDictionaryIsEmpty,
                                        Locater.CurrentLocationName);
            Logger.Log(LogSeverity.Warning, message);
            return;
        }

        foreach (KeyValuePair<string, string> keyValuePair in tokenValues)
        {
            UpdateTokenDictionary(keyValuePair);
        }
    }

    /// <summary>
    /// Replaces tokens in the given text line with their corresponding substitution values.
    /// </summary>
    /// <remarks>
    /// The token escape character will be removed from all escaped token delimiters in the
    /// <paramref name="text"/> string. Invalid tokens that were escaped will be output as is
    /// without any substitution.
    /// </remarks>
    /// <param name="text">
    /// A text string that contains zero or more tokens.
    /// </param>
    /// <returns>
    /// The original <paramref name="text"/> string with all valid tokens replaced by their
    /// corresponding substitution values.
    /// </returns>
    public string ReplaceTokens(string text)
    {
        _ = ModifiedText.Clear();
        _ = ModifiedText.Append(text);
        TokenParser.InitializeParser(text);

        while (!TokenParser.EndOfText)
        {
            TokenInfo tokenInfo = TokenParser.GetNextToken();

            if (TokenParser.ValidTokenFound)
            {
                if (TokenDictionary.TryGetValue(tokenInfo.TokenName, out string? value))
                {
                    string replacementValue = GetReplacementValue(tokenInfo, value);

                    _ = ModifiedText.Replace(tokenInfo.TokenString, replacementValue);
                }
                else
                {
                    string message = GetMessage(MsgTokenNameNotFound,
                                                Locater.CurrentLocationName,
                                                tokenInfo.TokenName);
                    Logger.Log(LogSeverity.Error, message);
                }
            }
        }

        _ = ModifiedText.Replace(TokenEscape + TokenStart, TokenStart);
        _ = ModifiedText.Replace(TokenEscape + TokenEnd, TokenEnd);
        return ModifiedText.ToString();
    }

    /// <summary>
    /// Resets the token delimiters and token escape character to their default values.
    /// </summary>
    public void ResetTokenDelimiters()
    {
        TokenStart = DefaultTokenStartDelimiter;
        TokenEnd = DefaultTokenEndDelimiter;
        TokenEscape = DefaultTokenEscapeCharacter;
        TokenParser.InitializeDelimiters(TokenStart, TokenEnd, TokenEscape);
    }

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
    /// <see langword="true"/> if the delimiter values were successfully changed. Otherwise, returns
    /// <see langword="false"/>.
    /// </returns>
    public bool SetTokenDelimiters(string tokenStart, string tokenEnd, char tokenEscapeChar)
    {
        if (tokenStart is null)
        {
            string message = GetMessage(MsgTokenStartDelimiterIsNull);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (tokenEnd is null)
        {
            string message = GetMessage(MsgTokenEndDelimiterIsNull);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (string.IsNullOrWhiteSpace(tokenStart))
        {
            string message = GetMessage(MsgTokenStartDelimiterIsEmpty);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (string.IsNullOrWhiteSpace(tokenEnd))
        {
            string message = GetMessage(MsgTokenEndDelimiterIsEmpty);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (tokenStart == tokenEnd)
        {
            string message = GetMessage(MsgTokenStartAndTokenEndAreSame,
                                        tokenStart,
                                        tokenEnd);
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (tokenStart == tokenEscapeChar.ToString())
        {
            string message = GetMessage(MsgTokenStartAndTokenEscapeAreSame,
                                        tokenStart,
                                        tokenEscapeChar.ToString());
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (tokenEnd == tokenEscapeChar.ToString())
        {
            string message = GetMessage(MsgTokenEndAndTokenEscapeAreSame,
                                        tokenEnd,
                                        tokenEscapeChar.ToString());
            Logger.Log(LogSeverity.Error, message);
            return false;
        }

        if (tokenStart[^1] is LowercaseFlag or UppercaseFlag or SameCaseFlag)
        {
            string message = GetMessage(MsgTokenStartDelimiterWarning);
            Logger.Log(LogSeverity.Warning, message);
        }

        TokenStart = tokenStart;
        TokenEnd = tokenEnd;
        TokenEscape = tokenEscapeChar;
        TokenParser.InitializeDelimiters(TokenStart, TokenEnd, TokenEscape);
        return true;
    }

    /// <summary>
    /// Gets the replacement value for the given token.
    /// </summary>
    /// <param name="tokenInfo">
    /// The <see cref="TokenInfo"/> object that describes the token being replaced.
    /// </param>
    /// <param name="tokenValue">
    /// The unadjusted token replacement value that was retrieved from the Token Dictionary.
    /// </param>
    /// <returns>
    /// The <paramref name="tokenValue"/> with the first character converted to upper- or lowercase
    /// if so indicated by the <paramref name="tokenInfo"/> details.
    /// </returns>
    private string GetReplacementValue(TokenInfo tokenInfo, string tokenValue)
    {
        string replacementValue = string.Empty;

        if (string.IsNullOrEmpty(tokenValue))
        {
            string message = GetMessage(MsgTokenValueIsEmpty,
                                        Locater.CurrentLocationName,
                                        tokenInfo.TokenName);
            Logger.Log(LogSeverity.Warning, message);
        }
        else
        {
            string firstChar = tokenValue[0..1];
            string remaining = tokenValue.Length > 1
                ? tokenValue[1..]
                : string.Empty;

            replacementValue = tokenInfo.Case is LowercaseFlag
                ? firstChar.ToLowerInvariant() + remaining
                : tokenInfo.Case is UppercaseFlag
                    ? firstChar.ToUpperInvariant() + remaining
                    : tokenValue;
        }

        return replacementValue;
    }

    /// <summary>
    /// Searches the Token Dictionary for the specified token name and updates its replacement value
    /// with the specified value.
    /// </summary>
    /// <param name="keyValuePair">
    /// A key/value pair comprised of a token name and its replacement value.
    /// </param>
    private void UpdateTokenDictionary(KeyValuePair<string, string> keyValuePair)
    {
        string tokenName = keyValuePair.Key;
        string tokenValue = keyValuePair.Value;

        if (IsValidName(tokenName))
        {
            if (TokenDictionary.ContainsKey(tokenName))
            {
                if (tokenValue is null)
                {
                    string message = GetMessage(MsgTokenWithNullValue,
                                                Locater.CurrentLocationName,
                                                tokenName);
                    Logger.Log(LogSeverity.Error, message);
                    tokenValue = string.Empty;
                }
                else if (string.IsNullOrEmpty(tokenValue))
                {
                    string message = GetMessage(MsgTokenWithEmptyValue,
                                                Locater.CurrentLocationName,
                                                tokenName);
                    Logger.Log(LogSeverity.Warning, message);
                }

                TokenDictionary[tokenName] = tokenValue;
            }
            else
            {
                string message = GetMessage(MsgUnknownTokenName,
                                            Locater.CurrentLocationName,
                                            tokenName);
                Logger.Log(LogSeverity.Warning, message);
            }
        }
        else
        {
            string message = GetMessage(MsgTokenDictionaryContainsInvalidTokenName,
                                        Locater.CurrentLocationName,
                                        tokenName);
            Logger.Log(LogSeverity.Error, message);
        }
    }
}