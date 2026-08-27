using System.ComponentModel.DataAnnotations;

namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class EnumHelperTests
{
    private enum TestEnum
    {
        [Display(Name = "Friendly Name")]
        ValueWithDisplayName,

        [Display(Name = null)]
        ValueWithNullDisplayName,

        ValueWithoutDisplayName
    }

    [Fact]
    public void GetFriendlyNameWhenDisplayNameIsDefined_ShouldReturnFriendlyName()
    {
        // Arrange
        TestEnum enumValue = TestEnum.ValueWithDisplayName;
        string expected = "Friendly Name";

        // Act
        string actual = enumValue.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFriendlyNameWhenDisplayNameIsNull_ShouldReturnEnumName()
    {
        // Arrange
        TestEnum enumValue = TestEnum.ValueWithNullDisplayName;
        string expected = "ValueWithNullDisplayName";

        // Act
        string actual = enumValue.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFriendlyNameWhenDisplayNameNotDefined_ShouldReturnEnumName()
    {
        // Arrange
        TestEnum enumValue = TestEnum.ValueWithoutDisplayName;
        string expected = "ValueWithoutDisplayName";

        // Act
        string actual = enumValue.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}