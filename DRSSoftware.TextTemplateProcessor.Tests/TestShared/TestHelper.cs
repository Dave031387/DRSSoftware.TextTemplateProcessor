namespace DRSSoftware.TextTemplateProcessor.TestShared;

[ExcludeFromCodeCoverage]
internal static class TestHelper
{
    internal const string ArgumentNullMessage = "Value cannot be null. (Parameter '{0}')";

    internal const string EmptyString = "";

    internal const char Space = ' ';

    internal const string Whitespace = "\t\n\v\f\r \u0085\u00a0\u2002\u2003\u2028\u2029";

    internal static string[] SampleText => ["Line 1", "Line 2", "Line 3"];

    internal static void AssertException<T>(Action action, string message)
             where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(message);
    }

    internal static void AssertException<T>(Action action, string inner, string outer)
        where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(outer)
            .WithInnerExceptionExactly<T>()
            .WithMessage(inner);
    }

    internal static void AssertException<TInner, TOuter>(Action action, string inner, string outer)
        where TInner : Exception
        where TOuter : Exception
    {
        action
            .Should()
            .ThrowExactly<TOuter>()
            .WithMessage(outer)
            .WithInnerExceptionExactly<TInner>()
            .WithMessage(inner);
    }

    internal static string CreateToken(string tokenName, char initialCaseFlag = Space, bool isEscaped = false)
    {
        return isEscaped
            ? initialCaseFlag == Space
                ? $"{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultTokenEscapeCharacter}{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}"
            : initialCaseFlag == Space
                ? $"{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}";
    }

    internal static string GetNullDependencyMessage(string className, string serviceName, string parameterName)
                        => GetMessage(MsgDependencyIsNull, className, serviceName) + $" (Parameter '{parameterName}')";
}