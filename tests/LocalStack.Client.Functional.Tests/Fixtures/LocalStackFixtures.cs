namespace LocalStack.Client.Functional.Tests.Fixtures;

public abstract class LocalStackFixtureBase : IAsyncLifetime, ILocalStackFixture
{
    public LocalStackFixtureBase(LocalStackBuilder localStackBuilder)
    {
        LocalStackContainer = localStackBuilder.Build();
    }

    public LocalStackContainer LocalStackContainer { get; }

    public async Task InitializeAsync()
    {
        await LocalStackContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await LocalStackContainer.StopAsync();
    }
}

public sealed class LocalStackFixtureV131 : LocalStackFixtureBase
{
    public LocalStackFixtureV131() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV13))
    {
    }
}

public sealed class LocalStackFixtureV20 : LocalStackFixtureBase
{
    public LocalStackFixtureV20() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV20))
    {
    }
}

public sealed class LocalStackFixtureV22 : LocalStackFixtureBase
{
    public LocalStackFixtureV22() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV22))
    {
    }
}

public class LocalStackLegacyFixture : LocalStackFixtureBase
{
    public LocalStackLegacyFixture() : base(TestContainers.LocalStackLegacyBuilder)
    {
    }
}

public interface ILocalStackFixture
{
    LocalStackContainer LocalStackContainer { get; }
}