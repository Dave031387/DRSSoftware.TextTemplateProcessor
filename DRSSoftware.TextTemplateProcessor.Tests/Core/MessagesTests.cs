namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class MessagesTests
{
    [Theory]
    [InlineData("This is a test message with one format item: {0}.", "This is a test message with one format item: one.", "one", "two")]
    [InlineData("This is a test message with two format items: {0} and {1}.", "This is a test message with two format items: one and two.", "one", "two", "three")]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", "This is a test message with three format items: one, two, and three.", "one", "two", "three", "four")]
    public void FormatMessageHavingFewerFormatItemsThanArguments_ShouldReturnFormattedMessage(string message, string expected, params string[] args)
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
    public void FormatMessageHavingFormatItemsAndMatchingNumberOfArguments_ShouldReturnFormattedMessage(string message, string expected, params string[] args)
    {
        // Arrange/Act
        string actual = FormatMessage(message, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("This is a test message with one format item: {0}.", $"This is a test message with one format item: {NullStringValue}.", null)]
    [InlineData("This is a test message with two format items: {0} and {1}.", $"This is a test message with two format items: one and {NullStringValue}.", "one", null)]
    [InlineData("This is a test message with two format items: {0} and {1}.", $"This is a test message with two format items: {NullStringValue} and {NullStringValue}.", null, null)]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", $"This is a test message with three format items: {NullStringValue}, two, and {NullStringValue}.", null, "two", null)]
    public void FormatMessageHavingFormatItemsWithNullArguments_ShouldReturnFormattedMessage(string message, string expected, params string?[] args)
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
    public void FormatMessageHavingMoreFormatItemsThanArguments_ShouldReturnFormattedMessage(string message, params string[] args)
    {
        // Arrange - This was tested under .NET 10. The format of the following message may change
        // in future versions of .NET, so this test may need to be updated if the message changes.
        string expected = "Index (zero based) must be greater than or equal to zero and less than the size of the argument list.";

        // Act
        void action() => FormatMessage(message, args);

        // Assert
        AssertException<FormatException>(action, expected);
    }

    [Fact]
    public void FormatMessageHavingNoFormatItems_ShouldReturnMessageUnchanged()
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
    public void FormatMessageHavingNoFormatItemsButWithArguments_ShouldReturnMessageUnchanged()
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