#pragma warning disable CA1304,CA1311,MA0011

using DotNet.Testcontainers.Builders;

namespace LocalStack.Client.Functional.Tests;

internal static class TestContainers
{
    public static readonly LocalStackBuilder LocalStackLegacyBuilder = new LocalStackBuilder().WithImage($"localstack/localstack:0.11.4")
                                                                                              .WithName($"localStack-0.11.4-{Guid.NewGuid().ToString().ToLower()}")
                                                                                              .WithEnvironment("DEFAULT_REGION", "eu-central-1")
                                                                                              .WithEnvironment("SERVICES", "s3,dynamodb,sqs,sns")
                                                                                              .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                                                              .WithEnvironment("DEBUG", "1")
                                                                                              .WithPortBinding(AwsServiceEndpointMetadata.DynamoDb.Port, AwsServiceEndpointMetadata.DynamoDb.Port)
                                                                                              .WithPortBinding(AwsServiceEndpointMetadata.Sqs.Port, AwsServiceEndpointMetadata.Sqs.Port)
                                                                                              .WithPortBinding(AwsServiceEndpointMetadata.S3.Port, AwsServiceEndpointMetadata.S3.Port)
                                                                                              .WithPortBinding(AwsServiceEndpointMetadata.Sns.Port, AwsServiceEndpointMetadata.Sns.Port)
                                                                                              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(AwsServiceEndpointMetadata.DynamoDb.Port))
                                                                                              .WithCleanUp(true);

    public static LocalStackBuilder LocalStackBuilder(string version)
    {
        return new LocalStackBuilder().WithImage($"localstack/localstack:{version}")
                                      .WithName($"localStack-{version}-{Guid.NewGuid().ToString().ToLower()}")
                                      .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                      .WithEnvironment("DEBUG", "1")
                                      .WithEnvironment("LS_LOG", "trace-internal")
                                      .WithPortBinding(4566, true)
                                      .WithCleanUp(true);
    }
}