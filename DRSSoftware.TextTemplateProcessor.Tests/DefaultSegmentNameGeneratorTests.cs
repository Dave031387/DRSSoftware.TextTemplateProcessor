namespace DRSSoftware.TextTemplateProcessor;

[ExcludeFromCodeCoverage]
public class DefaultSegmentNameGeneratorTests
{
    [Fact]
    public void Next_ShouldReturnUniqueDefaultSegmentNamesInSequence()
    {
        // Arrange
        DefaultSegmentNameGenerator generator = new();
        string expectedName1 = $"{DefaultSegmentNamePrefix}1";
        string expectedName2 = $"{DefaultSegmentNamePrefix}2";
        string expectedName3 = $"{DefaultSegmentNamePrefix}3";

        // Act
        string actualName1 = generator.Next;
        string actualName2 = generator.Next;
        string actualName3 = generator.Next;

        // Assert
        actualName1
            .Should()
            .Be(expectedName1);
        actualName2
            .Should()
            .Be(expectedName2);
        actualName3
            .Should()
            .Be(expectedName3);
    }

    [Fact]
    public void Reset_ShouldResetCounterToZero()
    {
        // Arrange
        DefaultSegmentNameGenerator generator = new();
        string expected = $"{DefaultSegmentNamePrefix}1";
        _ = generator.Next; // Increment the counter
        _ = generator.Next; // Increment the counter
        _ = generator.Next; // Increment the counter

        // Act
        generator.Reset();
        string actual = generator.Next;

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}