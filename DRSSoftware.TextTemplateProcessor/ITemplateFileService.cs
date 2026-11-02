namespace DRSSoftware.TextTemplateProcessor;

/// <summary>
/// This interface defines the contract for a text template file service.
/// </summary>
internal interface ITemplateFileService : IEnumerable<string>
{
    /// <summary>
    /// Gets a value indicating whether or not the text template file is empty.
    /// </summary>
    bool IsEmptyFile
    {
        get;
    }

    /// <summary>
    /// Gets the file name of the text template file that was last read.
    /// </summary>
    string TemplateFileName
    {
        get;
    }

    /// <summary>
    /// Gets the full file path of the text template file that was last read.
    /// </summary>
    string TemplateFilePath
    {
        get;
    }

    /// <summary>
    /// Gets an enumerator that iterates over the lines read from the text template file.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{T}"/> object that can be used to iterate over the text lines from
    /// the text template file.
    /// </returns>
    void ReadTemplateFile(string filePath);
}