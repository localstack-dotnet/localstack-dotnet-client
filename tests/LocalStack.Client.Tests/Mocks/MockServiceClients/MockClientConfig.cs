using Amazon.Runtime;
using Amazon.Util.Internal;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public class MockClientConfig : ClientConfig
    {
        public override string ServiceVersion => "1.0.0.0";
        public override string UserAgent => InternalSDKUtils.BuildUserAgentString(ServiceVersion);
        public override string RegionEndpointServiceName => "mock-service";
    }
}