namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// This interface defines the contract for an option parser service.
/// </summary>
internal interface IOptionParser
{
    /// <summary>
    /// Parse the given option string and return the option name and value if they are valid.
    /// </summary>
    /// <param name="optionString">
    /// The option string to be parsed and validated.
    /// </param>
    /// <param name="validator">
    /// The validator delegate used to validate the option name contained with the option string.
    /// </param>
    /// <param name="optionGroupName">
    /// A name that gives the option group currently being parsed. Valid values are "file header"
    /// and "segment header".
    /// </param>
    /// <param name="identifier">
    /// An optional identifier name. (Currently used to pass in the segment name found on the
    /// segment header being parsed.)
    /// </param>
    /// <returns>
    /// An <see cref="Option"/> instance containing the option name and option value that was found
    /// in the given option string.
    /// </returns>
    Option Parse(string optionString, OptionParser.OptionValidator validator, string optionGroupName, string? identifier = null);
}