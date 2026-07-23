namespace DRSSoftware.TextTemplateProcessor.IO;

using System.IO;

internal class PathValidator : IPathValidator
{
    /// <summary>
    /// Validates a path string to verify that it represents a valid directory path or file path.
    /// <br /> Also, optionally validates that the directory or file exists if requested.
    /// </summary>
    /// <param name="path">
    /// A file path or directory path to be validated.
    /// </param>
    /// <param name="isFilePath">
    /// Indicates whether the <paramref name="path" /> argument is a file path (
    /// <see langword="true" />) or a directory path ( <see langword="false" />). The default if not
    /// specified is directory path ( <see langword="false" />).
    /// </param>
    /// <param name="shouldExist">
    /// Indicates whether or not the file or directory must already exist. The default if not
    /// specified is <see langword="false" /> (doesn't have to exist).
    /// </param>
    /// <returns>
    /// The full path if <paramref name="path" /> represents a valid path string. Otherwise, returns
    /// an empty string.
    /// </returns>
    /// <exception cref="PathValidatorException">
    /// Exception is thrown if <paramref name="path" /> isn't valid or if the path doesn't exist and
    /// <paramref name="shouldExist" /> is set to <see langword="true" />.
    /// </exception>
    public string ValidateFullPath(string path, bool isFilePath = false, bool shouldExist = false)
    {
        path = CheckForNullOrEmpty(path, isFilePath);
        GetDirectoryAndFileNameParts(path, isFilePath, out string directoryPart, out string fileNamePart);
        CheckDirectoryPath(directoryPart);

        string fullDirectoryPath = GetFullDirectoryPath(directoryPart);
        string fullPath;

        if (isFilePath)
        {
            CheckFileName(fileNamePart);
            fullPath = Path.Combine(fullDirectoryPath, fileNamePart);
        }
        else
        {
            fullPath = fullDirectoryPath;
        }

        if (shouldExist)
        {
            VerifyExists(fullPath, isFilePath);
        }

        return fullPath;
    }

