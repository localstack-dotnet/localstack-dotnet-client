using System;

using LocalStack.Client.Contracts;
using LocalStack.Client.Models;
using LocalStack.Client.Options;
using LocalStack.Client.Utils;

namespace LocalStack.Client
{
    public class SessionStandalone : ISessionStandalone
    {
        private ISessionOptions _sessionOptions;
        private IConfigOptions _configOptions;

        private SessionStandalone()
        {
        }

        [Obsolete("This method is obsolete, use WithSessionOptions with ISessionOptions parameter")]
        public ISessionStandalone WithSessionOptions(string awsAccessKeyId = null, string awsAccessKey = null, string awsSessionToken = null, string regionName = null)
        {
            _sessionOptions = new SessionOptions(awsAccessKeyId, awsAccessKey, awsSessionToken, regionName);

            return this;
        }

        [Obsolete("This method is obsolete, use WithConfig")]
        public ISessionStandalone WithConfig(string localStackHost = null)
        {
            _configOptions = new ConfigOptions(localStackHost);

            return this;
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
}