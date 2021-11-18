namespace LocalStack.Client.Tests.ConfigTests;

public class ConfigTests
{
    [Fact]
    public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Host_Property_Equals_Set_LocalStackHost_Property_Of_ConfigOptions()
    {
        const string localStackHost = "myLocalHost";

        var config = new Config(new ConfigOptions(localStackHost));
        AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(localStackHost, awsServiceEndpoint.Host);

        awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(localStackHost, awsServiceEndpoint.Host);
    }

    [Fact]
    public void GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Host_Property_Of_Every_Item_Equals_By_Set_LocalStackHost_Property_Of_ConfigOptions()
    {
        const string localStackHost = "myLocalHost";

        var config = new Config(new ConfigOptions(localStackHost));
        IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

        Assert.NotNull(awsServiceEndpoints);
        Assert.NotEmpty(awsServiceEndpoints);
        Assert.All(awsServiceEndpoints, endpoint => Assert.Equal(localStackHost, endpoint.Host));
    }

    [Theory, InlineData(true, "https:"), InlineData(false, "http:")]
    public void
        GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Protocol_Property_Equals_To_Https_Or_Http_If_Set_UseSsl_Property_Of_ConfigOptions_Equals_True_Or_False(
            bool useSsl, string expectedProtocol)
    {
        var config = new Config(new ConfigOptions(useSsl: useSsl));
        AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

        Assert.NotNull(awsServiceEndpoint);
        Assert.StartsWith(expectedProtocol, awsServiceEndpoint.ServiceUrl);

        awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

        Assert.NotNull(awsServiceEndpoint);
        Assert.StartsWith(expectedProtocol, awsServiceEndpoint.ServiceUrl);
    }

    [Theory, InlineData(true, "https:"), InlineData(false, "http:")]
    public void
        GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Protocol_Property_Of_Every_Item_Equals_To_Https_Or_Http_If_Set_UseSsl_Property_Of_ConfigOptions_Equals_True_Or_False(
            bool useSsl, string expectedProtocol)
    {
        var config = new Config(new ConfigOptions(useSsl: useSsl));
        IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

        Assert.NotNull(awsServiceEndpoints);
        Assert.NotEmpty(awsServiceEndpoints);
        Assert.All(awsServiceEndpoints, endpoint => Assert.StartsWith(expectedProtocol, endpoint.ServiceUrl));
    }

    [Fact]
    public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Port_Property_Equals_To_Set_EdgePort_Property_Of_ConfigOptions_If_UseLegacyPorts_Property_Is_False()
    {
        const int edgePort = 1234;

        var config = new Config(new ConfigOptions(useLegacyPorts: false, edgePort: edgePort));
        AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(edgePort, awsServiceEndpoint.Port);

        awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(edgePort, awsServiceEndpoint.Port);
    }

    [Theory,
     InlineData(true, false, "localhost", 1111, "http://localhost"),
     InlineData(true, true, "localhost", 1111, "https://localhost"),
     InlineData(false, false, "localhost", 1111, "http://localhost:1111"),
     InlineData(false, true, "localhost", 1111, "https://localhost:1111"),
     InlineData(true, false, "myHost", 2222, "http://myHost"),
     InlineData(true, true, "myHost2", 3334, "https://myHost2"),
     InlineData(false, false, "myHost3", 4432, "http://myHost3:4432"),
     InlineData(false, true, "myHost4", 2124, "https://myHost4:2124")]
    public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_ServiceUrl_Property_Equals_To_Combination_Of_Host_Protocol_And_Port(bool useLegacyPorts, bool useSsl, string localStackHost, int edgePort, string expectedServiceUrl)
    {
        var config = new Config(new ConfigOptions(localStackHost, useLegacyPorts: useLegacyPorts, edgePort: edgePort, useSsl: useSsl));
        AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGatewayV2);

        if (useLegacyPorts)
        {
            expectedServiceUrl = $"{expectedServiceUrl}:{awsServiceEndpoint.Port}";
        }

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(expectedServiceUrl, awsServiceEndpoint.ServiceUrl);

        awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

