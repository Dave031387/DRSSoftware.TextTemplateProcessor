namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class ConsoleLoggerTests : MoqUnitTestBase
{
    private const string SampleLogMessage = "Sample log message for {0} operation type.";

    private Mock<ILocater> LocaterMock
    {
        get;
    } = new(MockBehavior.Strict);

    private Mock<IMessageWriter> MessageWriterMock
    {
        get;
    } = new(MockBehavior.Strict);

    [Fact]
    public void CreateConsoleLoggerWithNullLocater_ShouldThrowException()
    {
        // Arrange
        ILocater locater = null!;
        string expected = GetNullDependencyMessage(nameof(LoggerBase), nameof(ILocater), nameof(locater));

        // Act
        Action action = () => _ = new ConsoleLogger(locater, GetMock<IMessageWriter>().Object);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateConsoleLoggerWithNullMessageWriter_ShouldThrowException()
    {
        // Arrange
        IMessageWriter messageWriter = null!;
        string expected = GetNullDependencyMessage(nameof(ConsoleLogger), nameof(IMessageWriter), nameof(messageWriter));

        // Act
        Action action = () => _ = new ConsoleLogger(GetMock<ILocater>().Object, messageWriter);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateConsoleLoggerWithValidDependencies_ShouldSucceed()
    {
        // Act
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Assert
        consoleLogger
            .Should()
            .NotBeNull();
        consoleLogger.CurrentOperationType
            .Should()
            .Be(DefaultOperationType);
    }

    [Fact]
    public void ErrorCount_ShouldIncrementForErrorMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        string message = "Error message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(1);
        VerifyMocks();
    }

    [Fact]
    public void ErrorCount_ShouldNotIncrementForDebugMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Debug;
        string message = "Debug message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void ErrorCount_ShouldNotIncrementForInformationMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        string message = "Information message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void ErrorCount_ShouldNotIncrementForWarningMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        string message = "Warning message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForGeneratingOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType currentOperationType = OperationType.Loading;
        OperationType operationType = OperationType.Generating;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForLoadingOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.Parsing;
        OperationType operationType = OperationType.Loading;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("TemplateFile", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForParsingOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType currentOperationType = OperationType.Reset;
        OperationType operationType = OperationType.Parsing;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForResetOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType currentOperationType = OperationType.Setup;
        OperationType operationType = OperationType.Reset;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForSetupOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.User;
        OperationType operationType = OperationType.Setup;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForStatusOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.User;
        OperationType operationType = OperationType.Status;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForUserOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType currentOperationType = OperationType.Writing;
        OperationType operationType = OperationType.User;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForWritingOperationType_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType currentOperationType = OperationType.Generating;
        OperationType operationType = OperationType.Writing;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("OutputFile", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, message);

        // Assert
        consoleLogger.CurrentOperationType
            .Should()
            .Be(currentOperationType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenGeneratingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType operationType = OperationType.Generating;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenLoadingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Loading;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("TemplateFile", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenParsingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType operationType = OperationType.Parsing;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenResetOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType operationType = OperationType.Reset;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenSetupOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Setup;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenStatusOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Status;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenUserOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType operationType = OperationType.User;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        SetupMocks(logSeverity, operationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenWritingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType operationType = OperationType.Writing;
        string message = string.Format(SampleLogMessage, operationType.ToString());
        Location location = new("OutputFile", 42);
        SetupMocks(logSeverity, operationType, message, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogProcessingSummary_ShouldWriteSummaryMessagesToConsole()
    {
        // Arrange
        int errorCount = 2;
        string errorMessage = "Error message";
        LogEntry errorLogEntry = new(LogSeverity.Error, DefaultOperationType, Location.Empty, errorMessage);
        string errorCountMessage = GetMessage(MsgErrorCount, errorCount.ToString());
        LogEntry errorCountLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, errorCountMessage);
        int warningCount = 3;
        string warningMessage = "Warning message";
        LogEntry warningLogEntry = new(LogSeverity.Warning, DefaultOperationType, Location.Empty, warningMessage);
        string warningCountMessage = GetMessage(MsgWarningCount, warningCount.ToString());
        LogEntry warningCountLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, warningCountMessage);
        LogEntry processingStatusLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, MsgProcessingSummary);
        Mock<IMessageWriter> messageWriterMock = GetMock<IMessageWriter>();
        messageWriterMock
            .Setup(x => x.WriteLine(errorLogEntry.ToString()))
            .Verifiable(Times.Exactly(errorCount));
        messageWriterMock
            .Setup(x => x.WriteLine(warningLogEntry.ToString()))
            .Verifiable(Times.Exactly(warningCount));
        messageWriterMock
            .Setup(x => x.WriteLine(processingStatusLogEntry.ToString()))
            .Verifiable(Times.Once);
        messageWriterMock
            .Setup(x => x.WriteLine(errorCountLogEntry.ToString()))
            .Verifiable(Times.Once);
        messageWriterMock
            .Setup(x => x.WriteLine(warningCountLogEntry.ToString()))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        for (int i = 0; i < errorCount; i++)
        {
            consoleLogger.Log(LogSeverity.Error, errorMessage);
        }

        for (int i = 0; i < warningCount; i++)
        {
            consoleLogger.Log(LogSeverity.Warning, warningMessage);
        }

        // Act
        consoleLogger.LogProcessingSummary();

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(errorCount);
        consoleLogger.WarningCount
            .Should()
            .Be(warningCount);
        VerifyMocks();
    }

    [Fact]
    public void ResetCounters_ShouldResetErrorAndWarningCountsToZero()
    {
        // Arrange
        string message = "Log message";
        SetupMocks(LogSeverity.Error, DefaultOperationType, message, Location.Empty);
        SetupMocks(LogSeverity.Warning, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();
        consoleLogger.Log(LogSeverity.Error, message);
        consoleLogger.Log(LogSeverity.Warning, message);

        // Act
        consoleLogger.ResetCounters();

        // Assert
        consoleLogger.ErrorCount
            .Should()
            .Be(0);
        consoleLogger.WarningCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void WarningCount_ShouldIncrementForWarningMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Warning;
        string message = "Warning message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.WarningCount
            .Should()
            .Be(1);
        VerifyMocks();
    }

    [Fact]
    public void WarningCount_ShouldNotIncrementForDebugMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Debug;
        string message = "Debug message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.WarningCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void WarningCount_ShouldNotIncrementForErrorMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Error;
        string message = "Error message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.WarningCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    [Fact]
    public void WarningCount_ShouldNotIncrementForInformationMessages()
    {
        // Arrange
        LogSeverity logSeverity = LogSeverity.Information;
        string message = "Information message";
        SetupMocks(logSeverity, DefaultOperationType, message, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Act
        consoleLogger.Log(logSeverity, message);

        // Assert
        consoleLogger.WarningCount
            .Should()
            .Be(0);
        VerifyMocks();
    }

    private ConsoleLogger GetConsoleLogger()
        => new(GetMock<ILocater>().Object,
               GetMock<IMessageWriter>().Object);

    private ConsoleLogger GetConsoleLogger(OperationType operationType)
        => new(GetMock<ILocater>().Object,
               GetMock<IMessageWriter>().Object)
        {
            CurrentOperationType = operationType
        };

    private void SetupMocks(LogSeverity logSeverity, OperationType operationType, string message, Location location)
    {
        string logEntryText = new LogEntry(logSeverity, operationType, location, message).ToString();

        if (!location.IsEmpty)
        {
            GetMock<ILocater>()
                .Setup(x => x.Location)
                .Returns(location)
                .Verifiable(Times.Once);
        }

        GetMock<IMessageWriter>()
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
    }
}