namespace DRSSoftware.TextTemplateProcessor.Core;

using DRSSoftware.TextTemplateProcessor.Console;
using DRSSoftware.TextTemplateProcessor.IO;

/// <summary>
/// The static <see cref="ClassNames" /> class provides a centralized location for storing the names
/// of various classes used in the application. These names are used for logging and error reporting
/// purposes.
/// </summary>
internal static class ClassNames
{
    public static readonly string ConsoleReaderClass = nameof(ConsoleReader);
    public static readonly string ConsoleWriterClass = nameof(ConsoleWriter);
    public static readonly string FileAndDirectoryServiceClass = nameof(FileAndDirectoryService);
    public static readonly string LocaterClass = nameof(Locater);
}