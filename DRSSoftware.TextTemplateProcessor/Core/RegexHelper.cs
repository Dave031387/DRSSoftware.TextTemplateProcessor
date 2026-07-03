namespace DRSSoftware.TextTemplateProcessor.Core;

using System.Text.RegularExpressions;

/// <summary>
/// Helper class for handling various <see cref="Regex" /> pattern matching operations against given
/// string values.
/// </summary>
internal static partial class RegexHelper
{
    /// <summary>
    /// Gets the number of format items in a composite string.
    /// </summary>
    /// <param name="compositeString">
    /// The composite string containing zero or more format items.
    /// </param>
    /// <returns>
    /// An integer value that is equal to the number of format items that were found in the
    /// composite string.
    /// </returns>
    public static int GetFormatItemCount(string compositeString)
        => FormatItemRegex().Count(compositeString);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with an
    /// absolute indent indicator string.
    /// </summary>
    /// <remarks>
    /// Both normal and one-time absolute indent indicator strings are considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with an absolute indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasAbsoluteIndent(string textLine)
        => AbsoluteIndentRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with an
    /// absolute indent indicator string.
    /// </summary>
    /// <remarks>
    /// Only normal absolute indent indicator strings are considered valid. One-time absolute indent
    /// indicator strings are not considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with an absolute indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasAbsoluteIndentCode(string textLine)
        => AbsoluteIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with an
    /// absolute one-time indent indicator string.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with an absolute one-time
    /// indent indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasAbsoluteOneTimeIndentCode(string textLine)
        => AbsoluteOneTimeIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with
    /// any valid indent indicator string.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with any valid indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasIndentCode(string textLine)
        => IndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> doesn't start
    /// with any control code.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> doesn't start with any control code;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasNoControlCode(string textLine)
        => NoControlCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with
    /// any one-time indent indicator string.
    /// </summary>
    /// <remarks>
    /// Both absolute and relative one-time indent indicator strings are considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with any one-time indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasOneTimeIndent(string textLine)
        => OneTimeIndentRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// relative indent indicator string.
    /// </summary>
    /// <remarks>
    /// Both normal and one-time relative indent indicator strings are considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasRelativeIndent(string textLine)
        => RelativeIndentRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// relative left indent indicator string.
    /// </summary>
    /// <remarks>
    /// Only normal relative left indent strings are considered valid. One-time relative left indent
    /// strings are not considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative left indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasRelativeLeftIndentCode(string textLine)
        => RelativeLeftIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// relative one-time left indent indicator string.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative one-time left
    /// indent indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasRelativeLeftOneTimeIndentCode(string textLine)
        => RelativeLeftOneTimeIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// relative right indent indicator string.
    /// </summary>
    /// <remarks>
    /// Only normal relative right indent strings are considered valid. One-time relative right
    /// indent strings are not considered valid.
    /// </remarks>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative right indent
    /// indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasRelativeRightIndentCode(string textLine)
        => RelativeRightIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// relative one-time right indent indicator string.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative one-time
    /// right indent indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasRelativeRightOneTimeIndentCode(string textLine)
        => RelativeRightOneTimeIndentCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> begins with a
    /// valid control code or no control code.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a valid control code or
    /// no control code; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasValidControlCode(string textLine)
        => ValidControlCodeRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> is a comment
    /// line.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> is a comment line; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool IsCommentLine(string textLine)
        => CommentLineRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="textLine" /> is a segment
    /// header line.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> is a segment header line; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool IsSegmentHeader(string textLine)
        => SegmentHeaderRegex().IsMatch(textLine);

    /// <summary>
    /// Gets a value indicating whether or not the given <paramref name="name" /> is valid.
    /// </summary>
    /// <param name="name">
    /// The name to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="name" /> is not <see langword="null" /> and
    /// constitutes a valid name. Otherwise, returns <see langword="false" />.
    /// </returns>
    public static bool IsValidName(string? name)
        => name is not null && ValidNameRegex().IsMatch(name);

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^@=[0-9] ", RegexOptions.Compiled)]
    private static partial Regex AbsoluteIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent or absolute one-time indent indicator string.
    /// </summary>
    /// <remarks>
    /// A <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent or absolute one-time indent indicator string.
    /// </remarks>
    [GeneratedRegex(@"^(O|@)=[0-9] ", RegexOptions.Compiled)]
    private static partial Regex AbsoluteIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// one-time indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// one-time indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^O=[0-9] ", RegexOptions.Compiled)]
    private static partial Regex AbsoluteOneTimeIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a comment
    /// line indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a comment line
    /// indicator string.
    /// </returns>
    [GeneratedRegex(@"^/// ", RegexOptions.Compiled)]
    private static partial Regex CommentLineRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching format items in a composite string.
    /// </summary>
    /// <remarks>
    /// Only format items with index values between 0 and 2 (inclusive) are matched. Format items
    /// with index values greater than 2 are not matched.
    /// </remarks>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching format items in a composite string.
    /// </returns>
    [GeneratedRegex(@"\{[0-2]}", RegexOptions.Compiled)]
    private static partial Regex FormatItemRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with any valid
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with any valid indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex(@"^[@O][\+\-=][0-9] ", RegexOptions.Compiled)]
    private static partial Regex IndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that don't begin with any
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that don't begin with any indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex(@"^    ", RegexOptions.Compiled)]
    private static partial Regex NoControlCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a one-time
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a one-time indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex(@"^O[\-\+=][0-9] ", RegexOptions.Compiled)]
    private static partial Regex OneTimeIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex(@"^(@|O)[\-\+][0-9] ", RegexOptions.Compiled)]
    private static partial Regex RelativeIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// left indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative left
    /// indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^@-[0-9] ", RegexOptions.Compiled)]
    private static partial Regex RelativeLeftIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time left indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time left indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^O-[0-9]", RegexOptions.Compiled)]
    private static partial Regex RelativeLeftOneTimeIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// right indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative right
    /// indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^@\+[0-9] ", RegexOptions.Compiled)]
    private static partial Regex RelativeRightIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time right indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time right indent indicator string.
    /// </returns>
    [GeneratedRegex(@"^O\+[0-9]", RegexOptions.Compiled)]
    private static partial Regex RelativeRightOneTimeIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text template lines that begin with a
    /// segment header indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text template lines that begin with a
    /// segment header indicator string.
    /// </returns>
    [GeneratedRegex(@"^### ", RegexOptions.Compiled)]
    private static partial Regex SegmentHeaderRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text template lines that begin with any
    /// valid indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text template lines that begin with any
    /// valid indicator string.
    /// </returns>
    [GeneratedRegex(@"^([@O][+-=][0-9]|   |///|###) ", RegexOptions.Compiled)]
    private static partial Regex ValidControlCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for validating segment names on a segment header
    /// line.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for validating segment names on a segment header line.
    /// </returns>
    [GeneratedRegex(@"^([a-z])+([a-z]|[0-9]|_)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex ValidNameRegex();
}