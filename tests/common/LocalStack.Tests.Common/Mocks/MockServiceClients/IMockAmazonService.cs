namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public interface IMockAmazonService : IDisposable, IAmazonService
{
#if NET8_0_OR_GREATER
#pragma warning disable CA1033
    static ClientConfig IAmazonService.CreateDefaultClientConfig() => MockClientConfig.CreateDefaultMockClientConfig();

    static IAmazonService IAmazonService.CreateDefaultServiceClient(AWSCredentials awsCredentials, ClientConfig clientConfig)
    {
        return new MockAmazonServiceClient(awsCredentials, MockClientConfig.CreateDefaultMockClientConfig());
    }
#pragma warning restore CA1033
#endif
}