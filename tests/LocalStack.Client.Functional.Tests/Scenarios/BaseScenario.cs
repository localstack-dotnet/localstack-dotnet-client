namespace LocalStack.Client.Functional.Tests.Scenarios;

public abstract class BaseScenario
{
    protected BaseScenario(TestFixture testFixture, string configFile, bool useServiceUrl = false)
    {
        ConfigurationBuilder configurationBuilder = testFixture.CreateConfigureAppConfiguration(configFile);
        Configuration = configurationBuilder.Build();

        IServiceCollection serviceCollection = testFixture.CreateServiceCollection(Configuration);

        serviceCollection
            .AddAwsService<IAmazonS3>(useServiceUrl: useServiceUrl)
            .AddAwsService<IAmazonDynamoDB>(useServiceUrl: useServiceUrl)
            .AddAwsService<IAmazonSQS>(useServiceUrl: useServiceUrl)
            .AddAwsService<IAmazonSimpleNotificationService>(useServiceUrl: useServiceUrl);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected IConfiguration Configuration { get; set; }

    protected ServiceProvider ServiceProvider { get; private set; }
}
