using LocalStack.Client.Models;

using Xunit;

namespace LocalStack.Client.Tests
{
    public class ConstantsTests
    {
        [Fact]
        public void All_Constants_Should_Be_Equal_Appropriate_Defaults()
        {
            Assert.Equal("localhost", Constants.LocalStackHost);
            Assert.False(Constants.UseSsl);
            Assert.False(Constants.UseLegacyPorts);
            Assert.Equal(4566, Constants.EdgePort);
            Assert.Equal("accessKey", Constants.AwsAccessKeyId);
            Assert.Equal("secretKey", Constants.AwsAccessKey);
            Assert.Equal("token", Constants.AwsSessionToken);
            Assert.Equal("us-east-1", Constants.RegionName);
        }
    }
}
