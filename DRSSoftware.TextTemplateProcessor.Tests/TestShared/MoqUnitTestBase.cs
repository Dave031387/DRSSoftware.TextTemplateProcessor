namespace DRSSoftware.TextTemplateProcessor.TestShared;

[ExcludeFromCodeCoverage]
public abstract class MoqUnitTestBase
{
    private readonly Dictionary<string, Mock> _mocks = [];

    public Mock<T> GetMock<T>() where T : class
    {
        string typeName = typeof(T).Name;

        if (_mocks.TryGetValue(typeName, out Mock? value))
        {
            return (value as Mock<T>)!;
        }
        else
        {
            _mocks.Add(typeName, new Mock<T>(MockBehavior.Strict));
            return (_mocks[typeName] as Mock<T>)!;
        }
    }

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