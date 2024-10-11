namespace LocalStack.Client.Functional.Tests.Scenarios;

public abstract class BaseScenario
{
    protected BaseScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig, bool useServiceUrl = false)
    {
        ArgumentNullException.ThrowIfNull(testFixture);
        ArgumentNullException.ThrowIfNull(localStackFixture);

        ushort mappedPublicPort = localStackFixture.LocalStackContainer.GetMappedPublicPort(4566);
        ConfigurationBuilder configurationBuilder = testFixture.CreateConfigureAppConfiguration(configFile, mappedPublicPort);
        Configuration = configurationBuilder.Build();

        IServiceCollection serviceCollection = testFixture.CreateServiceCollection(Configuration);

        serviceCollection.AddAwsService<IAmazonS3>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonDynamoDB>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonSQS>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonSimpleNotificationService>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonCloudFormation>(useServiceUrl: useServiceUrl);

        serviceCollection.AddLogging();

        ServiceProvider = serviceCollection.BuildServiceProvider();
        LocalStackFixture = localStackFixture;
    }

    protected ILocalStackFixture LocalStackFixture { get; set; }

    protected IConfiguration Configuration { get; set; }

    protected ServiceProvider ServiceProvider { get; private set; }
}