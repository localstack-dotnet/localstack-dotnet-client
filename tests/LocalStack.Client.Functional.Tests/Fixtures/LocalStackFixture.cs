namespace LocalStack.Client.Functional.Tests.Fixtures;

public class LocalStackFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _localStackContainer;

    public LocalStackFixture()
    {
        ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                                                                            .WithName($"LocalStack-0.14.2-{DateTime.Now.Ticks}")
                                                                            .WithImage("localstack/localstack:0.14.2")
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
