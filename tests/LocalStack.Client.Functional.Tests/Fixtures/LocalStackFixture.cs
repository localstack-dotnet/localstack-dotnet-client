namespace LocalStack.Client.Functional.Tests.Fixtures;

public class LocalStackFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public LocalStackFixture()
    {
        ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithName($"LocalStack-0.13.0-{DateTime.Now.Ticks}")
            .WithImage("localstack/localstack:0.13.0")
            .WithCleanUp(true)
            .WithEnvironment("DEFAULT_REGION", "eu-central-1")
            .WithEnvironment("SERVICES", "s3,dynamodb,sqs")
            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
            .WithEnvironment("LS_LOG", "info")
            .WithPortBinding(4566, 4566);

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
