namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// The <see cref="ISegmentHeaderParser" /> interface defines the contract for parsing segment
/// headers in a text template and extracting the pertinent information.
/// </summary>
internal interface ISegmentHeaderParser
{
    /// <summary>
    /// This method parses a segment header line from a text template file and extracts the segment
    /// name and control information.
    /// </summary>
    /// <param name="headerLine">
    /// A segment header line from a text template file.
    /// </param>
    /// <returns>
    /// A <see cref="ControlItem" /> object containing the segment name and control information.
    /// </returns>
    ControlItem ParseSegmentHeader(string headerLine);
}