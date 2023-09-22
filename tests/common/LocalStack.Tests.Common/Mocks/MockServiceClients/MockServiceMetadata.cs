namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockServiceMetadata : IServiceMetadata
{
    public const string MockServiceId = "Mock Amazon Service";

    public string ServiceId => MockServiceId;

    public IDictionary<string, string> OperationNameMapping { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
}