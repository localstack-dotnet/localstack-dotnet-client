#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
#pragma warning disable CS8600,CS8603 // Not possible to get null value from this private field
#pragma warning disable CA1802 // We need to use reflection to access private fields for service metadata
namespace LocalStack.Client.Extensions;

public sealed class AwsClientFactoryWrapper : IAwsClientFactoryWrapper
{
    private static readonly string ClientFactoryFullName = "Amazon.Extensions.NETCore.Setup.ClientFactory";
    private static readonly string CreateServiceClientMethodName = "CreateServiceClient";

    public AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions? awsOptions) where TClient : IAmazonService
    {
        Type? clientFactoryType = typeof(ConfigurationException).Assembly.GetType(ClientFactoryFullName);

        if (clientFactoryType == null)
        {
            throw new LocalStackClientConfigurationException($"Failed to find internal ClientFactory in {ClientFactoryFullName}");
        }

        ConstructorInfo? constructorInfo =
            clientFactoryType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Type), typeof(AWSOptions) }, null);

        if (constructorInfo == null)
        {
            throw new LocalStackClientConfigurationException("ClientFactory missing a constructor with parameters Type and AWSOptions.");
        }

        Type clientType = typeof(TClient);

        object clientFactory = constructorInfo.Invoke(new object[] { clientType, awsOptions! });

        MethodInfo? methodInfo = clientFactory.GetType().GetMethod(CreateServiceClientMethodName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo == null)
        {
            throw new LocalStackClientConfigurationException($"Failed to find internal method {CreateServiceClientMethodName} in {ClientFactoryFullName}");
        }

        object serviceInstance = methodInfo.Invoke(clientFactory, new object[] { provider });

        return (AmazonServiceClient)serviceInstance;
    }
}