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
    /// A constant value representing the found end token delimiter.
    /// </summary>
    private const int EndDelimiter = 0;

    /// <summary>
    /// A constant value representing the first found start token delimiter.
    /// </summary>
    private const int FirstStartDelimiter = 1;

    /// <summary>
    /// A constant value representing the second found start token delimiter.
    /// </summary>
    private const int SecondStartDelimiter = 2;

    /// <summary>
    /// An array used to determine the order of the found end token and start tokens.
    /// </summary>
    private readonly int[] _foundDelimiters = [NotFound, NotFound, NotFound];

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
        FoundTokenName = string.Empty;
        FoundTokenString = string.Empty;
        TemplateText = string.Empty;
        ModifiedText = new(200, 1000);
        IsTextModified = false;
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
    private char DelimiterEscapeCharacter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the found end delimiter follows the first found start delimiter.
    /// </summary>
    private bool EndDelimiterFollowsFirstStartDelimiter => _foundDelimiters[1] is EndDelimiter;

    /// <summary>
    /// Gets a value indicating whether the found end delimiter follows the second found start delimiter.
    /// </summary>
    private bool EndDelimiterFollowsSecondStartDelimiter => _foundDelimiters[2] is EndDelimiter;

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
    private bool EndDelimiterPrecedesFirstStartDelimiter => _foundDelimiters[0] is EndDelimiter;

    /// <summary>
    /// Gets a value indicating whether the found end delimiter appears between the first and second
    /// found start delimiters in the current template text line.
    /// </summary>
    private bool EndDelimiterPrecedesSecondStartDelimiter => _foundDelimiters[1] is EndDelimiter;

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
    }

    /// <summary>
    /// Gets or sets the complete token string that was found in the current template text line.
    /// </summary>
    private string FoundTokenString
    {
        get;
        set;
    }

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
    }

    /// <summary>
    /// Gets a value indicating whether no tokens were found in the current template text line.
    /// </summary>
    private bool NoMoreDelimitersFound => EndDelimiterIsNotFound && FirstStartDelimiterIsNotFound && SecondStartDelimiterIsNotFound;

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
    private int StartSearchIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the current line of text from the template.
    /// </summary>
    private string TemplateText
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the token end delimiter.
    /// </summary>
    private string TokenEndDelimiter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether a token string has been found in the current template text line.
    /// </summary>
    private bool TokenIsFound => FirstStartDelimiterIsNotEscaped && EndDelimiterIsNotEscaped && EndDelimiterFollowsFirstStartDelimiter;

    /// <summary>
    /// Gets or sets the value of the token start delimiter.
    /// </summary>
    private string TokenStartDelimiter
    {
        get;
        set;
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
    /// Appends the token delimiter escape character to the end of the modified template text line.
    /// </summary>
    private void AppendDelimiterEscapeCharacter() => ModifiedText.Append(DelimiterEscapeCharacter);

    /// <summary>
    /// Appends the next token end delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token end delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the end delimiter. <br/> If the <paramref
    /// name="segmentStart"/> value is lower than the token end delimiter index position then the
    /// text from the <paramref name="segmentStart"/> position up to the end delimiter position is
    /// appended first.
    /// </remarks>
    /// <param name="segmentStart">
    /// The starting index position of the portion of text containing the found token end delimiter
    /// within the template text line.
    /// </param>
    private void AppendEndDelimiter(int segmentStart)
    {
        bool shouldEscapeEndDelimiter = EndDelimiterIsNotEscaped && (FoundInvalidTokenString
            || (FirstStartDelimiterIsNotEscaped && SecondStartDelimiterIsNotFound && EndDelimiterPrecedesFirstStartDelimiter)
            || (FirstStartDelimiterIsEscaped && SecondStartDelimiterIsFound && EndDelimiterFollowsFirstStartDelimiter)
            || (FirstStartDelimiterIsFound && SecondStartDelimiterIsFound && EndDelimiterPrecedesFirstStartDelimiter));

        // TODO log an error message if the delimiter should be escaped
        AppendTokenDelimiter(segmentStart, EndDelimiterIndex, TokenEndDelimiter, shouldEscapeEndDelimiter);
    }

    /// <summary>
    /// Appends the first found token start delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token start delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the start delimiter. <br/> If the <paramref
    /// name="segmentStart"/> value is lower than the token start delimiter index position then the
    /// text from the <paramref name="segmentStart"/> position up to the token start delimiter
    /// position is appended first.
    /// </remarks>
    /// <param name="segmentStart">
    /// The starting index position of the portion of text containing the found token start
    /// delimiter within the template text line.
    /// </param>
    private void AppendFirstStartDelimiter(int segmentStart)
    {
        bool shouldEscapeFirstStartDelimiter = FirstStartDelimiterIsNotEscaped && (FoundInvalidTokenString
            || (SecondStartDelimiterIsNotFound && EndDelimiterIsNotFound)
            || (SecondStartDelimiterIsNotFound && EndDelimiterIsEscaped && EndDelimiterFollowsFirstStartDelimiter)
            || (SecondStartDelimiterIsNotEscaped && EndDelimiterIsEscaped && EndDelimiterFollowsSecondStartDelimiter)
            || (SecondStartDelimiterIsEscaped && EndDelimiterFollowsSecondStartDelimiter)
            || (SecondStartDelimiterIsFound && EndDelimiterIsEscaped && EndDelimiterPrecedesSecondStartDelimiter));

        // TODO log an error message if the delimiter should be escaped
        AppendTokenDelimiter(segmentStart, FirstStartDelimiterIndex, TokenStartDelimiter, shouldEscapeFirstStartDelimiter);
    }

    /// <summary>
    /// Appends the second found token start delimiter to the end of the modified text template line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token start delimiter should be escaped. If so, the
    /// delimiter escape character is inserted ahead of the start delimiter. <br/> If the <paramref
    /// name="segmentStart"/> value is lower than the token start delimiter index position then the
    /// text from the <paramref name="segmentStart"/> position up to the token start delimiter
    /// position is appended first.
    /// </remarks>
    /// <param name="segmentStart">
    /// The starting index position of the portion of text containing the found token start
    /// delimiter within the template text line.
    /// </param>
    private void AppendSecondStartDelimiter(int segmentStart)
    {
        bool shouldEscapeSecondStartDelimiter = SecondStartDelimiterIsNotEscaped && EndDelimiterIsEscaped && EndDelimiterFollowsSecondStartDelimiter;

        // TODO log an error message if the delimiter should be escaped
        AppendTokenDelimiter(segmentStart, SecondStartDelimiterIndex, TokenStartDelimiter, shouldEscapeSecondStartDelimiter);
    }

    /// <summary>
    /// Appends the given text string onto the end of the modified template text line.
    /// </summary>
    /// <param name="textSegment">
    /// The text string to be appended to the modified template text line.
    /// </param>
    private void AppendTextSegment(string textSegment) => ModifiedText.Append(textSegment);

    /// <summary>
    /// Appends the specified portion of text from the current template text line to the end of the
    /// modified template text line.
    /// </summary>
    /// <param name="segmentStart">
    /// The index position of the start of the desired portion of text in the current template text line.
    /// </param>
    /// <param name="segmentEnd">
    /// The index position of the next character after the end of the desired portion of text in the
    /// current template text line. <br/> Set the value to the template text line length in order to
    /// append the remainder of the current template text line to the modified template text line.
    /// </param>
    private void AppendTextSegment(int segmentStart, int segmentEnd) => ModifiedText.Append(TemplateText[segmentStart..segmentEnd]);

    /// <summary>
    /// Appends the given token delimiter string to the end of the modified template text line.
    /// </summary>
    /// <remarks>
    /// A determination is made whether the token delimiter should be escaped. If so, the delimiter
    /// escape character is inserted ahead of the token delimiter. <br/> If the <paramref
    /// name="segmentStart"/> value is lower than the token delimiter index position then the text
    /// from the <paramref name="segmentStart"/> position up to the token delimiter position is
    /// appended first.
    /// </remarks>
    /// <param name="segmentStart">
    /// The starting index position of the portion of text containing the token delimiter within the
    /// template text line.
    /// </param>
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
    private void AppendTokenDelimiter(int segmentStart, int delimiterIndex, string delimiter, bool shouldEscapeDelimiter)
    {
        bool shouldInsertEscapeCharacter = shouldEscapeDelimiter;

        if (delimiterIndex > segmentStart)
        {
            AppendTextSegment(segmentStart, delimiterIndex);
            shouldInsertEscapeCharacter = IsDelimiterEscaped(delimiterIndex);
        }

        if (shouldInsertEscapeCharacter)
        {
            AppendDelimiterEscapeCharacter();
            IsTextModified = true;
        }

        AppendTextSegment(delimiter);
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
                        _foundDelimiters[0] = EndDelimiter;
                        _foundDelimiters[1] = FirstStartDelimiter;
                        _foundDelimiters[2] = SecondStartDelimiter;
                    }
                    else if (EndDelimiterIndex < SecondStartDelimiterIndex)
                    {
                        _foundDelimiters[0] = FirstStartDelimiter;
                        _foundDelimiters[1] = EndDelimiter;
                        _foundDelimiters[2] = SecondStartDelimiter;
                    }
                    else
                    {
                        _foundDelimiters[0] = FirstStartDelimiter;
                        _foundDelimiters[1] = SecondStartDelimiter;
                        _foundDelimiters[2] = EndDelimiter;
                    }
                }
                else if (EndDelimiterIndex < FirstStartDelimiterIndex)
                {
                    _foundDelimiters[0] = EndDelimiter;
                    _foundDelimiters[1] = FirstStartDelimiter;
                    _foundDelimiters[2] = NotFound;
                }
                else
                {
                    _foundDelimiters[0] = FirstStartDelimiter;
                    _foundDelimiters[1] = EndDelimiter;
                    _foundDelimiters[2] = NotFound;
                }
            }
            else if (SecondStartDelimiterIsFound)
            {
                _foundDelimiters[0] = FirstStartDelimiter;
                _foundDelimiters[1] = SecondStartDelimiter;
                _foundDelimiters[2] = NotFound;
            }
            else
            {
                _foundDelimiters[0] = FirstStartDelimiter;
                _foundDelimiters[1] = NotFound;
                _foundDelimiters[2] = NotFound;
            }
        }
        else if (EndDelimiterIsFound)
        {
            _foundDelimiters[0] = EndDelimiter;
            _foundDelimiters[1] = NotFound;
            _foundDelimiters[2] = NotFound;
        }
        else
        {
            _foundDelimiters[0] = NotFound;
            _foundDelimiters[1] = NotFound;
            _foundDelimiters[2] = NotFound;
        }
    }

    /// <summary>
    /// Escapes all occurrences of the given delimiter string within the current template text line
    /// and appends the results to the modified template text line.
    /// </summary>
    /// <param name="segmentStart">
    /// The index position within the current template text line to start looking for the specified
    /// delimiter string.
    /// </param>
    /// <param name="delimiter">
    /// The delimiter string to be escaped.
    /// </param>
    private void EscapeAllDelimiters(int segmentStart, string delimiter)
    {
        int startIndex = segmentStart;
        int delimiterIndex;

        do
        {
            delimiterIndex = FindNextDelimiter(startIndex, delimiter);

            if (delimiterIndex > NotFound)
            {
                AppendTokenDelimiter(segmentStart, delimiterIndex, delimiter, true);
                startIndex = delimiterIndex + delimiter.Length;
            }
        } while (delimiterIndex > NotFound);

        if (startIndex < TemplateText.Length)
        {
            AppendTextSegment(startIndex, TemplateText.Length);
        }
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
    private int FindNextDelimiter(int startIndex, string delimiter) => TemplateText.IndexOf(delimiter, startIndex, StringComparison.Ordinal);

    /// <summary>
    /// Finds the next token within the current template text line.
    /// </summary>
    private void FindNextToken()
    {
        FirstStartDelimiterIndex = FindNextDelimiter(StartSearchIndex, TokenStartDelimiter);

        if (FirstStartDelimiterIndex > 0 && FirstStartDelimiterIndex < TemplateText.Length - TokenStartDelimiter.Length)
        {
            SecondStartDelimiterIndex = FindNextDelimiter(FirstStartDelimiterIndex + TokenStartDelimiter.Length, TokenStartDelimiter);
        }

        EndDelimiterIndex = FindNextDelimiter(StartSearchIndex, TokenEndDelimiter);

        // The following method calls must happen in the order given because each method depends on
        // the results of the previous methods.
        DetermineOrderOfDelimiters();

        if (TokenIsFound)
        {
            VerifyFoundToken();
        }

        if (IsInitialPass)
        {
            UpdateModifiedText();
        }

        if (!EndOfText)
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
            && (SecondStartDelimiterIsEscaped
                || (FirstStartDelimiterIsEscaped && SecondStartDelimiterIsNotEscaped && EndDelimiterIsEscaped)))
        {
            nextSearchIndex = SecondStartDelimiterIndex + TokenStartDelimiter.Length;
        }
        else if ((SecondStartDelimiterIsNotFound && EndDelimiterIsNotFound)
            || (FirstStartDelimiterIsNotFound && SecondStartDelimiterIsNotFound && EndDelimiterIsFound)
            || (FirstStartDelimiterIsNotEscaped && SecondStartDelimiterIsFound && EndDelimiterIsNotFound)
            || (FirstStartDelimiterIsNotEscaped && SecondStartDelimiterIsNotFound && EndDelimiterIsEscaped && EndDelimiterFollowsFirstStartDelimiter)
            || (FirstStartDelimiterIsEscaped && SecondStartDelimiterIsFound && EndDelimiterIsNotFound)
            || (FirstStartDelimiterIsEscaped && SecondStartDelimiterIsNotFound && EndDelimiterIsNotEscaped))
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
    /// Updates the modified template text line by appending the portion of the current template
    /// text line that we are finished parsing. Invalid token delimiters are escaped as necessary.
    /// </summary>
    private void UpdateModifiedText()
    {
        if (NoMoreDelimitersFound)
        {
            AppendTextSegment(StartSearchIndex, TemplateText.Length);
            StartSearchIndex = TemplateText.Length;
            return;
        }

        if (FirstStartDelimiterIsFound && EndDelimiterIsNotFound)
        {
            // TODO log an error message
            EscapeAllDelimiters(StartSearchIndex, TokenStartDelimiter);
            StartSearchIndex = TemplateText.Length;
            return;
        }

        if ((FirstStartDelimiterIsNotFound || FirstStartDelimiterIsEscaped) && SecondStartDelimiterIsNotFound && EndDelimiterIsNotEscaped)
        {
            // TODO log an error message
            EscapeAllDelimiters(StartSearchIndex, TokenEndDelimiter);
            StartSearchIndex = TemplateText.Length;
            return;
        }

        int segmentStart = StartSearchIndex;

        for (int i = 0; i < _foundDelimiters.Length; i++)
        {
            bool done = false;

            switch (_foundDelimiters[i])
            {
                case FirstStartDelimiter:
                    AppendFirstStartDelimiter(segmentStart);
                    segmentStart += TokenStartDelimiter.Length;
                    break;

                case SecondStartDelimiter:
                    AppendSecondStartDelimiter(segmentStart);
                    segmentStart += TokenStartDelimiter.Length;
                    break;

                case EndDelimiter:
                    AppendEndDelimiter(segmentStart);
                    segmentStart += TokenEndDelimiter.Length;
                    break;

                default:
                    done = true;
                    break;
            }

            if (done)
            {
                break;
            }
        }
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