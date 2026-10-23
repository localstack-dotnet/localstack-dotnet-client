#pragma warning disable CA1304,CA1311,MA0011

namespace LocalStack.Client.Functional.Tests;

internal static class TestContainers
{
    public static LocalStackBuilder LocalStackBuilder(string version, string? authToken = null)
    {
        var builder = new LocalStackBuilder($"localstack/localstack:{version}")
                                      .WithName($"localStack-{version}-{Guid.NewGuid().ToString().ToLower()}")
                                      .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                      .WithEnvironment("DEBUG", "1")
                                      .WithEnvironment("LS_LOG", "trace-internal")
                                      .WithPortBinding(4566, assignRandomHostPort: true)
                                      .WithCleanUp(true);

        if (authToken is not null)
        {
            builder = builder.WithEnvironment(TestConstants.LocalStackAuthTokenVariable, authToken);
        }

        return builder;
    }
}