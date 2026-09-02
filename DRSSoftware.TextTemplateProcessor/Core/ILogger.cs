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
    OperationType CurrentOperationType
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the number of error messages that have been written to the log.
    /// </summary>
    int ErrorCount
    {
        get;
    }

    /// <summary>
    /// Gets the number of warning messages that have been written to the log.
    /// </summary>
    int WarningCount
    {
        get;
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
    void Log(LogSeverity logSeverity, string message, params string?[] args);

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
    void Log(LogSeverity logSeverity, OperationType operationType, string message, params string?[] args);

    /// <summary>
    /// Logs a processing summary that includes the counts of errors and warnings.
    /// </summary>
    /// <remarks>
    /// Writes three informational log entries with OperationType.Status: a header line and separate
    /// lines for the current ErrorCount and WarningCount.
    /// </remarks>
    void LogProcessingSummary();

    /// <summary>
    /// Resets the error and warning counters to zero.
    /// </summary>
    /// <remarks>
    /// This operation should typically be performed after completing the processing of a text
    /// template file, so that the counters are ready for the next file to be processed.
    /// </remarks>
    void ResetCounters();
}