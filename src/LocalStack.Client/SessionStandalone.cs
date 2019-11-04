using LocalStack.Client.Contracts;
using LocalStack.Client.Models;
using LocalStack.Client.Utils;

namespace LocalStack.Client
{
    public class SessionStandalone : ISessionStandalone
    {
        private string _awsAccessKey;
        private string _awsAccessKeyId;
        private string _awsSessionToken;
        private string _localStackHost;
        private string _regionName;

        private SessionStandalone()
        {
        }

        public ISessionStandalone WithSessionOptions(string awsAccessKeyId = null, string awsAccessKey = null, string awsSessionToken = null, string regionName = null)
        {
            _awsAccessKeyId = awsAccessKeyId;
            _awsAccessKey = awsAccessKey;
            _awsSessionToken = awsSessionToken;
            _regionName = regionName;

            return this;
        }

        public ISessionStandalone WithConfig(string localStackHost = null)
        {
            _localStackHost = localStackHost;

            return this;
        }

        public ISession Create()
        {
            var sessionOptions = new SessionOptions(_awsAccessKeyId, _awsAccessKey, _awsSessionToken, _regionName);
            var config = new Config(_localStackHost);
            var sessionReflection = new SessionReflection();

            return new Session(sessionOptions, config, sessionReflection);
        }

        public static ISessionStandalone Init()
        {
            return new SessionStandalone();
        }
    }
}