        Assert.NotNull(awsServiceEndpoint);
        Assert.Equal(expectedServiceUrl, awsServiceEndpoint.ServiceUrl);
    }

    [Theory,
     InlineData(true, false, "localhost", 1111, "http://localhost"),
     InlineData(true, true, "localhost", 1111, "https://localhost"),
     InlineData(false, false, "localhost", 1111, "http://localhost:1111"),
     InlineData(false, true, "localhost", 1111, "https://localhost:1111"),
     InlineData(true, false, "myHost", 2222, "http://myHost"),
     InlineData(true, true, "myHost2", 3334, "https://myHost2"),
     InlineData(false, false, "myHost3", 4432, "http://myHost3:4432"),
     InlineData(false, true, "myHost4", 2124, "https://myHost4:2124")]
    public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_ServiceUrl_Property_Property_Of_Every_Item_Equals_To_Combination_Of_Host_Protocol_And_Port(bool useLegacyPorts, bool useSsl, string localStackHost, int edgePort, string expectedServiceUrl)
    {
        var config = new Config(new ConfigOptions(localStackHost, useLegacyPorts: useLegacyPorts, edgePort: edgePort, useSsl: useSsl));

        IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

        Assert.NotNull(awsServiceEndpoints);
        Assert.NotEmpty(awsServiceEndpoints);
        Assert.All(awsServiceEndpoints, endpoint =>
        {
            string serviceUrl = useLegacyPorts ? $"{expectedServiceUrl}:{endpoint.Port}" : expectedServiceUrl;

            Assert.Equal(serviceUrl, endpoint.ServiceUrl);
        });
    }

    [Fact]
    public void GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Port_Property_Of_Every_Item_Equals_To_Set_EdgePort_Property_Of_ConfigOptions_If_UseLegacyPorts_Property_Is_False()
    {
        const int edgePort = 1234;

        var config = new Config(new ConfigOptions(useLegacyPorts: false, edgePort: edgePort));

        IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

        Assert.NotNull(awsServiceEndpoints);
        Assert.NotEmpty(awsServiceEndpoints);
        Assert.All(awsServiceEndpoints, endpoint => Assert.Equal(edgePort, endpoint.Port));
    }

    [Fact]
    public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Port_Property_Not_Equals_To_Default_Edge_Port_If_UseLegacyPorts_Property_Is_True()
    {
        var config = new Config(new ConfigOptions(useLegacyPorts: true));
        AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

        Assert.NotNull(awsServiceEndpoint);
        Assert.NotEqual(Constants.EdgePort, awsServiceEndpoint.Port);

        awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

        Assert.NotNull(awsServiceEndpoint);
        Assert.NotEqual(Constants.EdgePort, awsServiceEndpoint.Port);
    }

    [Fact]
    public void GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Port_Property_Of_Every_Item_Not_Equals_To_Default_Edge_Port_If_UseLegacyPorts_Property_Is_True()
    {
        var config = new Config(new ConfigOptions(useLegacyPorts: true));

        IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

        Assert.NotNull(awsServiceEndpoints);
        Assert.NotEmpty(awsServiceEndpoints);
        Assert.All(awsServiceEndpoints, endpoint => Assert.NotEqual(Constants.EdgePort, endpoint.Port));
    }

    [Fact]
    public void GetAwsServicePort_Should_Return_Integer_Port_Value_That_Equals_To_Port_Property_Of_Related_AwsServiceEndpoint_If_UseLegacyPorts_Property_Is_True()
    {
        var config = new Config(new ConfigOptions(useLegacyPorts: true));

        foreach (AwsServiceEndpointMetadata awsServiceEndpointMetadata in AwsServiceEndpointMetadata.All)
        {
            int awsServicePort = config.GetAwsServicePort(awsServiceEndpointMetadata.Enum);

            Assert.Equal(awsServiceEndpointMetadata.Port, awsServicePort);
        }
    }

    [Fact]
    public void GetAwsServicePort_Should_Return_Integer_Port_Value_That_Equals_To_Set_EdgePort_Property_Of_ConfigOptions_If_UseLegacyPorts_Property_Is_False()
    {
        const int edgePort = 1234;

        var config = new Config(new ConfigOptions(useLegacyPorts: false, edgePort: edgePort));

        foreach (AwsServiceEndpointMetadata awsServiceEndpointMetadata in AwsServiceEndpointMetadata.All)
        {
            int awsServicePort = config.GetAwsServicePort(awsServiceEndpointMetadata.Enum);

            Assert.Equal(edgePort, awsServicePort);
        }
    }

    [Fact]
    public void
        GetAwsServicePorts_Should_Return_AwsServiceEnum_And_Integer_Port_Value_Pair_That_Port_Property_Of_The_Pair_Equals_To_Port_Property_Of_Related_AwsServiceEndpoint_If_UseLegacyPorts_Property_Is_True()
    {
        var config = new Config(new ConfigOptions(useLegacyPorts: true));

        IDictionary<AwsServiceEnum, int> awsServicePorts = config.GetAwsServicePorts();

        foreach (AwsServiceEndpointMetadata awsServiceEndpointMetadata in AwsServiceEndpointMetadata.All)
        {
            KeyValuePair<AwsServiceEnum, int> keyValuePair = awsServicePorts.First(pair => pair.Key == awsServiceEndpointMetadata.Enum);

            Assert.Equal(awsServiceEndpointMetadata.Enum, keyValuePair.Key);
            Assert.Equal(awsServiceEndpointMetadata.Port, keyValuePair.Value);
        }
    }

    [Fact]
    public void
        GetAwsServicePorts_Should_Return_AwsServiceEnum_And_Integer_Port_Value_Pair_That_Port_Property_Of_The_Pair_Equals_To_Set_EdgePort_Property_Of_ConfigOptions_If_UseLegacyPorts_Property_Is_False()
    {
        const int edgePort = 1234;
        var config = new Config(new ConfigOptions(useLegacyPorts: false, edgePort: edgePort));

        IDictionary<AwsServiceEnum, int> awsServicePorts = config.GetAwsServicePorts();

        foreach (AwsServiceEndpointMetadata awsServiceEndpointMetadata in AwsServiceEndpointMetadata.All)
        {
            KeyValuePair<AwsServiceEnum, int> keyValuePair = awsServicePorts.First(pair => pair.Key == awsServiceEndpointMetadata.Enum);

            Assert.Equal(awsServiceEndpointMetadata.Enum, keyValuePair.Key);
            Assert.Equal(edgePort, keyValuePair.Value);
        }
    }
}
