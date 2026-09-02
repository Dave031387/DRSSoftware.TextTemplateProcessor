namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class ConsoleLoggerTests
{
    private const string SampleLogMessage = SampleLogMessagePrefix + "{0}" + SampleLogMessageSuffix;
    private const string SampleLogMessagePrefix = "Sample log message for ";
    private const string SampleLogMessageSuffix = " operation type.";

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
        InitializeMocks();
        ILocater locater = null!;
        string expected = GetNullDependencyMessage(nameof(LoggerBase), nameof(ILocater), nameof(locater));

        // Act
        Action action = () => _ = new ConsoleLogger(locater, MessageWriterMock.Object);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateConsoleLoggerWithNullMessageWriter_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IMessageWriter messageWriter = null!;
        string expected = GetNullDependencyMessage(nameof(ConsoleLogger), nameof(IMessageWriter), nameof(messageWriter));

        // Act
        Action action = () => _ = new ConsoleLogger(LocaterMock.Object, messageWriter);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void CreateConsoleLoggerWithValidDependencies_ShouldSucceed()
    {
        // Arrange
        InitializeMocks();

        // Act
        ConsoleLogger consoleLogger = GetConsoleLogger();

        // Assert
        consoleLogger
            .Should()
            .NotBeNull();
        consoleLogger.CurrentOperationType
            .Should()
            .Be(DefaultOperationType);
        MocksVerifyNoOtherCalls();
    }

    [Fact]
    public void ErrorCount_ShouldIncrementForErrorMessages()
    {
        // Arrange
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType currentOperationType = OperationType.Loading;
        OperationType operationType = OperationType.Generating;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.Parsing;
        OperationType operationType = OperationType.Loading;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        Location location = new("TemplateFile", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType currentOperationType = OperationType.Reset;
        OperationType operationType = OperationType.Parsing;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType currentOperationType = OperationType.Setup;
        OperationType operationType = OperationType.Reset;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.User;
        OperationType operationType = OperationType.Setup;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType currentOperationType = OperationType.User;
        OperationType operationType = OperationType.Status;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType currentOperationType = OperationType.Writing;
        OperationType operationType = OperationType.User;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType currentOperationType = OperationType.Generating;
        OperationType operationType = OperationType.Writing;
        string operationTypeText = operationType.ToString();
        string formattedMessage = SampleLogMessagePrefix + operationTypeText + SampleLogMessageSuffix;
        Location location = new("OutputFile", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(currentOperationType);

        // Act
        consoleLogger.Log(logSeverity, operationType, SampleLogMessage, operationTypeText);

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
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType operationType = OperationType.Generating;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Generating.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Generating");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenLoadingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Loading;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        Location location = new("TemplateFile", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Loading");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenParsingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Debug;
        OperationType operationType = OperationType.Parsing;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Parsing");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenResetOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType operationType = OperationType.Reset;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Reset");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenSetupOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Setup;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Setup");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenStatusOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        OperationType operationType = OperationType.Status;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Status");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenUserOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        OperationType operationType = OperationType.User;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        SetupMocks(logSeverity, operationType, formattedMessage, Location.Empty);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "User");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageWhenWritingOperationTypeIsCurrent_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        OperationType operationType = OperationType.Writing;
        string formattedMessage = SampleLogMessagePrefix + operationType.ToString() + SampleLogMessageSuffix;
        Location location = new("OutputFile", 42);
        SetupMocks(logSeverity, operationType, formattedMessage, location);
        ConsoleLogger consoleLogger = GetConsoleLogger(operationType);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Writing");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogProcessingSummary_ShouldWriteSummaryMessagesToConsole()
    {
        // Arrange
        InitializeMocks();
        int errorCount = 2;
        string errorMessage = "Error message";
        LogEntry errorLogEntry = new(LogSeverity.Error, DefaultOperationType, Location.Empty, errorMessage);
        string errorCountMessage = FormatMessage(MsgErrorCount, errorCount.ToString());
        LogEntry errorCountLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, errorCountMessage);
        int warningCount = 3;
        string warningMessage = "Warning message";
        LogEntry warningLogEntry = new(LogSeverity.Warning, DefaultOperationType, Location.Empty, warningMessage);
        string warningCountMessage = FormatMessage(MsgWarningCount, warningCount.ToString());
        LogEntry warningCountLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, warningCountMessage);
        LogEntry processingStatusLogEntry = new(LogSeverity.Information, OperationType.Status, Location.Empty, MsgProcessingSummary);
        MessageWriterMock
            .Setup(x => x.WriteLine(errorLogEntry.ToString()))
            .Verifiable(Times.Exactly(errorCount));
        MessageWriterMock
            .Setup(x => x.WriteLine(warningLogEntry.ToString()))
            .Verifiable(Times.Exactly(warningCount));
        MessageWriterMock
            .Setup(x => x.WriteLine(processingStatusLogEntry.ToString()))
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(errorCountLogEntry.ToString()))
            .Verifiable(Times.Once);
        MessageWriterMock
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
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
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
        InitializeMocks();
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
        => new(LocaterMock.Object, MessageWriterMock.Object);

    private ConsoleLogger GetConsoleLogger(OperationType operationType)
        => new(LocaterMock.Object, MessageWriterMock.Object)
        {
            CurrentOperationType = operationType
        };

    private void InitializeMocks()
    {
        LocaterMock.Reset();
        MessageWriterMock.Reset();
    }

    private void MocksVerifyNoOtherCalls()
    {
        LocaterMock.VerifyNoOtherCalls();
        MessageWriterMock.VerifyNoOtherCalls();
    }

    private void SetupMocks(LogSeverity logSeverity, OperationType operationType, string message, Location location)
    {
        string logEntryText = new LogEntry(logSeverity, operationType, location, message).ToString();

        if (!location.IsEmpty)
        {
            LocaterMock
                .Setup(x => x.Location)
                .Returns(location)
                .Verifiable(Times.Once);
        }

        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
    }

    private void VerifyMocks()
    {
        if (LocaterMock.Setups.Any())
        {
            LocaterMock.VerifyAll();
        }

        if (MessageWriterMock.Setups.Any())
        {
            MessageWriterMock.VerifyAll();
        }

        MocksVerifyNoOtherCalls();
    }
}