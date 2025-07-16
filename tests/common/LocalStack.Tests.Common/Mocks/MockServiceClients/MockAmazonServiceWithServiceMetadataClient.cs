using Amazon.Runtime.Credentials;

#pragma warning disable S1144, CA1823
namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockAmazonServiceWithServiceMetadataClient : AmazonServiceClient, IMockAmazonServiceWithServiceMetadata
{
    private static IServiceMetadata serviceMetadata = new MockServiceMetadata();

    public MockAmazonServiceWithServiceMetadataClient() : base(DefaultAWSCredentialsIdentityResolver.GetCredentials(), MockClientConfig.CreateDefaultMockClientConfig())
    {
    }

    public MockAmazonServiceWithServiceMetadataClient(AWSCredentials credentials, MockClientConfig clientConfig) : base(credentials, clientConfig)
    {
    }

    public MockAmazonServiceWithServiceMetadataClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, ClientConfig clientConfig) : base(
        awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
    {
    }

    public MockAmazonServiceWithServiceMetadataClient(string awsAccessKeyId, string awsSecretAccessKey, ClientConfig clientConfig) : base(
        awsAccessKeyId, awsSecretAccessKey, clientConfig)
    {
    }

#if NET8_0_OR_GREATER
    public static ClientConfig CreateDefaultClientConfig()
    {
        return MockClientConfig.CreateDefaultMockClientConfig();
    }

    public static IAmazonService CreateDefaultServiceClient(AWSCredentials awsCredentials, ClientConfig clientConfig)
    {
        return new MockAmazonServiceWithServiceMetadataClient(awsCredentials, MockClientConfig.CreateDefaultMockClientConfig());
    }
#endif
}