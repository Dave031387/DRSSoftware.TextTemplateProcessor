namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="Option"/> class models any program option consisting of an option name and value.
/// </summary>
internal class Option
{
    /// <summary>
    /// Gets a value indicating whether or not this <see cref="Option"/> instance is empty.
    /// </summary>
    /// <remarks>
    /// An empty option is any option whose Name or Value property is empty or <see langword="null"/>.
    /// </remarks>
    internal bool IsEmpty => string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Value);

    /// <summary>
    /// Gets or sets the option name.
    /// </summary>
    internal string Name
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    internal string Value
    {
        get;
        set;
    } = string.Empty;
}