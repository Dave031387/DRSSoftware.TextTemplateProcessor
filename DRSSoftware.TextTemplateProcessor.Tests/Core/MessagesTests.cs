namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class MessagesTests
{
    [Theory]
    [InlineData("This is a test message with one format item: {0}.", "This is a test message with one format item: one.", "one", "two")]
    [InlineData("This is a test message with two format items: {0} and {1}.", "This is a test message with two format items: one and two.", "one", "two", "three")]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", "This is a test message with three format items: one, two, and three.", "one", "two", "three", "four")]
    public void FormatMessageHavingFewerFormatItemsThanArguments_ReturnsFormattedMessage(string message, string expected, params string[] args)
    {
        // Arrange/Act
        string actual = FormatMessage(message, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("This is a test message with one format item: {0}.", "This is a test message with one format item: one.", "one")]
    [InlineData("This is a test message with two format items: {0} and {1}.", "This is a test message with two format items: one and two.", "one", "two")]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", "This is a test message with three format items: one, two, and three.", "one", "two", "three")]
    public void FormatMessageHavingFormatItemsAndMatchingNumberOfArguments_ReturnsFormattedMessage(string message, string expected, params string[] args)
    {
        // Arrange/Act
        string actual = FormatMessage(message, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("This is a test message with one format item: {0}.")]
    [InlineData("This is a test message with two format items: {0} and {1}.", "one")]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", "one", "two")]
    public void FormatMessageHavingMoreFormatItemsThanArguments_ReturnsFormattedMessage(string message, params string[] args)
    {
        // Arrange
        string expected = "Index (zero based) must be greater than or equal to zero and less than the size of the argument list.";

        // Act
        void action() => FormatMessage(message, args);

        // Assert
        AssertException<FormatException>(action, expected);
    }

    [Fact]
    public void FormatMessageHavingNoFormatItems_ReturnsMessageUnchanged()
    {
        // Arrange
        string expected = "This is a test message with no format items.";

        // Act
        string actual = FormatMessage(expected);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void FormatMessageHavingNoFormatItemsButWithArguments_ReturnsMessageUnchanged()
    {
        // Arrange
        string expected = "This is a test message with no format items.";
        string[] args = ["one", "two", "three"];

        // Act
        string actual = FormatMessage(expected, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}