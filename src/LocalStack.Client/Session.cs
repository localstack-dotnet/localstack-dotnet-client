#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - disabled because of it's not possible for this case
#pragma warning disable CS8603 // Possible null reference return. - disabled because of it's not possible for this case
namespace LocalStack.Client;

public class Session : ISession
{
    private readonly IConfig _config;
    private readonly ISessionOptions _sessionOptions;
    private readonly ISessionReflection _sessionReflection;

#if NET8_0_OR_GREATER
    [DynamicDependency(nameof(RegisterGeneratedClient), typeof(Session))]
    [DynamicDependency(nameof(RegisterGeneratedConfig), typeof(Session))]
#endif
    public Session(ISessionOptions sessionOptions, IConfig config, ISessionReflection sessionReflection)
    {
        _sessionOptions = sessionOptions;
        _config = config;
        _sessionReflection = sessionReflection;
    }

    public TClient CreateClientByImplementation<TClient>(bool useServiceUrl = false) where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return (TClient)CreateClientByImplementation(clientType, useServiceUrl);
    }

    public AmazonServiceClient CreateClientByImplementation(
        Type implType,
        bool useServiceUrl = false)
    {
        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        IServiceMetadata serviceMetadata = _sessionReflection.ExtractServiceMetadata(implType);
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = _sessionReflection.CreateClientConfig(implType);

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        _sessionReflection.SetForcePathStyle(clientConfig);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            clientConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(_sessionOptions.RegionName);
        }

        var clientInstance = CreateImplementation(implType, awsCredentials, clientConfig);

        return clientInstance;
    }

    public AmazonServiceClient CreateClientByInterface<TClient>(bool useServiceUrl = false) where TClient : IAmazonService
    {
        Type serviceInterfaceType = typeof(TClient);

        return CreateClientByInterface(serviceInterfaceType, useServiceUrl);
    }

    public AmazonServiceClient CreateClientByInterface(
        Type serviceInterfaceType,
        bool useServiceUrl = false)
    {
        if (serviceInterfaceType == null)
        {
            throw new ArgumentNullException(nameof(serviceInterfaceType));
        }

        var clientTypeName = $"{serviceInterfaceType.Namespace}.{serviceInterfaceType.Name.Substring(1)}Client";
        Type? clientType = FindClientType(serviceInterfaceType.GetTypeInfo().Assembly, clientTypeName);

        if (clientType == null)
        {
            throw new AmazonClientException($"Failed to find service client {clientTypeName} which implements {serviceInterfaceType.FullName}.");
        }

        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        IServiceMetadata serviceMetadata = _sessionReflection.ExtractServiceMetadata(clientType);

        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);

        ClientConfig clientConfig = _sessionReflection.CreateClientConfig(clientType);

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        _sessionReflection.SetForcePathStyle(clientConfig);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            clientConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(_sessionOptions.RegionName);
        }

        ConstructorInfo? constructor = FindClientConstructor(clientType, clientConfig.GetType());

        if (constructor == null)
        {
            var message = $"Service client {clientTypeName} missing a constructor with parameters AWSCredentials and {clientConfig.GetType().FullName}.";

            throw new AmazonClientException(message);
        }

        var client = (AmazonServiceClient)constructor.Invoke(new object[] { awsCredentials, clientConfig });

        return client;
    }

    private static Type? FindClientType(Assembly assembly, string name)
    {
#if NET8_0_OR_GREATER
        Type? generated = GeneratedClientRegistry.FindClient(assembly, name);
        if (generated != null || !System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            return generated;
        }
#endif
        return FindLegacyClientType(assembly, name);
    }

    private static ConstructorInfo? FindClientConstructor(Type type, Type configType)
    {
#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(type, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.Constructor(configType);
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            return null;
        }
#endif
        return FindLegacyClientConstructor(type, configType);
    }

    private static AmazonServiceClient CreateImplementation(Type type, AWSCredentials credentials, ClientConfig config)
    {
#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(type, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.Create(credentials, config);
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new NotSupportedException($"Client type {type} was not present when the application was compiled.");
        }
#endif
        return CreateLegacyImplementation(type, credentials, config);
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client types require their original reflection metadata.")]
#endif
    private static Type? FindLegacyClientType(Assembly assembly, string name) => assembly.GetType(name);

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client constructors require their original reflection metadata.")]
#endif
    private static ConstructorInfo? FindLegacyClientConstructor(Type type, Type configType) => type.GetConstructor(new[] { typeof(AWSCredentials), configType });

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client constructors require their original reflection metadata.")]
#endif
    private static AmazonServiceClient CreateLegacyImplementation(Type type, AWSCredentials credentials, ClientConfig config) =>
        (AmazonServiceClient)Activator.CreateInstance(type, credentials, config)!;

#if NET8_0_OR_GREATER
    // The generated caller uses UnsafeAccessor. Only delegates cross this boundary: no DAM
    // requirements are imposed on an accessor signature or on existing consumer Type APIs.
    private static void RegisterGeneratedClient(Type type, Func<IServiceMetadata> metadata, Func<ClientConfig> config,
                                                Func<Type, ConstructorInfo?> constructor, Func<AWSCredentials, ClientConfig, AmazonServiceClient> create) =>
        GeneratedClientRegistry.RegisterClient(type, metadata, config, constructor, create);

    private static void RegisterGeneratedConfig(Type type, Func<ClientConfig, bool, bool> style, Action<object, string> region) =>
        GeneratedClientRegistry.RegisterConfig(type, style, region);
#endif
}
