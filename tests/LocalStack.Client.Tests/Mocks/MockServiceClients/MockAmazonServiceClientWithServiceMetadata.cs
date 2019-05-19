using Amazon.Runtime.Internal;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public class MockAmazonServiceClientWithServiceMetadata : MockAmazonServiceClient
    {
        private static IServiceMetadata serviceMetadata = new MockServiceMetadata();
    }
}