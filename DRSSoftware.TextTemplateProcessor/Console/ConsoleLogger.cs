namespace DRSSoftware.TextTemplateProcessor.Console;

/// <summary>
/// The <see cref="ConsoleLogger" /> class is a logger implementation that writes log messages to
/// the console.
/// </summary>
internal class ConsoleLogger : DependencyCheckerBase, ILogger
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
    internal ConsoleLogger(ILocater locater, IMessageWriter messageWriter)
    {
        NullDependencyCheck(messageWriter,
                            nameof(ConsoleLogger),
                            nameof(IMessageWriter),
                            nameof(messageWriter));

        NullDependencyCheck(locater,
                            nameof(ConsoleLogger),
                            nameof(ILocater),
                            nameof(locater));

        MessageWriter = messageWriter;
        Locater = locater;
    }

    /// <summary>
    /// Gets or sets the type of log entry currently being processed.
    /// </summary>
    /// <remarks>
    /// This property is initialized to <see cref="LogEntryType.Setup" /> when the
    /// <see cref="ConsoleLogger" /> object is created.
    /// </remarks>
    public LogEntryType CurrentLogEntryType
    {
        get;
        set;
    } = DefaultLogEntryType;

    /// <summary>
    /// An <see cref="ILocater" /> object that is used to locate the current position in the text
    /// template file.
    /// </summary>
    private ILocater Locater
    {
        get;
        init;
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
    /// Creates a new <see cref="LogEntry" /> object and writes it to the console.
    /// </summary>
    /// <param name="message">
    /// The log message that is being written to the console.
    /// </param>
    /// <param name="args">
    /// An array of <see langword="string" /> values to be substituted for the format arguments in
    /// the <paramref name="message" /> parameter.
    /// </param>
    public void Log(string message, params string[] args)
    {
        string formattedMessage = FormatMessage(message, args);

        switch (CurrentLogEntryType)
        {
            case LogEntryType.Setup:
            case LogEntryType.Loading:
            case LogEntryType.Writing:
            case LogEntryType.Reset:
            case LogEntryType.User:
                MessageWriter.WriteLine(new LogEntry(CurrentLogEntryType, Location.Empty, formattedMessage).ToString());
                break;

            case LogEntryType.Parsing:
            case LogEntryType.Generating:
                MessageWriter.WriteLine(new LogEntry(CurrentLogEntryType, Locater.Location, formattedMessage).ToString());
                break;

            default:
                break;
        }
    }
}