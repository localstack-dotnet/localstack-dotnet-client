using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace LocalStack.Client.Contracts
{
    public interface ISessionReflection
    {
        IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient, new();

        ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient, new();

        bool SetForcePathStyle(ClientConfig clientConfig, bool value = true);
    }
}