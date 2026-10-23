using LocalStack.Client.Contracts;

namespace LocalStack.Client.Tests.SessionTests;

public class SessionCompatibilityTests
{
    private static readonly string[] ExpectedCalls = { "metadata", "config", "style" };
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateClient_Should_Use_Injected_Reflection_For_Metadata_Config_Style_And_Credentials(bool byInterface)
    {
        var reflection = new RecordingReflection();
        var session = new Session(new SessionOptions("fixture-id", "fixture-secret", "fixture-token", "eu-west-2"),
                                  new Config(new ConfigOptions(localStackHost: "fixture-host", useLegacyPorts: true)), reflection);
        using var client = (MockAmazonServiceClient)(byInterface
            ? session.CreateClientByInterface<IMockAmazonService>()
            : session.CreateClientByImplementation<MockAmazonServiceClient>());

        Assert.Equal(ExpectedCalls, reflection.Calls, StringComparer.Ordinal);
        Assert.Same(reflection.Config, client.Config);
        Assert.Equal(TimeSpan.FromSeconds(42), client.Config.Timeout);
        Assert.Equal(4569, client.Config.ProxyPort);
        Assert.Equal("fixture-host", client.Config.ProxyHost);
        Assert.Equal("eu-west-2", client.Config.RegionEndpoint.SystemName);
        Assert.True(client.Config.UseHttp);
        Assert.False(reflection.Config.ForcePathStyle);
        ImmutableCredentials credentials = client.AwsCredentials.GetCredentials();
        Assert.Equal("fixture-id", credentials.AccessKey);
        Assert.Equal("fixture-secret", credentials.SecretKey);
        Assert.Equal("fixture-token", credentials.Token);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Validate_Region_Before_Calling_Injected_Reflection()
    {
        var reflection = new RecordingReflection();
        var session = new Session(new SessionOptions(regionName: ""), new Config(new ConfigOptions()), reflection);
        Assert.Throws<MisconfiguredClientException>(() => session.CreateClientByImplementation<MockAmazonServiceClient>());
        Assert.Empty(reflection.Calls);
        // Interface convention lookup still happens before region validation.
        Assert.Throws<AmazonClientException>(() => session.CreateClientByInterface(typeof(ISession)));
        Assert.Empty(reflection.Calls);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Rethrow_Injected_Failure_And_Make_No_Later_Calls()
    {
        var reflection = new RecordingReflection { Failure = new InvalidOperationException("fixture metadata failure") };
        var session = new Session(new SessionOptions(), new Config(new ConfigOptions()), reflection);
        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => session.CreateClientByImplementation<MockAmazonServiceClient>());
        Assert.Same(reflection.Failure, error);
        Assert.Equal("metadata", Assert.Single(reflection.Calls));
    }

    [Fact]
    public void SessionReflection_Should_Use_Actual_Properties_Including_Private_Inherited_Setters_And_Shadowing()
    {
        var reflection = new SessionReflection();
        var config = new StyleConfig();
        Assert.True(reflection.SetForcePathStyle(config));
        Assert.True(config.ForcePathStyle);
        Assert.Throws<ArgumentException>(() => reflection.SetForcePathStyle(new InheritedStyleConfig()));
        using var client = new MockAmazonServiceClient(new BasicAWSCredentials("fixture-id", "fixture-secret"), config);
        reflection.SetClientRegion(client, "eu-west-2");
        Assert.Equal("eu-west-2", config.RegionEndpoint?.SystemName);
        Assert.Null(((ClientConfig)config).RegionEndpoint);
    }

    [Fact]
    public void SetForcePathStyle_Should_Keep_Property_Type_Errors_And_Setter_Exception_Wrapping()
    {
        var reflection = new SessionReflection();
        var wrong = new WrongStyleConfig { ForcePathStyle = "initial" };
        Assert.Throws<ArgumentException>(() => reflection.SetForcePathStyle(wrong));
        Assert.Equal("initial", wrong.ForcePathStyle);
        var throwing = new ThrowingStyleConfig();
        Assert.False(throwing.ForcePathStyle);
        Assert.Same(ThrowingStyleConfig.Failure, Assert.Throws<InvalidOperationException>(() => throwing.ForcePathStyle = true));
        TargetInvocationException error = Assert.Throws<TargetInvocationException>(() => reflection.SetForcePathStyle(throwing));
        Assert.Same(ThrowingStyleConfig.Failure, error.InnerException);
    }

    [Fact]
    public void ExtractServiceMetadata_Should_Not_Read_A_Private_Base_Class_Field()
    {
        var reflection = new SessionReflection();
        Assert.Same(MetadataClient.Metadata, reflection.ExtractServiceMetadata<MetadataClient>());
        using var derived = new DerivedMetadataClient();
        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => reflection.ExtractServiceMetadata(derived.GetType()));
        Assert.Equal($"Invalid service type {typeof(DerivedMetadataClient)}", error.Message);
    }

