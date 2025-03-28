#pragma warning disable CA1304,CA1311,MA0011

namespace LocalStack.Client.Functional.Tests;

internal static class TestContainers
{
    public static LocalStackBuilder LocalStackBuilder(string version)
    {
        return new LocalStackBuilder().WithImage($"localstack/localstack:{version}")
                                      .WithName($"localStack-{version}-{Guid.NewGuid().ToString().ToLower()}")
                                      .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                      .WithEnvironment("DEBUG", "1")
                                      .WithEnvironment("LS_LOG", "trace-internal")
                                      .WithPortBinding(4566, assignRandomHostPort: true)
                                      .WithCleanUp(true);
    }
}