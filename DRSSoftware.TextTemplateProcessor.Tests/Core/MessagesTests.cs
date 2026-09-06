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
        string actual = GetMessage(message, args);

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
        string actual = GetMessage(message, args);

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
        string actual = GetMessage(message, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("This is a test message with one format item: {0}.", $"This is a test message with one format item: {NullStringValue}.")]
    [InlineData("This is a test message with two format items: {0} and {1}.", $"This is a test message with two format items: one and {NullStringValue}.", "one")]
    [InlineData("This is a test message with three format items: {0}, {1}, and {2}.", $"This is a test message with three format items: one, two, and {NullStringValue}.", "one", "two")]
    public void FormatMessageHavingMoreFormatItemsThanArguments_ShouldSubstituteDefaultValuesForMissingArguments(string message, string expected, params string[] args)
    {
        // Arrange/Act
        string actual = GetMessage(message, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void FormatMessageHavingNoFormatItems_ShouldReturnMessageUnchanged()
    {
        // Arrange
        string expected = "This is a test message with no format items.";

        // Act
        string actual = GetMessage(expected);

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
        string actual = GetMessage(expected, args);

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}