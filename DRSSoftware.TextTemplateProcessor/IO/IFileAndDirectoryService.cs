namespace DRSSoftware.TextTemplateProcessor.IO;

/// <summary>
/// The <see cref="IFileAndDirectoryService" /> interface defines the public methods that are needed
/// to work with files and directories.
/// </summary>
internal interface IFileAndDirectoryService
{
    /// <summary>
    /// Clears the contents of the given <paramref name="directoryPath" /> if the directory exists.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory whose contents are to be cleared.
    /// </param>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while trying to clear the specified
    /// <paramref name="directoryPath" />.
    /// </exception>
    public void ClearDirectory(string directoryPath);

    /// <summary>
    /// Combine the given path strings ( <paramref name="path1" /> and <paramref name="path2" />) to
    /// create the resulting file path string.
    /// </summary>
    /// <param name="path1">
    /// The first part of the path that is to be combined to generate the file path.
    /// </param>
    /// <param name="path2">
    /// The second part of the path that is to be combined to generate the file path.
    /// </param>
    /// <returns>
    /// The file path obtained by combining <paramref name="path1" /> and <paramref name="path2" />.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while attempting to combine the given file paths.
    /// </exception>
    public string CombinePaths(string path1, string path2);

    /// <summary>
    /// Creates the given <paramref name="directoryPath" /> if it doesn't already exist.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory path to be created.
    /// </param>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while attempting to create the given directory.
    /// </exception>
    public void CreateDirectory(string directoryPath);

    /// <summary>
    /// Validates the given <paramref name="path" /> and then creates the directory if it doesn't
    /// exist.
    /// </summary>
    /// <param name="path">
    /// The directory path (either relative or absolute).
    /// </param>
    /// <param name="rootDirectory">
    /// The directory that is used as the root of the full directory path if the
    /// <paramref name="path" /> parameter is a relative path.
    /// </param>
    /// <returns>
    /// The string representation of the full directory path.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while attempting to create the given directory.
    /// </exception>
    public string CreateDirectory(string path, string rootDirectory);

    /// <summary>
    /// Gets the directory name from the given <paramref name="path" /> string.
    /// </summary>
    /// <remarks>
    /// This method assumes that <paramref name="path" /> is a well-formed file path string, or it
    /// is null, empty, or only whitespace. <br /> The directory name is the portion of the
    /// <paramref name="path" /> string up to but not including the last directory separator
    /// character found in the string.
    /// </remarks>
    /// <param name="path">
    /// A directory or file path string.
    /// </param>
    /// <returns>
    /// The directory path string from the <paramref name="path" />, or an empty string if the
    /// directory name can't be determined.
    /// </returns>
    public string GetDirectoryName(string path);

    /// <summary>
    /// Gets the file name from the given <paramref name="path" /> string.
    /// </summary>
    /// <remarks>
    /// This method assumes that <paramref name="path" /> is a well-formed file path string, or it
    /// is null, empty, or only whitespace. <br /> The portion of <paramref name="path" /> following
    /// the last directory separator character is considered to be the file name.
    /// </remarks>
    /// <param name="path">
    /// A directory or file path string.
    /// </param>
    /// <returns>
    /// The file name string from the <paramref name="path" />, or an empty string if the file name
    /// can't be determined.
    /// </returns>
    public string GetFileName(string path);

    /// <summary>
    /// Gets the full path for the given <paramref name="path" /> string.
    /// </summary>
    /// <remarks>
    /// The current working directory will be returned if <paramref name="path" /> and
    /// <paramref name="rootPath" /> are both empty. <br /> The full root directory path is returned
    /// if only <paramref name="path" /> is empty.
    /// </remarks>
    /// <param name="path">
    /// A file or directory path (may be absolute or relative).
    /// </param>
    /// <param name="rootPath">
    /// The default rooted path that will be used to construct the full file path if the
    /// <paramref name="path" /> parameter represents a relative path.
    /// </param>
    /// <param name="isFilePath">
    /// An optional boolean parameter that indicates whether or not the given
    /// <paramref name="path" /> parameter represents a file path.
    /// <para>
    /// The default is <see langword="false" /> (i.e., <paramref name="path" /> is a directory
    /// path).
    /// </para>
    /// </param>
    /// <returns>
    /// The string representation of the full file path or directory path.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// An exception is thrown if either <paramref name="path" /> or <paramref name="rootPath" /> is
    /// null or if some other issue is encountered while trying to determine the full path string.
    /// </exception>
    public string GetFullPath(string path, string rootPath, bool isFilePath = false);

    /// <summary>
    /// Gets the full path to the solution directory.
    /// </summary>
    /// <returns>
    /// The string representation of the full solution directory path.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while trying to locate the solution directory.
    /// </exception>
    public string GetSolutionDirectory();

    /// <summary>
    /// Reads the text file located at the given <paramref name="fullFilePath" />.
    /// </summary>
    /// <param name="fullFilePath">
    /// The full file path of the text file to be read.
    /// </param>
    /// <returns>
    /// A collection of text strings read in from the text file.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while trying to read from the text file.
    /// </exception>
    public IEnumerable<string> ReadTextFile(string fullFilePath);

    /// <summary>
    /// Writes a collection of <paramref name="textLines" /> to the file that is located at the
    /// specified <paramref name="filePath" />.
    /// </summary>
    /// <param name="filePath">
    /// The file path where the text is to be written to.
    /// </param>
    /// <param name="textLines">
    /// The collection of text strings to be written to the text file.
    /// </param>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while trying to write to the text file.
    /// </exception>
    public void WriteTextFile(string filePath, IEnumerable<string> textLines);
}