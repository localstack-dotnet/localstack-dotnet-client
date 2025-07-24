namespace LocalStack.Client;

public class SessionStandalone : ISessionStandalone
{
    private ISessionOptions? _sessionOptions;
    private IConfigOptions? _configOptions;

    private SessionStandalone()
    {
    }

    public ISessionStandalone WithSessionOptions(ISessionOptions sessionOptions)
    {
        _sessionOptions = sessionOptions;

        return this;
    }

    public ISessionStandalone WithConfigurationOptions(IConfigOptions configOptions)
    {
        _configOptions = configOptions;

        return this;
    }

    public ISession Create()
    {
        ISessionOptions sessionOptions = _sessionOptions ?? new SessionOptions();
        IConfig config = new Config(_configOptions ?? new ConfigOptions());

#if NETFRAMEWORK || NETSTANDARD
        ISessionReflection sessionReflection = new SessionReflection();

        return new Session(sessionOptions, config, sessionReflection);
#elif NET8_0_OR_GREATER
        return new Session(sessionOptions, config);
#else
        throw new NotSupportedException("This library is only supported on .NET Framework, .NET Standard, and .NET 8.0 and above.");
#endif
    }

    public static ISessionStandalone Init()
    {
        return new SessionStandalone();
    }
}