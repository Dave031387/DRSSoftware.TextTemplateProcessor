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
    public static string ConsoleReaderService = nameof(IConsoleReader);
    public static string ConsoleWriterService = nameof(IConsoleWriter);
    public static string FileAndDirectoryService = nameof(IFileAndDirectoryService);
    public static string LocaterService = nameof(ILocater);
}