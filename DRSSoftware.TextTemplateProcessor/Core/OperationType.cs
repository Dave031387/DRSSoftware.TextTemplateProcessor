namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// This is an enumeration of the different types of operations that can be performed while
/// processing a text template file.
/// </summary>
internal enum OperationType
{
    /// <summary>
    /// Identifies setup operations performed while processing the text template file.
    /// </summary>
    Setup,

    /// <summary>
    /// Identifies operations pertaining to reading and loading text template files.
    /// </summary>
    Loading,

    /// <summary>
    /// Identifies operations pertaining to parsing of text template files.
    /// </summary>
    Parsing,

    /// <summary>
    /// Identifies operations pertaining to the generation of the text output file.
    /// </summary>
    Generating,

    /// <summary>
    /// Identifies operations pertaining to the writing of the text output file.
    /// </summary>
    Writing,

    /// <summary>
    /// Identifies reset operations performed while processing the text template file.
    /// </summary>
    Reset,

    /// <summary>
    /// Identifies operations pertaining to user interactions while processing the text template
    /// file.
    /// </summary>
    User
}