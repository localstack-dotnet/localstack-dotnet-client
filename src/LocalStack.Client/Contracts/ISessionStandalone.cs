namespace LocalStack.Client.Contracts
{
    public interface ISessionStandalone
    {
        ISessionStandalone WithSessionOptions(string awsAccessKeyId = null, string awsAccessKey = null, string awsSessionToken = null, string regionName = null);

        ISessionStandalone WithConfig(string localStackHost = null);

        ISession Create();
    }
}