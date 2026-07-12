namespace DRSSoftware.TextTemplateProcessor.Console;

[ExcludeFromCodeCoverage]
public class MessageWriterTests
{
    private readonly Mock<IConsoleWriter> _consoleWriter = new(MockBehavior.Strict);

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
