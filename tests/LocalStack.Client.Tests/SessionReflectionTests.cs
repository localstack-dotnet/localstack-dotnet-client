using Amazon.Runtime;
using Amazon.Runtime.Internal;
using LocalStack.Client.Tests.Mocks.MockServiceClients;
using LocalStack.Client.Utils;
using System;
using Xunit;

namespace LocalStack.Client.Tests
{
    public class SessionReflectionTests
    {
        [Fact]
        public void
            ExtractServiceMetadata_Should_Throw_InvalidOperationException_If_Given_Generic_Service_Client_Type_Has_Not_Service_Metadata_Field()
        {
            var sessionReflection = new SessionReflection();

            Assert.Throws<InvalidOperationException>(() => sessionReflection.ExtractServiceMetadata<MockAmazonServiceClient>());
        }

        [Fact]
        public void ExtractServiceMetadata_Should_Return_Extracted_ServiceMetadata()
        {
            var sessionReflection = new SessionReflection();

            IServiceMetadata serviceMetadata = sessionReflection.ExtractServiceMetadata<MockAmazonServiceClientWithServiceMetadata>();

            Assert.NotNull(serviceMetadata);
            Assert.Equal(MockServiceMetadata.MockServiceId, serviceMetadata.ServiceId);
        }

        [Fact]
        public void CreateClientConfig_Should_Create_ClientConfig_By_Given_Generic_Service_Client_Type()
        {
            var sessionReflection = new SessionReflection();

            ClientConfig clientConfig = sessionReflection.CreateClientConfig<MockAmazonServiceClient>();

            Assert.NotNull(clientConfig);
            Assert.IsType<MockClientConfig>(clientConfig);
        }

        [Fact]
        public void SetForcePathStyle_Should_Return_False_If_Given_ClientConfig_Does_Not_Have_ForcePathStyle()
        {
            var sessionReflection = new SessionReflection();
            var clientConfig = new MockClientConfig();

            bool set = sessionReflection.SetForcePathStyle(clientConfig, true);

            Assert.False(set);
        }

        [Fact]
        public void SetForcePathStyle_Should_Set_ForcePathStyle_Of_ClientConfig_If_It_Exists()
        {
            var sessionReflection = new SessionReflection();
            var clientConfig = new MockClientConfigWithForcePathStyle();

            Assert.False(clientConfig.ForcePathStyle);

            bool set = sessionReflection.SetForcePathStyle(clientConfig, true);

            Assert.True(set);
            Assert.True(clientConfig.ForcePathStyle);
        }
    }
}