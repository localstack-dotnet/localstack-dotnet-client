namespace LocalStack.Tests.Common.Mocks;

public class MockSession : Session
{
#if NETFRAMEWORK || NETSTANDARD
    private MockSession(Mock<ISessionOptions> sessionOptionsMock, Mock<IConfig> configMock, Mock<ISessionReflection> sessionReflectionMock) : base(
        sessionOptionsMock.Object, configMock.Object, sessionReflectionMock.Object)
    {
        SessionOptionsMock = sessionOptionsMock;
        ConfigMock = configMock;
        SessionReflectionMock = sessionReflectionMock;
    }
#elif NET8_0_OR_GREATER
    private MockSession(Mock<ISessionOptions> sessionOptionsMock, Mock<IConfig> configMock) : base(
        sessionOptionsMock.Object, configMock.Object)
    {
        SessionOptionsMock = sessionOptionsMock;
        ConfigMock = configMock;
    }
#endif

    public Mock<ISessionOptions> SessionOptionsMock { get; }

    public Mock<IConfig> ConfigMock { get; }

#if NETFRAMEWORK || NETSTANDARD
    public Mock<ISessionReflection> SessionReflectionMock { get; }

    public static MockSession Create()
    {
        return new MockSession(new Mock<ISessionOptions>(MockBehavior.Strict), new Mock<IConfig>(MockBehavior.Strict), new Mock<ISessionReflection>(MockBehavior.Strict));
    }
#elif NET8_0_OR_GREATER
    public static MockSession Create()
    {
        return new MockSession(new Mock<ISessionOptions>(MockBehavior.Strict), new Mock<IConfig>(MockBehavior.Strict));
    }
#endif
}