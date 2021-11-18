namespace LocalStack.Client;

public class Config : IConfig
{
    private readonly AwsServiceEndpointMetadata[] _serviceEndpointMetadata = AwsServiceEndpointMetadata.All;
    private readonly IEnumerable<AwsServiceEndpoint> _awsServiceEndpoints;

    public Config()
        : this(new ConfigOptions())
    {
    }

    public Config(IConfigOptions configOptions)
    {
        string localStackHost = configOptions.LocalStackHost;
        string protocol = configOptions.UseSsl ? "https" : "http";
        bool useLegacyPorts = configOptions.UseLegacyPorts;
        int edgePort = configOptions.EdgePort;

        int GetServicePort(int metadataPort) => useLegacyPorts ? metadataPort : edgePort;

        _awsServiceEndpoints = _serviceEndpointMetadata.Select(metadata => new AwsServiceEndpoint(metadata.ServiceId, 
                                                                                                  metadata.CliName, 
                                                                                                  metadata.Enum, 
                                                                                                  GetServicePort(metadata.Port), 
                                                                                                  localStackHost, 
                                                                                                  metadata.GetServiceUrl(protocol, localStackHost, GetServicePort(metadata.Port))));
    }

    public IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints()
    {
        return _awsServiceEndpoints;
    }

    public AwsServiceEndpoint GetAwsServiceEndpoint(AwsServiceEnum awsServiceEnum)
    {
        return _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.AwsServiceEnum == awsServiceEnum);
    }

    public AwsServiceEndpoint GetAwsServiceEndpoint(string serviceId)
    {
        return _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.ServiceId == serviceId);
    }

    public IDictionary<AwsServiceEnum, int> GetAwsServicePorts()
    {
        return _awsServiceEndpoints.ToDictionary(endpoint => endpoint.AwsServiceEnum, endpoint => endpoint.Port);
    }

    public int GetAwsServicePort(AwsServiceEnum awsServiceEnum)
    {
        return _awsServiceEndpoints
               .First(endpoint => endpoint.AwsServiceEnum == awsServiceEnum)
               .Port;
    }
}