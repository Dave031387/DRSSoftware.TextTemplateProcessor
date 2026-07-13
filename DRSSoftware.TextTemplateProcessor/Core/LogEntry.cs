namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="LogEntry" /> record represents a single entry in the log.
/// </summary>
/// <param name="LogEntryType">
/// A <see cref="LogEntryType.LogEntryType" /> <see langword="enum" /> value corresponding to the
/// type of this <see cref="LogEntry" /> object.
/// </param>
/// <param name="Location">
/// The <see cref="Location" /> instance representing the segment and line number in the text
/// template file where the log message was triggered.
/// </param>
/// <param name="Message">
/// The log message.
/// </param>
internal record LogEntry(LogEntryType LogEntryType, Location Location, string Message)
{
    /// <summary>
    /// Generates a string representation of this <see cref="LogEntry" /> object.
    /// </summary>
    /// <returns>
    /// A <see langword="string" /> that represents this <see cref="LogEntry" /> object.
    /// </returns>
    public override string ToString() => Location.IsEmpty
        ? $"<{LogEntryType}> {Message}"
        : $"<{LogEntryType}> {Location.SegmentName}[{Location.LineNumber}] : {Message}";
}