namespace LocalStack.Client.Tests.Mocks.MockServiceClients;

public record MockAwsServiceEndpoint() 
    : AwsServiceEndpoint(MockServiceMetadata.MockServiceId, "mockService", AwsService.ApiGateway, 1234, "localhost", new Uri("http://localhost:1234/"));

