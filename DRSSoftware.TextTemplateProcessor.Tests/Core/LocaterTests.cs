namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class LocaterTests
{
    [Fact]
    public void InstantiateLocater_ShouldInitializeProperties()
    {
        // Arrange & Act
        var locater = new Locater();

        // Assert
        locater.CurrentSegment
            .Should()
            .BeEmpty();
        locater.LineNumber
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("Segment1", 5)]
    public void Location_ShouldReturnCurrentSegmentAndLineNumber(string expectedSegmentName, int expectedLineNumber)
    {
        // Arrange
        Locater locater = new();

        if (!string.IsNullOrEmpty(expectedSegmentName))
        {
            locater.CurrentSegment = expectedSegmentName;
            locater.LineNumber = expectedLineNumber;
        }

        // Act
        (string actualSegmentName, int actualLineNumber) = locater.Location;

        // Assert
        actualSegmentName
            .Should()
            .Be(expectedSegmentName);
        actualLineNumber
            .Should()
            .Be(expectedLineNumber);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("Segment1", 42)]
    public void Reset_ShouldResetCurrentSegmentAndLineNumber(string initialSegmentName, int initialLineNumber)
    {
        // Arrange
        Locater locater = new();

        if (!string.IsNullOrEmpty(initialSegmentName))
        {
            locater.CurrentSegment = initialSegmentName;
            locater.LineNumber = initialLineNumber;
        }

        // Act
        locater.Reset();

        // Assert
        locater.CurrentSegment
            .Should()
            .BeEmpty();
        locater.LineNumber
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData("", 0, "")]
    [InlineData("Segment1", 42, "Segment1[42]")]
    public void ToString_ShouldReturnFormattedString(string segmentName, int lineNumber, string expected)
    {
        // Arrange
        Locater locater = new();

        if (!string.IsNullOrEmpty(segmentName))
        {
            locater.CurrentSegment = segmentName;
            locater.LineNumber = lineNumber;
        }

        // Act
        string actual = locater.ToString();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}
