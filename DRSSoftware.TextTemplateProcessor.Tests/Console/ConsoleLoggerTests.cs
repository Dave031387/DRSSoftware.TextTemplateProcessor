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
        string expected = GetNullDependencyMessage(nameof(ConsoleLogger), nameof(ILocater), nameof(locater));

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
        consoleLogger.CurrentLogEntryType
            .Should()
            .Be(DefaultLogEntryType);
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForGeneratingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Generating.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        string logEntryText = new LogEntry(LogEntryType.Generating, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Generating);

        // Act
        consoleLogger.Log(SampleLogMessage, "Generating");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForLoadingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Loading.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(LogEntryType.Loading, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Loading);

        // Act
        consoleLogger.Log(SampleLogMessage, "Loading");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForParsingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Parsing.ToString() + SampleLogMessageSuffix;
        Location location = new("SampleSegment", 42);
        string logEntryText = new LogEntry(LogEntryType.Parsing, location, formattedMessage).ToString();
        LocaterMock
            .Setup(x => x.Location)
            .Returns(location)
            .Verifiable(Times.Once);
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Parsing);

        // Act
        consoleLogger.Log(SampleLogMessage, "Parsing");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForResetLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Reset.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(LogEntryType.Reset, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Reset);

        // Act
        consoleLogger.Log(SampleLogMessage, "Reset");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForSetupLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Setup.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(LogEntryType.Setup, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Setup);

        // Act
        consoleLogger.Log(SampleLogMessage, "Setup");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForUserLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.User.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(LogEntryType.User, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.User);

        // Act
        consoleLogger.Log(SampleLogMessage, "User");

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void LogMessageForWritingLogEntryType_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string formattedMessage = SampleLogMessagePrefix + LogEntryType.Writing.ToString() + SampleLogMessageSuffix;
        string logEntryText = new LogEntry(LogEntryType.Writing, Location.Empty, formattedMessage).ToString();
        MessageWriterMock
            .Setup(x => x.WriteLine(logEntryText))
            .Verifiable(Times.Once);
        ConsoleLogger consoleLogger = GetConsoleLogger(LogEntryType.Writing);

        // Act
        consoleLogger.Log(SampleLogMessage, "Writing");

        // Assert
        VerifyMocks();
    }

    private ConsoleLogger GetConsoleLogger()
        => new(LocaterMock.Object, MessageWriterMock.Object);

    private ConsoleLogger GetConsoleLogger(LogEntryType logEntryType)
        => new(LocaterMock.Object, MessageWriterMock.Object)
        {
            CurrentLogEntryType = logEntryType
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