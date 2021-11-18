namespace LocalStack.Client.Tests.Mocks.MockServiceClients;

public record MockAwsServiceEndpoint() 
    : AwsServiceEndpoint(MockServiceMetadata.MockServiceId, "mockService", AwsServiceEnum.ApiGateway, 1234, "localhost", "http://localhost:1234");

