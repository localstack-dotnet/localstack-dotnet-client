namespace LocalStack.Client.Contracts;

public interface ISessionStandalone
{
    ISessionStandalone WithSessionOptions(ISessionOptions sessionOptions);

    ISessionStandalone WithConfigurationOptions(IConfigOptions configOptions);

    ISession Create();
}