namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class ControlItemBuilderTests
{
    [Theory]
    [InlineData(0, EmptyString, EmptyString, 0)]
    [InlineData(1, EmptyString, "Segment1", 0)]
    [InlineData(0, "PadSegment1", "Segment2", 3)]
    [InlineData(0, EmptyString, UnknownSegmentName, 0)]
    [InlineData(2, "PadSegment2", "Segment3", 2)]
    public void Build_ShouldReturnControlItemWithSameValuesAsBuilder(int firstTimeIndent, string padSegment, string segmentName, int tabSize)
    {
        // Arrange
        ControlItemBuilder builder = new()
        {
            FirstTimeIndent = firstTimeIndent,
            PadSegment = padSegment,
            SegmentName = segmentName,
            TabSize = tabSize
        };

        // Act
        ControlItem actual = builder.Build();

        // Assert
        actual
            .Should()
            .NotBeNull();
        actual.FirstTimeIndent
            .Should()
            .Be(firstTimeIndent);
        actual.PadSegment
            .Should()
            .Be(padSegment);
        actual.SegmentName
            .Should()
            .Be(segmentName);
        actual.TabSize
            .Should()
            .Be(tabSize);
        actual.IsFirstTime
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Initialize_ShouldResetAllPropertiesToDefaultValues()
    {
        // Arrange
        ControlItemBuilder builder = new()
        {
            FirstTimeIndent = 5,
            PadSegment = "PadSegment",
            SegmentName = "SegmentName",
            TabSize = 3
        };

        // Act
        builder.Initialize();

        // Assert
        builder.FirstTimeIndent
            .Should()
            .Be(0);
        builder.PadSegment
            .Should()
            .BeEmpty();
        builder.SegmentName
            .Should()
            .BeEmpty();
        builder.TabSize
            .Should()
            .Be(0);
    }
}