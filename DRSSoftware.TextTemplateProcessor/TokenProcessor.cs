using System.Text;

namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="TokenProcessor" /> class is responsible for processing tokens in text templates.
/// It provides methods to extract tokens from text, load token values, replace tokens with their
/// corresponding values, and manage token delimiters and escape characters.
/// </summary>
internal class TokenProcessor : DependencyCheckerBase, ITokenProcessor
{
    /// <summary>
    /// This record is used to store information about a token found in a text string.
    /// </summary>
    /// <param name="TokenString">
    /// The entire token string that was found, including the start and end delimiters.
    /// </param>
    /// <param name="TokenName">
    /// The name of the token.
    /// </param>
    /// <param name="Case">
    /// A character flag indicating how to handle the first character of the value that gets
    /// assigned to the token.
    /// </param>
    private record TokenInfo(string TokenString, string TokenName, char Case);

    /// <summary>
    /// This record is used to store information about the results obtained when searching for token
    /// start and end delimiters in a text string.
    /// </summary>
    /// <param name="IsValid">
    /// Indicates whether the search was successful.
    /// </param>
    /// <param name="IndexValue">
    /// The index of the found delimiter.
    /// </param>
    private record TokenSearchResult(bool IsValid, int IndexValue);

    /// <summary>
    /// A constructor that creates an instance of the <see cref="TokenProcessor" /> class and
    /// initializes the dependencies.
    /// </summary>
    /// <param name="logger">
    /// A reference to a logger object used for logging messages.
    /// </param>
    /// <param name="locater">
    /// A reference to a locater object for keeping track of the current location in a text template
    /// file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are
    /// <see langword="null" />.
    /// </exception>
    internal TokenProcessor(ILocater locater, ILogger logger)
    {
        Locater = NullDependencyCheck(locater,
                                      nameof(TokenProcessor),
                                      nameof(ILocater),
                                      nameof(locater));
        Logger = NullDependencyCheck(logger,
                                     nameof(TokenProcessor),
                                     nameof(ILogger),
                                     nameof(logger));
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
    /// The escape character is used to indicate that a token start delimiter should be treated as
    /// an ordinary string of text rather than the start of a token.
    /// </remarks>
    internal char TokenEscapeChar { get; private set; } = DefaultTokenEscapeCharacter;

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
    /// Clears all tokens from the token dictionary.
    /// </summary>
    public void ClearTokens() => TokenDictionary.Clear();

    /// <summary>
    /// Searches for valid tokens in the given line of text and adds any tokens found to the token
    /// dictionary.
    /// </summary>
    /// <param name="text">
    /// A line of text possibly containing one or more tokens.
    /// </param>
    /// <remarks>
    /// If the <paramref name="text" /> parameter contains any invalid tokens, the text will be
    /// modified to insert a token escape character ahead of the token start delimiter of each
    /// invalid token.
    /// </remarks>
    public void ExtractTokens(ref string text)
    {
        int startIndex = 0;

        while (startIndex < text.Length - 1)
        {
            TokenInfo tokenInfo = FindToken(ref startIndex, ref text);

            if (string.IsNullOrEmpty(tokenInfo.TokenString))
            {
                continue;
            }

            _ = TokenDictionary.TryAdd(tokenInfo.TokenName, string.Empty);
        }
    }

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
    public string ReplaceTokens(string text)
    {
        StringBuilder builder = new(text);
        int startIndex = 0;

        while (startIndex < text.Length)
        {
            TokenInfo tokenInfo = FindToken(ref startIndex, ref text);

            if (string.IsNullOrEmpty(tokenInfo.TokenName))
            {
                break;
            }

            if (TokenDictionary.TryGetValue(tokenInfo.TokenName, out string? value))
            {
                string tokenValue = value;
                string replacementValue = GetReplacementValue(tokenInfo, tokenValue);

                _ = builder.Replace(tokenInfo.TokenString, replacementValue);
            }
            else
            {
                string message = GetMessage(MsgTokenNameNotFound,
                                            Locater.CurrentLocationName,
                                            tokenInfo.TokenName);
                Logger.Log(LogSeverity.Error, message);
            }
        }

        builder = builder.Replace(TokenEscapeChar + TokenStart, TokenStart);
        return builder.ToString();
    }

    /// <summary>
    /// Resets the token delimiters and token escape character to their default values.
    /// </summary>
    public void ResetTokenDelimiters()
    {
        TokenStart = DefaultTokenStartDelimiter;
        TokenEnd = DefaultTokenEndDelimiter;
        TokenEscapeChar = DefaultTokenEscapeCharacter;
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
    /// <see langword="true" /> if the delimiter values were successfully changed. Otherwise,
    /// returns <see langword="false" />.
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
        TokenEscapeChar = tokenEscapeChar;
        return true;
    }

    private TokenInfo ExtractToken(int tokenStart, int tokenEnd, ref string text)
    {
        int tokenNameStart = tokenStart + TokenStart.Length;
        int tokenNameEnd = tokenEnd;
        tokenEnd += TokenEnd.Length;
        string tokenString = text[tokenStart..tokenEnd];
        string tokenName;
        bool isValidToken = true;
        char initCase = SameCaseFlag;

        if (text[tokenNameStart] is LowercaseFlag or UppercaseFlag or SameCaseFlag)
        {
            initCase = text[tokenNameStart];
            tokenNameStart++;
        }

        tokenName = text[tokenNameStart..tokenNameEnd].Trim();

        if (string.IsNullOrWhiteSpace(tokenName))
        {
            string message = GetMessage(MsgMissingTokenName);
            Logger.Log(LogSeverity.Error, message);
            isValidToken = false;
        }
        else if (!IsValidName(tokenName))
        {
            string message = GetMessage(MsgTokenHasInvalidName,
                                        tokenName);
            Logger.Log(LogSeverity.Error, message);
            isValidToken = false;
        }

        if (!isValidToken)
        {
            text = InsertEscapeCharacter(tokenStart, text);
            tokenString = string.Empty;
            tokenName = string.Empty;
        }

        return new(tokenString, tokenName, initCase);
    }

    private TokenInfo FindToken(ref int startIndex, ref string text)
    {
        TokenInfo result = new(string.Empty, string.Empty, SameCaseFlag);

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        while (startIndex < text.Length
            && string.IsNullOrEmpty(result.TokenString))
        {
            TokenSearchResult tokenStart = LocateTokenStartDelimiter(startIndex, text);

            if (!tokenStart.IsValid)
            {
                startIndex = tokenStart.IndexValue;
                continue;
            }

            TokenSearchResult tokenEnd = LocateTokenEndDelimiter(tokenStart.IndexValue, ref text);

            if (!tokenEnd.IsValid)
            {
                startIndex = tokenEnd.IndexValue;
                break;
            }

            result = ExtractToken(tokenStart.IndexValue, tokenEnd.IndexValue, ref text);
            startIndex = tokenEnd.IndexValue;
        }

        return result;
    }

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

            replacementValue = tokenInfo.Case == LowercaseFlag
                ? firstChar.ToLowerInvariant() + remaining
                : tokenInfo.Case == UppercaseFlag
                    ? firstChar.ToUpperInvariant() + remaining
                    : tokenValue;
        }

        return replacementValue;
    }

    private string InsertEscapeCharacter(int tokenStart, string text) => text.Insert(tokenStart, TokenEscapeChar.ToString());

    private TokenSearchResult LocateTokenEndDelimiter(int tokenStart, ref string text)
    {
        int tokenEnd = text.IndexOf(TokenEnd, tokenStart, StringComparison.Ordinal);

        if (tokenEnd < 0)
        {
            string message = GetMessage(MsgTokenMissingEndDelimiter);
            Logger.Log(LogSeverity.Warning, message);
            text = InsertEscapeCharacter(tokenStart, text);
            return new(false, text.Length);
        }

        return new(true, tokenEnd);
    }

    private TokenSearchResult LocateTokenStartDelimiter(int startIndex, string text)
    {
        int tokenStart = text.IndexOf(TokenStart, startIndex, StringComparison.Ordinal);

        return tokenStart < 0
            ? new(false, text.Length)
            : tokenStart > 0 && text[tokenStart - 1] == TokenEscapeChar
            ? new(false, tokenStart + TokenStart.Length)
            : new(true, tokenStart);
    }

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