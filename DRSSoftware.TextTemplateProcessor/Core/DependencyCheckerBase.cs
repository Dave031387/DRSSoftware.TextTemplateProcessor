namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="DependencyCheckerBase" /> class provides a base implementation for checking
/// dependencies and throwing exceptions when they are not met.
/// </summary>
internal abstract class DependencyCheckerBase
{
    /// <summary>
    /// Check to see if the given <paramref name="dependencyObject" /> is <see langword="null" />
    /// and if it is throw an <see cref="ArgumentNullException" /> with a formatted message.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the dependency object to check for null. This type must be a reference type.
    /// </typeparam>
    /// <param name="dependencyObject">
    /// The dependency object to check for null. If this object is null, an exception will be
    /// thrown.
    /// </param>
    /// <param name="className">
    /// The class name associated with the dependency.
    /// </param>
    /// <param name="serviceName">
    /// The service name associated with the dependency.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter associated with the dependency.
    /// </param>
    /// <returns>
    /// The dependency object if it is not <see langword="null" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="dependencyObject" /> is <see langword="null" />.
    /// </exception>
    protected virtual T NullDependencyCheck<T>(T dependencyObject,
                                               string className,
                                               string serviceName,
                                               string parameterName) where T : class
    {
        if (dependencyObject is null)
        {
            string message = FormatMessage(MsgDependencyIsNull, className, serviceName);
            throw new ArgumentNullException(parameterName, message);
        }

        return dependencyObject;
    }
}