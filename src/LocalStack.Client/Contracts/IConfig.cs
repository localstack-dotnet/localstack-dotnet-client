namespace LocalStack.Client.Contracts;

public interface IConfig
{
    IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints();

    AwsServiceEndpoint? GetAwsServiceEndpoint(AwsService awsService);

    AwsServiceEndpoint? GetAwsServiceEndpoint(string serviceId);

    IDictionary<AwsService, int> GetAwsServicePorts();

    int GetAwsServicePort(AwsService awsService);

    IConfigOptions GetConfigOptions();
}