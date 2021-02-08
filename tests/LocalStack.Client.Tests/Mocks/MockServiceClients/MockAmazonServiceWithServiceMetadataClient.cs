using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public class MockAmazonServiceWithServiceMetadataClient : AmazonServiceClient, IAmazonService, IMockAmazonServiceWithServiceMetadata
    {
        private static IServiceMetadata serviceMetadata = new MockServiceMetadata();

        public MockAmazonServiceWithServiceMetadataClient() : base(FallbackCredentialsFactory.GetCredentials(), new MockClientConfig())
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

        public AWSCredentials AwsCredentials => Credentials;

        protected override AbstractAWSSigner CreateSigner()
        {
            return new NullSigner();
        }
    }
}