    /// <summary>
    /// Validates a path string to verify that it represents a valid directory path or file path.
    /// <br /> Also, optionally validates that the directory or file exists if requested.
    /// </summary>
    /// <param name="path">
    /// A file path or directory path to be validated.
    /// </param>
    /// <param name="isFilePath">
    /// Indicates whether the <paramref name="path" /> argument is a file path (
    /// <see langword="true" />) or a directory path ( <see langword="false" />). The default if not
    /// specified is directory path ( <see langword="false" />).
    /// </param>
    /// <param name="shouldExist">
    /// Indicates whether or not the file or directory must already exist. The default if not
    /// specified is <see langword="false" /> (doesn't have to exist).
    /// </param>
    /// <exception cref="PathValidatorException">
    /// Exception is thrown if <paramref name="path" /> isn't valid or if the path doesn't exist and
    /// <paramref name="shouldExist" /> is set to <see langword="true" />.
    /// </exception>
    public void ValidatePath(string path, bool isFilePath = false, bool shouldExist = false)
    {
        path = CheckForNullOrEmpty(path, isFilePath);
        GetDirectoryAndFileNameParts(path, isFilePath, out string directoryPart, out string fileNamePart);
        CheckDirectoryPath(directoryPart);

        if (isFilePath)
        {
            CheckFileName(fileNamePart);
        }

        if (shouldExist)
        {
            string fullPath = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);

            VerifyExists(fullPath, isFilePath);
        }
    }

    /// <summary>
    /// Check the given <paramref name="directoryPath" /> to ensure that it doesn't contain any
    /// invalid characters.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory path to be checked.
    /// </param>
    /// <exception cref="PathValidatorException">
    /// Thrown if the given <paramref name="directoryPath" /> contains one or more invalid
    /// characters.
    /// </exception>
    private static void CheckDirectoryPath(string directoryPath)
    {
        if (directoryPath.IndexOfAny(Path.GetInvalidPathChars()) > -1)
        {
            throw new PathValidatorException(MsgInvalidDirectoryCharacters);
        }
    }

    /// <summary>
    /// Check the given <paramref name="fileName" /> to ensure it isn't null or whitespace and
    /// doesn't contain any invalid characters.
    /// </summary>
    /// <param name="fileName">
    /// The file name to be checked.
    /// </param>
    /// <exception cref="PathValidatorException">
    /// Thrown if the given <paramref name="fileName" /> is null or whitespace or if it contains one
    /// or more invalid characters.
    /// </exception>
    private static void CheckFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new PathValidatorException(MsgMissingFileName);
        }

        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
        {
            throw new PathValidatorException(MsgInvalidFileNameCharacters);
        }
    }

    /// <summary>
    /// Check the given <paramref name="path" /> to ensure it isn't null or whitespace.
    /// </summary>
    /// <param name="path">
    /// The path to be checked.
    /// </param>
    /// <param name="isFilePath">
    /// Value indicating whether the given <paramref name="path" /> is a file path (
    /// <see langword="true" />) or directory path ( <see langword="false" />).
    /// </param>
    /// <returns>
    /// The given <paramref name="path" /> string after trimming any leading or trailing whitespace
    /// from the string.
    /// </returns>
    /// <exception cref="PathValidatorException">
    /// Thrown if the given <paramref name="path" /> is null or whitespace.
    /// </exception>
    private static string CheckForNullOrEmpty(string path, bool isFilePath)
    {
        if (path is null)
        {
            string msg = isFilePath ? MsgNullFilePath : MsgNullDirectoryPath;
            throw new PathValidatorException(msg);
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            string msg = isFilePath ? MsgFilePathIsEmptyOrWhitespace : MsgDirectoryPathIsEmptyOrWhitespace;
            throw new PathValidatorException(msg);
        }

        return path.Trim();
    }

    /// <summary>
    /// Get the directory path part and file name part from the given <paramref name="path" />
    /// string.
    /// </summary>
    /// <param name="path">
    /// The path to be checked.
    /// </param>
    /// <param name="isFilePath">
    /// Value indicating whether the given <paramref name="path" /> is a file path (
    /// <see langword="true" />) or directory path ( <see langword="false" />).
    /// </param>
    /// <param name="directoryPart">
    /// Parameter that will hold the directory part of the given <paramref name="path" />.
    /// </param>
    /// <param name="fileNamePart">
    /// Parameter that will hold the file name part of the given <paramref name="path" />.
    /// </param>
    private static void GetDirectoryAndFileNameParts(
        string path,
        bool isFilePath,
        out string directoryPart,
        out string fileNamePart)
    {
        int indexOfLastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar);
        int fileNameStart = indexOfLastSeparator + 1;

        if (indexOfLastSeparator < 0)
        {
            directoryPart = isFilePath ? string.Empty : path;
            fileNamePart = isFilePath ? path : string.Empty;
        }
        else
        {
            if (isFilePath)
            {
                directoryPart = indexOfLastSeparator > 0 ? path[..indexOfLastSeparator] : string.Empty;
                fileNamePart = fileNameStart < path.Length ? path[fileNameStart..] : string.Empty;
            }
            else
            {
                directoryPart = path;
                fileNamePart = string.Empty;
            }
        }
    }

    /// <summary>
    /// Get the full directory path string for the given <paramref name="directoryPath" />.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory path for which we want the full path string.
    /// </param>
    /// <returns>
    /// The current working directory path if <paramref name="directoryPath" /> is null or
    /// whitespace. Otherwise, the full directory path string for the given
    /// <paramref name="directoryPath" />.
    /// </returns>
    private static string GetFullDirectoryPath(string directoryPath)
    {
        return string.IsNullOrWhiteSpace(directoryPath)
            ? Directory.GetCurrentDirectory()
            : Path.IsPathRooted(directoryPath) ? directoryPath : Path.GetFullPath(directoryPath);
    }

    /// <summary>
    /// Verify that the expected file or directory path actually exists.
    /// </summary>
    /// <remarks>
    /// The assumption is made that the given <paramref name="fullPath" /> is the full file or
    /// directory path string.
    /// </remarks>
    /// <param name="fullPath">
    /// The file or directory path to be checked.
    /// </param>
    /// <param name="isFilePath">
    /// Value indicating whether the given <paramref name="fullPath" /> is a file path (
    /// <see langword="true" />) or directory path ( <see langword="false" />).
    /// </param>
    /// <exception cref="PathValidatorException">
    /// Thrown if the given <paramref name="fullPath" /> doesn't exist.
    /// </exception>
    private static void VerifyExists(string fullPath, bool isFilePath)
    {
        if (isFilePath)
        {
            if (!File.Exists(fullPath))
            {
                string message = FormatMessage(MsgFileNotFound, fullPath);
                throw new PathValidatorException(message);
            }
        }
        else
        {
            if (!Directory.Exists(fullPath))
            {
                string message = FormatMessage(MsgDirectoryNotFound, fullPath);
                throw new PathValidatorException(message);
            }
        }
    }
}