namespace DRSSoftware.TextTemplateProcessor.TestShared;

using System.IO;

[ExcludeFromCodeCoverage]
internal static class TestFileHelper
{
    private static int _counter = 0;

    static TestFileHelper()
    {
        string path = CurrentDirectory;
        int pathIndex;

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

    public static string CurrentDirectory => Directory.GetCurrentDirectory();

    public static string NextAbsoluteFilePath => Path.Combine(NextAbsoluteName, NextFileName);

    public static string NextAbsoluteName => Path.Combine(VolumeRoot, $"absolute{++_counter}");

    public static string NextFileName => $"file{++_counter}.txt";

    public static string NextRelativeFilePath => Path.Combine(NextRelativeName, NextFileName);

    public static string NextRelativeName => $"relative{++_counter}";

    public static string NextRootedName => $"{Path.DirectorySeparatorChar}rooted{++_counter}";

    public static string NextRootName => $"root{++_counter}";

    public static string SolutionDirectory
    {
        get;
        private set;
    }

    public static string TemplateDirectoryPath => Path.Combine(TestDirectoryRoot, "Templates");

    public static string TestDirectoryRoot => Path.Combine(VolumeRoot, "Test");

    public static string VolumeRoot => CurrentDirectory[0..2];

    public static string CreateTestFiles(string path, string[] text)
    {
        string directoryPath = GetFullPath(path);
        DeleteTestFiles(directoryPath);
        Directory.CreateDirectory(directoryPath);
        string fileName = NextFileName;
        string fullFilePath = Path.Combine(directoryPath, fileName);
        File.WriteAllLines(fullFilePath, text);
        return fullFilePath;
    }

    public static void CreateTestFiles(string path, int numFiles)
    {
        string directoryPath = GetFullPath(path);
        DeleteTestFiles(directoryPath);
        Directory.CreateDirectory(directoryPath);

        for (int i = 0; i < numFiles; i++)
        {
            string filePath = Path.Combine(directoryPath, NextFileName);
            File.WriteAllLines(filePath, []);
        }
    }

    public static string CreateTestFiles(string path, bool directoryOnly = false)
    {
        string directoryPath = GetFullPath(path);
        DeleteTestFiles(directoryPath);
        Directory.CreateDirectory(directoryPath);

        if (directoryOnly)
        {
            return string.Empty;
        }

        string fileName = NextFileName;
        string fullFilePath = Path.Combine(directoryPath, fileName);
        File.WriteAllLines(fullFilePath, []);
        return fileName;
    }

    public static void DeleteTestFiles(string? path = null)
    {
        path ??= TestDirectoryRoot;
        string directoryPath = GetFullPath(path);

        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }

    public static string GetFullPath(string path)
        => Path.IsPathRooted(path)
        ? Path.GetFullPath(path)
        : Path.Combine(SolutionDirectory, path);
}