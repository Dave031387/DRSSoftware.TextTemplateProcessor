namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="IMessageWriter" /> interface defines the contract for a message writer that can
/// write messages to the console.
/// </summary>
internal interface IMessageWriter
{
    /// <summary>
    /// Write the given <paramref name="message" /> to the console after formatting it with the
    /// given <paramref name="args" />.
    /// </summary>
    /// <param name="message">
    /// The message to write to the console. This message can contain format item placeholders that
    /// will be replaced by the provided <paramref name="args" />.
    /// </param>
    /// <param name="args">
    /// Zero or more arguments to use for formatting the message.
    /// </param>
    public void WriteLine(string message, params string[] args);
}