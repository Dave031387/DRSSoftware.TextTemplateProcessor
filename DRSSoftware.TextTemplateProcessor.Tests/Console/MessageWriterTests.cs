namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class MessageWriterTests
{
    private readonly Mock<IConsoleWriter> _consoleWriter = new(MockBehavior.Strict);

    [Fact]
    public void CreateMessageWriterWithNullDependency_ShouldThrowException()
    {
        // Arrange
        InitializeMocks();
        IConsoleWriter consoleWriter = null!;
        string expected = GetNullDependencyMessage(nameof(MessageWriter), nameof(IConsoleWriter), nameof(consoleWriter));

        // Act
        Action action = () => _ = new MessageWriter(consoleWriter);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
        VerifyMocks();
    }

    [Fact]
    public void CreateMessageWriterWithValidDependency_ShouldSucceed()
    {
        // Arrange
        InitializeMocks();

        // Act
        MessageWriter messageWriter = GetMessageWriter();

        // Assert
        messageWriter
            .Should()
            .NotBeNull();
        VerifyMocks();
    }

    [Theory]
    [InlineData("This {0} a test message.", "is")]
    [InlineData("{1} {0} a test message.", "is", "This")]
    [InlineData("{1} {0} a test {2}.", "is", "This", "message")]
    [InlineData("{1} {0} {3} test {2}.", "is", "This", "message", "a")]
    [InlineData("{1} {0} {3} {4} {2}.", "is", "This", "message", "a", "test")]
    public void WriteLineWithFormatItems_ShouldWriteFormattedMessageToConsole(string message, params string[] args)
    {
        // Arrange
        InitializeMocks();
        string expected = "This is a test message.";
        _consoleWriter
            .Setup(x => x.WriteLine(expected))
            .Verifiable(Times.Once);
        MessageWriter messageWriter = GetMessageWriter();

        // Act
        messageWriter.WriteLine(message, args);

        // Assert
        VerifyMocks();
    }

    [Fact]
    public void WriteLineWithNoFormatItems_ShouldWriteMessageToConsole()
    {
        // Arrange
        InitializeMocks();
        string message = "This is a test message.";
        _consoleWriter
            .Setup(x => x.WriteLine(message))
            .Verifiable(Times.Once);
        MessageWriter messageWriter = GetMessageWriter();

        // Act
        messageWriter.WriteLine(message);

        // Assert
        VerifyMocks();
    }

    private MessageWriter GetMessageWriter()
        => new(_consoleWriter.Object);

    private void InitializeMocks()
        => _consoleWriter.Reset();

    private void MocksVerifyNoOtherCalls()
        => _consoleWriter.VerifyNoOtherCalls();

    private void VerifyMocks()
    {
        if (_consoleWriter.Setups.Any())
        {
            _consoleWriter.Verify();
        }

        MocksVerifyNoOtherCalls();
    }
}