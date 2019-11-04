using LocalStack.Client.Enums;

namespace LocalStack.Client.Models
{
    public class AwsServiceEndpoint
    {
        public AwsServiceEndpoint(string serviceId, string cliName, AwsServiceEnum @enum, int port, string host, string serviceUrl)
        {
            ServiceId = serviceId;
            CliName = cliName;
            AwsServiceEnum = @enum;
            Port = port;
            Host = host;
            ServiceUrl = serviceUrl;
        }

        public string ServiceId { get; }

        public string CliName { get; }

        public AwsServiceEnum AwsServiceEnum { get; }

        public int Port { get; }

        public string Host { get; }

        public string ServiceUrl { get; }

        public override string ToString()
        {
            return $"{ServiceId} - {ServiceUrl}";
        }
    }
}