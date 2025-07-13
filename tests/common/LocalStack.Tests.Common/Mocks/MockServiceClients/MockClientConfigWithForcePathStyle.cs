namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

public class MockClientConfigWithForcePathStyle : MockClientConfig
{
    public MockClientConfigWithForcePathStyle(IDefaultConfigurationProvider configurationProvider, bool forcePathStyle) : base(configurationProvider)
    {
        ForcePathStyle = forcePathStyle;
    }

    public bool ForcePathStyle { get; set; }

    public static MockClientConfigWithForcePathStyle CreateDefaultMockClientConfigWithForcePathStyle() => new(new MockConfigurationProvider(), forcePathStyle: false);
}