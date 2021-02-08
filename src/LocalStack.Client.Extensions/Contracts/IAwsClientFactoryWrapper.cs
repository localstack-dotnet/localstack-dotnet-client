using System;

using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;

namespace LocalStack.Client.Extensions.Contracts
{
    public interface IAwsClientFactoryWrapper
    {
        AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions awsOptions) where TClient : IAmazonService;
    }
}