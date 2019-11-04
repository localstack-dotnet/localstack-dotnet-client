using LocalStack.Client.Enums;
using LocalStack.Client.Models;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public class MockAwsServiceEndpoint : AwsServiceEndpoint
    {
        public MockAwsServiceEndpoint() : base(MockServiceMetadata.MockServiceId, "mockService", AwsServiceEnum.ApiGateway, 1234, "localhost", "http://localhost:1234")
        {
        }
    }
}