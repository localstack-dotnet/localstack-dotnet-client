using Amazon.Runtime;

namespace LocalStack.Client.Contracts
{
    public interface ISession
    {
        TClient GetClient<TClient>() where TClient : AmazonServiceClient, new();
    }
}