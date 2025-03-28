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

public sealed class LocalStackFixtureV37 : LocalStackFixtureBase
{
    public LocalStackFixtureV37() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV37))
    {
    }
}

public sealed class LocalStackFixtureV43 : LocalStackFixtureBase
{
    public LocalStackFixtureV43() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV43))
    {
    }
}

public interface ILocalStackFixture
{
    LocalStackContainer LocalStackContainer { get; }
}