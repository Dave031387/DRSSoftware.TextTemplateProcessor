namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ILogger" /> interface defines the contract for a logger that can be used to log
/// messages in the application.
/// </summary>
internal interface ILogger
{
    /// <summary>
    /// Gets or sets the type of log entry currently being processed.
    /// </summary>
    /// <remarks>
    /// This property is initialized to <see cref="LogEntryType.Setup" /> when the logger object is
    /// created.
    /// </remarks>
    public LogEntryType CurrentLogEntryType
    {
        get;
        set;
    }

    /// <summary>
    /// Creates a new <see cref="LogEntry" /> object and writes it to the log.
    /// </summary>
    /// <param name="message">
    /// The log message that is being written to the log.
    /// </param>
    /// <param name="args">
    /// An array of <see langword="string" /> values to be substituted for the format arguments in
    /// the <paramref name="message" /> parameter.
    /// </param>
    public void Log(string message, params string[] args);
}