using System;
using System.Collections.Generic;
using System.Linq;
using LocalStack.Client.Models;

namespace LocalStack.Client
{
    public static class Config
    {
        private static readonly string EnvLocalStackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOST");
        private static readonly string EnvUseSsl = Environment.GetEnvironmentVariable("USE_SSL");

        public static AwsServiceEndpoint GetAwsServiceEndpoint(AwsServiceEnum awsServiceEnum) => GetAwsServiceEndpoints().SingleOrDefault(endpoint => endpoint.AwsServiceEnum == awsServiceEnum);

        public static IEnumerable<AwsServiceEndpoint> GetAwsServiceEndpoints(string localStackHost = null)
        {
            localStackHost = localStackHost ?? (EnvLocalStackHost ?? "localhost");
            string protocol = EnvUseSsl != null && (EnvUseSsl == "1" || EnvUseSsl == "true") ? "https" : "http";

            var serviceEndpointMetadata = AwsServiceEndpointMetadata.ServicesEndpointMetadata;

            return serviceEndpointMetadata
                .Select(metadata => new AwsServiceEndpoint(metadata.ServiceId, metadata.CliName, metadata.Enum,
                    metadata.Port, localStackHost, metadata.ToString(protocol, localStackHost)));
        }
    }
}
