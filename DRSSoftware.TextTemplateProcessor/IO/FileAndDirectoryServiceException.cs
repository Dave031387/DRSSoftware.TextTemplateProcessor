namespace DRSSoftware.TextTemplateProcessor.IO;

/// <summary>
/// This exception class is used for all exceptions related to file path and directory path issues.
/// </summary>
[Serializable]
public class FileAndDirectoryServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileAndDirectoryServiceException" /> class.
    /// </summary>
    public FileAndDirectoryServiceException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAndDirectoryServiceException" /> class with
    /// the specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public FileAndDirectoryServiceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAndDirectoryServiceException" /> class with
    /// the specified error message and a reference to the inner exception that is the cause of this
    /// error.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    /// <param name="inner">
    /// The exception that is the cause of the current exception.
    /// </param>
    public FileAndDirectoryServiceException(string message, Exception inner) : base(message, inner)
    {
    }
}