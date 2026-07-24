using System.Runtime.CompilerServices;
using DRSSoftware.TextTemplateProcessor.Console;

[assembly: InternalsVisibleTo("DRSSoftware.TextTemplateProcessor.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This static class defines certain constant values and fixed properties that are used throughout
/// the Text Template Processor application.
/// </summary>
internal static class GlobalConstants
{
    /// <summary>
    /// Character used to represent an absolute indent.
    /// </summary>
    public const string Absolute = "=";

    /// <summary>
    /// Pattern string for matching any digit in a Regex expression.
    /// </summary>
    public const string AnyDigit = "[0-9]";

    /// <summary>
    /// Pattern string for matching any letter in a Regex expression.
    /// </summary>
    /// <remarks>
    /// Assumes that the option <c> RegexOptions.IgnoreCase </c> is used when compiling the Regex
    /// expression.
    /// </remarks>
    public const string AnyLetter = "[a-z]";

    /// <summary>
    /// The string that is used to indicate the start of a comment line in a text template file.
    /// </summary>
    public const string CommentCode = "///";

    /// <summary>
    /// The string that is used for denoting debug severity level in a log message.
    /// </summary>
    public const string DebugSeverity = "DEBUG--->";

    /// <summary>
    /// The default string that is used to indicate the end of a token in a text template file.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DefaultStartDelimiter" /> to define the start and end of
    /// a token.
    /// </remarks>
    public const string DefaultEndDelimiter = "#>";

    /// <summary>
    /// The default character that is used to escape special characters in a text template file.
    /// </summary>
    public const char DefaultEscapeCharacter = '\\';

    /// <summary>
    /// The prefix string that appears at the start of any default file name.
    /// </summary>
    public const string DefaultFileNamePrefix = "File_";

    /// <summary>
    /// The suffix string that appears at the end of any default file name.
    /// </summary>
    public const string DefaultFileNameSuffix = ".txt";

    /// <summary>
    /// The default <see cref="OperationType" /> that is used when creating a new instance of the
    /// <see cref="ConsoleLogger" /> class.
    /// </summary>
    public const OperationType DefaultOperationType = OperationType.Setup;

    /// <summary>
    /// The prefix string that appears at the start of any default segment name.
    /// </summary>
    public const string DefaultSegmentNamePrefix = "DefaultSegment";

    /// <summary>
    /// The default string that is used to indicate the start of a token in a text template file.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DefaultEndDelimiter" /> to define the start and end of a
    /// token.
    /// </remarks>
    public const string DefaultStartDelimiter = "<#";

    /// <summary>
    /// The number of space characters making up the default tab size.
    /// </summary>
    public const int DefaultTabSize = 4;

    /// <summary>
    /// The string that is used for denoting error severity level in a log message.
    /// </summary>
    public const string ErrorSeverity = "ERROR--->";

    /// <summary>
    /// The string that represents the First Time Indent option.
    /// </summary>
    public const string FirstTimeIndentOption = "FTI";

    /// <summary>
    /// The string that is used for denoting information severity level in a log message.
    /// </summary>
    public const string InfoSeverity = "INFO---->";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be converted
    /// to lowercase when it is inserted into the output text.
    /// </summary>
    public const char LowercaseFlag = '-';

    /// <summary>
    /// The maximum supported indent value.
    /// </summary>
    public const int MaxIndentValue = 9;

    /// <summary>
    /// The maximum supported tab size value.
    /// </summary>
    public const int MaxTabSize = 9;

    /// <summary>
    /// The minimum supported indent value.
    /// </summary>
    public const int MinIndentValue = -9;

    /// <summary>
    /// The minimum supported tab size value.
    /// </summary>
    public const int MinTabSize = 1;

    /// <summary>
    /// Placeholder value indicating the absence of a control code.
    /// </summary>
    public const string NoControlCode = "   ";

    /// <summary>
    /// Character used to indicate a normal indent (as opposed to a one-time indent).
    /// </summary>
    public const string Normal = "@";

    /// <summary>
    /// Character used to indicate a one-time indent (as opposed to a normal indent).
    /// </summary>
    public const string OneTime = "O";

    /// <summary>
    /// The string that represents the Pad Segment Name option.
    /// </summary>
    public const string PadSegmentNameOption = "PAD";

    /// <summary>
    /// Character used to indicate a relative left indent in Regex patterns.
    /// </summary>
    public const string RelativeLeft = $@"\{RelativeLeftChar}";

    /// <summary>
    /// Character used to indicate a relative left indent.
    /// </summary>
    public const string RelativeLeftChar = "-";

    /// <summary>
    /// Character used to indicate a relative right indent in Regex patterns.
    /// </summary>
    public const string RelativeRight = $@"\{RelativeRightChar}";

    /// <summary>
    /// Character used to indicate a relative right indent.
    /// </summary>
    public const string RelativeRightChar = "+";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be left as-is
    /// when it is inserted into the output text.
    /// </summary>
    public const char SameCaseFlag = '=';

    /// <summary>
    /// The string that is used to indicate the start of a segment header line in a text template
    /// file.
    /// </summary>
    public const string SegmentHeaderCode = "###";

    /// <summary>
    /// The search pattern that is used in locating the solution directory.
    /// </summary>
    public const string SolutionFileSearchPattern1 = "*.sln";

    /// <summary>
    /// The search pattern that is used in locating the solution directory.
    /// </summary>
    public const string SolutionFileSearchPattern2 = "*.slnx";

    /// <summary>
    /// The string that represents the Tab Size option.
    /// </summary>
    public const string TabSizeOption = "TAB";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be converted
    /// to uppercase when it is inserted into the output text.
    /// </summary>
    public const char UppercaseFlag = '+';

    /// <summary>
    /// The string that is used for denoting warning severity level in a log message.
    /// </summary>
    public const string WarningSeverity = "WARNING->";

    /// <summary>
    /// Gets an array of characters that can be used for separating options on a segment header
    /// line.
    /// </summary>
    public static char[] SeparatorChars { get; } = [' ', ','];
}