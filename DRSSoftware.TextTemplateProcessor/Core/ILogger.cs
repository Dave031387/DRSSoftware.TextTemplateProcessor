namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ILogger" /> interface defines the contract for a logger that can be used to log
/// messages in the application.
/// </summary>
internal interface ILogger
{
    /// <summary>
    /// Gets or sets the type of operation currently being performed against the text template file.
    /// </summary>
    public OperationType CurrentOperationType
    {
        get;
        set;
    }

    /// <summary>
    /// Formats a new log entry and writes it to the log.
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
    public void Log(LogSeverity logSeverity, string message, params string[] args);

    /// <summary>
    /// Formats a new log entry and writes it to the log.
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
    public void Log(LogSeverity logSeverity, OperationType operationType, string message, params string[] args);

    /// <summary>
    /// Write the given <paramref name="logEntry" /> to the log.
    /// </summary>
    /// <param name="logEntry">
    /// The <see cref="LogEntry" /> instance that is to be written out to the log.
    /// </param>
    public void WriteLogEntry(LogEntry logEntry);
}