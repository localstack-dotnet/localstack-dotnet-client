namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockAmazonServiceClient : AmazonServiceClient, IMockAmazonService
{
    public MockAmazonServiceClient() : base(new MockCredentials(), new MockClientConfig())
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

    public AWSCredentials AwsCredentials => Credentials;

    protected override AbstractAWSSigner CreateSigner()
    {
        return new NullSigner();
    }
}
