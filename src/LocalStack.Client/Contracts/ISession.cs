using Amazon.Runtime;

namespace LocalStack.Client.Contracts
{
    public interface ISession
    {
        TClient CreateClient<TClient>() where TClient : AmazonServiceClient;
    }
}