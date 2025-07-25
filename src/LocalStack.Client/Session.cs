﻿#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - disabled because of it's not possible for this case
#pragma warning disable CS8603 // Possible null reference return. - disabled because of it's not possible for this case
namespace LocalStack.Client;

public class Session : ISession
{
    private readonly IConfig _config;
    private readonly ISessionOptions _sessionOptions;
    private readonly ISessionReflection _sessionReflection;

    public Session(ISessionOptions sessionOptions, IConfig config, ISessionReflection sessionReflection)
    {
        _sessionOptions = sessionOptions;
        _config = config;
        _sessionReflection = sessionReflection;
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Uses Activator/CreateInstance and private‑field reflection; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public TClient CreateClientByImplementation<TClient>(bool useServiceUrl = false) where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return (TClient)CreateClientByImplementation(clientType, useServiceUrl);
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Uses Activator/CreateInstance and private‑field reflection; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public AmazonServiceClient CreateClientByImplementation(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicFields)]
#endif
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

        var clientInstance = (AmazonServiceClient)Activator.CreateInstance(implType, awsCredentials, clientConfig);

        return clientInstance;
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Uses Activator/CreateInstance and private‑field reflection; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public AmazonServiceClient CreateClientByInterface<TClient>(bool useServiceUrl = false) where TClient : IAmazonService
    {
        Type serviceInterfaceType = typeof(TClient);

        return CreateClientByInterface(serviceInterfaceType, useServiceUrl);
    }

    public AmazonServiceClient CreateClientByInterface(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicFields)]
#endif
        Type serviceInterfaceType,
        bool useServiceUrl = false)
    {
        if (serviceInterfaceType == null)
        {
            throw new ArgumentNullException(nameof(serviceInterfaceType));
        }

        var clientTypeName = $"{serviceInterfaceType.Namespace}.{serviceInterfaceType.Name.Substring(1)}Client";
        Type clientType = serviceInterfaceType.GetTypeInfo().Assembly.GetType(clientTypeName);

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

        ConstructorInfo? constructor = clientType.GetConstructor(new[] { typeof(AWSCredentials), clientConfig.GetType() });

        if (constructor == null)
        {
            var message = $"Service client {clientTypeName} missing a constructor with parameters AWSCredentials and {clientConfig.GetType().FullName}.";

            throw new AmazonClientException(message);
        }

        var client = (AmazonServiceClient)constructor.Invoke(new object[] { awsCredentials, clientConfig });

        return client;
    }
}