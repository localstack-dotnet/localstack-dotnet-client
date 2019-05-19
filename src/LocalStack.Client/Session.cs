using System;
using System.Linq;
using System.Reflection;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using LocalStack.Client.Contracts;
using LocalStack.Client.Models;

namespace LocalStack.Client
{
    public class Session : ISession
    {
        private readonly string _awsAccessKeyId;
        private readonly string _awsAccessKey;
        private readonly string _awsSessionToken;
        private readonly string _regionName;
        private readonly AwsServiceEndpoint[] _awsServiceEndpoints;

        public Session(string awsAccessKeyId = "accessKey", string awsAccessKey = "secretKey", string awsSessionToken = "token", string regionName = "us-east-1", string localStackHost = null)
        {
            _awsAccessKeyId = awsAccessKeyId;
            _awsAccessKey = awsAccessKey;
            _awsSessionToken = awsSessionToken;
            _regionName = regionName;

            _awsServiceEndpoints = Config.GetAwsServiceEndpoints(localStackHost).ToArray();
        }

        public TClient GetClient<TClient>() where TClient : AmazonServiceClient, new()
        {
            var clientType = typeof(TClient);

            var serviceMetadataField = clientType
                .GetField("serviceMetadata", BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Invalid service type {typeof(TClient)}");

            var clientConstructorInfo = FindConstructor(clientType) 
                                        ?? throw new InvalidOperationException($"Invalid service type {typeof(TClient)}");


            var serviceMetadata = serviceMetadataField.GetValue(null) as IServiceMetadata 
                                  ?? throw new InvalidOperationException("Invalid service metadata");


            var awsServiceEndpoint = _awsServiceEndpoints
                                         .SingleOrDefault(endpoint => endpoint.ServiceId == serviceMetadata.ServiceId)
                                     ?? throw new InvalidOperationException($"{serviceMetadata.ServiceId} is not supported by this mock session.");

            var clientConfigParam = clientConstructorInfo.GetParameters()[1];

            AWSCredentials awsCredentials = new SessionAWSCredentials(_awsAccessKeyId, _awsAccessKey, _awsSessionToken);

            var clientConfigObj = Activator.CreateInstance(clientConfigParam.ParameterType);
            var clientConfig = (ClientConfig) clientConfigObj;
            clientConfig.ServiceURL = awsServiceEndpoint.ServiceUrl;
            clientConfig.UseHttp = true;
            SetForcePathStyle(clientConfigObj);
            clientConfig.ProxyHost = awsServiceEndpoint.Host;
            clientConfig.ProxyPort = awsServiceEndpoint.Port;


            var clientInstance = (TClient)Activator.CreateInstance(clientType, awsCredentials, clientConfig);

            return clientInstance;
        }

        private ConstructorInfo FindConstructor(Type clientType)
        {
            return clientType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(info =>
                {
                    var parameterInfos = info.GetParameters();

                    if (parameterInfos.Length != 2)
                    {
                        return false;
                    }

                    ParameterInfo credentialsParameter = parameterInfos[0];
                    ParameterInfo clientConfigParameter = parameterInfos[1];

                    return credentialsParameter.Name == "credentials"
                           && credentialsParameter.ParameterType == typeof(AWSCredentials)
                           && clientConfigParameter.Name == "clientConfig"
                           && clientConfigParameter.ParameterType.IsSubclassOf(typeof(ClientConfig));

                }).SingleOrDefault();

        }

        private void SetForcePathStyle(object clientConfig, bool value = true)
        {
            var forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

            if (forcePathStyleProperty == null)
            {
                return;
            }

            forcePathStyleProperty.SetValue(clientConfig, value);
        }
    }
}
