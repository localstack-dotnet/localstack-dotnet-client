using LocalStack.Client.Contracts;

namespace LocalStack.Client.Models
{
    public class SessionOptions : ISessionOptions
    {
        private const string AwsAccessKeyIdConst = "accessKey";
        private const string AwsAccessKeyConst = "secretKey";
        private const string AwsSessionTokenConst = "token";
        private const string RegionNameConst = "us-east-1";

        public SessionOptions(
            string awsAccessKeyId = AwsAccessKeyIdConst,
            string awsAccessKey = AwsAccessKeyConst,
            string awsSessionToken = AwsSessionTokenConst,
            string regionName = RegionNameConst)
        {
            AwsAccessKeyId = awsAccessKeyId ?? AwsAccessKeyIdConst;
            AwsAccessKey = awsAccessKey ?? AwsAccessKeyConst;
            AwsSessionToken = awsSessionToken ?? AwsSessionTokenConst;
            RegionName = regionName ?? RegionNameConst;
        }

        public string AwsAccessKeyId { get; }

        public string AwsAccessKey { get; }

        public string AwsSessionToken { get; }

        public string RegionName { get; }
    }
}