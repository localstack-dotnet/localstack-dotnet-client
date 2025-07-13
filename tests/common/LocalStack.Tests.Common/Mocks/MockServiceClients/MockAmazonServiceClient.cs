#pragma warning disable S2325,CA1822

namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockAmazonServiceClient : AmazonServiceClient, IMockAmazonService
{
    public MockAmazonServiceClient() : base(new MockCredentials(), new MockClientConfig(new MockConfigurationProvider()))
    {
    }

    public MockAmazonServiceClient(AWSCredentials credentials, MockClientConfig clientConfig) : base(credentials, clientConfig)
    {
    }

    public MockAmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, ClientConfig clientConfig) : base(
        awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
    {
    }

    public MockAmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, ClientConfig clientConfig) : base(
        awsAccessKeyId, awsSecretAccessKey, clientConfig)
    {
    }

    public AWSCredentials AwsCredentials => Config.DefaultAWSCredentials;

#if NET8_0_OR_GREATER
    public static ClientConfig CreateDefaultClientConfig()
    {
        return MockClientConfig.CreateDefaultMockClientConfig();
    }

    public static IAmazonService CreateDefaultServiceClient(AWSCredentials awsCredentials, ClientConfig clientConfig)
    {
        return new MockAmazonServiceClient(awsCredentials, MockClientConfig.CreateDefaultMockClientConfig());
    }
#endif
}