namespace LocalStack.Client;

public class Config : IConfig
{
    private readonly AwsServiceEndpointMetadata[] _serviceEndpointMetadata = AwsServiceEndpointMetadata.All;
    private readonly IEnumerable<AwsServiceEndpoint> _awsServiceEndpoints;

    private readonly IConfigOptions _configOptions;

    public Config() : this(new ConfigOptions())
    {
    }

    public Config(IConfigOptions configOptions)
    {
        if (configOptions == null)
        {
            throw new ArgumentNullException(nameof(configOptions));
        }

        string localStackHost = configOptions.LocalStackHost;
        string protocol = configOptions.UseSsl ? "https" : "http";
        bool useLegacyPorts = configOptions.UseLegacyPorts;
        int edgePort = configOptions.EdgePort;

        _awsServiceEndpoints = _serviceEndpointMetadata.Select(metadata =>
        {
            Uri serviceUrl = metadata.GetServiceUrl(protocol, localStackHost, GetServicePort(metadata.Port));

            return new AwsServiceEndpoint(metadata.ServiceId, metadata.CliName, metadata.Enum, GetServicePort(metadata.Port), localStackHost, serviceUrl);
        });

        _configOptions = configOptions;

        int GetServicePort(int metadataPort) => useLegacyPorts ? metadataPort : edgePort;
    }

    public IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints()
    {
        return _awsServiceEndpoints;
    }

    public AwsServiceEndpoint? GetAwsServiceEndpoint(AwsService awsService)
    {
        return _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.AwsService == awsService);
    }

    public AwsServiceEndpoint? GetAwsServiceEndpoint(string serviceId)
    {
        return _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.ServiceId == serviceId);
    }

    public IDictionary<AwsService, int> GetAwsServicePorts()
    {
        return _awsServiceEndpoints.ToDictionary(endpoint => endpoint.AwsService, endpoint => endpoint.Port);
    }

    public int GetAwsServicePort(AwsService awsService)
    {
        return _awsServiceEndpoints.First(endpoint => endpoint.AwsService == awsService).Port;
    }

    public IConfigOptions GetConfigOptions() => _configOptions;
}