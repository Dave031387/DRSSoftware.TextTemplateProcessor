using System.Runtime.CompilerServices;

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
    /// The string that represents the First Time Indent option.
    /// </summary>
    public const string FirstTimeIndentOption = "FTI";

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
    /// The string that represents the Pad Segment Name option.
    /// </summary>
    public const string PadSegmentNameOption = "PAD";

    /// <summary>
    /// Used in a token to indicate that the first character of the token value should be left as-is
    /// when it is inserted into the output text.
    /// </summary>
    public const char SameCaseFlag = '=';

    /// <summary>
    /// The search pattern that is used in locating the solution directory.
    /// </summary>
    public const string SolutionFileSearchPattern = "*.sln";

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
    /// Gets an array of characters that can be used for separating options on a segment header
    /// line.
    /// </summary>
    public static char[] SeparatorChars { get; } = [' ', ','];
}