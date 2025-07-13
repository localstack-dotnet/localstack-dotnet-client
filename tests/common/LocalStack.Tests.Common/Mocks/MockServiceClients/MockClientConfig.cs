using Amazon.Runtime.Endpoints;

namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockClientConfig : ClientConfig, IClientConfig
{
    public MockClientConfig() : this(new MockConfigurationProvider())
    {
    }

    public MockClientConfig(IDefaultConfigurationProvider configurationProvider) : base(configurationProvider)
    {
        ServiceURL = "http://localhost";
    }

    public override string ServiceVersion => "1.0.0.0";

    public override string UserAgent => InternalSDKUtils.BuildUserAgentString(ServiceVersion);

    public override Endpoint DetermineServiceOperationEndpoint(ServiceOperationEndpointParameters parameters)
    {
        return new Endpoint(ServiceURL);
    }

    public override string RegionEndpointServiceName => "mock-service";

    public static MockClientConfig CreateDefaultMockClientConfig() => new(new MockConfigurationProvider());
}