namespace DRSSoftware.TextTemplateProcessor.Core;

using static ServiceNames;

/// <summary>
/// The static <see cref="ServiceParameterNames" /> class provides a centralized location for
/// storing the names of various service parameters used in the application. These names are used
/// for logging and error reporting purposes.
/// </summary>
internal static class ServiceParameterNames
{
    public static readonly string ConsoleReaderParameter = GetParameterName(ConsoleReaderService);
    public static readonly string ConsoleWriterParameter = GetParameterName(ConsoleWriterService);
    public static readonly string FileAndDirectoryServiceParameter = GetParameterName(FileAndDirectoryService);
    public static readonly string LocaterParameter = GetParameterName(LocaterService);

    /// <summary>
    /// Get the parameter name corresponding to the given <paramref name="serviceName" />.
    /// </summary>
    /// <remarks>
    /// The service name is expected to be in the format "IServiceName", and the parameter name is
    /// derived by removing the leading "I" and converting the first character of the remaining
    /// string to lowercase. For example, if the service name is "IConsoleReader", the corresponding
    /// parameter name will be "consoleReader".
    /// </remarks>
    /// <param name="serviceName">
    /// The name of the service whose parameter name we wish to retrieve.
    /// </param>
    /// <returns>
    /// The parameter name corresponding to the given service name.
    /// </returns>
    private static string GetParameterName(string serviceName)
        => serviceName[1..2].ToLowerInvariant() + serviceName[2..];
}