namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public record MockAwsServiceEndpoint() : AwsServiceEndpoint(MockServiceMetadata.MockServiceId, "mockService", Client.Enums.AwsService.ApiGateway, 1234, "localhost",
                                                            new Uri("http://localhost:1234/"));