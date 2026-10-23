namespace LocalStack.Client.Tests.SessionTests;

public class SessionReflectionTests
{
#if NET10_0_OR_GREATER
    [Fact]
    public void SessionReflection_Should_Return_Private_Field_Metadata_And_Apply_Config_Effects_For_Registered_Clients()
    {
        Type clientType = typeof(MetadataClient);
        Assert.True(GeneratedClientRegistry.TryGetClient(clientType, out GeneratedClientRegistry.ClientEntry? entry));
        Assert.Same(MetadataClient.MetadataValue, entry.Metadata());
        var reflection = new SessionReflection();
        Assert.Same(MetadataClient.MetadataValue, reflection.ExtractServiceMetadata<MetadataClient>());
        Assert.Same(MetadataClient.MetadataValue, reflection.ExtractServiceMetadata(clientType));
        var config = Assert.IsType<StyleConfig>(reflection.CreateClientConfig<MetadataClient>());
        Assert.IsType<StyleConfig>(reflection.CreateClientConfig(clientType));
        Assert.True(GeneratedClientRegistry.TryGetConfig(typeof(StyleConfig), out GeneratedClientRegistry.ConfigEntry? configEntry));
        Assert.NotNull(configEntry.Style);
        Assert.True(configEntry.PlainRegion);
        Assert.False(config.ForcePathStyle);
        Assert.True(reflection.SetForcePathStyle(config));
        Assert.True(config.ForcePathStyle);
        var credentials = new BasicAWSCredentials("fixture-id", "fixture-secret");
        using AmazonServiceClient client = Assert.IsType<MetadataClient>(entry.Create(credentials, config));
        Assert.Same(config, client.Config);
        reflection.SetClientRegion(client, "eu-west-2");
        Assert.Equal("eu-west-2", config.RegionEndpoint.SystemName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateClientByInterface_Should_Keep_Injected_Config_Identity_And_Error_Order_With_Generated_Operations(bool byInterface)
    {
        Assert.True(GeneratedClientRegistry.TryGetClient(typeof(MockAmazonServiceClient), out GeneratedClientRegistry.ClientEntry? entry));
        Assert.NotNull(entry.CreateConfig);
        var config = new StyleConfig();
        var calls = new List<string>();
        var metadata = new Mock<IServiceMetadata>();
        metadata.SetupGet(value => value.ServiceId).Returns("SQS");
        var reflection = new Mock<Contracts.ISessionReflection>(MockBehavior.Strict);
        Type expectedType = typeof(MockAmazonServiceClient);
        reflection.Setup(helper => helper.ExtractServiceMetadata(expectedType))
            .Returns(() =>
            {
                calls.Add("metadata");
                return metadata.Object;
            });
        reflection.Setup(helper => helper.CreateClientConfig(expectedType))
            .Returns(() =>
            {
                calls.Add("config");
                return config;
            });
        reflection.Setup(helper => helper.SetForcePathStyle(config, true))
            .Returns(() =>
            {
                calls.Add("style");
                config.ForcePathStyle = true;
                return true;
            });
        var session = new Session(new SessionOptions(regionName: "eu-west-2"), new Config(new ConfigOptions()), reflection.Object);
        using AmazonServiceClient client = byInterface
            ? session.CreateClientByInterface<IMockAmazonService>()
            : session.CreateClientByImplementation<MockAmazonServiceClient>();
        Assert.IsType<MockAmazonServiceClient>(client);
        Assert.Same(config, client.Config);
        Assert.True(config.ForcePathStyle);
        Assert.Equal("eu-west-2", client.Config.RegionEndpoint.SystemName);
        Assert.Collection(calls, call => Assert.Equal("metadata", call), call => Assert.Equal("config", call), call => Assert.Equal("style", call));
    }

    internal sealed class StyleConfig : MockClientConfig
    {
        public bool ForcePathStyle { get; set; }
    }

    internal sealed class MetadataClient : AmazonServiceClient
    {
        private static readonly IServiceMetadata serviceMetadata = new MockServiceMetadata();
        public static IServiceMetadata MetadataValue => serviceMetadata;
        public MetadataClient(AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }
    }
#endif

    [Fact]
    public void ExtractServiceMetadata_Should_Throw_InvalidOperationException_If_Given_Generic_Service_Client_Type_Has_Not_Service_Metadata_Field()
    {
        var sessionReflection = new SessionReflection();

        Assert.Throws<InvalidOperationException>(() => sessionReflection.ExtractServiceMetadata<MockAmazonServiceClient>());
    }

    [Fact]
    public void ExtractServiceMetadata_Should_Return_Extracted_ServiceMetadata()
    {
        var sessionReflection = new SessionReflection();

        IServiceMetadata serviceMetadata = sessionReflection.ExtractServiceMetadata<MockAmazonServiceWithServiceMetadataClient>();

        Assert.NotNull(serviceMetadata);
        Assert.Equal(MockServiceMetadata.MockServiceId, serviceMetadata.ServiceId);
    }

    [Fact]
    public void CreateClientConfig_Should_Create_ClientConfig_By_Given_Generic_Service_Client_Type()
    {
        var sessionReflection = new SessionReflection();

        ClientConfig clientConfig = sessionReflection.CreateClientConfig<MockAmazonServiceClient>();

        Assert.NotNull(clientConfig);
        Assert.IsType<MockClientConfig>(clientConfig);
    }

    [Fact]
    public void SetForcePathStyle_Should_Return_False_If_Given_ClientConfig_Does_Not_Have_ForcePathStyle()
    {
        var sessionReflection = new SessionReflection();
        var mockClientConfig = MockClientConfig.CreateDefaultMockClientConfig();

        bool set = sessionReflection.SetForcePathStyle(mockClientConfig, true);

        Assert.False(set);
    }

    [Fact]
    public void SetForcePathStyle_Should_Set_ForcePathStyle_Of_ClientConfig_If_It_Exists()
    {
        var sessionReflection = new SessionReflection();
        var clientConfig = MockClientConfigWithForcePathStyle.CreateDefaultMockClientConfigWithForcePathStyle();

        Assert.False(clientConfig.ForcePathStyle);

        bool set = sessionReflection.SetForcePathStyle(clientConfig, true);

        Assert.True(set);
        Assert.True(clientConfig.ForcePathStyle);
    }

    [Theory,
     InlineData("eu-central-1"),
     InlineData("us-west-1"),
     InlineData("af-south-1"),
     InlineData("ap-southeast-1"),
     InlineData("ca-central-1"),
     InlineData("eu-west-2"),
     InlineData("sa-east-1")]
    public void SetClientRegion_Should_Set_RegionEndpoint_Of_The_Given_Client_By_System_Name(string systemName)
    {
        var sessionReflection = new SessionReflection();
        using var mockAmazonServiceClient = new MockAmazonServiceClient();

        Assert.Null(mockAmazonServiceClient.Config.RegionEndpoint);

        sessionReflection.SetClientRegion(mockAmazonServiceClient, systemName);

        Assert.NotNull(mockAmazonServiceClient.Config.RegionEndpoint);
        Assert.Equal(RegionEndpoint.GetBySystemName(systemName), mockAmazonServiceClient.Config.RegionEndpoint);
    }
}
