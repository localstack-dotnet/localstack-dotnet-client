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

public sealed class LocalStackFixtureV414 : LocalStackFixtureBase
{
    public LocalStackFixtureV414() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackV414))
    {
    }
}

public sealed class LocalStackFixtureCurrent : LocalStackFixtureBase
{
    public LocalStackFixtureCurrent() : base(TestContainers.LocalStackBuilder(TestConstants.LocalStackCurrent, GetAuthToken()))
    {
    }

    private static string GetAuthToken()
    {
        string? authToken = Environment.GetEnvironmentVariable(TestConstants.LocalStackAuthTokenVariable);
        if (string.IsNullOrWhiteSpace(authToken))
        {
            throw new System.InvalidOperationException($"LocalStack {TestConstants.LocalStackCurrent} needs an auth token. Set the {TestConstants.LocalStackAuthTokenVariable} environment variable, or exclude these tests with --filter \"LocalStackAuth!=Required\".");
        }

        return authToken;
    }
}

public interface ILocalStackFixture
{
    LocalStackContainer LocalStackContainer { get; }
}