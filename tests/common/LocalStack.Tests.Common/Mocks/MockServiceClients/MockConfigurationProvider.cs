using Amazon;

namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockConfigurationProvider : IDefaultConfigurationProvider
{
    public IDefaultConfiguration GetDefaultConfiguration(RegionEndpoint clientRegion, DefaultConfigurationMode? requestedConfigurationMode = null)
    {
        return new MockConfiguration();
    }
}