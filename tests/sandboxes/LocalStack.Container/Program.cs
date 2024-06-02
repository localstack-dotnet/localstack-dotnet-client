Console.WriteLine("Press any key to start LocalStack container");
Console.ReadLine();

string containerId = Guid.NewGuid().ToString().ToUpperInvariant();
LocalStackBuilder localStackBuilder = new LocalStackBuilder().WithImage($"localstack/localstack:3.4.0")
                                                             .WithName($"localStack-latest-{containerId}")
                                                             .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                             .WithEnvironment("DEBUG", "1")
                                                             .WithEnvironment("LS_LOG", "trace-internal")
                                                             .WithPortBinding(4566, 4566)
                                                             .WithCleanUp(true);

LocalStackContainer container = localStackBuilder.Build();

Console.WriteLine("Starting LocalStack Container");
await container.StartAsync();
Console.WriteLine("LocalStack Container started");

Console.WriteLine("Press any key to stop LocalStack container");
Console.ReadLine();

Console.WriteLine("Stopping LocalStack Container");
await container.DisposeAsync();
Console.WriteLine("LocalStack Container stopped");