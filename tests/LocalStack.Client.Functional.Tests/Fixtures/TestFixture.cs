using System.IO;

using LocalStack.Client.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocalStack.Client.Functional.Tests.Fixtures
{
    public class TestFixture
    {
        public ConfigurationBuilder CreateConfigureAppConfiguration(string configFile)
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddJsonFile(configFile, optional: true);
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
}
