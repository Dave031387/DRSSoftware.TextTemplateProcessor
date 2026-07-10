namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class RegexHelperTests
{
    private const int PrefixLength = 4;
    private const string Space = " ";
    private const string Spaces = "  ";
    private readonly string _testString = "This is a test string.";

    [Theory]
    [InlineData(Normal + Absolute + "10")]
    [InlineData(Space + Normal + Absolute + "1")]
    [InlineData(Normal + Absolute + "a ")]
    [InlineData(Normal + Absolute + "  ")]
    [InlineData(Normal + RelativeRightChar + "3 ")]
    [InlineData(Normal + " 3 ")]
    [InlineData(Normal + RelativeLeftChar + "7 ")]
    [InlineData(OneTime + Absolute + "21")]
    [InlineData(Space + OneTime + Absolute + "1")]
    [InlineData(OneTime + Absolute + "x ")]
    [InlineData(OneTime + Absolute + Spaces)]
    [InlineData(OneTime + RelativeRightChar + "5 ")]
    [InlineData(OneTime + RelativeLeftChar + "2 ")]
    [InlineData("D" + Absolute + "1 ")]
    [InlineData(Absolute + "1  ")]
    [InlineData(SegmentHeaderCode + " ")]
    [InlineData(CommentCode + " ")]
    [InlineData(NoControlCode + " ")]
    public void MatchOnAbsoluteIndent_ShouldReturnFalseIfTextDoesNotBeginWithAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasAbsoluteIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(Normal + Absolute + "0 ")]
    [InlineData(OneTime + Absolute + "0 ")]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(OneTime + Absolute + "1 ")]
    [InlineData(Normal + Absolute + "9 ")]
    [InlineData(OneTime + Absolute + "9 ")]
    public void MatchOnAbsoluteIndent_ShouldReturnTrueIfTextBeginsWithAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasAbsoluteIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + RelativeRightChar + "4 ")]
    [InlineData(Normal + RelativeLeftChar + "8 ")]
    [InlineData(Normal + Absolute + "13")]
    [InlineData(Space + Normal + Absolute + "6")]
    [InlineData(OneTime + Absolute + "1 ")]
    [InlineData(OneTime + RelativeRightChar + "2 ")]
    [InlineData(OneTime + RelativeLeftChar + "3 ")]
    [InlineData(Normal + Absolute + Spaces)]
    [InlineData(Absolute + "8  ")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnAbsoluteIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasAbsoluteIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(Normal + Absolute + "0 ")]
    [InlineData(Normal + Absolute + "5 ")]
    [InlineData(Normal + Absolute + "9 ")]
    public void MatchOnAbsoluteIndentCode_ShouldReturnTrueIfTextBeginsWithAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasAbsoluteIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + Absolute + "10")]
    [InlineData(OneTime + Absolute + "1x")]
    [InlineData("o" + Absolute + "2 ")]
    [InlineData(Space + Normal + Absolute + "1")]
    [InlineData(Space + RelativeLeftChar + "5 ")]
    [InlineData(Space + RelativeRightChar + "6 ")]
    [InlineData(Space + Absolute + "7 ")]
    [InlineData(Normal + ".3 ")]
    [InlineData("X" + RelativeRightChar + "4 ")]
    [InlineData(Normal + Absolute + Spaces)]
    [InlineData(OneTime + RelativeLeftChar + Spaces)]
    [InlineData(OneTime + RelativeRightChar + Spaces)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnAnyIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithAnyIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(Normal + Absolute + "0 ")]
    [InlineData(Normal + RelativeLeftChar + "1 ")]
    [InlineData(Normal + RelativeRightChar + "2 ")]
    [InlineData(OneTime + Absolute + "3 ")]
    [InlineData(OneTime + RelativeLeftChar + "4 ")]
    [InlineData(OneTime + RelativeRightChar + "5 ")]
    public void MatchOnAnyIndentCode_ShouldReturnTrueIfTextBeginsWithAnyIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("/   ")]
    [InlineData("//  ")]
    [InlineData(Space + CommentCode)]
    [InlineData(CommentCode + "6")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(NoControlCode + Space)]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(Normal + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "7 ")]
    [InlineData(OneTime + RelativeLeftChar + "9 ")]
    [InlineData(OneTime + RelativeRightChar + "4 ")]
    public void MatchOnCommentCode_ShouldReturnFalseIfTextDoesNotBeginWithCommentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = IsCommentLine(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnCommentCode_ShouldReturnTrueIfTextBeginsWithCommentCode()
    {
        // Arrange
        string text = $"{CommentCode} {_testString}";

        // Act
        bool actual = IsCommentLine(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("Sample text with no format items.")]
    [InlineData("{0 No format item here")]
    [InlineData("No format item here 1}")]
    [InlineData("No format {2 item here")]
    [InlineData("No format 3} item here")]
    [InlineData("No format item {4 } here")]
    [InlineData("No { 5} format item here")]
    public void MatchOnFormatItems_ShouldReturnFalseIfTextDoesNotContainFormatItems(string text)
    {
        // Arrange/Act
        bool actual = HasFormatItems(text);

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData("{0} Test string with format item.")]
    [InlineData("Test string with format item. {1}")]
    [InlineData("Test string with {2} format item.")]
    public void MatchOnFormatItems_ShouldReturnTrueIfTextContainsFormatItems(string text)
    {
        // Arrange/Act
        bool actual = HasFormatItems(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Normal + Absolute + "9 ")]
    [InlineData(Normal + RelativeLeftChar + "8 ")]
    [InlineData(Normal + RelativeRightChar + "7 ")]
    [InlineData(OneTime + Absolute + "6 ")]
    [InlineData(OneTime + RelativeLeftChar + "5 ")]
    [InlineData(OneTime + RelativeRightChar + "4 ")]
    [InlineData("ABC ")]
    [InlineData(NoControlCode + "X")]
    public void MatchOnNoControlCode_ShouldReturnFalseIfTextBeginsWithAnyControlCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasNoControlCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnNoControlCode_ShouldReturnTrueIfTextDoesNotBeginWithAnyControlCode()
    {
        // Arrange
        string text = $"{NoControlCode} {_testString}";

        // Act
        bool actual = HasNoControlCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(OneTime + RelativeRightChar + "4 ")]
    [InlineData(OneTime + RelativeLeftChar + "8 ")]
    [InlineData(OneTime + Absolute + "13")]
    [InlineData(Space + OneTime + Absolute + "6")]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeRightChar + "2 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(OneTime + Absolute + Spaces)]
    [InlineData(Absolute + "8  ")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnOneTimeAbsoluteIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithOneTimeAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeAbsoluteIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(OneTime + Absolute + "0 ")]
    [InlineData(OneTime + Absolute + "5 ")]
    [InlineData(OneTime + Absolute + "9 ")]
    public void MatchOnOneTimeAbsoluteIndentCode_ShouldReturnTrueIfTextBeginsWithOneTimeAbsoluteIndentIndicator(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeAbsoluteIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeRightChar + "3 ")]
    [InlineData(Normal + RelativeLeftChar + "5 ")]
    [InlineData(OneTime + Absolute + Spaces)]
    [InlineData(OneTime + Absolute + "K ")]
    [InlineData(Space + OneTime + RelativeRightChar + "1")]
    [InlineData(OneTime + " 7 ")]
    [InlineData(OneTime + Absolute + "10")]
    [InlineData(OneTime + Absolute + "6x")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnOneTimeIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithOneTimeIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(OneTime + Absolute + "9 ")]
    [InlineData(OneTime + RelativeLeftChar + "8 ")]
    [InlineData(OneTime + RelativeRightChar + "7 ")]
    public void MatchOnOneTimeIndentCode_ShouldReturnTrueIfTextBeginsWithOneTimeIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(Normal + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "7 ")]
    [InlineData(OneTime + Absolute + "9 ")]
    [InlineData(OneTime + Space + RelativeLeftChar + "2")]
    [InlineData(OneTime + RelativeLeftChar + "04")]
    [InlineData(NoControlCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Space + OneTime + RelativeLeftChar + "6")]
    public void MatchOnOneTimeRelativeLeftIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithOneTimeRelativeLeftIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeRelativeLeftIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnOneTimeRelativeLeftIndentCode_ShouldReturnTrueIfTextBeginsWithOneTimeRelativeLeftIndentCode()
    {
        // Arrange
        string text = $"{OneTime}{RelativeLeftChar}9 {_testString}";

        // Act
        bool actual = HasOneTimeRelativeLeftIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(Normal + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "7 ")]
    [InlineData(OneTime + Absolute + "9 ")]
    [InlineData(OneTime + Space + RelativeRightChar + "2")]
    [InlineData(OneTime + RelativeRightChar + "04")]
    [InlineData(NoControlCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Space + OneTime + RelativeRightChar + "6")]
    public void MatchOnOneTimeRelativeRightIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithOneTimeRelativeRightIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasOneTimeRelativeRightIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnOneTimeRelativeRightIndentCode_ShouldReturnTrueIfTextBeginsWithOneTimeRelativeRightIndentCode()
    {
        // Arrange
        string text = $"{OneTime}{RelativeRightChar}9 {_testString}";

        // Act
        bool actual = HasOneTimeRelativeRightIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Normal + Absolute + "6 ")]
    [InlineData(OneTime + Absolute + "4 ")]
    [InlineData(Normal + RelativeLeftChar + "30")]
    [InlineData(Normal + RelativeRightChar + "1r")]
    [InlineData(OneTime + Space + RelativeLeftChar + "2")]
    [InlineData(OneTime + Space + RelativeRightChar + "5")]
    [InlineData(OneTime + RelativeLeftChar + "0G")]
    [InlineData(OneTime + RelativeRightChar + "11")]
    [InlineData(Space + RelativeLeftChar + "5 ")]
    [InlineData(Space + RelativeRightChar + "8 ")]
    [InlineData("X" + RelativeRightChar + "6 ")]
    [InlineData(OneTime + RelativeRightChar + Spaces)]
    [InlineData(OneTime + RelativeLeftChar + Spaces)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnRelativeIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithRelativeIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasRelativeIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(Normal + RelativeRightChar + "2 ")]
    [InlineData(Normal + RelativeLeftChar + "4 ")]
    [InlineData(OneTime + RelativeRightChar + "6 ")]
    [InlineData(OneTime + RelativeLeftChar + "8 ")]
    public void MatchOnRelativeIndentCode_ShouldReturnTrueIfTextBeginsWithRelativeIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasRelativeIndent(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(OneTime + RelativeLeftChar + "1 ")]
    [InlineData(OneTime + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "0 ")]
    [InlineData(Normal + Absolute + "2 ")]
    [InlineData(Normal + RelativeRightChar + "3 ")]
    [InlineData(Space + RelativeLeftChar + "4 ")]
    [InlineData(Normal + RelativeLeftChar + "08")]
    [InlineData(Normal + RelativeLeftChar + " 9")]
    [InlineData(Space + Normal + RelativeLeftChar + "7")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnRelativeLeftIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithRelativeLeftIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasRelativeLeftIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnRelativeLeftIndentCode_ShouldReturnTrueIfTextBeginsWithRelativeLeftIndentCode()
    {
        // Arrange
        string text = $"{Normal}{RelativeLeftChar}3 {_testString}";

        // Act
        bool actual = HasRelativeLeftIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(OneTime + RelativeLeftChar + "1 ")]
    [InlineData(OneTime + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "0 ")]
    [InlineData(Normal + Absolute + "2 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(Space + RelativeRightChar + "4 ")]
    [InlineData(Normal + RelativeRightChar + "08")]
    [InlineData(OneTime + RelativeRightChar + " 9")]
    [InlineData(Space + OneTime + RelativeRightChar + "7")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    public void MatchOnRelativeRightIndentCode_ShouldReturnFalseIfTextDoesNotBeginWithRelativeRightIndentCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasRelativeRightIndentCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnRelativeRightIndentCode_ShouldReturnTrueIfTextBeginsWithRelativeRightIndentCode()
    {
        // Arrange
        string text = $"{Normal}{RelativeRightChar}1 {_testString}";

        // Act
        bool actual = HasRelativeRightIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("#   ")]
    [InlineData("##  ")]
    [InlineData(Space + SegmentHeaderCode)]
    [InlineData(SegmentHeaderCode + "6")]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    [InlineData(Normal + Absolute + "1 ")]
    [InlineData(Normal + RelativeLeftChar + "3 ")]
    [InlineData(Normal + RelativeRightChar + "5 ")]
    [InlineData(OneTime + Absolute + "7 ")]
    [InlineData(OneTime + RelativeLeftChar + "9 ")]
    [InlineData(OneTime + RelativeRightChar + "4 ")]
    public void MatchOnSegmentHeaderCode_ShouldReturnFalseIfTextDoesNotBeginWithSegmentHeaderCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = IsSegmentHeader(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnSegmentHeaderCode_ShouldReturnTrueIfTextBeginsWithSegmentHeaderCode()
    {
        // Arrange
        string text = $"{SegmentHeaderCode} {_testString}";

        // Act
        bool actual = IsSegmentHeader(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(Space + SegmentHeaderCode)]
    [InlineData(Space + CommentCode)]
    [InlineData(Space + Normal + Absolute + "0")]
    [InlineData(Space + Normal + RelativeLeftChar + "1")]
    [InlineData(Space + Normal + RelativeRightChar + "2")]
    [InlineData(Space + OneTime + Absolute + "3")]
    [InlineData(Space + OneTime + RelativeLeftChar + "4")]
    [InlineData(Space + OneTime + RelativeRightChar + "5")]
    [InlineData(Normal + Space + Absolute + "6")]
    [InlineData(Normal + Space + RelativeLeftChar + "7")]
    [InlineData(Normal + Space + RelativeRightChar + "8")]
    [InlineData(OneTime + Space + Absolute + "9")]
    [InlineData(OneTime + Space + RelativeLeftChar + "0")]
    [InlineData(OneTime + Space + RelativeRightChar + "1")]
    [InlineData(Normal + Absolute + " 2")]
    [InlineData(Normal + RelativeLeftChar + " 3")]
    [InlineData(Normal + RelativeRightChar + " 4")]
    [InlineData(OneTime + Absolute + " 5")]
    [InlineData(OneTime + RelativeLeftChar + " 6")]
    [InlineData(OneTime + RelativeRightChar + " 7")]
    [InlineData(Normal + Absolute + "81")]
    [InlineData(Normal + RelativeLeftChar + "92")]
    [InlineData(Normal + RelativeRightChar + "03")]
    [InlineData(OneTime + Absolute + "14")]
    [InlineData(OneTime + RelativeLeftChar + "25")]
    [InlineData(OneTime + RelativeRightChar + "36")]
    [InlineData(Space + Absolute + "4 ")]
    [InlineData(Space + RelativeLeftChar + "5 ")]
    [InlineData(Space + RelativeRightChar + "6 ")]
    [InlineData(Normal + Absolute + Spaces)]
    [InlineData(Normal + RelativeLeftChar + Spaces)]
    [InlineData(Normal + RelativeRightChar + Spaces)]
    [InlineData(OneTime + Absolute + Spaces)]
    [InlineData(OneTime + RelativeLeftChar + Spaces)]
    [InlineData(OneTime + RelativeRightChar + Spaces)]
    [InlineData(Normal + " 7 ")]
    [InlineData(OneTime + " 8 ")]
    public void MatchOnValidControlCode_ShouldReturnFalseIfTextDoesNotBeginWithValidControlCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasValidControlCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(NoControlCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Normal + Absolute + "9 ")]
    [InlineData(Normal + RelativeLeftChar + "8 ")]
    [InlineData(Normal + RelativeRightChar + "7 ")]
    [InlineData(OneTime + Absolute + "6 ")]
    [InlineData(OneTime + RelativeLeftChar + "5 ")]
    [InlineData(OneTime + RelativeRightChar + "4 ")]
    public void MatchOnValidControlCode_ShouldReturnTrueIfTextBeginsWithValidControlCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = HasValidControlCode(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("123ABC", "Name must not begin with a numeric")]
    [InlineData("_Nope", "Name must not begin with an underscore")]
    [InlineData("A,bc", "Name must not contain commas")]
    [InlineData("A-bc", "Name must not contain hyphens")]
    [InlineData("Abc:", "Name must not contain colons")]
    [InlineData("A(BC)", "Name must not contain parenthesis")]
    [InlineData("Ab=c", "Name must not contain an equals sign")]
    [InlineData("ABC?", "Name must not contain a question mark")]
    [InlineData("A@bc", "Name must not contain an at sign")]
    [InlineData("ab*c", "Name must not contain an asterisk")]
    [InlineData("ab$c", "Name must not contain a dollar sign")]
    [InlineData("a&bc", "Name must not contain an ampersand")]
    [InlineData("a.bc", "Name must not contain a period")]
    [InlineData(null, "Name must not be null")]
    public void MatchOnValidName_ShouldReturnFalseIfNameIsInvalid(string? name, string reason)
    {
        // Arrange/Act
        bool actual = IsValidName(name);

        // Assert
        actual
            .Should()
            .BeFalse(reason);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("z")]
    [InlineData("Z_")]
    [InlineData("a1")]
    [InlineData("Good_Bye")]
    [InlineData("F150")]
    [InlineData("B_36CR")]
    [InlineData("M__1x")]
    [InlineData("A_b_c_")]
    public void MatchOnValidName_ShouldReturnTrueIfNameIsValid(string name)
    {
        // Arrange/Act
        bool actual = IsValidName(name);

        // Assert
        actual
            .Should()
            .BeTrue();
    }
}