namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockConfiguration : IDefaultConfiguration
{
    public DefaultConfigurationMode Name { get; }

    public RequestRetryMode RetryMode { get; }

    public S3UsEast1RegionalEndpointValue S3UsEast1RegionalEndpoint { get; }

    public TimeSpan? ConnectTimeout { get; }

    public TimeSpan? TlsNegotiationTimeout { get; }

    public TimeSpan? TimeToFirstByteTimeout { get; }

    public TimeSpan? HttpRequestTimeout { get; }
}