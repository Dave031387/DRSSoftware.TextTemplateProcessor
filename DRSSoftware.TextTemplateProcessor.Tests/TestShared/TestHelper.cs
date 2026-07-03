namespace DRSSoftware.TextTemplateProcessor.TestShared;

[ExcludeFromCodeCoverage]
internal static class TestHelper
{
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
}