namespace DRSSoftware.TextTemplateProcessor.TestShared;

[ExcludeFromCodeCoverage]
internal static class TestHelper
{
    public const string Whitespace = "\t\n\v\f\r \u0085\u00a0\u2002\u2003\u2028\u2029";

    internal static void AssertException<T>(Action action, string message) where T : Exception
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
        where TInner : Exception where TOuter : Exception
    {
        action
            .Should()
            .ThrowExactly<TOuter>()
            .WithMessage(outer)
            .WithInnerExceptionExactly<TInner>()
            .WithMessage(inner);
    }

    public static string GetNullDependencyMessage(string className, string serviceName, string parameterName)
        => string.Format(MsgDependencyIsNull, className, serviceName) + $" (Parameter '{parameterName}')";
}