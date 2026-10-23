#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - disabled because of it's not possible for this case
#pragma warning disable CS8603 // Possible null reference return. - disabled because of it's not possible for this case
namespace LocalStack.Client;

public class Session : ISession
{
    private readonly IConfig _config;
    private readonly ISessionOptions _sessionOptions;
    private readonly ISessionReflection _sessionReflection;

#if NET10_0_OR_GREATER
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

#if NET10_0_OR_GREATER
        if (implType == null)
        {
            throw new ArgumentNullException(nameof(implType));
        }
#endif

        var clientInstance = CreateImplementation(implType, awsCredentials, clientConfig);

        return clientInstance;
    }

    public AmazonServiceClient CreateClientByInterface<TClient>(bool useServiceUrl = false) where TClient : IAmazonService
    {
#if NET10_0_OR_GREATER
        if (_sessionReflection.GetType() == typeof(SessionReflection))
        {
            return CreateClientByStaticFactory<TClient>(useServiceUrl);
        }
#endif
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
        Type? clientType = FindClientType(serviceInterfaceType.Assembly, clientTypeName);

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

        AmazonServiceClient? client = CreateInterface(clientType, awsCredentials, clientConfig);

        if (client == null)
        {
            var message = $"Service client {clientTypeName} missing a constructor with parameters AWSCredentials and {clientConfig.GetType().FullName}.";

            throw new AmazonClientException(message);
        }

        return client;
    }

#if NET10_0_OR_GREATER
    private AmazonServiceClient CreateClientByStaticFactory<TClient>(bool useServiceUrl) where TClient : IAmazonService
    {
        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        ClientConfig clientConfig = TClient.CreateDefaultClientConfig();
        string serviceId = clientConfig.ServiceId;
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceId) ??
                                                throw new NotSupportedClientException($"{serviceId} is not supported by this mock session.");
        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        S3ForcePathStyle.TrySet(clientConfig, true);
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

        IAmazonService client;
        try
        {
            client = TClient.CreateDefaultServiceClient(awsCredentials, clientConfig);
        }
        catch (Exception exception)
        {
            throw new TargetInvocationException(exception);
        }

        return (AmazonServiceClient)client;
    }
#endif

    private static Type? FindClientType(Assembly assembly, string name)
    {
#if NET10_0_OR_GREATER
        Type? generated = GeneratedClientRegistry.FindClient(assembly, name);
        if (generated != null)
        {
            return generated;
        }
        if (ReflectionFallback.IsSupported)
        {
            return FindLegacyClientType(assembly, name);
        }

        return null;
#else
        return FindLegacyClientType(assembly, name);
#endif
    }

    private static AmazonServiceClient? CreateInterface(Type type, AWSCredentials credentials, ClientConfig config)
    {
#if NET10_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(type, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return CreateGenerated(entry, credentials, config);
        }

        if (ReflectionFallback.IsSupported)
        {
            return CreateLegacyInterface(type, credentials, config);
        }
        else
        {
            return null;
        }
#else
        return CreateLegacyInterface(type, credentials, config);
#endif
    }

    private static AmazonServiceClient CreateImplementation(Type type, AWSCredentials credentials, ClientConfig config)
    {
#if NET10_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(type, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return CreateGenerated(entry, credentials, config)
                ?? throw new MissingMethodException($"Constructor on type '{type.FullName}' not found.");
        }

        if (ReflectionFallback.IsSupported)
        {
            return CreateLegacyImplementation(type, credentials, config);
        }
        else
        {
            throw new NotSupportedException($"Client type {type} was not present when the application was compiled.");
        }
#else
        return CreateLegacyImplementation(type, credentials, config);
#endif
    }

#if NET10_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client types require their original reflection metadata.")]
#endif
    private static Type? FindLegacyClientType(Assembly assembly, string name) => assembly.GetType(name);

#if NET10_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client constructors require their original reflection metadata.")]
#endif
    private static AmazonServiceClient? CreateLegacyInterface(Type type, AWSCredentials credentials, ClientConfig config) =>
        (AmazonServiceClient?)type.GetConstructor(new[] { typeof(AWSCredentials), config.GetType() })?.Invoke(new object[] { credentials, config });

#if NET10_0_OR_GREATER
    private static AmazonServiceClient? CreateGenerated(GeneratedClientRegistry.ClientEntry entry, AWSCredentials credentials, ClientConfig config)
    {
        try
        {
            return entry.Create(credentials, config);
        }
        catch (Exception exception)
        {
            throw new TargetInvocationException(exception);
        }
    }

    [RequiresUnreferencedCode("Dynamically loaded client constructors require their original reflection metadata.")]
#endif
    private static AmazonServiceClient CreateLegacyImplementation(Type type, AWSCredentials credentials, ClientConfig config) =>
        (AmazonServiceClient)Activator.CreateInstance(type, credentials, config)!;

#if NET10_0_OR_GREATER
    // The generated caller uses UnsafeAccessor. Only delegates cross this boundary: no DAM
    // requirements are imposed on an accessor signature or on existing consumer Type APIs.
    private static void RegisterGeneratedClient(Type type, Func<IServiceMetadata> metadata, Func<ClientConfig> config,
                                                Func<AWSCredentials, ClientConfig, AmazonServiceClient?> create) =>
        GeneratedClientRegistry.RegisterClient(type, metadata, config, create);

    private static void RegisterGeneratedConfig(Type type, Func<ClientConfig, bool, bool>? style, bool plainRegion) =>
        GeneratedClientRegistry.RegisterConfig(type, style, plainRegion);
#endif
}
