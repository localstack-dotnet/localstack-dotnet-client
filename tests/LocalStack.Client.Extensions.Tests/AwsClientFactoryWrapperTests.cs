namespace LocalStack.Client.Extensions.Tests;

public class AwsClientFactoryWrapperTests
{
    private readonly IAwsClientFactoryWrapper _awsClientFactoryWrapper;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly AWSOptions _awsOptions;

    public AwsClientFactoryWrapperTests()
    {
        _awsClientFactoryWrapper = new AwsClientFactoryWrapper();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _awsOptions = new AWSOptions();
    }

    [Fact]
    public void CreateServiceClient_Should_Throw_LocalStackClientConfigurationException_When_ClientFactoryType_Is_Null()
    {
        Type type = _awsClientFactoryWrapper.GetType();
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        FieldInfo? clientFactoryFullNameField = type.GetField("ClientFactoryFullName", bindingFlags);
        FieldInfo? createServiceClientMethodNameFieldInfo = type.GetField("CreateServiceClientMethodName", bindingFlags);

        Assert.NotNull(clientFactoryFullNameField);
        Assert.NotNull(createServiceClientMethodNameFieldInfo);

        SetPrivateReadonlyField(clientFactoryFullNameField, "NonExistingType");
        SetPrivateReadonlyField(createServiceClientMethodNameFieldInfo, "NonExistingMethod");

        Assert.Throws<LocalStackClientConfigurationException>(
            () => _awsClientFactoryWrapper.CreateServiceClient<MockAmazonServiceClient>(_mockServiceProvider.Object, _awsOptions));
    }

    [Fact]
    public void CreateServiceClient_Should_Create_Client_When_UseLocalStack_False()
    {
        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.AddInMemoryCollection(new KeyValuePair<string, string?>[] { new("LocalStack:UseLocalStack", "false") });
        IConfigurationRoot configurationRoot = configurationBuilder.Build();

        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "AKIAIOSFODNN7EXAMPLE");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
        Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-west-2");

        ServiceCollection serviceCollection = new();
        serviceCollection.AddLocalStack(configurationRoot);
        serviceCollection.AddAWSServiceLocalStack<IAmazonS3>();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        var requiredService = serviceProvider.GetRequiredService<IAmazonS3>();

        Assert.NotNull(requiredService);
    }

    private static void SetPrivateReadonlyField(FieldInfo field, string value)
    {
        var method = new DynamicMethod("SetReadOnlyField", null, new[] { typeof(object), typeof(object) }, typeof(AwsClientFactoryWrapper), true);
        var il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, field.DeclaringType!);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, field);
        il.Emit(OpCodes.Ret);

        method.Invoke(null, new object[] { null!, value });
    }
}