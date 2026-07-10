namespace DRSSoftware.TextTemplateProcessor.Core;

using System.Text.RegularExpressions;

/// <summary>
/// Helper class for handling various <see cref="Regex" /> pattern matching operations against given
/// string values.
/// </summary>
internal static partial class RegexHelper
{
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
    /// Gets a value indicating whether the given <paramref name="compositeString" /> contains any
    /// format items.
    /// </summary>
    /// <param name="compositeString">
    /// The composite string containing zero or more format items.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="compositeString" /> contains format items;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasFormatItems(string compositeString)
        => FormatItemRegex().IsMatch(compositeString);

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
    public static bool HasOneTimeAbsoluteIndentCode(string textLine)
        => OneTimeAbsoluteIndentCodeRegex().IsMatch(textLine);

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
    /// relative one-time left indent indicator string.
    /// </summary>
    /// <param name="textLine">
    /// The text template line to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="textLine" /> begins with a relative one-time left
    /// indent indicator string; otherwise, <see langword="false" />.
    /// </returns>
    public static bool HasOneTimeRelativeLeftIndentCode(string textLine)
        => OneTimeRelativeLeftIndentCodeRegex().IsMatch(textLine);

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
    public static bool HasOneTimeRelativeRightIndentCode(string textLine)
        => OneTimeRelativeRightIndentCodeRegex().IsMatch(textLine);

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
    [GeneratedRegex($@"^{Normal}{Absolute}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex AbsoluteIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent or absolute one-time indent indicator string.
    /// </summary>
    /// <remarks>
    /// A <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// indent or absolute one-time indent indicator string.
    /// </remarks>
    [GeneratedRegex($@"^[{Normal}{OneTime}]{Absolute}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex AbsoluteIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a comment
    /// line indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a comment line
    /// indicator string.
    /// </returns>
    [GeneratedRegex($@"^{CommentCode} ", RegexOptions.Compiled)]
    private static partial Regex CommentLineRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching format items in a composite string.
    /// </summary>
    /// <remarks>
    /// Only format items with index values between 0 and 9 (inclusive) are matched. Format items
    /// with index values greater than 9 are not matched.
    /// </remarks>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching format items in a composite string.
    /// </returns>
    [GeneratedRegex($@"\{{{AnyDigit}}}", RegexOptions.Compiled)]
    private static partial Regex FormatItemRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with any valid
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with any valid indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex($@"^[{Normal}{OneTime}][{Absolute}{RelativeLeft}{RelativeRight}]{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex IndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that don't begin with any
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that don't begin with any indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex($@"^{NoControlCode} ", RegexOptions.Compiled)]
    private static partial Regex NoControlCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// one-time indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with an absolute
    /// one-time indent indicator string.
    /// </returns>
    [GeneratedRegex($@"^{OneTime}{Absolute}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex OneTimeAbsoluteIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a one-time
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a one-time indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex($@"^{OneTime}[{Absolute}{RelativeLeft}{RelativeRight}]{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex OneTimeIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time left indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time left indent indicator string.
    /// </returns>
    [GeneratedRegex($@"^{OneTime}{RelativeLeft}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex OneTimeRelativeLeftIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time right indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// one-time right indent indicator string.
    /// </returns>
    [GeneratedRegex($@"^{OneTime}{RelativeRight}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex OneTimeRelativeRightIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative indent
    /// indicator string.
    /// </returns>
    [GeneratedRegex($@"^[{Normal}{OneTime}][{RelativeLeft}{RelativeRight}]{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex RelativeIndentRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// left indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative left
    /// indent indicator string.
    /// </returns>
    [GeneratedRegex($@"^{Normal}{RelativeLeft}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex RelativeLeftIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text lines that begin with a relative
    /// right indent indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text lines that begin with a relative right
    /// indent indicator string.
    /// </returns>
    [GeneratedRegex($@"^{Normal}{RelativeRight}{AnyDigit} ", RegexOptions.Compiled)]
    private static partial Regex RelativeRightIndentCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text template lines that begin with a
    /// segment header indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text template lines that begin with a
    /// segment header indicator string.
    /// </returns>
    [GeneratedRegex($@"^{SegmentHeaderCode} ", RegexOptions.Compiled)]
    private static partial Regex SegmentHeaderRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for matching text template lines that begin with any
    /// valid indicator string.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for matching text template lines that begin with any
    /// valid indicator string.
    /// </returns>
    [GeneratedRegex($@"^([{Normal}{OneTime}][{Absolute}{RelativeLeft}{RelativeRight}]{AnyDigit}|{NoControlCode}|{CommentCode}|{SegmentHeaderCode}) ", RegexOptions.Compiled)]
    private static partial Regex ValidControlCodeRegex();

    /// <summary>
    /// Gets a <see cref="Regex" /> object used for validating segment names on a segment header
    /// line.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex" /> object used for validating segment names on a segment header line.
    /// </returns>
    [GeneratedRegex($@"^({AnyLetter})({AnyLetter}|{AnyDigit}|_)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex ValidNameRegex();
}