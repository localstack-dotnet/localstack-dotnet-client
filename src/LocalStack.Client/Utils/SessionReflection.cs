namespace LocalStack.Client.Utils;

/// <summary>
/// Platform-specific SessionReflection facade that chooses the appropriate implementation
/// based on the target framework. Uses modern UnsafeAccessor pattern for .NET 8+ 
/// and traditional reflection for legacy frameworks.
/// </summary>
public class SessionReflection : ISessionReflection
{
#if NET8_0_OR_GREATER
    private static readonly ISessionReflection _implementation = new SessionReflectionModern();
#else
    private static readonly ISessionReflection _implementation = new SessionReflectionLegacy();
#endif

    public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient
    {
        return _implementation.ExtractServiceMetadata<TClient>();
    }

    public IServiceMetadata ExtractServiceMetadata(Type clientType)
    {
        return _implementation.ExtractServiceMetadata(clientType);
    }

    public ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient
    {
        return _implementation.CreateClientConfig<TClient>();
    }

    public ClientConfig CreateClientConfig(Type clientType)
    {
        return _implementation.CreateClientConfig(clientType);
    }

    public void SetClientRegion(AmazonServiceClient amazonServiceClient, string systemName)
    {
        _implementation.SetClientRegion(amazonServiceClient, systemName);
    }

    public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
    {
        return _implementation.SetForcePathStyle(clientConfig, value);
    }
}