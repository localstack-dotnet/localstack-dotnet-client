using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace LocalStack.Client.Contracts
{
    public interface ISessionReflection
    {
        IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient;

        ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient;

        bool SetForcePathStyle(ClientConfig clientConfig, bool value = true);
    }
}