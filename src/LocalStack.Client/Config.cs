using System;
using System.Collections.Generic;
using System.Linq;
using LocalStack.Client.Contracts;
using LocalStack.Client.Models;

namespace LocalStack.Client
{
    public class Config : IConfig
    {
        private static readonly string EnvLocalStackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOST");
        private static readonly string EnvUseSsl = Environment.GetEnvironmentVariable("USE_SSL");
        private static readonly AwsServiceEndpointMetadata[] ServiceEndpointMetadata = AwsServiceEndpointMetadata.All;

        private readonly IEnumerable<AwsServiceEndpoint> _awsServiceEndpoints;

        public Config(string localStackHost = null)
        {
            localStackHost = localStackHost ?? (EnvLocalStackHost ?? "localhost");
            string protocol = EnvUseSsl != null && (EnvUseSsl == "1" || EnvUseSsl == "true") ? "https" : "http";

            _awsServiceEndpoints = ServiceEndpointMetadata
                .Select(metadata => new AwsServiceEndpoint(metadata.ServiceId, metadata.CliName, metadata.Enum,
                    metadata.Port, localStackHost, metadata.ToString(protocol, localStackHost)));
        }

        public IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints() => _awsServiceEndpoints;

        public AwsServiceEndpoint GetAwsServiceEndpoint(AwsServiceEnum awsServiceEnum) => _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.AwsServiceEnum == awsServiceEnum);

        public AwsServiceEndpoint GetAwsServiceEndpoint(string serviceId) => _awsServiceEndpoints.SingleOrDefault(endpoint => endpoint.ServiceId == serviceId);
    }
}
