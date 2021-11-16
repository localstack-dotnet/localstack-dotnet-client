Console.WriteLine("Press any key to start Sandbox application");
Console.ReadLine();

Console.WriteLine(Environment.OSVersion.VersionString);
Console.WriteLine(GetNetCoreVersion());

await new HostBuilder().ConfigureHostConfiguration(configHost => configHost.AddEnvironmentVariables())
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

static string GetNetCoreVersion()
{
    Assembly assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
    string[] assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
    int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
    if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
    {
        return assemblyPath[netCoreAppIndex + 1];
    }

    return null;
}