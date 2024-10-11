#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Fixtures;

public abstract class LocalStackFixtureBase : IAsyncLifetime, ILocalStackFixture
{
    protected LocalStackFixtureBase(LocalStackBuilder localStackBuilder)
    {
        ArgumentNullException.ThrowIfNull(localStackBuilder);

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

public sealed class LocalStackFixtureV23 : LocalStackFixtureBase
{
    public LocalStackFixtureV23() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV23))
    {
    }
}

public sealed class LocalStackFixtureV34 : LocalStackFixtureBase
{
    public LocalStackFixtureV34() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV34))
    {
    }
}

public sealed class LocalStackFixtureV38 : LocalStackFixtureBase
{
    public LocalStackFixtureV38() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV38))
    {
    }
}

public interface ILocalStackFixture
{
    LocalStackContainer LocalStackContainer { get; }
}