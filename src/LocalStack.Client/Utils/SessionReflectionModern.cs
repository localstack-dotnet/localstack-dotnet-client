#if NET8_0_OR_GREATER
namespace LocalStack.Client.Utils;

/// <summary>
/// Modern reflection implementation for .NET 8+ using UnsafeAccessor pattern.
/// This implementation uses generated accessors registered in AwsAccessorRegistry.
/// Completely eliminates reflection API usage for AOT compatibility.
/// </summary>
internal sealed class SessionReflectionModern : ISessionReflection
{
    public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient
    {
        var accessor = AwsAccessorRegistry.Get(typeof(TClient));
        return accessor.GetServiceMetadata();
    }

    public IServiceMetadata ExtractServiceMetadata(Type clientType)
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

        var accessor = AwsAccessorRegistry.Get(clientType);
        return accessor.GetServiceMetadata();
    }

    public ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient
    {
        var accessor = AwsAccessorRegistry.Get(typeof(TClient));
        return accessor.CreateClientConfig();
    }

    public ClientConfig CreateClientConfig(Type clientType)
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

        var accessor = AwsAccessorRegistry.Get(clientType);
        return accessor.CreateClientConfig();
    }

    public void SetClientRegion(AmazonServiceClient amazonServiceClient, string systemName)
    {
        if (amazonServiceClient == null)
        {
            throw new ArgumentNullException(nameof(amazonServiceClient));
        }
        
        if (string.IsNullOrEmpty(systemName))
        {
            throw new ArgumentNullException(nameof(systemName));
        }

        var regionEndpoint = RegionEndpoint.GetBySystemName(systemName);
        var accessor = AwsAccessorRegistry.Get(amazonServiceClient.GetType());
        
        // Cast to concrete ClientConfig - all AWS configs inherit from this
        var clientConfig = (ClientConfig)amazonServiceClient.Config;
        accessor.SetRegion(clientConfig, regionEndpoint);
    }

    public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
    {
        if (clientConfig == null)
        {
            throw new ArgumentNullException(nameof(clientConfig));
        }

        // Get the client type from the config type
        // AWS configs follow pattern: AmazonS3Config -> AmazonS3Client  
        var configType = clientConfig.GetType();
        var configTypeName = configType.FullName;
        
        if (configTypeName == null || !configTypeName.EndsWith("Config", StringComparison.Ordinal))
        {
            return false;
        }

        var clientTypeName = configTypeName.Replace("Config", "Client", StringComparison.Ordinal);
        
        // For AOT builds, we cannot use Assembly.GetType() as it's not trim-safe
        // Instead, we'll find the matching registered client type
        var matchingClientType = AwsAccessorRegistry.RegisteredClientTypes
            .FirstOrDefault(type => string.Equals(type.FullName, clientTypeName, StringComparison.Ordinal));
        
        if (matchingClientType != null)
        {
            var accessor = AwsAccessorRegistry.Get(matchingClientType);
            return accessor.TrySetForcePathStyle(clientConfig, value);
        }

        return false;
    }
}
#endif