#if NET8_0_OR_GREATER
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - disabled because of it's not possible for this case
#pragma warning disable CS8603 // Possible null reference return. - disabled because of it's not possible for this case
#pragma warning disable MA0048 // File name must match type name

namespace LocalStack.Client;

/// <summary>
/// Modern Session implementation for .NET 8+ using generated accessors.
/// Zero reflection dependencies - pure AOT compatibility.
/// </summary>
public class Session : ISession
{
    private readonly IConfig _config;
    private readonly ISessionOptions _sessionOptions;

    public Session(ISessionOptions sessionOptions, IConfig config)
    {
        _sessionOptions = sessionOptions;
        _config = config;
    }

    public TClient CreateClientByImplementation<TClient>(bool useServiceUrl = false) where TClient : AmazonServiceClient
    {
        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        // Modern: Direct accessor-based approach - zero reflection
        var accessor = AwsAccessorRegistry.Get(typeof(TClient));
        IServiceMetadata serviceMetadata = accessor.GetServiceMetadata();
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = accessor.CreateClientConfig();

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        accessor.TrySetForcePathStyle(clientConfig, true);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            accessor.SetRegion(clientConfig, RegionEndpoint.GetBySystemName(_sessionOptions.RegionName));
        }

        var clientInstance = (TClient)accessor.CreateClient(awsCredentials, clientConfig);
        return clientInstance;
    }

    public AmazonServiceClient CreateClientByImplementation(Type implType, bool useServiceUrl = false)
    {
        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        // Modern: Direct accessor-based approach - zero reflection
        var accessor = AwsAccessorRegistry.Get(implType);
        IServiceMetadata serviceMetadata = accessor.GetServiceMetadata();
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = accessor.CreateClientConfig();

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        accessor.TrySetForcePathStyle(clientConfig, true);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            accessor.SetRegion(clientConfig, RegionEndpoint.GetBySystemName(_sessionOptions.RegionName));
        }

        var clientInstance = accessor.CreateClient(awsCredentials, clientConfig);
        return clientInstance;
    }

    public AmazonServiceClient CreateClientByInterface<TClient>(bool useServiceUrl = false) where TClient : IAmazonService
    {
        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        // Modern: Direct registry-based interface-to-client mapping - zero reflection
        var accessor = AwsAccessorRegistry.GetByInterface<TClient>();
        IServiceMetadata serviceMetadata = accessor.GetServiceMetadata();
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = accessor.CreateClientConfig();

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        accessor.TrySetForcePathStyle(clientConfig, true);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            accessor.SetRegion(clientConfig, RegionEndpoint.GetBySystemName(_sessionOptions.RegionName));
        }

        var client = accessor.CreateClient(awsCredentials, clientConfig);
        return client;
    }

    public AmazonServiceClient CreateClientByInterface(Type serviceInterfaceType, bool useServiceUrl = false)
    {
        if (serviceInterfaceType == null)
        {
            throw new ArgumentNullException(nameof(serviceInterfaceType));
        }

        if (!useServiceUrl && string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            throw new MisconfiguredClientException($"{nameof(_sessionOptions.RegionName)} must be set if {nameof(useServiceUrl)} is false.");
        }

        // Modern: Direct registry-based interface-to-client mapping with Type parameter - zero reflection
        var accessor = AwsAccessorRegistry.GetByInterface(serviceInterfaceType);
        IServiceMetadata serviceMetadata = accessor.GetServiceMetadata();
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = accessor.CreateClientConfig();

        clientConfig.UseHttp = !_config.GetConfigOptions().UseSsl;
        accessor.TrySetForcePathStyle(clientConfig, true);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (useServiceUrl)
        {
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl.AbsoluteUri;
        }
        else if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            accessor.SetRegion(clientConfig, RegionEndpoint.GetBySystemName(_sessionOptions.RegionName));
        }

        var client = accessor.CreateClient(awsCredentials, clientConfig);
        return client;
    }
}
#endif