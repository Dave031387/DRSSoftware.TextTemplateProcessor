namespace DRSSoftware.TextTemplateProcessor.Core;

internal abstract class DependencyCheckerBase
{
    /// <summary>
    /// Check to see if the given <paramref name="dependencyObject" /> is <see langword="null" />
    /// and if it is throw an <see cref="ArgumentNullException" /> with a formatted message.
    /// </summary>
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
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="dependencyObject" /> is <see langword="null" />.
    /// </exception>
    protected virtual void NullDependencyCheck(object? dependencyObject,
                                               string className,
                                               string serviceName,
                                               string parameterName)
    {
        if (dependencyObject is null)
        {
            string message = FormatMessage(MsgDependencyIsNull, className, serviceName);
            throw new ArgumentNullException(parameterName, message);
        }
    }
}
