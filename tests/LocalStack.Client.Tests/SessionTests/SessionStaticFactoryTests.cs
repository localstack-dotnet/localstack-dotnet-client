#if NET10_0_OR_GREATER
namespace LocalStack.Client.Tests.SessionTests;

public class SessionStaticFactoryTests
{
    [Fact]
    public void Default_reflection_uses_static_factory_without_client_naming_convention()
    {
        Session session = CreateSession();

        using AmazonServiceClient client = session.CreateClientByInterface<IMockFactoryAmazonService>();

        Assert.IsType<FactoryCreatedMockClient>(client);
        Assert.True(client.Config.UseHttp);
        Assert.Equal("localhost", client.Config.ProxyHost);
        Assert.Equal(1234, client.Config.ProxyPort);
        Assert.Equal(RegionEndpoint.EUWest2, client.Config.RegionEndpoint);
    }

    [Fact]
    public void Default_reflection_wraps_static_factory_failure_preserving_exception_identity()
    {
        Session session = CreateSession();

        TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() => session.CreateClientByInterface<IMockThrowingFactoryAmazonService>());

        Assert.Same(IMockThrowingFactoryAmazonService.Failure, exception.InnerException);
    }

    [Fact]
    public void Static_factory_receives_the_configured_instance_with_service_url()
    {
        Session session = CreateSession();
        FactoryCreatedMockClient.RecordedConfig = null;

        using AmazonServiceClient client = session.CreateClientByInterface<IMockFactoryAmazonService>(useServiceUrl: true);

        Assert.Same(FactoryCreatedMockClient.RecordedConfig, client.Config);
        Assert.Equal("http://localhost:1234/", client.Config.ServiceURL);
    }

    private static Session CreateSession()
    {
        var config = new Mock<Contracts.IConfig>(MockBehavior.Strict);
        config.Setup(value => value.GetAwsServiceEndpoint(MockServiceMetadata.MockServiceId)).Returns(new MockAwsServiceEndpoint());
        config.Setup(value => value.GetConfigOptions()).Returns(new ConfigOptions());
        return new Session(new SessionOptions(regionName: "eu-west-2"), config.Object, new SessionReflection());
    }
}
#endif
