namespace LocalStack.Client.Functional.Tests.Scenarios;

public abstract class BaseScenario
{
    protected BaseScenario(TestFixture testFixture, string configFile)
    {
        TestFixture testFixture1 = testFixture;

        ConfigurationBuilder configurationBuilder = testFixture1.CreateConfigureAppConfiguration(configFile);
        Configuration = configurationBuilder.Build();

        IServiceCollection serviceCollection = testFixture1.CreateServiceCollection(Configuration);
        serviceCollection
            .AddAwsService<IAmazonS3>()
            .AddAwsService<IAmazonDynamoDB>()
            .AddAwsService<IAmazonSQS>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected IConfiguration Configuration { get; set; }

    protected ServiceProvider ServiceProvider { get; private set; }
}
