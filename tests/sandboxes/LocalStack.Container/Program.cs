using System;
using System.Threading.Tasks;

using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;

namespace LocalStack.Container
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Press any key to start LocalStack container");
            Console.ReadLine();

            ITestcontainersBuilder<TestcontainersContainer> localStackBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                                                                                .WithName("LocalStack-0.12.6")
                                                                                .WithImage("localstack/localstack:0.12.6")
                                                                                .WithCleanUp(true)
                                                                                .WithEnvironment("DEFAULT_REGION", "eu-central-1")
                                                                                .WithEnvironment("SERVICES", "s3,dynamodb,sqs")
                                                                                .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                                                                                .WithEnvironment("LS_LOG", "info")
                                                                                .WithPortBinding(4566, 4566);

            TestcontainersContainer container = localStackBuilder.Build();

            Console.WriteLine("Starting LocalStack Container");
            await container.StartAsync();
            Console.WriteLine("LocalStack Container started");

            Console.WriteLine("Press any key to stop LocalStack container");
            Console.ReadLine();

            Console.WriteLine("Stopping LocalStack Container");
            await container.StopAsync();
            Console.WriteLine("LocalStack Container stopped");
        }
    }
}
