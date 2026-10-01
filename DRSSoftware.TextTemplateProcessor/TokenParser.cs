using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="TokenParser"/> class parses lines of text and extracts the token details from
/// that text. It also escapes any tokens that are found to be invalid.
/// </summary>
internal class TokenParser : DependencyCheckerBase, ITokenParser
{
    /// <summary>
    /// An array used to determine the order of the found end token and start tokens.
    /// </summary>
    private readonly DelimiterType[] _foundDelimiters
        = [DelimiterType.NotFound, DelimiterType.NotFound, DelimiterType.NotFound];

    /// <summary>
    /// Creates a new instance of the <see cref="TokenParser"/> class.
    /// </summary>
    /// <param name="logger">
    /// A reference to a logger object.
    /// </param>
    public TokenParser(ILogger logger)
    {
        Logger = NullDependencyCheck(logger,
                                     nameof(TokenParser),
                                     nameof(ILogger),
                                     nameof(logger));
        InitializeDelimiters(DefaultTokenStartDelimiter, DefaultTokenEndDelimiter, DefaultTokenEscapeCharacter);
    }

    /// <summary>
    /// An enumeration of found delimiter types.
    /// </summary>
    private enum DelimiterType
    {
        /// <summary>
        /// No delimiter found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The first token start delimiter was found.
        /// </summary>
        FirstStartDelimiter,

        /// <summary>
        /// The second token start delimiter was found.
        /// </summary>
        SecondStartDelimiter,

        /// <summary>
        /// The token end delimiter was found.
        /// </summary>
        EndDelimiter
    }

    /// <summary>
    /// Gets a value indicating whether the parser has reached the end of the current text template line.
    /// </summary>
    public bool EndOfText => StartSearchIndex >= TemplateText.Length;

    /// <summary>
    /// Gets a value indicating whether the current template text line has been modified to escape
    /// any invalid token delimiters.
    /// </summary>
    public bool IsTextModified
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets a value indicating whether a valid token string has been found.
    /// </summary>
    public bool ValidTokenFound
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the value of the delimiter escape character.
    /// </summary>
    internal char DelimiterEscapeCharacter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of the token end delimiter.
    /// </summary>
    internal string TokenEndDelimiter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of the token start delimiter.
    /// </summary>
    internal string TokenStartDelimiter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the found end delimiter follows the first found start delimiter.
    /// </summary>
    private bool EndDelimiterFollowsFirstStartDelimiter => _foundDelimiters[1] is DelimiterType.EndDelimiter;

    /// <summary>
    /// Gets a value indicating whether the found end delimiter follows the second found start delimiter.
    /// </summary>
    private bool EndDelimiterFollowsSecondStartDelimiter => _foundDelimiters[2] is DelimiterType.EndDelimiter;

