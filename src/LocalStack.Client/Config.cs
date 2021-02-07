using LocalStack.Client.Contracts;
using LocalStack.Client.Models;

using System;
using System.Collections.Generic;
using System.Linq;

using LocalStack.Client.Enums;
using LocalStack.Client.Options;

namespace LocalStack.Client
{
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

        [Obsolete("This constructor is obsolete, use default or constructor with IConfigOptions parameter")]
        public Config(string localStackHost = null)
        {
            string envLocalStackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOST");
            string envUseSsl = Environment.GetEnvironmentVariable("USE_SSL");
            string envUseLegacyPorts = Environment.GetEnvironmentVariable("USE_LEGACY_PORTS");
            string envEdgePort = Environment.GetEnvironmentVariable("EDGE_PORT");

            localStackHost ??= (envLocalStackHost ?? "localhost");
            string protocol = !string.IsNullOrEmpty(envUseSsl) && (envUseSsl == "1" || envUseSsl == "true") ? "https" : "http";
            bool useLegacyPorts = string.IsNullOrEmpty(envUseLegacyPorts) || (envUseLegacyPorts == "1" || envUseLegacyPorts == "true");
            int edgePort = int.TryParse(envEdgePort, out int port) ? port : Constants.EdgePort;

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
}