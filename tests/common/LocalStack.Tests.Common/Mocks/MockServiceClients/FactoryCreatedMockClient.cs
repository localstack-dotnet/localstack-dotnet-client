#if NET10_0_OR_GREATER
namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public sealed class FactoryCreatedMockClient(AWSCredentials credentials, MockClientConfig clientConfig)
    : MockAmazonServiceClient(credentials, clientConfig), IMockFactoryAmazonService
{
    public static ClientConfig? RecordedConfig { get; set; }
}
#endif
