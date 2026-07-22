namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="LogEntry" /> record represents a single entry in the log.
/// </summary>
/// <param name="LogSeverity">
/// The severity level of the log message.
/// </param>
/// <param name="OperationType">
/// The type of operation being performed against the text template file when the log message was
/// issued.
/// </param>
/// <param name="Location">
/// An object representing the current location within the text template
/// file when the log message was issued.
/// </param>
/// <param name="Message">
/// The message to be logged.
/// </param>
internal record LogEntry(LogSeverity LogSeverity, OperationType OperationType, Location Location, string Message)
{
    /// <summary>
    /// Generates a string representation of this <see cref="LogEntry" /> object.
    /// </summary>
    /// <returns>
    /// A <see langword="string" /> that represents this <see cref="LogEntry" /> object.
    /// </returns>
    public override string ToString()
    {
        string severityString = LogSeverity.GetFriendlyName();

        return Location.IsEmpty
            ? $"{severityString} {OperationType} : {Message}"
            : $"{severityString} {OperationType} {Location} : {Message}";
    }
}