using System;

namespace LocalStack.Client.Contracts
{
    public interface ISessionStandalone
    {
        [Obsolete("This method is obsolete, use WithSessionOptions with ISessionOptions parameter")]
        ISessionStandalone WithSessionOptions(string awsAccessKeyId = null, string awsAccessKey = null, string awsSessionToken = null, string regionName = null);

        [Obsolete("This method is obsolete, use WithConfig")]
        ISessionStandalone WithConfig(string localStackHost = null);

        ISessionStandalone WithSessionOptions(ISessionOptions sessionOptions);

        ISessionStandalone WithConfigurationOptions(IConfigOptions configOptions);

        ISession Create();
    }
}