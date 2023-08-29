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
        ISessionReflection sessionReflection = new SessionReflection();

        return new Session(sessionOptions, config, sessionReflection);
    }

    public static ISessionStandalone Init()
    {
        return new SessionStandalone();
    }
}