namespace LocalStack.Client.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLocalStack_Should_Configure_LocalStackOptions_If_There_Is_Not_LocalStack_Section()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        LocalStackOptions localStackOptions = provider.GetRequiredService<IOptions<LocalStackOptions>>()?.Value;

        Assert.NotNull(localStackOptions);
        Assert.False(localStackOptions.UseLocalStack);
        Assert.True(new SessionOptions().DeepEquals(localStackOptions.Session));
        Assert.True(new ConfigOptions().DeepEquals(localStackOptions.Config));
    }

    [Fact]
    public void AddLocalStack_Should_Configure_LocalStackOptions_By_LocalStack_Section()
    {
        var configurationValue = new Dictionary<string, string> { { "LocalStack:UseLocalStack", "true" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        LocalStackOptions localStackOptions = provider.GetRequiredService<IOptions<LocalStackOptions>>()?.Value;

        Assert.NotNull(localStackOptions);
        Assert.True(localStackOptions.UseLocalStack);
        Assert.True(new SessionOptions().DeepEquals(localStackOptions.Session));
        Assert.True(new ConfigOptions().DeepEquals(localStackOptions.Config));
    }

    [Fact]
    public void AddLocalStack_Should_Configure_SessionOptions_If_There_Is_Not_Session_Section()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>()?.Value;

        Assert.NotNull(sessionOptions);
        Assert.True(new SessionOptions().DeepEquals(sessionOptions));
    }

    [Fact]
    public void AddLocalStack_Should_Configure_SessionOptions_By_Session_Section()
    {
        const string awsAccessKeyId = "myawsakid";
        const string awsAccessKey = "myawsak";
        const string awsSessionToken = "myawsst";
        const string regionName = "mars-central-1";

        var configurationValue = new Dictionary<string, string>
            {
                {"LocalStack:Session:AwsAccessKeyId", awsAccessKeyId},
                {"LocalStack:Session:AwsAccessKey", awsAccessKey},
                {"LocalStack:Session:AwsSessionToken", awsSessionToken},
                {"LocalStack:Session:RegionName", regionName}
            };

        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>()?.Value;

        Assert.NotNull(sessionOptions);
        Assert.Equal(awsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(awsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(awsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(regionName, sessionOptions.RegionName);
    }

    [Fact]
    public void AddLocalStack_Should_Configure_ConfigOptions_If_There_Is_Not_Session_Section()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        ConfigOptions configOptions = provider.GetRequiredService<IOptions<ConfigOptions>>()?.Value;

        Assert.NotNull(configOptions);
        Assert.True(new ConfigOptions().DeepEquals(configOptions));
    }

    [Fact]
    public void AddLocalStack_Should_Configure_ConfigOptions_By_Session_Section()
    {
        const string localStackHost = "myhost";
        const bool useSsl = true;
        const bool useLegacyPorts = true;
        const int edgePort = 1245;

        var configurationValue = new Dictionary<string, string>
            {
                {"LocalStack:Config:LocalStackHost", localStackHost},
                {"LocalStack:Config:UseSsl", useSsl.ToString()},
                {"LocalStack:Config:UseLegacyPorts", useLegacyPorts.ToString()},
                {"LocalStack:Config:EdgePort", edgePort.ToString()}
            };

        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        ConfigOptions configOptions = provider.GetRequiredService<IOptions<ConfigOptions>>()?.Value;

        Assert.NotNull(configOptions);
        Assert.Equal(localStackHost, configOptions.LocalStackHost);
        Assert.Equal(useSsl, configOptions.UseSsl);
        Assert.Equal(useLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(edgePort, configOptions.EdgePort);
    }

    [Fact]
    public void AddLocalStackServices_Should_Add_IConfig_To_Container_As_Config()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var config = provider.GetRequiredService<IConfig>();

        Assert.NotNull(config);
        Assert.IsType<Config>(config);
    }

    [Fact]
    public void AddLocalStackServices_Should_Add_ISession_To_Container_As_Session()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var session = provider.GetRequiredService<ISession>();

        Assert.NotNull(session);
        Assert.IsType<Session>(session);
    }

    [Fact]
    public void AddLocalStackServices_Should_Add_IAwsClientFactoryWrapper_To_Container_As_AwsClientFactoryWrapper()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var awsClientFactoryWrapper = provider.GetRequiredService<IAwsClientFactoryWrapper>();
        Type factoryType = typeof(IAwsClientFactoryWrapper).Assembly.GetType("LocalStack.Client.Extensions.AwsClientFactoryWrapper");

        Assert.NotNull(factoryType);
        Assert.NotNull(awsClientFactoryWrapper);
        Assert.IsType(factoryType, awsClientFactoryWrapper);
    }

    [Fact]
    public void AddLocalStackServices_Should_Add_ISessionReflection_To_Container_As_SessionReflection()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration);

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var sessionReflection = provider.GetRequiredService<ISessionReflection>();

        Assert.NotNull(sessionReflection);
        Assert.IsType<SessionReflection>(sessionReflection);
    }

    [Theory,
     InlineData(true, "eu-central-1"),
     InlineData(true, "us-west-1"),
     InlineData(true, "af-south-1"),
     InlineData(true, "ap-southeast-1"),
     InlineData(true, "ca-central-1"),
     InlineData(true, "eu-west-2"),
     InlineData(true, "sa-east-1"),
     InlineData(false, "eu-central-1"),
     InlineData(false, "us-west-1"),
     InlineData(false, "af-south-1"),
     InlineData(false, "ap-southeast-1"),
     InlineData(false, "ca-central-1"),
     InlineData(false, "eu-west-2"),
     InlineData(false, "sa-east-1")]
    public void GetRequiredService_Should_Return_AmazonService_That_Configured_For_LocalStack_If_UseLocalStack_Is_True(bool useAlternateNameAddServiceMethod, string systemName)
    {
        var configurationValue = new Dictionary<string, string> { { "LocalStack:UseLocalStack", "true" }, { "LocalStack:Session:RegionName", systemName } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();

        var mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var configOptions = new ConfigOptions();

        var mockConfig = new Mock<IConfig>(MockBehavior.Strict);
        IConfig mockConfigObject = mockConfig.Object;

        mockConfig.Setup(config => config.GetAwsServiceEndpoint(It.Is<string>(s => s == mockServiceMetadata.ServiceId))).Returns(() => mockAwsServiceEndpoint);
        mockConfig.Setup(config => config.GetConfigOptions()).Returns(() => configOptions);

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection = serviceCollection
                            .AddLocalStack(configuration)
                            .Replace(ServiceDescriptor.Singleton(_ => mockConfigObject));

        if (!useAlternateNameAddServiceMethod)
        {
            serviceCollection.AddAwsService<IMockAmazonServiceWithServiceMetadata>();
        }
        else
        {
            serviceCollection.AddAWSServiceLocalStack<IMockAmazonServiceWithServiceMetadata>();
        }

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var mockAmazonService = provider.GetRequiredService<IMockAmazonServiceWithServiceMetadata>();

        IClientConfig clientConfig = mockAmazonService.Config;

        Assert.Equal(configOptions.UseSsl, !clientConfig.UseHttp);
        Assert.Equal(mockAwsServiceEndpoint.Host, clientConfig.ProxyHost);
        Assert.Equal(mockAwsServiceEndpoint.Port, clientConfig.ProxyPort);
        Assert.Equal(RegionEndpoint.GetBySystemName(systemName), clientConfig.RegionEndpoint);
    }

    [Theory,
     InlineData(false, false),
     InlineData(false, true),
     InlineData(true, true),
     InlineData(true, false)]
    public void GetRequiredService_Should_Return_AmazonService_That_Configured_For_LocalStack_If_UseLocalStack_Is_True_And_Should_Configure_ServiceUrl_Or_RegionEndpoint_By_Given_UseServiceUrl_Parameter(bool useAlternateNameAddServiceMethod, bool useServiceUrl)
    {
        var configurationValue = new Dictionary<string, string> { { "LocalStack:UseLocalStack", "true" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();

        var mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var configOptions = new ConfigOptions();

        var mockConfig = new Mock<IConfig>(MockBehavior.Strict);
        IConfig mockConfigObject = mockConfig.Object;

        mockConfig.Setup(config => config.GetAwsServiceEndpoint(It.Is<string>(s => s == mockServiceMetadata.ServiceId))).Returns(() => mockAwsServiceEndpoint);
        mockConfig.Setup(config => config.GetConfigOptions()).Returns(() => configOptions);

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection = serviceCollection
                            .AddLocalStack(configuration)
                            .Replace(ServiceDescriptor.Singleton(_ => mockConfigObject));

        if (!useAlternateNameAddServiceMethod)
        {
            serviceCollection.AddAwsService<IMockAmazonServiceWithServiceMetadata>(useServiceUrl: useServiceUrl);
        }
        else
        {
            serviceCollection.AddAWSServiceLocalStack<IMockAmazonServiceWithServiceMetadata>(useServiceUrl: useServiceUrl);
        }

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var mockAmazonService = provider.GetRequiredService<IMockAmazonServiceWithServiceMetadata>();

        IClientConfig clientConfig = mockAmazonService.Config;

        Assert.Equal(configOptions.UseSsl, !clientConfig.UseHttp);
        Assert.Equal(mockAwsServiceEndpoint.Host, clientConfig.ProxyHost);
        Assert.Equal(mockAwsServiceEndpoint.Port, clientConfig.ProxyPort);

        if (useServiceUrl)
        {
            Assert.Null(clientConfig.RegionEndpoint);
            Assert.NotNull(clientConfig.ServiceURL);
            Assert.Equal(mockAwsServiceEndpoint.ServiceUrl, clientConfig.ServiceURL);
        }
        else
        {
            Assert.Null(clientConfig.ServiceURL);
            Assert.NotNull(clientConfig.RegionEndpoint);
            Assert.Equal(RegionEndpoint.GetBySystemName(Constants.RegionName), clientConfig.RegionEndpoint);
        }
    }

    [Theory, 
     InlineData(false, false, false),
     InlineData(false, false, true), 
     InlineData(true, false, false),
     InlineData(true, false, true), 
     InlineData(false, true, false),
     InlineData(false, true, true),
     InlineData(true, true, false),
     InlineData(true, true,true)]
    public void GetRequiredService_Should_Use_Suitable_ClientFactory_To_Create_AwsService_By_UseLocalStack_Value(bool useLocalStack, bool useAlternateNameAddServiceMethod, bool useServiceUrl)
    {
        int sessionInvolved = useLocalStack ? 1 : 0;
        int awsClientFactoryInvolved = useLocalStack ? 0 : 1;

        var configurationValue = new Dictionary<string, string> { { "LocalStack:UseLocalStack", useLocalStack.ToString() } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValue).Build();

        var mockSession = new Mock<ISession>(MockBehavior.Strict);
        var mockClientFactory = new Mock<IAwsClientFactoryWrapper>(MockBehavior.Strict);

        ISession mockSessionObject = mockSession.Object;
        IAwsClientFactoryWrapper mockAwsClientFactoryWrapper = mockClientFactory.Object;

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddLocalStack(configuration)
                         .Replace(ServiceDescriptor.Singleton(_ => mockSessionObject))
                         .Replace(ServiceDescriptor.Singleton(_ => mockAwsClientFactoryWrapper));

        if (!useAlternateNameAddServiceMethod)
        {
            serviceCollection.AddAwsService<IMockAmazonService>(useServiceUrl:useServiceUrl);
        }
        else
        {
            serviceCollection.AddAWSServiceLocalStack<IMockAmazonService>(useServiceUrl:useServiceUrl);
        }

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        mockSession.Setup(session => session.CreateClientByInterface<IMockAmazonService>(It.IsAny<bool>())).Returns(() => new MockAmazonServiceClient("tsada", "sadasdas", "sadasda", new MockClientConfig()));
        mockClientFactory.Setup(wrapper => wrapper.CreateServiceClient<IMockAmazonService>(It.IsAny<IServiceProvider>(), It.IsAny<AWSOptions>())).Returns(() => new MockAmazonServiceClient("tsada", "sadasdas", "sadasda", new MockClientConfig()));

        var mockAmazonService = provider.GetRequiredService<IMockAmazonService>();

        Assert.NotNull(mockAmazonService);

        mockClientFactory.Verify(wrapper => wrapper.CreateServiceClient<IMockAmazonService>(It.IsAny<IServiceProvider>(), It.IsAny<AWSOptions>()), Times.Exactly(awsClientFactoryInvolved));
        mockSession.Verify(session => session.CreateClientByInterface<IMockAmazonService>(It.Is<bool>(b => b == useServiceUrl)), Times.Exactly(sessionInvolved));
    }
}
