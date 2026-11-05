namespace DRSSoftware.TextTemplateProcessor.Services;

/// <summary>
/// This interface defines the contract for a service that manages the generated text output files
/// from the text template processor.
/// </summary>
internal interface IGeneratedFileService
{
    /// <summary>
    /// Gets a value indicating whether or not the generated text file is empty.
    /// </summary>
    bool IsEmptyFile
    {
        get;
    }

    /// <summary>
    /// Gets the directory path where the generated text file will be written to.
    /// </summary>
    string OutputDirectoryPath
    {
        get;
    }

    /// <summary>
    /// Gets the file name of the generated text file.
    /// </summary>
    string OutputFileName
    {
        get;
    }

    /// <summary>
    /// Gets the full file path of the generated text file.
    /// </summary>
    string OutputFilePath
    {
        get;
    }

    /// <summary>
    /// Add the given <paramref name="text"/> string to the generated text.
    /// </summary>
    /// <param name="text">
    /// The text string to be added to the generated text.
    /// </param>
    void AddText(string text);

    /// <summary>
    /// Clears the generated text and sets the output file path and file name to empty strings.
    /// </summary>
    void Clear();

    /// <summary>
    /// Write the contents of the generated text lines to the given file path.
    /// </summary>
    /// <param name="filePath">
    /// The file path where the generated text lines are to be written.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the output file was successfully written to; otherwise, <see langword="false"/>
    /// </returns>
    bool WriteGeneratedText(string filePath);
}