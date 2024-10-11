#pragma warning disable CA1822 // Mark members as static - disabled because of readability
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static - disabled because of readability
namespace LocalStack.Client.Functional.Tests.Fixtures;

public class TestFixture
{
    public ConfigurationBuilder CreateConfigureAppConfiguration(string configFile, ushort hostPort = 4566)
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true);
        builder.AddJsonFile(configFile, optional: true);
        var keyValuePairs = new Dictionary<string, string>(StringComparer.Ordinal) { { "LocalStack:Config:EdgePort", hostPort.ToString(CultureInfo.InvariantCulture) }, };
        builder.AddInMemoryCollection(keyValuePairs!);
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