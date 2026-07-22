namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="ConsoleLogger" /> class is a logger implementation that writes log messages to
/// the console.
/// </summary>
internal class ConsoleLogger : LoggerBase
{
    /// <summary>
    /// Constructor that creates an instance of the <see cref="ConsoleLogger" /> class with the
    /// given dependencies.
    /// </summary>
    /// <param name="messageWriter">
    /// An <see cref="IMessageWriter" /> object that is used to write messages to the console.
    /// </param>
    /// <param name="locater">
    /// An <see cref="ILocater" /> object that is used to locate the current position in the text
    /// template file.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if any of the dependencies passed into the constructor are
    /// <see langword="null" />.
    /// </exception>
    internal ConsoleLogger(ILocater locater, IMessageWriter messageWriter) : base(locater)
    {
        NullDependencyCheck(messageWriter,
                            nameof(ConsoleLogger),
                            nameof(IMessageWriter),
                            nameof(messageWriter));

        MessageWriter = messageWriter;
    }

    /// <summary>
    /// An <see cref="IMessageWriter" /> object that is used to write messages to the console.
    /// </summary>
    private IMessageWriter MessageWriter
    {
        get;
        init;
    }

    /// <summary>
    /// Writes the given <paramref name="logEntry" /> to the console.
    /// </summary>
    /// <param name="logEntry">
    /// The log entry to be written to the console.
    /// </param>
    public override void WriteLogEntry(LogEntry logEntry) => MessageWriter.WriteLine(logEntry.ToString());
}