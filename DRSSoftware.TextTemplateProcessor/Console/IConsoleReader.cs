namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="IConsoleReader" /> interface defines a contract for reading user input from the
/// console.
/// </summary>
internal interface IConsoleReader
{
    /// <summary>
    /// Read a line of text from the console.
    /// </summary>
    /// <returns>
    /// The text that was entered in the console by the user, or an empty string if no input was
    /// provided.
    /// </returns>
    public string ReadLine();
}