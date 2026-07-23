namespace DRSSoftware.TextTemplateProcessor.IO;

/// <summary>
/// This exception class is used for all exceptions thrown by the <see cref="PathValidator" />
/// class.
/// </summary>
[Serializable]
public class PathValidatorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathValidatorException" /> class.
    /// </summary>
    public PathValidatorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathValidatorException" /> class with the
    /// specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public PathValidatorException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathValidatorException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// error.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    /// <param name="inner">
    /// The exception that is the cause of the current exception.
    /// </param>
    public PathValidatorException(string message, Exception inner) : base(message, inner)
    {
    }
}