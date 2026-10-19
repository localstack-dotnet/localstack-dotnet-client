namespace LocalStack.Client.Extensions.Tests;

public class AwsClientFactoryWrapperTests
{
    [Theory, InlineData(false), InlineData(true)]
    public void CreateServiceClient_Should_Use_Explicit_Options_Or_Provider_Defaults(bool explicitOptions)
    {
        var defaults = new AWSOptions { Credentials = new BasicAWSCredentials("default-key", "test-secret"), Region = RegionEndpoint.USWest2 };
        var options = new AWSOptions { Credentials = new BasicAWSCredentials("explicit-key", "test-secret"), Region = RegionEndpoint.EUWest1 };
        options.DefaultClientConfig.Timeout = TimeSpan.FromSeconds(17);
        var services = new ServiceCollection();
        services.AddDefaultAWSOptions(defaults);
        using ServiceProvider provider = services.BuildServiceProvider();

        using var client = (IAmazonS3)new AwsClientFactoryWrapper().CreateServiceClient<IAmazonS3>(provider, explicitOptions ? options : null);

        Assert.Equal(explicitOptions ? "eu-west-1" : "us-west-2", client.Config.RegionEndpoint.SystemName);
        if (explicitOptions)
        {
            Assert.Equal(TimeSpan.FromSeconds(17), client.Config.Timeout);
        }

        string url = client.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
        {
            BucketName = "test-bucket", Key = "test-key", Expires = DateTime.UtcNow.AddMinutes(1)
        });
        Assert.Contains(explicitOptions ? "explicit-key" : "default-key", url, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateServiceClient_Should_Not_Cache_Provider_Options_Or_Clients()
    {
        var firstServices = new ServiceCollection();
        firstServices.AddDefaultAWSOptions(new AWSOptions { Credentials = new AnonymousAWSCredentials(), Region = RegionEndpoint.USWest2 });
        var secondServices = new ServiceCollection();
        secondServices.AddDefaultAWSOptions(new AWSOptions { Credentials = new AnonymousAWSCredentials(), Region = RegionEndpoint.EUWest1 });
        using ServiceProvider firstProvider = firstServices.BuildServiceProvider();
        using ServiceProvider secondProvider = secondServices.BuildServiceProvider();
        var wrapper = new AwsClientFactoryWrapper();

        using var first = wrapper.CreateServiceClient<IAmazonS3>(firstProvider, null);
        using var second = wrapper.CreateServiceClient<IAmazonS3>(secondProvider, null);
        using var third = wrapper.CreateServiceClient<IAmazonS3>(firstProvider, null);

        Assert.Equal("us-west-2", first.Config.RegionEndpoint.SystemName);
        Assert.Equal("eu-west-1", second.Config.RegionEndpoint.SystemName);
        Assert.Equal("us-west-2", third.Config.RegionEndpoint.SystemName);
        Assert.NotSame(first, third);
    }

    [Fact]
    public void CreateServiceClient_Should_Preserve_Wrapped_Provider_Errors()
    {
        var failure = new InvalidOperationException("Provider failed.");
        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(Microsoft.Extensions.Logging.ILoggerFactory))).Throws(failure);
        var options = new AWSOptions { Credentials = new AnonymousAWSCredentials(), Region = RegionEndpoint.EUWest1 };

        TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
            new AwsClientFactoryWrapper().CreateServiceClient<IAmazonS3>(provider.Object, options));

        Assert.Same(failure, exception.InnerException);
    }

    [Theory, InlineData(ServiceLifetime.Singleton), InlineData(ServiceLifetime.Scoped), InlineData(ServiceLifetime.Transient)]
    public void CreateServiceClient_Should_Respect_The_Outer_Registration_Lifetime(ServiceLifetime lifetime)
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var options = new AWSOptions { Credentials = new AnonymousAWSCredentials(), Region = RegionEndpoint.EUWest1 };
        var services = new ServiceCollection();
        services.AddLocalStack(configuration).AddAwsService<IAmazonS3>(options, lifetime);
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope firstScope = provider.CreateScope();
        using IServiceScope secondScope = provider.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<IAmazonS3>();
        var repeated = firstScope.ServiceProvider.GetRequiredService<IAmazonS3>();
        var otherScope = secondScope.ServiceProvider.GetRequiredService<IAmazonS3>();

        Assert.Equal(lifetime != ServiceLifetime.Transient, ReferenceEquals(first, repeated));
        Assert.Equal(lifetime == ServiceLifetime.Singleton, ReferenceEquals(first, otherScope));
        Assert.Equal("eu-west-1", first.Config.RegionEndpoint.SystemName);
    }
}
