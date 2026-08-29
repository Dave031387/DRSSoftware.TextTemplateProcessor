namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="ServiceLocater" /> static class provides a centralized mechanism for managing and
/// resolving dependencies within the application through the use of a dependency injection
/// container.
/// </summary>
internal static class ServiceLocater
{
    /// <summary>
    /// Define and configure the dependency injection container.
    /// </summary>
    private static readonly IContainer _container = ContainerBuilder.GetInstance("DRS_TTP")
        .AddSingleton<IConsoleReader, ConsoleReader>()
        .AddSingleton<IConsoleWriter, ConsoleWriter>()
        .AddSingleton<IFileAndDirectoryService, FileAndDirectoryService>()
        .AddSingleton<ILocater, Locater>()
        .AddSingleton<ILogger, ConsoleLogger>()
        .AddSingleton<IMessageWriter, MessageWriter>()
        .AddSingleton<IPathValidator, PathValidator>()
        .Build();

    /// <summary>
    /// Gets the requested dependency from the container. If the requested type is not registered,
    /// an exception will be thrown.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the dependency to resolve.
    /// </typeparam>
    /// <returns>
    /// The resolved dependency.
    /// </returns>
    public static T GetService<T>() where T : class => _container.Resolve<T>();
}