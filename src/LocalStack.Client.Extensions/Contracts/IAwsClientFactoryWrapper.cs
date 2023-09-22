namespace LocalStack.Client.Extensions.Contracts;

public interface IAwsClientFactoryWrapper
{
    AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions? awsOptions) where TClient : IAmazonService;
}