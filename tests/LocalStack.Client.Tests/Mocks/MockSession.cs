namespace LocalStack.Client.Tests.Mocks;

public class MockSession : Session
{
    private MockSession(Mock<ISessionOptions> sessionOptionsMock, Mock<IConfig> configMock, Mock<ISessionReflection> sessionReflectionMock) : base(
        sessionOptionsMock.Object, configMock.Object, sessionReflectionMock.Object)
    {
        SessionOptionsMock = sessionOptionsMock;
        ConfigMock = configMock;
        SessionReflectionMock = sessionReflectionMock;
    }

    public Mock<ISessionOptions> SessionOptionsMock { get; }

    public Mock<IConfig> ConfigMock { get; }

    public Mock<ISessionReflection> SessionReflectionMock { get; }

    public static MockSession Create()
    {
        return new MockSession(new Mock<ISessionOptions>(MockBehavior.Strict), new Mock<IConfig>(MockBehavior.Strict),
                               new Mock<ISessionReflection>(MockBehavior.Strict));
    }
}
