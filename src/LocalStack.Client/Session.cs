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

    public TClient CreateClientByImplementation<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return (TClient)CreateClientByImplementation(clientType);
    }

    public AmazonServiceClient CreateClientByImplementation(Type implType)
    {
        IServiceMetadata serviceMetadata = _sessionReflection.ExtractServiceMetadata(implType);
        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new NotSupportedClientException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
        ClientConfig clientConfig = _sessionReflection.CreateClientConfig(implType);

        clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl;
        clientConfig.UseHttp = true;
        _sessionReflection.SetForcePathStyle(clientConfig);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            clientConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(_sessionOptions.RegionName);
        }

        var clientInstance = (AmazonServiceClient)Activator.CreateInstance(implType, awsCredentials, clientConfig);

        return clientInstance;
    }

    public AmazonServiceClient CreateClientByInterface<TClient>() where TClient : IAmazonService
    {
        Type serviceInterfaceType = typeof(TClient);

        return (AmazonServiceClient)CreateClientByInterface(serviceInterfaceType);
    }

    public AmazonServiceClient CreateClientByInterface(Type serviceInterfaceType)
    {
        var clientTypeName = $"{serviceInterfaceType.Namespace}.{serviceInterfaceType.Name.Substring(1)}Client";
        Type clientType = serviceInterfaceType.GetTypeInfo().Assembly.GetType(clientTypeName);

        if (clientType == null)
        {
            throw new AmazonClientException($"Failed to find service client {clientTypeName} which implements {serviceInterfaceType.FullName}.");
        }

        IServiceMetadata serviceMetadata = _sessionReflection.ExtractServiceMetadata(clientType);

        AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                throw new InvalidOperationException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

        AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);

        ClientConfig clientConfig = _sessionReflection.CreateClientConfig(clientType);

        clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl;
        clientConfig.UseHttp = true;
        _sessionReflection.SetForcePathStyle(clientConfig);
        clientConfig.ProxyHost = awsServiceEndpoint.Host;
        clientConfig.ProxyPort = awsServiceEndpoint.Port;

        ConstructorInfo constructor = clientType.GetConstructor(new[] { typeof(AWSCredentials), clientConfig.GetType() });

        if (!string.IsNullOrWhiteSpace(_sessionOptions.RegionName))
        {
            clientConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(_sessionOptions.RegionName);
        }

        if (constructor == null)
        {
            throw new AmazonClientException($"Service client {clientTypeName} missing a constructor with parameters AWSCredentials and {clientConfig.GetType().FullName}.");
        }

        var client = (AmazonServiceClient)constructor.Invoke(new object[] { awsCredentials, clientConfig });

        return client;
    }
}