namespace LocalStack.Client.Functional.Tests.Fixtures;

public class LocalStackLegacyFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public LocalStackLegacyFixture()
    {
        ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithName("LocalStackLegacy-0.11.4")
            .WithImage("localstack/localstack:0.11.4")
            .WithCleanUp(true)
            .WithEnvironment("DEFAULT_REGION", "eu-central-1")
            .WithEnvironment("SERVICES", "s3,dynamodb,sqs")
            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
            .WithEnvironment("DEBUG", "1")
            .WithPortBinding(4569, 4569) // dynamo
            .WithPortBinding(4576, 4576) // sqs
            .WithPortBinding(4572, 4572); // s3

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
