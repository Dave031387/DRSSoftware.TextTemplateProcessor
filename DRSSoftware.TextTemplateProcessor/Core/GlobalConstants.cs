using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DRSSoftware.TextTemplateProcessor.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This static class defines certain constant values and fixed properties that are used throughout
/// the Text Template Processor application.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class GlobalConstants
{
    /// <summary>
    /// Character value used to represent an absolute indent.
    /// </summary>
    public const char AbsoluteIndexChar = '=';

    /// <summary>
    /// String value used to represent an absolute indent.
    /// </summary>
    public const string AbsoluteIndexString = "=";

    /// <summary>
    /// The string that is used to indicate the start of a comment line in a text template file.
    /// </summary>
    public const string CommentCode = "///";

    /// <summary>
    /// An integer value equal to the length of the control code field of the text template file.
    /// Therefore, the first file header option will be the second argument.
    /// </summary>
    public const int ControlCodeLength = 3;

    /// <summary>
    /// The string that is used for denoting debug severity level in a log message.
    /// </summary>
    public const string DebugSeverity = "DEBUG::::";

    /// <summary>
    /// The default character used as the delimiter escape character in a text template file.
    /// </summary>
    /// <remarks>
    /// Used to escape the <see cref="DefaultTokenStartDelimiter"/> and <see
    /// cref="DefaultTokenEndDelimiter"/> when they should be treated as normal text rather than the
    /// start and end delimiters of a token.
    /// </remarks>
    public const char DefaultDelimiterEscapeCharacter = '\\';

    /// <summary>
    /// A constant string representation of <see cref="DefaultDelimiterEscapeCharacter"/>. Used
    /// mainly to simplify some unit test setups.
    /// </summary>
    public const string DefaultDelimiterEscapeString = "\\";

    /// <summary>
    /// The default <see cref="OperationType"/> that is used when creating a new instance of the
    /// <see cref="ConsoleLogger"/> class.
    /// </summary>
    public const OperationType DefaultOperationType = OperationType.Setup;

    /// <summary>
    /// The number of space characters making up the default tab size.
    /// </summary>
    public const int DefaultTabSize = 4;

    /// <summary>
    /// The default string that is used to indicate the end of a token in a text template file.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DefaultTokenStartDelimiter"/> to define the start and
    /// end of a token.
    /// </remarks>
    public const string DefaultTokenEndDelimiter = "#>";

    /// <summary>
    /// The default string that is used to indicate the start of a token in a text template file.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DefaultTokenEndDelimiter"/> to define the start and end
    /// of a token.
    /// </remarks>
    public const string DefaultTokenStartDelimiter = "<#";

    /// <summary>
    /// The string that represents the token delimiter escape option.
    /// </summary>
    public const string DelimiterEscapeOption = "ESC";

    /// <summary>
    /// The string that represents the token end delimiter option.
    /// </summary>
    public const string EndDelimiterOption = "EDEL";

    /// <summary>
    /// The string that is used for denoting error severity level in a log message.
    /// </summary>
    public const string ErrorSeverity = "ERROR::::";

    /// <summary>
    /// The string that is used to indicate the start of the file header line in the text template file.
    /// </summary>
    public const string FileHeaderCode = "@@@";

    /// <summary>
    /// The string used to identify the file header option group.
    /// </summary>
    public const string FileHeaderOptionGroup = "file header";

    /// <summary>
    /// The string that represents the First Time Indent option.
    /// </summary>
    public const string FirstTimeIndentOption = "FTI";

    /// <summary>
    /// The string that is used for denoting information severity level in a log message.
    /// </summary>
    public const string InfoSeverity = "INFO:::::";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be converted
    /// to lowercase when it is inserted into the output text.
    /// </summary>
    public const char LowercaseFlag = '-';

    /// <summary>
    /// The string representation of the lowercase flag.
    /// </summary>
    public const string LowercaseString = "-";

    /// <summary>
    /// The maximum supported indent value.
    /// </summary>
    public const int MaxIndentValue = 9;

    /// <summary>
    /// A string representation of the maximum indent value.
    /// </summary>
    public const string MaxIndentValueString = "9";

    /// <summary>
    /// The maximum supported tab size value.
    /// </summary>
    public const int MaxTabSize = 9;

    /// <summary>
    /// A string representation of the maximum tab size value.
    /// </summary>
    public const string MaxTabSizeString = "9";

    /// <summary>
    /// The minimum supported indent value.
    /// </summary>
    public const int MinIndentValue = -9;

    /// <summary>
    /// A string representation of the minimum indent value.
    /// </summary>
    public const string MinIndentValueString = "-9";

    /// <summary>
    /// The minimum supported tab size value.
    /// </summary>
    public const int MinTabSize = 1;

    /// <summary>
    /// A string representation of the minimum tab size value.
    /// </summary>
    public const string MinTabSizeString = "1";

    /// <summary>
    /// Placeholder value indicating the absence of a control code.
    /// </summary>
    public const string NoControlCode = "   ";

    /// <summary>
    /// String value used to indicate a normal indent (as opposed to a one-time indent).
    /// </summary>
    public const string NormalIndentString = "@";

    /// <summary>
    /// Represents the "not found" result of a search operation.
    /// </summary>
    public const int NotFound = -1;

    /// <summary>
    /// String value that is used to represent a null value when one is encountered while processing
    /// a text template file.
    /// </summary>
    public const string NullStringValue = "!!NULL!!";

    /// <summary>
    /// String value used to indicate a one-time indent (as opposed to a normal indent).
    /// </summary>
    public const string OneTimeIndentString = "O";

    /// <summary>
    /// Represents the character value used to separate an option name from its value in a segment
    /// header line in a text template file.
    /// </summary>
    public const char OptionValueSeparator = '=';

    /// <summary>
    /// The string that represents the Pad Segment Name option.
    /// </summary>
    public const string PadSegmentNameOption = "PAD";

    /// <summary>
    /// Pattern string for matching any digit in a Regex expression.
    /// </summary>
    public const string RegexAnyDigit = "[0-9]";

    /// <summary>
    /// Pattern string for matching any letter in a Regex expression.
    /// </summary>
    /// <remarks>
    /// Assumes that the option <c>RegexOptions.IgnoreCase</c> is used when compiling the Regex expression.
    /// </remarks>
    public const string RegexAnyLetter = "[a-z]";

    /// <summary>
    /// String used to indicate a relative left indent in Regex patterns.
    /// </summary>
    public const string RegexRelativeLeftIndent = $@"\{RelativeLeftIndentString}";

    /// <summary>
    /// String used to indicate a relative right indent in Regex patterns.
    /// </summary>
    public const string RegexRelativeRightIndent = $@"\{RelativeRightIndentString}";

    /// <summary>
    /// String value used to indicate a relative left indent.
    /// </summary>
    public const string RelativeLeftIndentString = "-";

    /// <summary>
    /// Character value used to indicate a relative right indent.
    /// </summary>
    public const char RelativeRightIndentChar = '+';

    /// <summary>
    /// String value used to indicate a relative right indent.
    /// </summary>
    public const string RelativeRightIndentString = "+";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be left as-is
    /// when it is inserted into the output text.
    /// </summary>
    public const char SameCaseFlag = '=';

    /// <summary>
    /// The string representation of the same case flag.
    /// </summary>
    public const string SameCaseString = "=";

    /// <summary>
    /// The string that is used to indicate the start of a segment header line in a text template file.
    /// </summary>
    public const string SegmentHeaderCode = "###";

    /// <summary>
    /// The string used to identify the segment header option group.
    /// </summary>
    public const string SegmentHeaderOptionGroup = "segment header";

    /// <summary>
    /// The search pattern that is used in locating the solution file with the ".sln" file extension.
    /// </summary>
    public const string SolutionFileSearchPattern1 = "*.sln";

    /// <summary>
    /// The search pattern that is used in locating the solution file with the ".slnx" file extension.
    /// </summary>
    public const string SolutionFileSearchPattern2 = "*.slnx";

    /// <summary>
    /// The string that represents the token start delimiter option.
    /// </summary>
    public const string StartDelimiterOption = "SDEL";

    /// <summary>
    /// The string that represents the Tab Size option.
    /// </summary>
    public const string TabSizeOption = "TAB";

    /// <summary>
    /// The string that represents an unknown or missing segment name.
    /// </summary>
    public const string UnknownSegmentName = "UnknownSegment";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be converted
    /// to uppercase when it is inserted into the output text.
    /// </summary>
    public const char UppercaseFlag = '+';

    /// <summary>
    /// The string representation of the uppercase flag.
    /// </summary>
    public const string UppercaseString = "+";

    /// <summary>
    /// The string that is used for denoting warning severity level in a log message.
    /// </summary>
    public const string WarningSeverity = "WARNING::";

    /// <summary>
    /// Gets an array of characters that can be used for separating options on a segment header line.
    /// </summary>
    public static char[] OptionSeparatorChars { get; } = [' ', ','];
}