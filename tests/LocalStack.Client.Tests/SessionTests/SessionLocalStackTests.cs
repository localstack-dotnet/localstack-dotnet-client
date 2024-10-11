namespace LocalStack.Client.Tests.SessionTests;

public class SessionLocalStackTests
{
    [Fact]
    public void CreateClientByImplementation_Should_Throw_NotSupportedClientException_If_Given_ServiceId_Is_Not_Supported()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();

        mockSession.SessionOptionsMock.SetupDefault();
        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => null);

        Assert.Throws<NotSupportedClientException>(() => mockSession.CreateClientByImplementation<MockAmazonServiceClient>());

        mockSession.ConfigMock.Verify(config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId => serviceId == mockServiceMetadata.ServiceId)), Times.Once);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Throw_MisconfiguredClientException_If_Given_RegionName_Property_Of_SessionOptions_IsNullOrEmpty_And_Given_UseServiceUrl_Is_False()
    {
        var mockSession = MockSession.Create();

        mockSession.SessionOptionsMock.SetupGet(options => options.RegionName).Returns((string)null!);
        Assert.Throws<MisconfiguredClientException>(() => mockSession.CreateClientByImplementation<MockAmazonServiceClient>(false));

        mockSession.SessionOptionsMock.SetupGet(options => options.RegionName).Returns(string.Empty);
        Assert.Throws<MisconfiguredClientException>(() => mockSession.CreateClientByImplementation<MockAmazonServiceClient>(false));
    }

    [Fact]
    public void CreateClientByImplementation_Should_Create_SessionAWSCredentials_With_AwsAccessKeyId_And_AwsAccessKey_And_AwsSessionToken()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        (string awsAccessKeyId, string awsAccessKey, string awsSessionToken, _) = mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>();
        Assert.NotNull(mockAmazonServiceClient);

        AWSCredentials awsCredentials = mockAmazonServiceClient.AwsCredentials;
        Assert.NotNull(awsCredentials);
        Assert.IsType<SessionAWSCredentials>(awsCredentials);

        var sessionAwsCredentials = (SessionAWSCredentials)awsCredentials;
        ImmutableCredentials immutableCredentials = sessionAwsCredentials.GetCredentials();

        Assert.Equal(awsAccessKeyId, immutableCredentials.AccessKey);
        Assert.Equal(awsAccessKey, immutableCredentials.SecretKey);
        Assert.Equal(awsSessionToken, immutableCredentials.Token);
    }

    [Theory,
     InlineData(true),
     InlineData(false)]
    public void CreateClientByImplementation_Should_Create_ClientConfig_With_UseHttp_Set_Bey_ConfigOptions_UseSsl(bool useSsl)
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions(useSsl: useSsl));

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>();
        Assert.NotNull(mockAmazonServiceClient);

        IClientConfig clientConfig = mockAmazonServiceClient.Config;

        Assert.Equal(useSsl, !clientConfig.UseHttp);

        mockSession.ConfigMock.Verify(config => config.GetConfigOptions(), Times.Once);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Create_ClientConfig_With_UseHttp_And_ProxyHost_And_ProxyPort_By_ServiceEndpoint_Configuration()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();
        var configOptions = new ConfigOptions();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => configOptions);

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>();
        Assert.NotNull(mockAmazonServiceClient);

        IClientConfig clientConfig = mockAmazonServiceClient.Config;

        Assert.Equal(configOptions.UseSsl, !clientConfig.UseHttp);
        Assert.Equal(mockAwsServiceEndpoint.Host, clientConfig.ProxyHost);
        Assert.Equal(mockAwsServiceEndpoint.Port, clientConfig.ProxyPort);
    }

    [Theory,
     InlineData("eu-central-1"),
     InlineData("us-west-1"),
     InlineData("af-south-1"),
     InlineData("ap-southeast-1"),
     InlineData("ca-central-1"),
     InlineData("eu-west-2"),
     InlineData("sa-east-1")]
    public void CreateClientByImplementation_Should_Set_RegionEndpoint_By_RegionName_Property_Of_SessionOptions_And_ServiceUrl_To_Null_If_RegionName_IsNotNull_Or_Empty(string systemName)
    {
        var mockSession = MockSession.Create();

        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        (_, _, _, string regionName) = mockSession.SessionOptionsMock.SetupDefault(regionName: systemName);

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>();

        Assert.NotNull(mockAmazonServiceClient);
        Assert.Null(mockAmazonServiceClient.Config.ServiceURL);
        Assert.Equal(RegionEndpoint.GetBySystemName(regionName), mockAmazonServiceClient.Config.RegionEndpoint);
    }

    [Theory,
     InlineData("sa-east-1"),
     InlineData(null)]
    public void CreateClientByImplementation_Should_Set_ServiceUrl_By_ServiceEndpoint_Configuration_And_RegionEndpoint_To_Null_If_Given_UseServiceUrl_Parameter_Is_True_Regardless_Of_Use_RegionName_Property_Of_SessionOptions_Has_Value_Or_Not(string? systemName)
    {
        var mockSession = MockSession.Create();

        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

#pragma warning disable CS8604 // Possible null reference argument.
        mockSession.SessionOptionsMock.SetupDefault(regionName: systemName);
#pragma warning restore CS8604 // Possible null reference argument.

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>(useServiceUrl: true);

        Assert.NotNull(mockAmazonServiceClient);
        Assert.Null(mockAmazonServiceClient.Config.RegionEndpoint);
        Assert.Equal(mockAwsServiceEndpoint.ServiceUrl.AbsoluteUri, mockAmazonServiceClient.Config.ServiceURL);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Pass_The_ClientConfig_To_SetForcePathStyle()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        mockSession.CreateClientByImplementation<MockAmazonServiceClient>();

        mockSession.SessionReflectionMock.Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
    }

    [Theory,
     InlineData(false),
     InlineData(true)]
    public void CreateClientByImplementation_Should_Create_AmazonServiceClient_By_Given_Generic_Type_And_Configure_ServiceUrl_Or_RegionEndpoint_By_Given_UseServiceUrl_Parameter(bool useServiceUrl)
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        (_, _, _, string regionName) = mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByImplementation<MockAmazonServiceClient>(useServiceUrl);

        Assert.NotNull(mockAmazonServiceClient);

        if (useServiceUrl)
        {
            Assert.Null(mockAmazonServiceClient.Config.RegionEndpoint);
            Assert.NotNull(mockAmazonServiceClient.Config.ServiceURL);
            Assert.Equal(mockAwsServiceEndpoint.ServiceUrl.AbsoluteUri, mockAmazonServiceClient.Config.ServiceURL);
        }
        else
        {
            Assert.Null(mockAmazonServiceClient.Config.ServiceURL);
            Assert.NotNull(mockAmazonServiceClient.Config.RegionEndpoint);
            Assert.Equal(RegionEndpoint.GetBySystemName(regionName), mockAmazonServiceClient.Config.RegionEndpoint);
        }

        mockSession.ConfigMock.Verify(config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId => serviceId == mockServiceMetadata.ServiceId)), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient))), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient))), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
    }

    [Fact]
    public void CreateClientByInterface_Should_Throw_AmazonClientException_If_Given_Generic_AmazonService_Could_Not_Found_In_Aws_Extension_Assembly()
    {
        var mockSession = MockSession.Create();

        Assert.Throws<AmazonClientException>(() => mockSession.CreateClientByInterface(typeof(MockAmazonServiceClient)));
    }

    [Fact]
    public void CreateClientByInterface_Should_Throw_NotSupportedClientException_If_Given_ServiceId_Is_Not_Supported()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();

        mockSession.SessionOptionsMock.SetupDefault();
        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => null);

        Assert.Throws<NotSupportedClientException>(() => mockSession.CreateClientByInterface<IMockAmazonService>());

        mockSession.ConfigMock.Verify(config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId => serviceId == mockServiceMetadata.ServiceId)), Times.Once);
    }

    [Fact]
    public void CreateClientByInterface_Should_Throw_MisconfiguredClientException_If_Given_RegionName_Property_Of_SessionOptions_IsNullOrEmpty_And_Given_UseServiceUrl_Is_False()
    {
        var mockSession = MockSession.Create();

        mockSession.SessionOptionsMock.SetupGet(options => options.RegionName).Returns(default(string)!);
        Assert.Throws<MisconfiguredClientException>(() => mockSession.CreateClientByInterface<IMockAmazonService>(false));

        mockSession.SessionOptionsMock.SetupGet(options => options.RegionName).Returns(string.Empty);
        Assert.Throws<MisconfiguredClientException>(() => mockSession.CreateClientByInterface<IMockAmazonService>(false));
    }

    [Theory,
     InlineData(true),
     InlineData(false)]
    public void CreateClientByInterface_Should_Create_ClientConfig_With_UseHttp_Set_Bey_ConfigOptions_UseSsl(bool useSsl)
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions(useSsl: useSsl));

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>() as MockAmazonServiceClient;
        Assert.NotNull(mockAmazonServiceClient);

        IClientConfig clientConfig = mockAmazonServiceClient.Config;

        Assert.Equal(useSsl, !clientConfig.UseHttp);

        mockSession.ConfigMock.Verify(config => config.GetConfigOptions(), Times.Once);
    }

    [Fact]
    public void CreateClientByInterface_Should_Create_SessionAWSCredentials_With_AwsAccessKeyId_And_AwsAccessKey_And_AwsSessionToken()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        (string awsAccessKeyId, string awsAccessKey, string awsSessionToken, _) = mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>() as MockAmazonServiceClient;
        Assert.NotNull(mockAmazonServiceClient);

        AWSCredentials awsCredentials = mockAmazonServiceClient.AwsCredentials;
        Assert.NotNull(awsCredentials);
        Assert.IsType<SessionAWSCredentials>(awsCredentials);

        var sessionAwsCredentials = (SessionAWSCredentials)awsCredentials;
        ImmutableCredentials immutableCredentials = sessionAwsCredentials.GetCredentials();

        Assert.Equal(awsAccessKeyId, immutableCredentials.AccessKey);
        Assert.Equal(awsAccessKey, immutableCredentials.SecretKey);
        Assert.Equal(awsSessionToken, immutableCredentials.Token);
    }

    [Fact]
    public void CreateClientByInterface_Should_Create_ClientConfig_With_UseHttp_And_ProxyHost_And_ProxyPort_By_ServiceEndpoint_Configuration()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();
        var configOptions = new ConfigOptions();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => configOptions);

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>() as MockAmazonServiceClient;
        Assert.NotNull(mockAmazonServiceClient);

        IClientConfig clientConfig = mockAmazonServiceClient.Config;

        Assert.Equal(configOptions.UseSsl, !clientConfig.UseHttp);
        Assert.Equal(mockAwsServiceEndpoint.Host, clientConfig.ProxyHost);
        Assert.Equal(mockAwsServiceEndpoint.Port, clientConfig.ProxyPort);

        mockSession.ConfigMock.Verify(config => config.GetConfigOptions(), Times.Once);
    }

    [Theory,
     InlineData("eu-central-1"),
     InlineData("us-west-1"),
     InlineData("af-south-1"),
     InlineData("ap-southeast-1"),
     InlineData("ca-central-1"),
     InlineData("eu-west-2"),
     InlineData("sa-east-1")]
    public void CreateClientByInterface_Should_Set_RegionEndpoint_By_RegionName_Property_Of_SessionOptions_And_ServiceUrl_To_Null_If_RegionName_IsNotNull_Or_Empty(string systemName)
    {
        var mockSession = MockSession.Create();

        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        (_, _, _, string regionName) = mockSession.SessionOptionsMock.SetupDefault(regionName: systemName);

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>() as MockAmazonServiceClient;

        Assert.NotNull(mockAmazonServiceClient);
        Assert.Null(mockAmazonServiceClient.Config.ServiceURL);
        Assert.Equal(RegionEndpoint.GetBySystemName(regionName), mockAmazonServiceClient.Config.RegionEndpoint);
    }


    [Theory,
     InlineData("sa-east-1"),
     InlineData(null)]
    public void CreateClientByInterface_Should_Set_ServiceUrl_By_ServiceEndpoint_Configuration_And_RegionEndpoint_To_Null_If_Given_UseServiceUrl_Parameter_Is_True_Regardless_Of_Use_RegionName_Property_Of_SessionOptions_Has_Value_Or_Not(string? systemName)
    {
        var mockSession = MockSession.Create();

        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

#pragma warning disable CS8604 // Possible null reference argument.
        mockSession.SessionOptionsMock.SetupDefault(regionName: systemName);
#pragma warning restore CS8604 // Possible null reference argument.

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>(useServiceUrl: true) as MockAmazonServiceClient;

        Assert.NotNull(mockAmazonServiceClient);
        Assert.Null(mockAmazonServiceClient.Config.RegionEndpoint);
        Assert.Equal(mockAwsServiceEndpoint.ServiceUrl.AbsoluteUri, mockAmazonServiceClient.Config.ServiceURL);
    }

    [Fact]
    public void CreateClientByInterface_Should_Pass_The_ClientConfig_To_SetForcePathStyle()
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();

        mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => new ConfigOptions());

        mockSession.CreateClientByInterface<IMockAmazonService>();

        mockSession.SessionReflectionMock.Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
        mockSession.ConfigMock.Verify(config => config.GetConfigOptions(), Times.Once);
    }

    [Theory,
     InlineData(false),
     InlineData(true)]
    public void CreateClientByInterface_Should_Create_AmazonServiceClient_By_Given_Generic_Type_And_Configure_ServiceUrl_Or_RegionEndpoint_By_Given_UseServiceUrl_Parameter(bool useServiceUrl)
    {
        var mockSession = MockSession.Create();
        IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
        var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
        var mockClientConfig = new MockClientConfig();
        var configOptions = new ConfigOptions();

        (_, _, _, string regionName) = mockSession.SessionOptionsMock.SetupDefault();

        mockSession.SessionReflectionMock.Setup(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockServiceMetadata);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient)))).Returns(() => mockClientConfig);
        mockSession.SessionReflectionMock.Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true)).Returns(() => true);

        mockSession.ConfigMock.Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>())).Returns(() => mockAwsServiceEndpoint);
        mockSession.ConfigMock.Setup(config => config.GetConfigOptions()).Returns(() => configOptions);

        var mockAmazonServiceClient = mockSession.CreateClientByInterface<IMockAmazonService>(useServiceUrl) as MockAmazonServiceClient;

        Assert.NotNull(mockAmazonServiceClient);

        if (useServiceUrl)
        {
            Assert.Null(mockAmazonServiceClient.Config.RegionEndpoint);
            Assert.NotNull(mockAmazonServiceClient.Config.ServiceURL);
            Assert.Equal(mockAwsServiceEndpoint.ServiceUrl.AbsoluteUri, mockAmazonServiceClient.Config.ServiceURL);
        }
        else
        {
            Assert.Null(mockAmazonServiceClient.Config.ServiceURL);
            Assert.NotNull(mockAmazonServiceClient.Config.RegionEndpoint);
            Assert.Equal(RegionEndpoint.GetBySystemName(regionName), mockAmazonServiceClient.Config.RegionEndpoint);
        }

        mockSession.ConfigMock.Verify(config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId => serviceId == mockServiceMetadata.ServiceId)), Times.Once);
        mockSession.ConfigMock.Verify(config => config.GetConfigOptions(), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.ExtractServiceMetadata(It.Is<Type>(type => type == typeof(MockAmazonServiceClient))), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.CreateClientConfig(It.Is<Type>(type => type == typeof(MockAmazonServiceClient))), Times.Once);
        mockSession.SessionReflectionMock.Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
    }
}