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
    /// Reads the contents of the text template file at the specified <paramref name="filePath"/>
    /// and retains the file contents for later processing.
    /// </summary>
    /// <param name="filePath">
    /// The file path of an existing text template file.
    /// </param>
    void ReadTemplateFile(string filePath);
}