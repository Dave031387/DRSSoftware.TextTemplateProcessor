namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class LocationTests
{
    [Fact]
    public void Empty_ShouldReturnEmptyLocation()
    {
        // Arrange
        Location expected = new(EmptyString, 0);

        // Act
        Location actual = Location.Empty;

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(EmptyString, 1)]
    [InlineData("Test1", 0)]
    [InlineData("Test2", 1)]
    public void IsEmpty_ShouldReturnFalseForNotEmptyLocations(string locationName, int lineNumber)
    {
        // Arrange
        Location location = new(locationName, lineNumber);

        // Act
        bool actual = location.IsEmpty;

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void IsEmpty_ShouldReturnTrueForEmptyLocation()
    {
        // Arrange
        Location location = new(EmptyString, 0);

        // Act
        bool actual = location.IsEmpty;

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(EmptyString, 0, EmptyString)]
    [InlineData("Test1", 0, "Test1[0]")]
    [InlineData("Test1", 1, "Test1[1]")]
    [InlineData(EmptyString, 2, "[2]")]
    public void ToString_ShouldReturnExpectedStringRepresentation(string locationName, int lineNumber, string expected)
    {
        // Arrange
        Location location = new(locationName, lineNumber);

        // Act
        string actual = location.ToString();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}