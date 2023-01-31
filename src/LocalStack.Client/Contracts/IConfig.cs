namespace LocalStack.Client.Contracts;

public interface IConfig
{
    IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints();

    AwsServiceEndpoint GetAwsServiceEndpoint(AwsServiceEnum awsServiceEnum);

    AwsServiceEndpoint GetAwsServiceEndpoint(string serviceId);

    IDictionary<AwsServiceEnum, int> GetAwsServicePorts();

    int GetAwsServicePort(AwsServiceEnum awsServiceEnum);

    IConfigOptions GetConfigOptions();
}