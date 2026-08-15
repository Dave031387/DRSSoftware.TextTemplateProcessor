using System.IO;

namespace DRSSoftware.TextTemplateProcessor.TestShared;

/// <summary>
/// This is a static class used for generating test files and directories for unit tests. It
/// provides methods for creating and deleting test files and directories, as well as generating
/// unique file and directory names.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TestFileHelper
{
    /// <summary>
    /// Counter used for generating unique file and directory names.
    /// </summary>
    private static int _counter = 0;

    /// <summary>
    /// Static constructor used to initialize the readonly properties of the TestFileHelper class.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the solution file cannot be located in the current working directory or any of its
    /// parent directories.
    /// </exception>
    static TestFileHelper()
    {
        // NOTE: This TestFileHelper class doesn't support UNC paths. If you need to support UNC
        // paths, you'll need to modify the logic accordingly.
        string path = Directory.GetCurrentDirectory();
        int pathIndex;

        CurrentDirectory = path;
        VolumeRootPath = path[..3];
        TestDirectoryPath = $"{VolumeRootPath}Test";
        TemplateDirectoryPath = $"{TestDirectoryPath}{Path.DirectorySeparatorChar}Templates";

        while (true)
        {
            pathIndex = path.LastIndexOf(Path.DirectorySeparatorChar);

            if (pathIndex < 0)
            {
                throw new DirectoryNotFoundException(MsgUnableToLocateSolutionDirectory);
            }

            path = path[..pathIndex];
            string[] files = Directory.GetFiles(path, SolutionFileSearchPattern1);

            if (files.Length > 0)
            {
                SolutionDirectory = path;
                break;
            }

            files = Directory.GetFiles(path, SolutionFileSearchPattern2);

            if (files.Length > 0)
            {
                SolutionDirectory = path;
                break;
            }
        }
    }

    /// <summary>
    /// Gets the current working directory path string.
    /// </summary>
    public static string CurrentDirectory
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the next absolute directory path string.
    /// </summary>
    public static string NextAbsoluteDirectoryPath => $"{VolumeRootPath}absolute{++_counter}";

    /// <summary>
    /// Gets the next absolute file path string.
    /// </summary>
    public static string NextAbsoluteFilePath => Path.Combine(NextAbsoluteDirectoryPath, NextFileName);

    /// <summary>
    /// Gets the next test file name.
    /// </summary>
    public static string NextFileName => $"file{++_counter}.txt";

    /// <summary>
    /// Gets the next relative directory path string.
    /// </summary>
    public static string NextRelativeDirectoryPath => $"relative{++_counter}";

    /// <summary>
    /// Gets the next relative file path string.
    /// </summary>
    public static string NextRelativeFilePath => Path.Combine(NextRelativeDirectoryPath, NextFileName);

    /// <summary>
    /// Gets the solution directory path string.
    /// </summary>
    public static string SolutionDirectory
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the full template directory path string.
    /// </summary>
    public static string TemplateDirectoryPath
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the full path string of the default test directory.
    /// </summary>
    public static string TestDirectoryPath
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the volume root path of the current working directory (e.g., "C:\").
    /// </summary>
    public static string VolumeRootPath
    {
        get;
        private set;
    }

    /// <summary>
    /// Creates a new test directory at the specified <paramref name="path" />. If the given
    /// directory already exists, then it and its contents are deleted and the directory is
    /// recreated.
    /// </summary>
    /// <remarks>
    /// If <paramref name="path" /> is a relative path, then the full path of the directory is
    /// determined by combining it with the solution directory path.
    /// </remarks>
    /// <param name="path">
    /// The path of the directory to create.
    /// </param>
    /// <returns>
    /// The full path of the created directory.
    /// </returns>
    public static string CreateTestDirectory(string path)
    {
        string directoryPath = GetFullPath(path);
        DeleteTestDirectory(directoryPath);
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }

    /// <summary>
    /// Creates a new test file in the specified directory <paramref name="path" /> with the
    /// specified <paramref name="text" /> content.
    /// </summary>
    /// <remarks>
    /// If the given directory <paramref name="path" /> already exists, then it and its contents are
    /// deleted and the directory is recreated before the new test file is created.
    /// </remarks>
    /// <param name="path">
    /// The directory path where the test file will be created.
    /// </param>
    /// <param name="text">
    /// The content for the new test file.
    /// </param>
    /// <returns>
    /// The full path of the created test file.
    /// </returns>
    public static string CreateTestFile(string path, string[] text)
    {
        string directoryPath = CreateTestDirectory(path);
        string fullFilePath = Path.Combine(directoryPath, NextFileName);
        File.WriteAllLines(fullFilePath, text);
        return fullFilePath;
    }

    /// <summary>
    /// Creates a new empty test file in the specified directory <paramref name="path" />.
    /// </summary>
    /// <remarks>
    /// If the given directory <paramref name="path" /> already exists, then it and its contents are
    /// deleted and the directory is recreated before the new test file is created.
    /// </remarks>
    /// <param name="path">
    /// The directory path where the test file will be created.
    /// </param>
    /// <returns>
    /// The name of the test file.
    /// </returns>
    public static string CreateTestFile(string path)
    {
        string directoryPath = CreateTestDirectory(path);
        string fileName = NextFileName;
        string fullFilePath = Path.Combine(directoryPath, fileName);
        File.WriteAllLines(fullFilePath, []);
        return fileName;
    }

    /// <summary>
    /// Create the specified number of empty test files in the specified directory
    /// <paramref name="path" />.
    /// </summary>
    /// <remarks>
    /// If the given directory <paramref name="path" /> already exists, then it and its contents are
    /// deleted and the directory is recreated before the new test files are created.
    /// </remarks>
    /// <param name="path">
    /// The directory path where the test files will be created.
    /// </param>
    /// <param name="numFiles">
    /// The number of empty test files to create.
    /// </param>
    public static void CreateTestFiles(string path, int numFiles)
    {
        string directoryPath = CreateTestDirectory(path);

        for (int i = 0; i < numFiles; i++)
        {
            string filePath = Path.Combine(directoryPath, NextFileName);
            File.WriteAllLines(filePath, []);
        }
    }

    /// <summary>
    /// Deletes the specified directory <paramref name="path" /> and all of its contents. If the
    /// given directory does not exist, no action is taken.
    /// </summary>
    /// <remarks>
    /// If <paramref name="path" /> is <see langword="null" /> or empty, the default test directory
    /// will be deleted.
    /// </remarks>
    /// <param name="path">
    /// The path of the directory to be deleted.
    /// </param>
    public static void DeleteTestDirectory(string? path = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = TestDirectoryPath;
        }

        string directoryPath = GetFullPath(path);

        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }

    /// <summary>
    /// Gets the full path of the specified <paramref name="path" />. If the given path is relative,
    /// it is combined with the solution directory path to form the full path.
    /// </summary>
    /// <param name="path">
    /// </param>
    /// <returns>
    /// The full path of the specified <paramref name="path" />.
    /// </returns>
    public static string GetFullPath(string path)
        => Path.IsPathRooted(path)
        ? path
        : Path.Combine(SolutionDirectory, path);
}