using System.Diagnostics.CodeAnalysis;

namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This record is used to store information about a token found in a text string.
/// </summary>
/// <param name="TokenString">
/// The entire token string that was found, including the start and end delimiters.
/// </param>
/// <param name="TokenName">
/// The name of the token.
/// </param>
/// <param name="CaseFlag">
/// A character flag indicating whether the first character of the token substitution string should
/// be lowercase, uppercase, or unchanged.
/// </param>
[ExcludeFromCodeCoverage]
internal record TokenInfo(string TokenString, string TokenName, char CaseFlag)
{
    /// <summary>
    /// Gets a value indicating whether or not this <see cref="TokenInfo"/> instance is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(TokenString);

    /// <summary>
    /// Gets an empty instance of the TokenInfo record.
    /// </summary>
    public static TokenInfo Empty => new(string.Empty, string.Empty, SameCaseFlag);
}