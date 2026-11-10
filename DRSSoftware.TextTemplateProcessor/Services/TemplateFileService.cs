namespace DRSSoftware.TextTemplateProcessor.Services;

/// <summary>
/// The <see cref="TemplateFileService"/> class handles reading a text template file and provides an
/// enumerator for processing all of the lines that are read from the file.
/// </summary>
internal class TemplateFileService : DependencyCheckerBase, ITemplateFileService
{
    /// <summary>
    /// The path string of the last text template file that has been read. Will be an empty string
    /// if no file has been read yet.
    /// </summary>
    private string _previousFilePath;

    /// <summary>
    /// Creates a new instance of the <see cref="TemplateFileService"/> class and initializes dependencies.
    /// </summary>
    /// <param name="fileAndDirectoryService">
    /// A reference to the file and directory service.
    /// </param>
    /// <param name="logger">
    /// A reference to the logger service.
    /// </param>
    /// <param name="pathValidator">
    /// A reference to the path validator service.
    /// </param>
    internal TemplateFileService(IFileAndDirectoryService fileAndDirectoryService, ILogger logger, IPathValidator pathValidator)
    {
        FileAndDirectoryService = NullDependencyCheck(fileAndDirectoryService, nameof(fileAndDirectoryService));
        Logger = NullDependencyCheck(logger, nameof(logger));
        PathValidator = NullDependencyCheck(pathValidator, nameof(pathValidator));
        _previousFilePath = string.Empty;
        TemplateFileName = string.Empty;
        TemplateFilePath = string.Empty;
        TemplateLines = [];
    }

    /// <summary>
    /// Gets a value indicating whether or not the text template file is empty.
    /// </summary>
    public bool IsEmptyFile => TemplateLines.Count is 0;

    /// <summary>
    /// Gets the file name of the text template file that was last read.
    /// </summary>
    public string TemplateFileName
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the full file path of the text template file that was last read.
    /// </summary>
    public string TemplateFilePath
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets a reference to the file and directory service.
    /// </summary>
    private IFileAndDirectoryService FileAndDirectoryService
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the logger service.
    /// </summary>
    private ILogger Logger
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a reference to the path validator service.
    /// </summary>
    private IPathValidator PathValidator
    {
        get;
        init;
    }

    /// <summary>
    /// Gets or sets a list of strings representing all of the text lines that were read from the
    /// text template file.
    /// </summary>
    private List<string> TemplateLines
    {
        get;
        set;
    }

    /// <summary>
    /// Gets an enumerator that iterates over the lines read from the text template file.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{T}"/> object that can be used to iterate over the text lines from
    /// the text template file.
    /// </returns>
    public IEnumerator<string> GetEnumerator() => TemplateLines.GetEnumerator();

    /// <summary>
    /// Gets an enumerator that iterates over the lines read from the text template file.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> object that can be used to iterate over the text lines from the
    /// text template file.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Reads the contents of the text template file at the specified <paramref name="filePath"/>
    /// and retains the file contents for later processing.
    /// </summary>
    /// <param name="filePath">
    /// The file path of an existing text template file.
    /// </param>
    public void ReadTemplateFile(string filePath)
    {
        if (filePath == _previousFilePath)
        {
            string message = GetMessage(MsgFileAlreadyLoaded, filePath);
            Logger.Log(LogSeverity.Warning, OperationType.Reading, message);
            return;
        }

        TemplateLines.Clear();
        _previousFilePath = filePath;

        if (IsValidFilePath(filePath))
        {
            try
            {
                string message = GetMessage(MsgAttemptingToReadFile, TemplateFileName);
                Logger.Log(LogSeverity.Information, OperationType.Reading, message);
                TemplateLines = FileAndDirectoryService.ReadTextFile(TemplateFilePath);
                message = GetMessage(MsgFileSuccessfullyRead, TemplateLines.Count.ToString());
                Logger.Log(LogSeverity.Information, OperationType.Reading, message);
            }
            catch (Exception ex)
            {
                string message = GetMessage(MsgTemplateFileCouldNotBeRead, ex.Message);
                Logger.Log(LogSeverity.Error, OperationType.Reading, message);
                TemplateLines.Clear();
            }
        }
    }

    /// <summary>
    /// Validates that the specified <paramref name="filePath"/> is valid and represents an existing file.
    /// </summary>
    /// <param name="filePath">
    /// The file path to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <paramref name="filePath"/> is valid; otherwise <see langword="false"/>
    /// </returns>
    private bool IsValidFilePath(string filePath)
    {
        try
        {
            TemplateFilePath = PathValidator.ValidatePath(filePath, true, true);
            TemplateFileName = FileAndDirectoryService.GetFileName(TemplateFilePath);
            return true;
        }
        catch (Exception ex)
        {
            string message = GetMessage(MsgUnableToSetTemplateFilePath, filePath, ex.Message);
            Logger.Log(LogSeverity.Error, message);
        }

        return false;
    }
}