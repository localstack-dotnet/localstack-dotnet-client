using System;

using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace LocalStack.Client.Contracts
{
    public interface ISessionReflection
    {
        IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient;

        IServiceMetadata ExtractServiceMetadata(Type clientType);

        ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient;

        ClientConfig CreateClientConfig(Type clientType);

        bool SetForcePathStyle(ClientConfig clientConfig, bool value = true);
    }
}