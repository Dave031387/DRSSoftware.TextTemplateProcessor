namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class ConsoleLoggerTests
{
    private const string SampleLogMessage = SampleLogMessagePrefix + "{0}" + SampleLogMessageSuffix;
    private const string SampleLogMessagePrefix = "Sample log message for ";
    private const string SampleLogMessageSuffix = " log entry type.";

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
        VerifyMocks();
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
        VerifyMocks();
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
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForGeneratingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Generating.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        string logEntryText = new LogEntry(logSeverity, OperationType.Generating, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Generating);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Generating");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForLoadingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Loading.ToString() + SampleLogMessageSuffix;
        Location location = new("TemplateFile", 42);
        string logEntryText = new LogEntry(logSeverity, OperationType.Loading, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Loading);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Loading");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForParsingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Debug;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Parsing.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        string logEntryText = new LogEntry(logSeverity, OperationType.Parsing, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Parsing);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Parsing");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForResetLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Reset.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(logSeverity, OperationType.Reset, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Reset);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Reset");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForSetupLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Information;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Setup.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(logSeverity, OperationType.Setup, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Setup);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Setup");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForUserLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Warning;
        string formattedMessage = SampleLogMessagePrefix + OperationType.User.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(logSeverity, OperationType.User, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.User);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "User");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForWritingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        LogSeverity logSeverity = LogSeverity.Error;
        string formattedMessage = SampleLogMessagePrefix + OperationType.Writing.ToString() + SampleLogMessageSuffix;
        Location location = new("OutputFile", 42);
        string logEntryText = new LogEntry(logSeverity, OperationType.Writing, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(OperationType.Writing);

        // Act
        consoleLogger.Log(logSeverity, SampleLogMessage, "Writing");

        // Assert
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