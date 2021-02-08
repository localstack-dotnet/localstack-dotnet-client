using System;

using Amazon.Runtime;

namespace LocalStack.Client.Contracts
{
    public interface ISession
    {
        [Obsolete("This method is obsolete, use CreateClientByImplementation")]
        TClient CreateClient<TClient>() where TClient : AmazonServiceClient;

        TClient CreateClientByImplementation<TClient>() where TClient : AmazonServiceClient;

        AmazonServiceClient CreateClientByImplementation(Type implType);

        AmazonServiceClient CreateClientByInterface<TClient>() where TClient: IAmazonService;

        AmazonServiceClient CreateClientByInterface(Type serviceInterfaceType);
    }
}