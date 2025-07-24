#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
#pragma warning disable CS8600,CS8603 // Not possible to get null value from this private field
#pragma warning disable CA1802 // We need to use reflection to access private fields for service metadata
namespace LocalStack.Client.Extensions;

public sealed class AwsClientFactoryWrapper : IAwsClientFactoryWrapper
{
    private static readonly string ClientFactoryGenericTypeName = "Amazon.Extensions.NETCore.Setup.ClientFactory`1";
    private static readonly string CreateServiceClientMethodName = "CreateServiceClient";

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Creates generic ClientFactory<T> and invokes internal members via reflection"),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions? awsOptions) where TClient : IAmazonService
    {
        Type? genericFactoryType = typeof(ConfigurationException).Assembly.GetType(ClientFactoryGenericTypeName);

        if (genericFactoryType == null)
        {
            throw new LocalStackClientConfigurationException($"Failed to find internal ClientFactory<T> in {ClientFactoryGenericTypeName}");
        }

        // Create ClientFactory<TClient>
        Type concreteFactoryType = genericFactoryType.MakeGenericType(typeof(TClient));
        ConstructorInfo? constructor = concreteFactoryType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(AWSOptions) }, null);

        if (constructor == null)
        {
            throw new LocalStackClientConfigurationException("ClientFactory<T> missing constructor with AWSOptions parameter.");
        }

        object factory = constructor.Invoke(new object[] { awsOptions! });
        MethodInfo? createMethod = factory.GetType().GetMethod(CreateServiceClientMethodName, BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IServiceProvider) }, null);

        if (createMethod == null)
        {
            throw new LocalStackClientConfigurationException($"ClientFactory<T> missing {CreateServiceClientMethodName}(IServiceProvider) method.");
        }

        object serviceInstance = createMethod.Invoke(factory, new object[] { provider });
        return (AmazonServiceClient)serviceInstance;
    }
}