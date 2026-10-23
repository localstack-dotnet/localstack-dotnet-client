#if NET10_0_OR_GREATER
namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public interface IMockFactoryAmazonService : IAmazonService
{
#pragma warning disable CA1033
    static ClientConfig IAmazonService.CreateDefaultClientConfig() => MockClientConfig.CreateDefaultMockClientConfig();

    static IAmazonService IAmazonService.CreateDefaultServiceClient(AWSCredentials awsCredentials, ClientConfig clientConfig)
    {
        FactoryCreatedMockClient.RecordedConfig = clientConfig;
        return new FactoryCreatedMockClient(awsCredentials, (MockClientConfig)clientConfig);
    }
#pragma warning restore CA1033
}
#endif
