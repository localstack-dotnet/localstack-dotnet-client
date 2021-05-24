using Amazon.Runtime;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    internal class MockCredentials :BasicAWSCredentials
    {
        public MockCredentials() 
            : base("testkey", "testsecret")
        {
        }
    }
}
