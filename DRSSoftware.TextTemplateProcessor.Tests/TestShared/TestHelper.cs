namespace DRSSoftware.TextTemplateProcessor.TestShared;

/// <summary>
/// This class contains various common fields and methods that are shared among the various unit
/// test classes in this project.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TestHelper
{
    /// <summary>
    /// This is the standard .NET <see cref="ArgumentNullException"/> message.
    /// </summary>
    internal const string ArgumentNullMessage = "Value cannot be null. (Parameter '{0}')";

    /// <summary>
    /// This is an empty string constant value.
    /// </summary>
    internal const string EmptyString = "";

    /// <summary>
    /// This is a space character constant value.
    /// </summary>
    internal const char Space = ' ';

    /// <summary>
    /// This is a string made up of various Unicode characters that are all considered to be
    /// whitespace characters.
    /// </summary>
    internal const string Whitespace = "\t\n\v\f\r \u0085\u00a0\u2002\u2003\u2028\u2029";

    /// <summary>
    /// Asserts that the given <paramref name="action"/> results in the given exception type
    /// <typeparamref name="T"/> being thrown with the given exception <paramref name="message"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of exception expected to be thrown.
    /// </typeparam>
    /// <param name="action">
    /// The action that should result in the exception being thrown.
    /// </param>
    /// <param name="message">
    /// The exception message associated with the thrown exception.
    /// </param>
    internal static void AssertException<T>(Action action, string message)
             where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(message);
    }

    /// <summary>
    /// Asserts that the given <paramref name="action"/> results in the given exception type
    /// <typeparamref name="T"/> being thrown with the given <paramref name="inner"/> and <paramref
    /// name="outer"/> exception messages.
    /// </summary>
    /// <typeparam name="T">
    /// The type of exception expected to be thrown (both inner and outer).
    /// </typeparam>
    /// <param name="action">
    /// The action that should result in the exception being thrown.
    /// </param>
    /// <param name="inner">
    /// The expected inner exception message.
    /// </param>
    /// <param name="outer">
    /// The expected outer exception message.
    /// </param>
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

    /// <summary>
    /// Asserts that the given <paramref name="action"/> results in the given <typeparamref
    /// name="TInner"/> and <typeparamref name="TOuter"/> exception types being thrown with the
    /// given <paramref name="inner"/> and <paramref name="outer"/> exception messages.
    /// </summary>
    /// <typeparam name="TInner">
    /// The type of the inner exception that should be thrown.
    /// </typeparam>
    /// <typeparam name="TOuter">
    /// The type of the outer exception that should be thrown.
    /// </typeparam>
    /// <param name="action">
    /// The action that should result in the exception being thrown.
    /// </param>
    /// <param name="inner">
    /// The expected inner exception message.
    /// </param>
    /// <param name="outer">
    /// The expected outer exception message.
    /// </param>
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

    /// <summary>
    /// Creates a token string for the given <paramref name="tokenName"/>.
    /// </summary>
    /// <param name="tokenName">
    /// The token name to be associated with the created token.
    /// </param>
    /// <param name="initialCaseFlag">
    /// The initial case flag for this token. Signifies whether the first character of the value
    /// assigned to the token should be lowercase (-), uppercase (+), or as-is ( ).
    /// </param>
    /// <param name="isEscaped">
    /// Indicates whether the created token should be escaped.
    /// </param>
    /// <returns>
    /// A token string built to the given specifications.
    /// </returns>
    internal static string CreateToken(string tokenName, char initialCaseFlag = Space, bool isEscaped = false)
    {
        return isEscaped
            ? initialCaseFlag == Space
                ? $"{DefaultDelimiterEscapeCharacter}{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultDelimiterEscapeCharacter}{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}"
            : initialCaseFlag == Space
                ? $"{DefaultTokenStartDelimiter} {tokenName} {DefaultTokenEndDelimiter}"
                : $"{DefaultTokenStartDelimiter}{initialCaseFlag} {tokenName} {DefaultTokenEndDelimiter}";
    }

    /// <summary>
    /// Gets the standard message string that gets generated by Moq when the Throws option is used
    /// in a Setup.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the exception that was thrown.
    /// </typeparam>
    /// <returns>
    /// The standard Moq exception message string for the given exception type.
    /// </returns>
    internal static string GetMoqExceptionString<T>() where T : Exception
    {
        string typeName = typeof(T).FullName!;
        return $"Exception of type '{typeName}' was thrown.";
    }

    /// <summary>
    /// Gets the message string that should be generated when a given dependency is a null reference.
    /// </summary>
    /// <param name="className">
    /// The class name that has the dependency.
    /// </param>
    /// <param name="serviceName">
    /// The interface name that is associated with the dependency.
    /// </param>
    /// <param name="parameterName">
    /// The parameter name assigned to the dependency.
    /// </param>
    /// <returns>
    /// The null dependency message string corresponding to the specified parameter values.
    /// </returns>
    internal static string GetNullDependencyMessage(string className, string serviceName, string parameterName)
        => GetMessage(MsgDependencyIsNull, className, serviceName) + $" (Parameter '{parameterName}')";
}