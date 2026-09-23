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
/// <param name="Case">
/// A character flag indicating how to handle the first character of the value that gets
/// assigned to the token.
/// </param>
[ExcludeFromCodeCoverage]
internal record TokenInfo(string TokenString, string TokenName, char Case);