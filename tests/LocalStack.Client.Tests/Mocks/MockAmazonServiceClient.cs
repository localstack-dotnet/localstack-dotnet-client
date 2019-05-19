using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Util.Internal;
using LocalStack.Client.Models;
using Xunit.Sdk;

namespace LocalStack.Client.Tests.Mocks
{
    public class MockAmazonServiceClient : AmazonServiceClient, IAmazonService
    {
        public MockAmazonServiceClient()
            : base(FallbackCredentialsFactory.GetCredentials(), new MockClientConfig())
        {
        }

        public MockAmazonServiceClient(AWSCredentials credentials, ClientConfig config) : base(credentials, config)
        {
        }

        public MockAmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, ClientConfig config) : base(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, config)
        {
        }

        public MockAmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, ClientConfig config) : base(awsAccessKeyId, awsSecretAccessKey, config)
        {
        }

        public AWSCredentials AwsCredentials => Credentials;

        protected override AbstractAWSSigner CreateSigner() => new NullSigner();
    }

    public class MockClientConfig : ClientConfig
    {
        public override string ServiceVersion => "1.0.0.0";
        public override string UserAgent => InternalSDKUtils.BuildUserAgentString(ServiceVersion);
        public override string RegionEndpointServiceName => "mock-service";
    }

    public class MockClientConfigWithForcePathStyle : MockClientConfig
    {
        public bool ForcePathStyle { get; set; } = false;
    }

    public class MockServiceMetadata : IServiceMetadata
    {
        public const string MockServiceId = "Mock Amazon Service";

        public string ServiceId => MockServiceId;

        public IDictionary<string, string> OperationNameMapping { get; }
    }

    public class MockAwsServiceEndpoint : AwsServiceEndpoint
    {
        public MockAwsServiceEndpoint() 
            : base(MockServiceMetadata.MockServiceId, "mockService", AwsServiceEnum.ApiGateway, 1234, "localhost", "http://localhost:1234")
        {
        }
    }
}
