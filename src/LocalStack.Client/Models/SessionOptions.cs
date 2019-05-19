using System;
using System.Collections.Generic;
using System.Text;
using LocalStack.Client.Contracts;

namespace LocalStack.Client.Models
{
    public class SessionOptions : ISessionOptions
    {
        public SessionOptions(
            string awsAccessKeyId = "accessKey", 
            string awsAccessKey = "secretKey", 
            string awsSessionToken = "token", 
            string regionName = "us-east-1", 
            string localStackHost = null)
        {
            AwsAccessKeyId = awsAccessKeyId;
            AwsAccessKey = awsAccessKey;
            AwsSessionToken = awsSessionToken;
            RegionName = regionName;
            LocalStackHost = localStackHost;
        }

        public string AwsAccessKeyId { get; }

        public string AwsAccessKey { get; }

        public string AwsSessionToken { get; }

        public string RegionName { get; }

        public string LocalStackHost { get; }
    }
}
