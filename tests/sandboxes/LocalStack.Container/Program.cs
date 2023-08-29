using System;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;


Console.WriteLine("Press any key to start LocalStack container");
Console.ReadLine();

ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                                                                    .WithName("LocalStack-1.3.1")
                                                                    .WithImage("localstack/localstack:1.3.1")
                                                                    .WithCleanUp(true)
                                                                    .WithEnvironment("DEFAULT_REGION", "eu-central-1")
                                                                    .WithEnvironment("SERVICES", "iam,lambda,dynamodb,apigateway,s3,sns,cloudformation,cloudwatch,sts")
                                                                    .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                                    .WithEnvironment("LS_LOG", "info")
                                                                    .WithPortBinding(4566, 4566);

TestcontainersContainer container = localStackBuilder.Build();

Console.WriteLine("Starting LocalStack Container");
await container.StartAsync().ConfigureAwait(false);
Console.WriteLine("LocalStack Container started");

Console.WriteLine("Press any key to stop LocalStack container");
Console.ReadLine();

Console.WriteLine("Stopping LocalStack Container");
await container.StopAsync().ConfigureAwait(false);
Console.WriteLine("LocalStack Container stopped");
