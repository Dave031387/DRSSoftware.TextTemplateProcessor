namespace DRSSoftware.TextTemplateProcessor.Core;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// An enumeration of the different levels of severity that can be assigned to log messages.
/// </summary>
internal enum LogSeverity
{
    /// <summary>
    /// Severity level indicating that the log message is for debugging purposes and may contain
    /// detailed information useful for diagnosing issues.
    /// </summary>
    [Display(Name = DebugSeverity)]
    Debug,

    /// <summary>
    /// Severity level indicating that the log message is for informational purposes and provides
    /// general information about the application's operation.
    /// </summary>
    [Display(Name = InfoSeverity)]
    Information,

    /// <summary>
    /// Severity level indicating that the log message is for warning purposes and indicates a
    /// potential issue that may require attention.
    /// </summary>
    [Display(Name = WarningSeverity)]
    Warning,

    /// <summary>
    /// Severity level indicating that the log message is for error purposes and indicates a serious
    /// issue that needs to be addressed.
    /// </summary>
    [Display(Name = ErrorSeverity)]
    Error
}