    [Fact]
    public void CreateClientConfig_Should_Match_Constructor_Parameter_Names_And_Fail_On_Zero_Or_Many_Matches()
    {
        var reflection = new SessionReflection();
        var credentials = new BasicAWSCredentials("fixture-id", "fixture-secret");
        using var renamed = new RenamedParametersClient(credentials, new MockClientConfig());
        using var ambiguous = new AmbiguousConfigClient(credentials, new StyleConfig());
        using var other = new AmbiguousConfigClient(credentials, new MockClientConfig());
        Assert.Throws<InvalidOperationException>(() => reflection.CreateClientConfig(renamed.GetType()));
        Assert.Throws<InvalidOperationException>(() => reflection.CreateClientConfig(ambiguous.GetType()));
        Assert.Throws<InvalidOperationException>(() => reflection.CreateClientConfig(other.GetType()));
        Assert.Same(ThrowingConfig.Failure, Assert.Throws<InvalidOperationException>(() => new ThrowingConfigClient(credentials, new ThrowingConfig())));
        TargetInvocationException error = Assert.Throws<TargetInvocationException>(() => reflection.CreateClientConfig<ThrowingConfigClient>());
        Assert.Same(ThrowingConfig.Failure, error.InnerException);
    }

    [Fact]
    public void CreateClientByImplementation_Should_Wrap_Client_Constructor_Exceptions_In_TargetInvocationException()
    {
        var session = new Session(new SessionOptions(), new Config(new ConfigOptions()), new RecordingReflection { ExpectedType = typeof(ThrowingClient) });
        Assert.Same(ThrowingClient.Failure, Assert.Throws<InvalidOperationException>(() => new ThrowingClient(new BasicAWSCredentials("fixture-id", "fixture-secret"), new MockClientConfig())));
        TargetInvocationException error = Assert.Throws<TargetInvocationException>(() => session.CreateClientByImplementation<ThrowingClient>());
        Assert.Same(ThrowingClient.Failure, error.InnerException);
    }

    private sealed class RecordingReflection : ISessionReflection
    {
        public List<string> Calls { get; } = new();
        public StyleConfig Config { get; } = new() { Timeout = TimeSpan.FromSeconds(42) };
        public Exception? Failure { get; set; }
        public Type ExpectedType { get; set; } = typeof(MockAmazonServiceClient);
        public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient => ExtractServiceMetadata(typeof(TClient));
        public IServiceMetadata ExtractServiceMetadata(Type clientType)
        {
            Assert.Equal(ExpectedType, clientType);
            Calls.Add("metadata");
            if (Failure != null)
            {
                throw Failure;
            }

            return new DynamoMetadata();
        }

        public ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient => CreateClientConfig(typeof(TClient));
        public ClientConfig CreateClientConfig(Type clientType)
        {
            Assert.Equal(ExpectedType, clientType);
            Calls.Add("config");
            return Config;
        }

        public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
        {
            Assert.Same(Config, clientConfig);
            Calls.Add("style");
            return false;
        }

        public void SetClientRegion(AmazonServiceClient amazonServiceClient, string systemName) => throw new NotSupportedException();
    }

    private sealed class DynamoMetadata : IServiceMetadata
    {
        public string ServiceId => "DynamoDB";
        public IDictionary<string, string> OperationNameMapping { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    private class StyleConfig : MockClientConfig
    {
        public StyleConfig()
        {
            ForcePathStyle = false;
            RegionEndpoint = null;
        }
        public bool ForcePathStyle { get; private set; }
        public new RegionEndpoint? RegionEndpoint { get; set; }
    }

    private sealed class InheritedStyleConfig : StyleConfig
    {
        public InheritedStyleConfig() : base() { }
    }

    private sealed class WrongStyleConfig : MockClientConfig
    {
        public string? ForcePathStyle { get; set; }
    }

    private sealed class ThrowingStyleConfig : MockClientConfig
    {
        private readonly bool _value;
        public ThrowingStyleConfig() => _value = false;
        public static InvalidOperationException Failure { get; } = new("fixture setter");
        public bool ForcePathStyle { get => _value; set => throw Failure; }
    }

    private class MetadataClient : MockAmazonServiceClient
    {
        private static readonly IServiceMetadata serviceMetadata = new DynamoMetadata();
        public static IServiceMetadata Metadata => serviceMetadata;
    }

    private sealed class DerivedMetadataClient : MetadataClient
    {
        public DerivedMetadataClient() : base() { }
    }

    private sealed class RenamedParametersClient(AWSCredentials credentials, MockClientConfig config) : MockAmazonServiceClient(credentials, config);

    private sealed class AmbiguousConfigClient : MockAmazonServiceClient
    {
        public AmbiguousConfigClient(AWSCredentials credentials, MockClientConfig clientConfig) : base(credentials, clientConfig) { }
        public AmbiguousConfigClient(AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }
    }

    private sealed class ThrowingConfig : MockClientConfig
    {
        public static InvalidOperationException Failure { get; } = new("fixture config constructor");
        public ThrowingConfig() => throw Failure;
    }

    private sealed class ThrowingConfigClient(AWSCredentials credentials, ThrowingConfig clientConfig) : MockAmazonServiceClient(credentials, clientConfig);

    private sealed class ThrowingClient : MockAmazonServiceClient
    {
        public static InvalidOperationException Failure { get; } = new("fixture client constructor");
        public ThrowingClient(AWSCredentials credentials, MockClientConfig clientConfig) : base(credentials, clientConfig) => throw Failure;
    }
}
