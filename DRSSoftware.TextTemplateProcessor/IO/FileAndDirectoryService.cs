namespace DRSSoftware.TextTemplateProcessor.IO;

using System.IO;
using System.Reflection;

/// <summary>
/// The <see cref="FileAndDirectoryService" /> class provides services for managing files and
/// directories.
/// </summary>
internal class FileAndDirectoryService : IFileAndDirectoryService
{
    /// <summary>
    /// Default constructor that creates an instance of the <see cref="FileAndDirectoryService" />
    /// class.
    /// </summary>
    public FileAndDirectoryService()
    {
    }

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
    public void ClearDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            try
            {
                DirectoryInfo directoryInfo = new(directoryPath);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (DirectoryNotFoundException)
            {
                // If the directory gets deleted while we are deleting files, then there is nothing
                // more to do.
            }
            catch (Exception ex)
            {
                string message = FormatMessage(MsgUnableToClearDirectory, directoryPath);
                throw new FileAndDirectoryServiceException(message, ex);
            }
        }
    }

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
    public string CombinePaths(string path1, string path2)
    {
        string combinedPath;

        try
        {
            combinedPath = Path.Combine(path1, path2);
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToCombineFilePaths, path1, path2);
            throw new FileAndDirectoryServiceException(message, ex);
        }

        return combinedPath;
    }

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
    public string CreateDirectory(string path, string rootDirectory)
    {
        string fullDirectoryPath;

        try
        {
            fullDirectoryPath = GetFullPath(path, rootDirectory);

            if (!Directory.Exists(fullDirectoryPath))
            {
                _ = Directory.CreateDirectory(fullDirectoryPath);
            }
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToCreateDirectory, path);
            throw new FileAndDirectoryServiceException(message, ex);
        }

        return fullDirectoryPath;
    }

    /// <summary>
    /// Creates the given <paramref name="directoryPath" /> if it doesn't already exist.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory path to be created.
    /// </param>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while attempting to create the given directory.
    /// </exception>
    public void CreateDirectory(string directoryPath)
    {
        try
        {
            string fullDirectoryPath = GetFullPath(directoryPath, string.Empty);

            _ = Directory.CreateDirectory(fullDirectoryPath);
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToCreateDirectory, directoryPath);
            throw new FileAndDirectoryServiceException(message, ex);
        }
    }

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
    public string GetDirectoryName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        int endOfDirectoryName = GetIndexOfLastDirectorySeparatorChar(path);

        return endOfDirectoryName is <= 0
            ? string.Empty
            : endOfDirectoryName is 2 && path[1] == Path.VolumeSeparatorChar
                ? string.Empty
                : path[..endOfDirectoryName];
    }

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
    public string GetFileName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        int startOfFileName = GetIndexOfLastDirectorySeparatorChar(path) + 1;

        return startOfFileName is <= 0
            ? string.Empty
            : startOfFileName >= path.Length
                ? string.Empty
                : path[startOfFileName..];
    }

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
    public string GetFullPath(string path, string rootPath, bool isFilePath = false)
    {
        if (path is null)
        {
            string msg = isFilePath ? MsgNullFilePath : MsgNullDirectoryPath;
            throw new FileAndDirectoryServiceException(msg);
        }

        if (rootPath is null)
        {
            throw new FileAndDirectoryServiceException(MsgRootPathIsNull);
        }

        string fullPath;

        try
        {
            fullPath = string.IsNullOrWhiteSpace(path)
                ? string.IsNullOrWhiteSpace(rootPath)
                    ? Directory.GetCurrentDirectory()
                    : Path.GetFullPath(rootPath)
                : Path.IsPathRooted(path)
                    ? Path.GetFullPath(path)
                    : string.IsNullOrWhiteSpace(rootPath)
                        ? Path.Combine(Directory.GetCurrentDirectory(), path)
                        : Path.GetFullPath(Path.Combine(rootPath, path));
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToGetFullPathString, path);
            throw new FileAndDirectoryServiceException(message, ex);
        }

        return fullPath;
    }

    /// <summary>
    /// Gets the full path to the solution directory.
    /// </summary>
    /// <returns>
    /// The string representation of the full solution directory path.
    /// </returns>
    /// <exception cref="FileAndDirectoryServiceException">
    /// Thrown if any issues are encountered while trying to locate the solution directory.
    /// </exception>
    public string GetSolutionDirectory()
    {
        Assembly assembly = GetType().Assembly;

        if (assembly.IsDynamic)
        {
            throw new FileAndDirectoryServiceException(MsgDynamicallyGeneratedAssembliesNotSupported);
        }

        string? path = Path.GetDirectoryName(assembly.Location);

        if (string.IsNullOrEmpty(path))
        {
            throw new FileAndDirectoryServiceException(MsgUnableToLocateSolutionDirectory);
        }

        while (true)
        {
            int pathIndex = GetIndexOfLastDirectorySeparatorChar(path);

            if (pathIndex < 0)
            {
                throw new FileAndDirectoryServiceException(MsgUnableToLocateSolutionDirectory);
            }

            path = path[..pathIndex];

            try
            {
                string[] files = Directory.GetFiles(path, SolutionFileSearchPattern1);

                if (files.Length > 0)
                {
                    break;
                }

                files = Directory.GetFiles(path, SolutionFileSearchPattern2);

                if (files.Length > 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new FileAndDirectoryServiceException(MsgUnableToLocateSolutionDirectory, ex);
            }
        }

        return path;
    }

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
    public IEnumerable<string> ReadTextFile(string fullFilePath)
    {
        if (!File.Exists(fullFilePath))
        {
            string message = FormatMessage(MsgFileNotFound, fullFilePath);
            throw new FileAndDirectoryServiceException(message);
        }

        List<string> textLines = [];

        try
        {
            using StreamReader reader = new(fullFilePath);
            while (!reader.EndOfStream)
            {
                string? textLine = reader.ReadLine();

                if (textLine is not null)
                {
                    textLines.Add(textLine);
                }
            }
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToReadTextFile, fullFilePath);
            throw new FileAndDirectoryServiceException(message, ex);
        }

        return textLines;
    }

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
    public void WriteTextFile(string filePath, IEnumerable<string> textLines)
    {
        try
        {
            using StreamWriter writer = new(filePath);
            foreach (string textLine in textLines)
            {
                writer.WriteLine(textLine);
            }
        }
        catch (Exception ex)
        {
            string message = FormatMessage(MsgUnableToWriteToTextFile, filePath);
            throw new FileAndDirectoryServiceException(message, ex);
        }
    }

    /// <summary>
    /// Get the index of the last occurrence of the directory separator character that is found in
    /// the given <paramref name="path" />.
    /// </summary>
    /// <param name="path">
    /// A file or directory path.
    /// </param>
    /// <returns>
    /// The index location of the last directory separator character found in
    /// <paramref name="path" />, or -1 if no directory separator character was found.
    /// </returns>
    private static int GetIndexOfLastDirectorySeparatorChar(string path)
    {
        char[] directorySeparatorChars = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
        return path.LastIndexOfAny(directorySeparatorChars);
    }
}