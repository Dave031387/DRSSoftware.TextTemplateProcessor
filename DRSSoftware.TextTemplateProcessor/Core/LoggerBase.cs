namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This is an abstract base class for logger implementations that are intended to be used by the
/// text template processor application.
/// </summary>
internal abstract class LoggerBase : DependencyCheckerBase, ILogger
{
    internal LoggerBase(ILocater locater)
    {
        NullDependencyCheck(locater,
                            nameof(LoggerBase),
                            nameof(ILocater),
                            nameof(locater));

        Locater = locater;
    }

    /// <summary>
    /// Gets or sets the type of operation currently being performed against the text template file.
    /// </summary>
    /// <remarks>
    /// This property is initialized to <see cref="OperationType.Setup" /> when the logger object is
    /// created.
    /// </remarks>
    public OperationType CurrentOperationType
    {
        get;
        set;
    } = DefaultOperationType;

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
    /// Creates a new <see cref="LogEntry" /> object and writes it to the log.
    /// </summary>
    /// <remarks>
    /// This version of the Log method uses the current operation type when constructing the
    /// <see cref="LogEntry" /> object.
    /// </remarks>
    /// <param name="logSeverity">
    /// The severity level of the log message.
    /// </param>
    /// <param name="message">
    /// The log message that is being written to the log.
    /// </param>
    /// <param name="args">
    /// An array of <see langword="string" /> values to be substituted for the format arguments in
    /// the <paramref name="message" /> parameter.
    /// </param>
    public void Log(LogSeverity logSeverity, string message, params string[] args)
    {
        LogEntry logEntry = CreateLogEntry(logSeverity, CurrentOperationType, message, args);

        WriteLogEntry(logEntry);
    }

    /// <summary>
    /// Creates a new <see cref="LogEntry" /> object and writes it to the log.
    /// </summary>
    /// <remarks>
    /// This version of the Log method uses the given <paramref name="operationType" /> instead of
    /// the current operation type when constructing the <see cref="LogEntry" /> object. <br /> The
    /// current operation type remains unchanged.
    /// </remarks>
    /// <param name="logSeverity">
    /// The severity level of the log message.
    /// </param>
    /// <param name="operationType">
    /// The type of operation being performed against the text template file when the log message
    /// was issued.
    /// </param>
    /// <param name="message">
    /// The log message that is being written to the log.
    /// </param>
    /// <param name="args">
    /// An array of <see langword="string" /> values to be substituted for the format arguments in
    /// the <paramref name="message" /> parameter.
    /// </param>
    public void Log(LogSeverity logSeverity, OperationType operationType, string message, params string[] args)
    {
        LogEntry logEntry = CreateLogEntry(logSeverity, operationType, message, args);

        WriteLogEntry(logEntry);
    }

    /// <summary>
    /// Write the given <paramref name="logEntry" /> to the log.
    /// </summary>
    /// <param name="logEntry">
    /// The <see cref="LogEntry" /> instance that is to be written out to the log.
    /// </param>
    public abstract void WriteLogEntry(LogEntry logEntry);

    /// <summary>
    /// Creates a <see cref="LogEntry" /> instance to be written to the log.
    /// </summary>
    /// <param name="logSeverity">
    /// The severity level of the log message.
    /// </param>
    /// <param name="operationType">
    /// The type of operation being performed against the text template file when the log message
    /// was issued.
    /// </param>
    /// <param name="message">
    /// The log message that is being written to the log.
    /// </param>
    /// <param name="args">
    /// An array of <see langword="string" /> values to be substituted for the format arguments in
    /// the <paramref name="message" /> parameter.
    /// </param>
    /// <returns>
    /// A <see cref="LogEntry" /> object representing the log message that is to be written to the
    /// log.
    /// </returns>
    private LogEntry CreateLogEntry(LogSeverity logSeverity, OperationType operationType, string message, params string[] args)
    {
        Location location = operationType switch
        {
            OperationType.Loading => Locater.Location,
            OperationType.Parsing => Locater.Location,
            OperationType.Generating => Locater.Location,
            OperationType.Writing => Locater.Location,
            _ => Location.Empty
        };
        string formattedMessage = FormatMessage(message, args);

        return new(logSeverity, operationType, location, formattedMessage);
    }
}