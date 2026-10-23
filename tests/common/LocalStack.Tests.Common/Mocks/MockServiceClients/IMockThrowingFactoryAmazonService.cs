#if NET10_0_OR_GREATER
namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public interface IMockThrowingFactoryAmazonService : IAmazonService
{
    static InvalidOperationException Failure { get; } = new("fixture static factory failure");

#pragma warning disable CA1033
    static ClientConfig IAmazonService.CreateDefaultClientConfig() => MockClientConfig.CreateDefaultMockClientConfig();

    static IAmazonService IAmazonService.CreateDefaultServiceClient(AWSCredentials awsCredentials, ClientConfig clientConfig) => throw Failure;
#pragma warning restore CA1033
}
#endif
