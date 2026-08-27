namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class LocaterTests
{
    [Fact]
    public void InstantiateLocater_ShouldInitializeProperties()
    {
        // Arrange
        string expectedLocationName = EmptyString;
        int expectedLineNumber = 0;

        // Act
        Locater locater = new();

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(expectedLocationName);
        locater.LineNumber
            .Should()
            .Be(expectedLineNumber);
    }

    [Theory]
    [InlineData("Segment1", 42)]
    [InlineData(EmptyString, 42)]
    [InlineData("Segment2", 0)]
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
    [InlineData(EmptyString, 0)]
    [InlineData("Segment1", 5)]
    public void Location_ShouldReturnCurrentLocationAndLineNumber(string expectedLocationName, int expectedLineNumber)
    {
        // Arrange
        Locater locater = new();

        if (!string.IsNullOrEmpty(expectedLocationName))
        {
            locater.CurrentLocationName = expectedLocationName;
            locater.LineNumber = expectedLineNumber;
        }

        // Act
        Location actual = locater.Location;

        // Assert
        actual.LocationName
            .Should()
            .Be(expectedLocationName);
        actual.LineNumber
            .Should()
            .Be(expectedLineNumber);
    }

    [Theory]
    [InlineData(EmptyString, 0)]
    [InlineData("Segment1", 42)]
    public void Reset_ShouldResetCurrentLocationAndLineNumber(string initialLocationName, int initialLineNumber)
    {
        // Arrange
        Locater locater = new();
        string expectedLocationName = EmptyString;
        int expectedLineNumber = 0;

        if (!string.IsNullOrEmpty(initialLocationName))
        {
            locater.CurrentLocationName = initialLocationName;
            locater.LineNumber = initialLineNumber;
        }

        // Act
        locater.Reset();

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(expectedLocationName);
        locater.LineNumber
            .Should()
            .Be(expectedLineNumber);
    }

    [Theory]
    [InlineData(" Segment1")]
    [InlineData("Segment1 ")]
    [InlineData(" Segment1 ")]
    [InlineData(Whitespace + "Segment1")]
    [InlineData("Segment1" + Whitespace)]
    [InlineData(Whitespace + "Segment1" + Whitespace)]
    public void SetCurrentLocationName_ShouldTrimWhitespace(string locationName)
    {
        // Arrange
        string expected = "Segment1";
        Locater locater = new()
        {
            // Act
            CurrentLocationName = locationName
        };

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyString)]
    [InlineData("   ")]
    [InlineData(Whitespace)]
    public void SetCurrentLocationNameToNullOrWhitespace_ShouldSetToEmpty(string? locationName)
    {
        // Arrange
        string expected = EmptyString;
        Locater locater = new()
        {
            // Act
            CurrentLocationName = locationName!
        };

        // Assert
        locater.CurrentLocationName
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(EmptyString, 0, EmptyString)]
    [InlineData("Segment1", 0, "Segment1[0]")]
    [InlineData("Segment2", 42, "Segment2[42]")]
    [InlineData(EmptyString, 7, "[7]")]
    public void ToString_ShouldReturnFormattedString(string locationName, int lineNumber, string expected)
    {
        // Arrange
        Locater locater = new()
        {
            CurrentLocationName = locationName,
            LineNumber = lineNumber
        };

        // Act
        string actual = locater.ToString();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}