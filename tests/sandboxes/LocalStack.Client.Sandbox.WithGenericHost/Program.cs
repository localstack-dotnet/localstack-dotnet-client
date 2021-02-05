using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Amazon.S3;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using LocalStack.Client.Extensions;

namespace LocalStack.Client.Sandbox.WithGenericHost
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine(Environment.OSVersion.VersionString);
            Console.WriteLine(GetNetCoreVersion());

            Task runConsoleAsync = new HostBuilder().ConfigureHostConfiguration(configHost => configHost.AddEnvironmentVariables())
                                                    .ConfigureAppConfiguration((hostContext, builder) =>
                                                    {
                                                        builder.SetBasePath(Directory.GetCurrentDirectory());
                                                        builder.AddJsonFile("appsettings.json", optional: true);
                                                        builder.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                                                        builder.AddEnvironmentVariables();
                                                        builder.AddCommandLine(args);
                                                    })
                                                    .ConfigureServices((hostContext, services) =>
                                                    {
                                                        services.AddLogging()
                                                                .AddLocalStack(hostContext.Configuration)
                                                                .AddAwsService<IAmazonS3>()
                                                                .AddHostedService<SampleS3Service>();
                                                    })
                                                    .ConfigureLogging((_, configLogging) => { configLogging.AddConsole(); })
                                                    .UseConsoleLifetime()
                                                    .RunConsoleAsync();

            await runConsoleAsync;
        }

        private static string GetNetCoreVersion() {
            Assembly assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            string[] assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
            {
                return assemblyPath[netCoreAppIndex + 1];
            }

            return null;
        }
    }
}
