namespace LocalStack.Client.Functional.Tests.Fixtures;

public class TestFixture
{
    public ConfigurationBuilder CreateConfigureAppConfiguration(string configFile, ushort hostPort = 4566)
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true);
        builder.AddJsonFile(configFile, optional: true);
        builder.AddInMemoryCollection(new Dictionary<string, string> { { "LocalStack:Config:EdgePort", hostPort.ToString() }, });
        builder.AddEnvironmentVariables();

        return builder;
    }

    public IServiceCollection CreateServiceCollection(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLocalStack(configuration);

        return serviceCollection;
    }
}