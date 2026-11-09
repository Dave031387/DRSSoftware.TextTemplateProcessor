namespace DRSSoftware.TextTemplateProcessor.Core;

[ExcludeFromCodeCoverage]
public class RegexHelperTests
{
    private const int PrefixLength = 4;
    private const string Space = " ";
    private const string Spaces = "  ";
    private readonly string _testString = "This is a test string.";

    [Theory]
    [InlineData("Test string with no format items.", 0)]
    [InlineData("{0} Test string with format item.", 1)]
    [InlineData("Test string with format item. {1}", 1)]
    [InlineData("Test string with {2} format item.", 1)]
    [InlineData("{0}{1} Test string with format items.", 2)]
    [InlineData("Test {0} string {1} with format item.", 2)]
    [InlineData("{0} Test string with format item. {1}", 2)]
    [InlineData("{0} Test {1}{2} string with format item.", 3)]
    [InlineData("Test {1}{2} string with {0} format item. {4}{3}", 5)]
    public void CountFormatItems_ShouldReturnCorrectNumberOfValidFormatItems(string text, int expected)
    {
        // Arrange/Act
        int actual = GetFormatItemCount(text);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData("Sample text with no format items.")]
    [InlineData("{0 No format item here")]
    [InlineData("No format item here 1}")]
    [InlineData("No format {2 item here")]
    [InlineData("No format 3} item here")]
    [InlineData("No format item {4 } here")]
    [InlineData("No { 5} format item here")]
    public void CountFormatItems_ShouldReturnZeroIfTextDoesNotContainValidFormatItems(string text)
    {
        // Arrange/Act
        int actual = GetFormatItemCount(text);
        int expected = 0;

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(NormalIndentString + AbsoluteIndexString + "10")]
    [InlineData(Space + NormalIndentString + AbsoluteIndexString + "1")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "a ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "  ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "3 ")]
    [InlineData(NormalIndentString + " 3 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "21")]
    [InlineData(Space + OneTimeIndentString + AbsoluteIndexString + "1")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "x ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "2 ")]
    [InlineData("D" + AbsoluteIndexString + "1 ")]
    [InlineData(AbsoluteIndexString + "1  ")]
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "9 ")]
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
    [InlineData(NormalIndentString + RelativeRightIndentString + "4 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "8 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "13")]
    [InlineData(Space + NormalIndentString + AbsoluteIndexString + "6")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "2 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(AbsoluteIndexString + "8  ")]
    [InlineData(FileHeaderCode + Space)]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "5 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "9 ")]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "10")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "1x")]
    [InlineData("o" + AbsoluteIndexString + "2 ")]
    [InlineData(Space + NormalIndentString + AbsoluteIndexString + "1")]
    [InlineData(Space + RelativeLeftIndentString + "5 ")]
    [InlineData(Space + RelativeRightIndentString + "6 ")]
    [InlineData(Space + AbsoluteIndexString + "7 ")]
    [InlineData(NormalIndentString + ".3 ")]
    [InlineData("X" + RelativeRightIndentString + "4 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + Spaces)]
    [InlineData(FileHeaderCode + Space)]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "1 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "2 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "3 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "4 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "5 ")]
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
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(NoControlCode + Space)]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "7 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "9 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
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
    [InlineData("#   ")]
    [InlineData("##  ")]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(Space + FileHeaderCode)]
    [InlineData(FileHeaderCode + "6")]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "7 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "9 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
    public void MatchOnFileHeaderCode_ShouldReturnFalseIfTextDoesNotBeginWithFileHeaderCode(string prefix)
    {
        // Arrange
        string text = $"{prefix}{_testString}";

        // Act
        bool actual = IsFileHeader(text);

        // Assert
        prefix.Length
            .Should()
            .Be(PrefixLength);
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void MatchOnFileHeaderCode_ShouldReturnTrueIfTextBeginsWithFileHeaderCode()
    {
        // Arrange
        string text = $"{FileHeaderCode} {_testString}";

        // Act
        bool actual = IsFileHeader(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NormalIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "8 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "6 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "5 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
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
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "8 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "13")]
    [InlineData(Space + OneTimeIndentString + AbsoluteIndexString + "6")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "2 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(AbsoluteIndexString + "8  ")]
    [InlineData(FileHeaderCode + Space)]
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
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "9 ")]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "K ")]
    [InlineData(Space + OneTimeIndentString + RelativeRightIndentString + "1")]
    [InlineData(OneTimeIndentString + " 7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "10")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "6x")]
    [InlineData(FileHeaderCode + Space)]
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
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "8 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "7 ")]
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
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(OneTimeIndentString + Space + RelativeLeftIndentString + "2")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "04")]
    [InlineData(NoControlCode + Space)]
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Space + OneTimeIndentString + RelativeLeftIndentString + "6")]
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
        string text = $"{OneTimeIndentString}{RelativeLeftIndentString}9 {_testString}";

        // Act
        bool actual = HasOneTimeRelativeLeftIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(OneTimeIndentString + Space + RelativeRightIndentString + "2")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "04")]
    [InlineData(NoControlCode + Space)]
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(Space + OneTimeIndentString + RelativeRightIndentString + "6")]
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
        string text = $"{OneTimeIndentString}{RelativeRightIndentString}9 {_testString}";

        // Act
        bool actual = HasOneTimeRelativeRightIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(NormalIndentString + AbsoluteIndexString + "6 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "4 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "30")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "1r")]
    [InlineData(OneTimeIndentString + Space + RelativeLeftIndentString + "2")]
    [InlineData(OneTimeIndentString + Space + RelativeRightIndentString + "5")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "0G")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "11")]
    [InlineData(Space + RelativeLeftIndentString + "5 ")]
    [InlineData(Space + RelativeRightIndentString + "8 ")]
    [InlineData("X" + RelativeRightIndentString + "6 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + Spaces)]
    [InlineData(FileHeaderCode + Space)]
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
    [InlineData(NormalIndentString + RelativeRightIndentString + "2 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "4 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "6 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "8 ")]
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
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "1 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "2 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "3 ")]
    [InlineData(Space + RelativeLeftIndentString + "4 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "08")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + " 9")]
    [InlineData(Space + NormalIndentString + RelativeLeftIndentString + "7")]
    [InlineData(FileHeaderCode + Space)]
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
        string text = $"{NormalIndentString}{RelativeLeftIndentString}3 {_testString}";

        // Act
        bool actual = HasRelativeLeftIndentCode(text);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "1 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "0 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "2 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(Space + RelativeRightIndentString + "4 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "08")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + " 9")]
    [InlineData(Space + OneTimeIndentString + RelativeRightIndentString + "7")]
    [InlineData(FileHeaderCode + Space)]
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
        string text = $"{NormalIndentString}{RelativeRightIndentString}1 {_testString}";

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
    [InlineData(FileHeaderCode + Space)]
    [InlineData(Space + SegmentHeaderCode)]
    [InlineData(SegmentHeaderCode + "6")]
    [InlineData(CommentCode + Space)]
    [InlineData(NoControlCode + Space)]
    [InlineData(NormalIndentString + AbsoluteIndexString + "1 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "3 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "5 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "7 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "9 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
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
    [InlineData(Space + FileHeaderCode)]
    [InlineData(Space + SegmentHeaderCode)]
    [InlineData(Space + CommentCode)]
    [InlineData(Space + NormalIndentString + AbsoluteIndexString + "0")]
    [InlineData(Space + NormalIndentString + RelativeLeftIndentString + "1")]
    [InlineData(Space + NormalIndentString + RelativeRightIndentString + "2")]
    [InlineData(Space + OneTimeIndentString + AbsoluteIndexString + "3")]
    [InlineData(Space + OneTimeIndentString + RelativeLeftIndentString + "4")]
    [InlineData(Space + OneTimeIndentString + RelativeRightIndentString + "5")]
    [InlineData(NormalIndentString + Space + AbsoluteIndexString + "6")]
    [InlineData(NormalIndentString + Space + RelativeLeftIndentString + "7")]
    [InlineData(NormalIndentString + Space + RelativeRightIndentString + "8")]
    [InlineData(OneTimeIndentString + Space + AbsoluteIndexString + "9")]
    [InlineData(OneTimeIndentString + Space + RelativeLeftIndentString + "0")]
    [InlineData(OneTimeIndentString + Space + RelativeRightIndentString + "1")]
    [InlineData(NormalIndentString + AbsoluteIndexString + " 2")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + " 3")]
    [InlineData(NormalIndentString + RelativeRightIndentString + " 4")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + " 5")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + " 6")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + " 7")]
    [InlineData(NormalIndentString + AbsoluteIndexString + "81")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "92")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "03")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "14")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "25")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "36")]
    [InlineData(Space + AbsoluteIndexString + "4 ")]
    [InlineData(Space + RelativeLeftIndentString + "5 ")]
    [InlineData(Space + RelativeRightIndentString + "6 ")]
    [InlineData(NormalIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(NormalIndentString + RelativeLeftIndentString + Spaces)]
    [InlineData(NormalIndentString + RelativeRightIndentString + Spaces)]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + Spaces)]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + Spaces)]
    [InlineData(NormalIndentString + " 7 ")]
    [InlineData(OneTimeIndentString + " 8 ")]
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
    [InlineData(FileHeaderCode + Space)]
    [InlineData(SegmentHeaderCode + Space)]
    [InlineData(CommentCode + Space)]
    [InlineData(NormalIndentString + AbsoluteIndexString + "9 ")]
    [InlineData(NormalIndentString + RelativeLeftIndentString + "8 ")]
    [InlineData(NormalIndentString + RelativeRightIndentString + "7 ")]
    [InlineData(OneTimeIndentString + AbsoluteIndexString + "6 ")]
    [InlineData(OneTimeIndentString + RelativeLeftIndentString + "5 ")]
    [InlineData(OneTimeIndentString + RelativeRightIndentString + "4 ")]
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