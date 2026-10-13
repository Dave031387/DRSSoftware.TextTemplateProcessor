namespace DRSSoftware.TextTemplateProcessor.TestShared;

[ExcludeFromCodeCoverage]
public static class TestData
{
#pragma warning disable IDE0028 // Simplify collection initialization

    /// <summary>
    /// A list of strings containing invalid file name characters.
    /// </summary>
    public static TheoryData<string> InvalidFileNameCharacters => new()
    {
        "\0",
        "\u0001",
        "\u0002",
        "\u0003",
        "\u0004",
        "\u0005",
        "\u0006",
        "\a",
        "\b",
        "\t",
        "\n",
        "\v",
        "\f",
        "\r",
        "\u000e",
        "\u000f",
        "\u0010",
        "\u0011",
        "\u0012",
        "\u0013",
        "\u0014",
        "\u0015",
        "\u0016",
        "\u0017",
        "\u0018",
        "\u0019",
        "\u001a",
        "\u001b",
        "\u001c",
        "\u001d",
        "\u001e",
        "\u001f",
        "\"",
        "*",
        ":",
        "<",
        ">",
        "?",
        "|"
    };

    /// <summary>
    /// A list of strings containing invalid path characters.
    /// </summary>
    public static TheoryData<string> InvalidPathCharacters => new()
    {
        "\0",
        "\u0001",
        "\u0002",
        "\u0003",
        "\u0004",
        "\u0005",
        "\u0006",
        "\a",
        "\b",
        "\t",
        "\n",
        "\v",
        "\f",
        "\r",
        "\u000e",
        "\u000f",
        "\u0010",
        "\u0011",
        "\u0012",
        "\u0013",
        "\u0014",
        "\u0015",
        "\u0016",
        "\u0017",
        "\u0018",
        "\u0019",
        "\u001a",
        "\u001b",
        "\u001c",
        "\u001d",
        "\u001e",
        "\u001f",
        "|"
    };

    /// <summary>
    /// A list of path strings for unit testing. The first string in each set is a path string, the
    /// second is the expected directory path, and the third is the expected file name.
    /// </summary>
    public static TheoryData<string?, string, string> PathStrings => new()
    {
        { null, string.Empty, string.Empty },
        { string.Empty, string.Empty, string.Empty },
        { @"C:\", @"C:\", string.Empty },
        { @"C:\file001", @"C:\", "file001" },
        { @"C:\test1\", @"C:\test1", string.Empty },
        { @"C:\test2\file002", @"C:\test2", "file002" },
        { @"C:\test3\test4\", @"C:\test3\test4", string.Empty },
        { "file003", string.Empty, "file003" },
        { @"test5\", "test5", string.Empty },
        { @"test6\file004", "test6", "file004" },
        { @"\file005", string.Empty, "file005" },
        { @"\test7\file006", "test7", "file006" },
        { @"\test8\test9\", @"test8\test9", string.Empty },
        {(@"\\server01", string.Empty, string.Empty) },
        {(@"\\server02\", string.Empty, string.Empty) },
        {(@"\\server03\share01", string.Empty, string.Empty) },
        {(@"\\server04\share02\", string.Empty, string.Empty) },
        {(@"\\server05\share03\file007", string.Empty, string.Empty) }
    };

    /// <summary>
    /// Combinations of two strings that can be empty or not empty. Used in the <see
    /// cref="TokenParserTests"/> class.
    /// </summary>
    public static TheoryData<string, string> StringValues2 => new()
    {
        {EmptyString, EmptyString },
        {EmptyString, "text1" },
        {"text1", EmptyString },
        {"text1", "text2" }
    };

    /// <summary>
    /// Combinations of three strings that can be empty or not empty. Used in the <see
    /// cref="TokenParserTests"/> class.
    /// </summary>
    public static TheoryData<string, string, string> StringValues3 => new()
    {
        {EmptyString, EmptyString, EmptyString },
        {EmptyString, EmptyString, "text1" },
        {EmptyString, "text1", EmptyString },
        {EmptyString, "text1", "text2" },
        {"text1", EmptyString, EmptyString },
        {"text1", EmptyString, "text2" },
        {"text1", "text2", EmptyString },
        {"text1", "text2", "text3" }
    };

    /// <summary>
    /// Combinations of four strings that can be empty or not empty. Used in the <see
    /// cref="TokenParserTests"/> class.
    /// </summary>
    public static TheoryData<string, string, string, string> StringValues4 => new()
    {
        {EmptyString, EmptyString, EmptyString, EmptyString },
        {"test1", EmptyString, EmptyString, EmptyString },
        {EmptyString, "text1", EmptyString, EmptyString },
        {EmptyString, EmptyString, "text1", EmptyString },
        {EmptyString, EmptyString, EmptyString, "text1" },
        {"text1", "text2", "text3", "text4" }
    };

    /// <summary>
    /// Combinations of four strings and a case flag. The first and last strings can be empty or
    /// not. The second and third can be empty or spaces. Used in the <see
    /// cref="TokenProcessorTests"/> class.
    /// </summary>
    public static TheoryData<char, string, string, string, string> StringValues4WithCaseFlag => new()
    {
        {Space, EmptyString, EmptyString, EmptyString, EmptyString },
        {SameCaseFlag, EmptyString, EmptyString, " ", "text1" },
        {LowercaseFlag, "text1", " ", EmptyString, EmptyString },
        {UppercaseFlag, "text1", " ", " ", "text2" }
    };

    /// <summary>
    /// A list of strings containing whitespace characters.
    /// </summary>
    public static TheoryData<string> Whitespace => new()
    {
        "",
        "\t",
        "\n",
        "\v",
        "\f",
        "\r",
        " ",
        "\u0085",
        "\u00a0",
        "\u2002",
        "\u2003",
        "\u2028",
        "\u2029",
        TestHelper.Whitespace
    };

#pragma warning restore IDE0028 // Simplify collection initialization
}