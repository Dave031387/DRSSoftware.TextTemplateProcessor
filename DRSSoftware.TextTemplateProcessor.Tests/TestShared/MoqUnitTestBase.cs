namespace DRSSoftware.TextTemplateProcessor.TestShared;

/// <summary>
/// This class manages creation/retrieval of mock objects and verifying setups on those mock objects.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class MoqUnitTestBase
{
    /// <summary>
    /// A dictionary of mock objects that have been created. The mock objects can be retrieved by
    /// using their type name as the key.
    /// </summary>
    private readonly Dictionary<string, Mock> _mocks = [];

    /// <summary>
    /// Retrieves a mock object of type <see cref="Mock{T}"/> where <typeparamref name="T"/> is the type of the interface
    /// that is being mocked.
    /// </summary>
    /// <remarks>
    /// The mock object will be created if it doesn't already exist. Subsequent requests for the
    /// same type will return the same instance that was created.
    /// </remarks>
    /// <typeparam name="T">
    /// The interface type that is being mocked.
    /// </typeparam>
    /// <returns>
    /// An object of type <see cref="Mock{T}"/>.
    /// </returns>
    public Mock<T> GetMock<T>() where T : class
    {
        string typeName = typeof(T).Name;

        if (_mocks.TryGetValue(typeName, out Mock? value))
        {
            return value is Mock<T> valueMock
                ? valueMock
                : throw new InvalidOperationException($"Unexpected issue occurred when attempting to retrieve mock object of type {typeName}.");
        }
        else
        {
            Mock<T> mock = new(MockBehavior.Strict);
            _mocks.Add(typeName, mock);
            return mock;
        }
    }

    /// <summary>
    /// Verifies all setups that have been defined for all created mock objects.
    /// </summary>
    public void VerifyMocks()
    {
        foreach (string key in _mocks.Keys)
        {
            if (_mocks[key].Setups.Any())
            {
                _mocks[key].VerifyAll();
            }
        }
    }
}