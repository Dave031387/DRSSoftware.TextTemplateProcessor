namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="MessageWriter" /> class is used by the Text Template Processor for writing
/// messages to the console.
/// </summary>
internal class MessageWriter : DependencyCheckerBase, IMessageWriter
{
    /// <summary>
    /// Create an instance of the <see cref="MessageWriter" /> class and initializes its
    /// dependencies.
    /// </summary>
    /// <param name="consoleWriter">
    /// The <see cref="IConsoleWriter" /> instance to use for writing text to the console.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="consoleWriter" /> parameter is <see langword="null" />.
    /// </exception>
    internal MessageWriter(IConsoleWriter consoleWriter)
    {
        NullDependencyCheck(consoleWriter,
                            nameof(MessageWriter),
                            nameof(IConsoleWriter),
                            nameof(consoleWriter));

        ConsoleWriter = consoleWriter;
    }

    /// <summary>
    /// Gets the <see cref="IConsoleWriter" /> instance used for writing text to the console.
    /// </summary>
    private IConsoleWriter ConsoleWriter
    {
        get; init;
    }

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
    public void WriteLine(string message, params string[] args)
        => ConsoleWriter.WriteLine(FormatMessage(message, args));
}