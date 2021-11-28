namespace LocalStack.Client.Contracts;

public interface ISession
{
    TClient CreateClientByImplementation<TClient>(bool useServiceUrl = false) where TClient : AmazonServiceClient;

    AmazonServiceClient CreateClientByImplementation(Type implType, bool useServiceUrl = false);

    AmazonServiceClient CreateClientByInterface<TClient>(bool useServiceUrl = false) where TClient: IAmazonService;

    AmazonServiceClient CreateClientByInterface(Type serviceInterfaceType, bool useServiceUrl = false);
}