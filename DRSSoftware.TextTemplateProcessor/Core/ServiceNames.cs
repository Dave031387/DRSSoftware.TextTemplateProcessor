namespace DRSSoftware.TextTemplateProcessor.Core;

using DRSSoftware.TextTemplateProcessor.Console;
using DRSSoftware.TextTemplateProcessor.IO;

/// <summary>
/// The static <see cref="ServiceNames" /> class provides a centralized location for storing the
/// names of various services used in the application. These names are used for logging and error
/// reporting purposes.
/// </summary>
internal static class ServiceNames
{
    public static readonly string ConsoleReaderService = nameof(IConsoleReader);
    public static readonly string ConsoleWriterService = nameof(IConsoleWriter);
    public static readonly string FileAndDirectoryService = nameof(IFileAndDirectoryService);
    public static readonly string LocaterService = nameof(ILocater);
}