    /// <summary>
    /// Gets or sets the index position of the found end delimiter within the current template text
    /// line. Will return -1 if no end delimiter was found.
    /// </summary>
    private int EndDelimiterIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the found end delimiter is escaped.
    /// </summary>
    private bool EndDelimiterIsEscaped => IsDelimiterEscaped(EndDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether or not an end delimiter was found.
    /// </summary>
    private bool EndDelimiterIsFound => EndDelimiterIndex > NotFound;

    /// <summary>
    /// Gets a value indicating whether the found end delimiter is not escaped.
    /// </summary>
    private bool EndDelimiterIsNotEscaped => IsDelimiterNotEscaped(EndDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether an end delimiter was not found.
    /// </summary>
    private bool EndDelimiterIsNotFound => EndDelimiterIndex <= NotFound;

    /// <summary>
    /// Gets a value indicating whether the found end delimiter appears before the first found start
    /// delimiter in the current template text line.
    /// </summary>
    private bool EndDelimiterPrecedesFirstStartDelimiter => _foundDelimiters[0] is DelimiterType.EndDelimiter;

    /// <summary>
    /// Gets or sets the index position of the first found start delimiter in the current template
    /// text line. Will return -1 if no start delimiter was found.
    /// </summary>
    private int FirstStartDelimiterIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the first found start delimiter is escaped.
    /// </summary>
    private bool FirstStartDelimiterIsEscaped => IsDelimiterEscaped(FirstStartDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether the first start delimiter was found.
    /// </summary>
    private bool FirstStartDelimiterIsFound => FirstStartDelimiterIndex > NotFound;

    /// <summary>
    /// Gets a value indicating whether the first found start delimiter is not escaped.
    /// </summary>
    private bool FirstStartDelimiterIsNotEscaped => IsDelimiterNotEscaped(FirstStartDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether the first start delimiter was not found.
    /// </summary>
    private bool FirstStartDelimiterIsNotFound => FirstStartDelimiterIndex <= NotFound;

    /// <summary>
    /// Gets or sets a value of the case flag associated with the token that was found in the
    /// current template text line.
    /// </summary>
    private char FoundCaseFlag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether an invalid token string has been found.
    /// </summary>
    private bool FoundInvalidTokenString => TokenIsFound && !ValidTokenFound;

    /// <summary>
    /// Gets or sets the name of the token that was found in the current template text line.
    /// </summary>
    private string FoundTokenName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the complete token string that was found in the current template text line.
    /// </summary>
    private string FoundTokenString
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this is the initial pass for parsing the current
    /// template text line.
    /// </summary>
    /// <remarks>
    /// The template text line can be modified only on the initial pass. After that it is assumed
    /// that all invalid token delimiters have been escaped.
    /// </remarks>
    private bool IsInitialPass
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a reference to the logger object.
    /// </summary>
    private ILogger Logger
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the modified form of the current template text line after all invalid tokens have been escaped.
    /// </summary>
    private StringBuilder ModifiedText
    {
        get;
        init;
    } = new(200, 1000);

    /// <summary>
    /// Gets or sets the index position of the second found start delimiter in the current template
    /// text line. Will return -1 if the second start delimiter wasn't found.
    /// </summary>
    private int SecondStartDelimiterIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the second found start delimiter is escaped.
    /// </summary>
    private bool SecondStartDelimiterIsEscaped => IsDelimiterEscaped(SecondStartDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether the second start delimiter was found in the current template
    /// text line.
    /// </summary>
    private bool SecondStartDelimiterIsFound => SecondStartDelimiterIndex > NotFound;

    /// <summary>
    /// Gets a value indicating whether the second found start delimiter is not escaped.
    /// </summary>
    private bool SecondStartDelimiterIsNotEscaped => IsDelimiterNotEscaped(SecondStartDelimiterIndex);

    /// <summary>
    /// Gets a value indicating whether the second start delimiter wasn't found in the current
    /// template text line.
    /// </summary>
    private bool SecondStartDelimiterIsNotFound => SecondStartDelimiterIndex <= NotFound;

    /// <summary>
    /// Gets or sets the index position to start searching for delimiters within the current
    /// template text line.
    /// </summary>
    /// <remarks>
    /// This index is also used to keep track of the next character in the template text line that
    /// is to be appended to the end of the modified text line.
    /// </remarks>
    private int StartSearchIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the current line of text from the text template.
    /// </summary>
    private string TemplateText
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether a token string has been found in the current template text line.
    /// </summary>
    private bool TokenIsFound => FirstStartDelimiterIsNotEscaped && EndDelimiterIsNotEscaped
        && EndDelimiterFollowsFirstStartDelimiter;

    /// <summary>
    /// In the unlikely event the last found token isn't able to be added to the token dictionary,
    /// this method provides a means of escaping the last found token.
    /// </summary>
    /// <remarks>
    /// Note that this method must be called before the <see cref="GetNextToken"/> method is called
    /// since it relies on the delimiter indexes still being set to the position of the last found token.
    /// </remarks>
    public void EscapeLastFoundToken()
    {
        if (ValidTokenFound && IsInitialPass)
        {
            string tokenStartString = TemplateText[FirstStartDelimiterIndex..EndDelimiterIndex];
            string tokenString = $"{tokenStartString}{TokenEndDelimiter}";
            string replacementString = $"{DelimiterEscapeCharacter}{tokenStartString}{DelimiterEscapeCharacter}{TokenEndDelimiter}";
            _ = ModifiedText.Replace(tokenString, replacementString);
            IsTextModified = true;
        }
    }

    /// <summary>
    /// Gets the modified template text line.
    /// </summary>
    /// <returns>
    /// The modified template text line with all invalid token delimiters escaped.
    /// </returns>
    public string GetModifiedText() => ModifiedText.ToString();

    /// <summary>
    /// Extracts and returns the next valid token from the current template text line.
    /// </summary>
    /// <returns>
    /// A <see cref="TokenInfo"/> object that either contains the details for the next valid token,
    /// or contains an empty default value if no valid token was found.
    /// </returns>
    public TokenInfo GetNextToken()
    {
        FoundTokenName = string.Empty;
        FoundTokenString = string.Empty;
        FoundCaseFlag = SameCaseFlag;
        ValidTokenFound = false;

        while (!(EndOfText || ValidTokenFound))
        {
            FindNextToken();
        }

        return new(FoundTokenString, FoundTokenName, FoundCaseFlag);
    }

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
    [MemberNotNull(nameof(TokenStartDelimiter), nameof(TokenEndDelimiter))]
    public void InitializeDelimiters(string tokenStartDelimiter, string tokenEndDelimiter, char delimiterEscapeCharacter)
    {
        TokenStartDelimiter = tokenStartDelimiter;
        TokenEndDelimiter = tokenEndDelimiter;
        DelimiterEscapeCharacter = delimiterEscapeCharacter;
    }

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
    public void InitializeParser(string templateText, bool isInitialPass = false)
    {
        TemplateText = templateText;
        _ = ModifiedText.Clear();
        IsTextModified = false;
        StartSearchIndex = 0;
        IsInitialPass = isInitialPass;
    }

    /// <summary>
    /// Appends the next token end delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token end delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the end delimiter. <br/> If the current
    /// search index value is lower than the token end delimiter index position then the text from
    /// the search index position up to the end delimiter position is appended first.
    /// </remarks>
    private void AppendEndDelimiter()
    {
        bool shouldEscapeEndDelimiter = EndDelimiterIsNotEscaped
            && ((EndDelimiterPrecedesFirstStartDelimiter
            && ((FirstStartDelimiterIsNotEscaped && SecondStartDelimiterIsNotFound)
            || (FirstStartDelimiterIsFound && SecondStartDelimiterIsFound)))
            || (EndDelimiterFollowsFirstStartDelimiter && FirstStartDelimiterIsEscaped && SecondStartDelimiterIsFound)
            || FoundInvalidTokenString);

        if (shouldEscapeEndDelimiter && !TokenIsFound)
        {
            string message = GetMessage(MsgTokenEndDelimiterWillBeEscaped);
            Logger.Log(LogSeverity.Error, message);
        }

        AppendTokenDelimiter(EndDelimiterIndex, TokenEndDelimiter, shouldEscapeEndDelimiter);
    }

    /// <summary>
    /// Appends the first found delimiter and any preceding text to the end of the modified text if appropriate.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the delimiter was appended; otherwise returns <see langword="false"/>
    /// </returns>
    private bool AppendFirstFoundDelimiter()
    {
        bool shouldCheckForSecondDelimiter = false;

        if ((FirstStartDelimiterIsFound && SecondStartDelimiterIsFound && EndDelimiterIsFound
            && (EndDelimiterFollowsFirstStartDelimiter || EndDelimiterFollowsSecondStartDelimiter))
            || (FirstStartDelimiterIsNotEscaped && SecondStartDelimiterIsNotFound
            && (EndDelimiterIsNotFound || EndDelimiterFollowsFirstStartDelimiter)))
        {
            AppendFirstStartDelimiter();
            shouldCheckForSecondDelimiter = true;
        }
        else if (EndDelimiterPrecedesFirstStartDelimiter
            && ((FirstStartDelimiterIsFound && SecondStartDelimiterIsFound && EndDelimiterIsFound)
            || (SecondStartDelimiterIsNotFound && ((FirstStartDelimiterIsFound && EndDelimiterIsEscaped)
            || (FirstStartDelimiterIsNotEscaped && EndDelimiterIsNotEscaped)))))
        {
            AppendEndDelimiter();
            shouldCheckForSecondDelimiter = true;
        }

        return shouldCheckForSecondDelimiter;
    }

    /// <summary>
    /// Appends the first found token start delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token end delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the end delimiter. <br/> If the current
    /// search index value is lower than the token end delimiter index position then the text from
    /// the search index position up to the end delimiter position is appended first.
    /// </remarks>
    private void AppendFirstStartDelimiter()
    {
        bool shouldEscapeFirstStartDelimiter = FirstStartDelimiterIsNotEscaped
            && ((SecondStartDelimiterIsNotFound && (EndDelimiterIsNotFound || (EndDelimiterIsEscaped && EndDelimiterFollowsFirstStartDelimiter)))
            || (SecondStartDelimiterIsFound && EndDelimiterIsFound && EndDelimiterFollowsSecondStartDelimiter)
            || FoundInvalidTokenString);

        if (shouldEscapeFirstStartDelimiter & !TokenIsFound)
        {
            string message = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
            Logger.Log(LogSeverity.Error, message);
        }

        AppendTokenDelimiter(FirstStartDelimiterIndex, TokenStartDelimiter, shouldEscapeFirstStartDelimiter);
    }

    /// <summary>
    /// Appends the remaining text from the text template line to the end of the modified text if appropriate.
    /// </summary>
    private void AppendRemainingText()
    {
        if (FirstStartDelimiterIsFound && SecondStartDelimiterIsFound && EndDelimiterIsNotFound)
        {
            string message = GetMessage(MsgMissingTokenEndDelimiter);
            Logger.Log(LogSeverity.Error, message);
            EscapeAllDelimiters(TokenStartDelimiter);
        }
        else if (SecondStartDelimiterIsNotFound
            && ((FirstStartDelimiterIsNotFound && EndDelimiterIsFound)
            || (EndDelimiterFollowsFirstStartDelimiter && (FirstStartDelimiterIsEscaped || EndDelimiterIsEscaped))
            || (FirstStartDelimiterIsEscaped && EndDelimiterIsNotEscaped && EndDelimiterPrecedesFirstStartDelimiter)))
        {
            string message = GetMessage(MsgMissingTokenStartDelimiter);
            Logger.Log(LogSeverity.Error, message);
            EscapeAllDelimiters(TokenEndDelimiter);
        }
        else if (SecondStartDelimiterIsNotFound || EndDelimiterIsNotFound)
        {
            _ = ModifiedText.Append(TemplateText[StartSearchIndex..TemplateText.Length]);
            StartSearchIndex = TemplateText.Length;
        }
    }

    /// <summary>
    /// Appends the second found delimiter and any preceding text to the end of the modified text if appropriate.
    /// </summary>
    private void AppendSecondFoundDelimiter()
    {
        if (EndDelimiterFollowsSecondStartDelimiter && FirstStartDelimiterIsFound
            && ((SecondStartDelimiterIsFound && EndDelimiterIsEscaped)
            || (SecondStartDelimiterIsEscaped && EndDelimiterIsNotEscaped)))
        {
            AppendSecondStartDelimiter();
        }
        else if (EndDelimiterFollowsFirstStartDelimiter
            && ((FirstStartDelimiterIsNotEscaped && ((SecondStartDelimiterIsNotFound && EndDelimiterIsNotEscaped)
            || (SecondStartDelimiterIsFound && EndDelimiterIsFound)))
            || (FirstStartDelimiterIsEscaped && SecondStartDelimiterIsFound && EndDelimiterIsFound)))
        {
            AppendEndDelimiter();
        }
    }

    /// <summary>
    /// Appends the second found token start delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token end delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the end delimiter. <br/> If the current
    /// search index value is lower than the token end delimiter index position then the text from
    /// the search index position up to the end delimiter position is appended first.
    /// </remarks>
    private void AppendSecondStartDelimiter()
    {
        bool shouldEscapeSecondStartDelimiter = SecondStartDelimiterIsNotEscaped && EndDelimiterIsEscaped && EndDelimiterFollowsSecondStartDelimiter;

        if (shouldEscapeSecondStartDelimiter)
        {
            string message = GetMessage(MsgTokenStartDelimiterWillBeEscaped);
            Logger.Log(LogSeverity.Error, message);
        }

        AppendTokenDelimiter(SecondStartDelimiterIndex, TokenStartDelimiter, shouldEscapeSecondStartDelimiter);
    }

    /// <summary>
    /// Appends the given token delimiter string to the end of the modified template text line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token delimiter should be escaped. If so, the delimiter
    /// escape character is inserted ahead of the token delimiter. <br/> If the current search index
    /// value is lower than the token delimiter index position then the text from the search index
    /// position up to the token delimiter position is appended first.
    /// </remarks>
    /// <param name="delimiterIndex">
    /// The index position of the token delimiter within the current template text line.
    /// </param>
    /// <param name="delimiter">
    /// The token delimiter string found at the delimiter index position.
    /// </param>
    /// <param name="shouldEscapeDelimiter">
    /// A value indicating whether or not the delimiter should be escaped when appending it to the
    /// modified template text line.
    /// </param>
    private void AppendTokenDelimiter(int delimiterIndex, string delimiter, bool shouldEscapeDelimiter)
    {
        bool shouldInsertEscapeCharacter = shouldEscapeDelimiter;

        if (delimiterIndex > StartSearchIndex)
        {
            _ = ModifiedText.Append(TemplateText[StartSearchIndex..delimiterIndex]);
        }

        if (shouldInsertEscapeCharacter && IsDelimiterNotEscaped(delimiterIndex))
        {
            _ = ModifiedText.Append(DelimiterEscapeCharacter);
            IsTextModified = true;
        }

        _ = ModifiedText.Append(delimiter);
        StartSearchIndex = delimiterIndex + delimiter.Length;
    }

    /// <summary>
    /// Determines the order of the first and second found token start delimiters and the found
    /// token end delimiter.
    /// </summary>
    private void DetermineOrderOfDelimiters()
    {
        if (FirstStartDelimiterIsFound)
        {
            if (EndDelimiterIsFound)
            {
                if (SecondStartDelimiterIsFound)
                {
                    if (EndDelimiterIndex < FirstStartDelimiterIndex)
                    {
                        SetDelimiterOrder(DelimiterType.EndDelimiter,
                                          DelimiterType.FirstStartDelimiter,
                                          DelimiterType.SecondStartDelimiter);
                    }
                    else if (EndDelimiterIndex < SecondStartDelimiterIndex)
                    {
                        SetDelimiterOrder(DelimiterType.FirstStartDelimiter,
                                          DelimiterType.EndDelimiter,
                                          DelimiterType.SecondStartDelimiter);
                    }
                    else
                    {
                        SetDelimiterOrder(DelimiterType.FirstStartDelimiter,
                                          DelimiterType.SecondStartDelimiter,
                                          DelimiterType.EndDelimiter);
                    }
                }
                else if (EndDelimiterIndex < FirstStartDelimiterIndex)
                {
                    SetDelimiterOrder(DelimiterType.EndDelimiter,
                                      DelimiterType.FirstStartDelimiter,
                                      DelimiterType.NotFound);
                }
                else
                {
                    SetDelimiterOrder(DelimiterType.FirstStartDelimiter,
                                      DelimiterType.EndDelimiter,
                                      DelimiterType.NotFound);
                }
            }
            else if (SecondStartDelimiterIsFound)
            {
                SetDelimiterOrder(DelimiterType.FirstStartDelimiter,
                                  DelimiterType.SecondStartDelimiter,
                                  DelimiterType.NotFound);
            }
            else
            {
                SetDelimiterOrder(DelimiterType.FirstStartDelimiter,
                                  DelimiterType.NotFound,
                                  DelimiterType.NotFound);
            }
        }
        else if (EndDelimiterIsFound)
        {
            SetDelimiterOrder(DelimiterType.EndDelimiter,
                              DelimiterType.NotFound,
                              DelimiterType.NotFound);
        }
        else
        {
            SetDelimiterOrder(DelimiterType.NotFound,
                              DelimiterType.NotFound,
                              DelimiterType.NotFound);
        }
    }

    /// <summary>
    /// Escapes all occurrences of the given delimiter string within the current template text line
    /// and appends the results to the modified template text line.
    /// </summary>
    /// <param name="delimiter">
    /// The delimiter string to be escaped.
    /// </param>
    private void EscapeAllDelimiters(string delimiter)
    {
        int delimiterIndex;

        do
        {
            delimiterIndex = FindNextDelimiter(StartSearchIndex, delimiter);

            if (delimiterIndex > NotFound)
            {
                AppendTokenDelimiter(delimiterIndex, delimiter, true);
                StartSearchIndex = delimiterIndex + delimiter.Length;
            }
        } while (delimiterIndex > NotFound);

        if (StartSearchIndex < TemplateText.Length)
        {
            _ = ModifiedText.Append(TemplateText[StartSearchIndex..TemplateText.Length]);
        }

        StartSearchIndex = TemplateText.Length;
    }

    /// <summary>
    /// Finds the next occurrence of the given token delimiter string in the current template text line.
    /// </summary>
    /// <param name="startIndex">
    /// The index position with the current template text line where the search will begin.
    /// </param>
    /// <param name="delimiter">
    /// The delimiter string to be searched for.
    /// </param>
    /// <returns>
    /// The index position of the next occurrence of the given token delimiter string, or -1 if no
    /// more occurrences are found.
    /// </returns>
    private int FindNextDelimiter(int startIndex, string delimiter)
        => TemplateText.IndexOf(delimiter, startIndex, StringComparison.Ordinal);

    /// <summary>
    /// Finds the next token within the current template text line.
    /// </summary>
    private void FindNextToken()
    {
        FirstStartDelimiterIndex = FindNextDelimiter(StartSearchIndex, TokenStartDelimiter);

        SecondStartDelimiterIndex = FirstStartDelimiterIndex > NotFound && FirstStartDelimiterIndex < TemplateText.Length - TokenStartDelimiter.Length
            ? FindNextDelimiter(FirstStartDelimiterIndex + TokenStartDelimiter.Length, TokenStartDelimiter)
            : NotFound;

        EndDelimiterIndex = FindNextDelimiter(StartSearchIndex, TokenEndDelimiter);

        DetermineOrderOfDelimiters();

        if (TokenIsFound)
        {
            VerifyFoundToken();
        }

        if (IsInitialPass)
        {
            UpdateModifiedText();
        }
        else if (!EndOfText)
        {
            StartSearchIndex = GetNextSearchIndex();
        }
    }

    /// <summary>
    /// Gets the starting index position for the next iteration of the token search.
    /// </summary>
    /// <returns>
    /// The starting index position for the next iteration of the token search. <br/> The position
    /// will be at the end of the line if it is determined that no more tokens exist on the current
    /// template text line.
    /// </returns>
    private int GetNextSearchIndex()
    {
        int nextSearchIndex = EndDelimiterIndex + TokenEndDelimiter.Length;

        if (FirstStartDelimiterIsFound && SecondStartDelimiterIsNotEscaped && EndDelimiterIsNotEscaped && EndDelimiterFollowsSecondStartDelimiter)
        {
            nextSearchIndex = SecondStartDelimiterIndex;
        }
        else if (EndDelimiterFollowsSecondStartDelimiter
            && ((SecondStartDelimiterIsEscaped && EndDelimiterIsFound)
                || (FirstStartDelimiterIsFound && SecondStartDelimiterIsNotEscaped && EndDelimiterIsEscaped)))
        {
            nextSearchIndex = SecondStartDelimiterIndex + TokenStartDelimiter.Length;
        }
        else if ((SecondStartDelimiterIsNotFound
            && (FirstStartDelimiterIsNotFound || EndDelimiterIsNotFound
            || (EndDelimiterFollowsFirstStartDelimiter && (FirstStartDelimiterIsEscaped || EndDelimiterIsEscaped))))
            || (FirstStartDelimiterIsFound && SecondStartDelimiterIsFound && EndDelimiterIsNotFound))
        {
            nextSearchIndex = TemplateText.Length;
        }

        return nextSearchIndex;
    }

    /// <summary>
    /// Gets a value indicating whether the token delimiter at the given index position within the
    /// current template text line is escaped.
    /// </summary>
    /// <param name="delimiterIndex">
    /// The index position of a found token delimiter within the current template text line.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the delimiter is escaped, otherwise <see langword="false"/>
    /// </returns>
    private bool IsDelimiterEscaped(int delimiterIndex)
        => delimiterIndex > 0 && TemplateText[delimiterIndex - 1] == DelimiterEscapeCharacter;

    /// <summary>
    /// Gets a value indicating whether the token delimiter at the given index position within the
    /// current template text line is not escaped.
    /// </summary>
    /// <param name="delimiterIndex">
    /// The index position of a found token delimiter within the current template text line.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the delimiter is not escaped, otherwise <see langword="false"/>
    /// </returns>
    private bool IsDelimiterNotEscaped(int delimiterIndex)
        => delimiterIndex == 0 || (delimiterIndex > 0 && TemplateText[delimiterIndex - 1] != DefaultTokenEscapeCharacter);

    /// <summary>
    /// Sets the found delimiter order based on the passed in parameter values.
    /// </summary>
    /// <param name="firstDelimiter">
    /// The delimiter type of the first found delimiter.
    /// </param>
    /// <param name="secondDelimiter">
    /// The delimiter type of the second found delimiter.
    /// </param>
    /// <param name="thirdDelimiter">
    /// The delimiter type of the third found delimiter.
    /// </param>
    private void SetDelimiterOrder(DelimiterType firstDelimiter,
                                   DelimiterType secondDelimiter,
                                   DelimiterType thirdDelimiter)
    {
        _foundDelimiters[0] = firstDelimiter;
        _foundDelimiters[1] = secondDelimiter;
        _foundDelimiters[2] = thirdDelimiter;
    }

    /// <summary>
    /// Updates the modified template text line by appending the portion of the current template
    /// text line that we are finished parsing. Invalid token delimiters are escaped as necessary.
    /// </summary>
    private void UpdateModifiedText()
    {
        bool shouldCheckForSecondDelimiter = AppendFirstFoundDelimiter();

        if (shouldCheckForSecondDelimiter)
        {
            AppendSecondFoundDelimiter();
        }

        AppendRemainingText();
    }

    /// <summary>
    /// Verifies that the token that was found is valid.
    /// </summary>
    /// <remarks>
    /// A valid token is one that contains only a valid token name and optional whitespace
    /// characters between the token start and token end delimiters. <br/> An optional case
    /// indicator flag character is also allowed immediately after the token start delimiter.
    /// </remarks>
    private void VerifyFoundToken()
    {
        int tokenNameStart = FirstStartDelimiterIndex + TokenStartDelimiter.Length;
        int tokenNameEnd = EndDelimiterIndex;
        int tokenStringStart = FirstStartDelimiterIndex;
        int tokenStringEnd = EndDelimiterIndex + TokenEndDelimiter.Length;
        string tokenString = TemplateText[tokenStringStart..tokenStringEnd];
        char caseFlag = SameCaseFlag;

        if (TemplateText[tokenNameStart] is UppercaseFlag or LowercaseFlag or SameCaseFlag)
        {
            caseFlag = TemplateText[tokenNameStart];
            tokenNameStart++;
        }

        string tokenName = TemplateText[tokenNameStart..tokenNameEnd].Trim();

        if (string.IsNullOrWhiteSpace(tokenName))
        {
            string message = GetMessage(MsgMissingTokenName);
            Logger.Log(LogSeverity.Error, message);
        }
        else if (IsValidName(tokenName))
        {
            FoundTokenString = tokenString;
            FoundTokenName = tokenName;
            FoundCaseFlag = caseFlag;
            ValidTokenFound = true;
        }
        else
        {
            string message = GetMessage(MsgTokenHasInvalidName,
                                        tokenName);
            Logger.Log(LogSeverity.Error, message);
        }
    }
}