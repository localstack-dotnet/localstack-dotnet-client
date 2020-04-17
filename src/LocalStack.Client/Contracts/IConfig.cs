using LocalStack.Client.Models;

using System.Collections.Generic;

using LocalStack.Client.Enums;

namespace LocalStack.Client.Contracts
{
    public interface IConfig
    {
        IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints();

        AwsServiceEndpoint GetAwsServiceEndpoint(AwsServiceEnum awsServiceEnum);

        AwsServiceEndpoint GetAwsServiceEndpoint(string serviceId);

        IDictionary<AwsServiceEnum, int> GetAwsServicePorts();

        int GetAwsServicePorts(AwsServiceEnum awsServiceEnum);
    }
}