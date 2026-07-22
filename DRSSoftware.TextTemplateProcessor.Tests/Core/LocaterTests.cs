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
        locater.CurrentLocationName
            .Should()
            .Be(Location.Empty.LocationName);
        locater.LineNumber
            .Should()
            .Be(Location.Empty.LineNumber);
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
            locater.CurrentLocationName = expectedSegmentName;
            locater.LineNumber = expectedLineNumber;
        }

        // Act
        Location actual = locater.Location;

        // Assert
        actual.LocationName
            .Should()
            .Be(expectedSegmentName);
        actual.LineNumber
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
            locater.CurrentLocationName = initialSegmentName;
            locater.LineNumber = initialLineNumber;
        }

        // Act
        locater.Reset();

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(Location.Empty.LocationName);
        locater.LineNumber
            .Should()
            .Be(Location.Empty.LineNumber);
    }

    [Theory]
    [InlineData(" Segment1")]
    [InlineData("Segment1 ")]
    [InlineData(" Segment1 ")]
    [InlineData(Whitespace + "Segment1")]
    [InlineData("Segment1" + Whitespace)]
    [InlineData(Whitespace + "Segment1" + Whitespace)]
    public void CurrentSegment_ShouldTrimWhitespace(string segmentName)
    {
        // Arrange
        string expected = "Segment1";
        Locater locater = new()
        {
            // Act
            CurrentLocationName = segmentName
        };

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("Segment1", 42)]
    [InlineData("", 42)]
    [InlineData("Segment1", 0)]
    public void IsEmpty_ShouldReturnFalseWhenLocationIsNotEmpty(string segmentName, int lineNumber)
    {
        // Arrange
        Locater locater = new()
        {
            CurrentLocationName = segmentName,
            LineNumber = lineNumber
        };

        // Act
        bool actual = locater.IsEmpty;

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void IsEmpty_ShouldReturnTrueWhenLocationIsEmpty()
    {
        // Arrange
        Locater locater = new();

        // Act
        bool actual = locater.IsEmpty;

        // Assert
        actual
            .Should()
            .BeTrue();
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
            locater.CurrentLocationName = segmentName;
            locater.LineNumber = lineNumber;
        }

        // Act
        string actual = locater.ToString();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData(null)]
    [InlineData(Whitespace)]
    public void TryToSetCurrentSegmentToNullOrWhitespace_ShouldSetToEmpty(string? segmentName)
    {
        // Arrange
        Locater locater = new()
        {
            CurrentLocationName = "InitialSegment"
        };

        // Act
        locater.CurrentLocationName = segmentName!;

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(Location.Empty.LocationName);
    }
}
