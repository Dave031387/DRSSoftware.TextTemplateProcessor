namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class MessageWriterTests : MoqUnitTestBase
{
    [Fact]
    public void CreateMessageWriterWithNullDependency_ShouldThrowException()
    {
        // Arrange
        IConsoleWriter consoleWriter = null!;
        string expected = GetNullDependencyMessage(nameof(MessageWriter), nameof(IConsoleWriter), nameof(consoleWriter));

        // Act
        Action action = () => _ = new MessageWriter(consoleWriter);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateMessageWriterWithValidDependency_ShouldSucceed()
    {
        // Act
        MessageWriter messageWriter = GetMessageWriter();

        // Assert
        messageWriter
            .Should()
            .NotBeNull();
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
        string expected = "This is a test message.";
        GetMock<IConsoleWriter>()
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
        string message = "This is a test message.";
        GetMock<IConsoleWriter>()
            .Setup(x => x.WriteLine(message))
            .Verifiable(Times.Once);
        MessageWriter messageWriter = GetMessageWriter();

        // Act
        messageWriter.WriteLine(message);

        // Assert
        VerifyMocks();
    }

    private MessageWriter GetMessageWriter()
        => new(GetMock<IConsoleWriter>().Object);
}