namespace LocalStack.Client.Extensions;

public sealed class AwsClientFactoryWrapper : IAwsClientFactoryWrapper
{
    public AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions? awsOptions) where TClient : IAmazonService
    {
        IServiceCollection services = new ServiceDescriptors();
        services.AddAWSService<TClient>(awsOptions!);
        Func<IServiceProvider, object> factory = services[0].ImplementationFactory ??
            throw new LocalStackClientConfigurationException($"AWS SDK registration did not provide a client factory for {typeof(TClient)}.");

        object serviceInstance;

        try
        {
            serviceInstance = factory(provider);
        }
        catch (Exception exception)
        {
            // Preserve the exception boundary of the former MethodInfo.Invoke call.
            throw new TargetInvocationException(exception);
        }

        return (AmazonServiceClient)serviceInstance;
    }

    // The netstandard2.0 dependency floor exposes IServiceCollection, but not ServiceCollection.
    private sealed class ServiceDescriptors : System.Collections.ObjectModel.Collection<ServiceDescriptor>, IServiceCollection
    {
        bool System.Collections.Generic.ICollection<ServiceDescriptor>.IsReadOnly => false;
    }
}
