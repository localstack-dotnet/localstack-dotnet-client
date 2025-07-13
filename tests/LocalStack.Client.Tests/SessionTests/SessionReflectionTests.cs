namespace LocalStack.Client.Tests.SessionTests;

public class SessionReflectionTests
{
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