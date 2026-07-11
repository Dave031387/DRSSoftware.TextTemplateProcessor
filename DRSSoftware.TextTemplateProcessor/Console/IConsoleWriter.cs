namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="IConsoleWriter" /> interface defines a contract for writing a string of text to
/// the console.
/// </summary>
internal interface IConsoleWriter
{
    /// <summary>
    /// Write a line of text to the console.
    /// </summary>
    /// <param name="text">
    /// The text string to be written to the console. If the string is null, it will be treated as
    /// an empty string.
    /// </param>
    public void WriteLine(string text);
}