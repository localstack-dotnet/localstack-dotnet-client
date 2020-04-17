using Amazon.Runtime;
using Amazon.Runtime.Internal;

using LocalStack.Client.Contracts;
using LocalStack.Client.Models;

using System;

namespace LocalStack.Client
{
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

        public TClient CreateClient<TClient>() where TClient : AmazonServiceClient
        {
            IServiceMetadata serviceMetadata = _sessionReflection.ExtractServiceMetadata<TClient>();
            AwsServiceEndpoint awsServiceEndpoint = _config.GetAwsServiceEndpoint(serviceMetadata.ServiceId) ??
                                                    throw new InvalidOperationException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

            AWSCredentials awsCredentials = new SessionAWSCredentials(_sessionOptions.AwsAccessKeyId, _sessionOptions.AwsAccessKey, _sessionOptions.AwsSessionToken);
            ClientConfig clientConfig = _sessionReflection.CreateClientConfig<TClient>();

            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl;
            clientConfig.UseHttp = true;
            _sessionReflection.SetForcePathStyle(clientConfig);
            clientConfig.ProxyHost = awsServiceEndpoint.Host;
            clientConfig.ProxyPort = awsServiceEndpoint.Port;

            var clientInstance = (TClient) Activator.CreateInstance(typeof(TClient), awsCredentials, clientConfig);

            return clientInstance;
        }
    }
}