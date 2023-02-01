namespace LocalStack.Client.Functional.Tests.Fixtures;

public class LocalStackFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public LocalStackFixture()
    {
        ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                                                                            .WithName($"LocalStack-1.3.1-{DateTime.Now.Ticks}")
                                                                            .WithImage("localstack/localstack:1.3.1")
                                                                            .WithCleanUp(true)
                                                                            .WithEnvironment("SERVICES", "s3,dynamodb,sqs,sns")
                                                                            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                                            .WithEnvironment("DEBUG", "1")
                                                                            .WithEnvironment("LS_LOG", "trace")
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
