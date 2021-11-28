namespace LocalStack.Client.Functional.Tests.Fixtures;

public class LocalStackLegacyFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public LocalStackLegacyFixture()
    {
        int dynamoDbPort = AwsServiceEndpointMetadata.DynamoDb.Port;
        int containerPort = AwsServiceEndpointMetadata.Sqs.Port;
        int hostPort = AwsServiceEndpointMetadata.S3.Port;
        int snsPort = AwsServiceEndpointMetadata.Sns.Port;

        ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                                                                            .WithName($"LocalStackLegacy-0.11.4-{DateTime.Now.Ticks}")
                                                                            .WithImage("localstack/localstack:0.11.4")
                                                                            .WithCleanUp(true)
                                                                            .WithEnvironment("DEFAULT_REGION", "eu-central-1")
                                                                            .WithEnvironment("SERVICES", "s3,dynamodb,sqs,sns")
                                                                            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                                            .WithEnvironment("DEBUG", "1")
                                                                            .WithPortBinding(dynamoDbPort, dynamoDbPort)
                                                                            .WithPortBinding(containerPort, containerPort)
                                                                            .WithPortBinding(hostPort, hostPort)
                                                                            .WithPortBinding(snsPort, snsPort);                                                                        

        _localStackContainer = localStackBuilder.Build();
    }
    public async Task InitializeAsync()
    {
        await _localStackContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _localStackContainer.StopAsync();
    }
}
