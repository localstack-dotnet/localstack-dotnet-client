using Amazon.Runtime;
using Amazon.Runtime.Internal;
using LocalStack.Client.Tests.Mocks;
using Moq;
using System;
using Xunit;

namespace LocalStack.Client.Tests
{
    public class SessionTests
    {
        [Fact]
        public void CreateClient_Should_Throw_InvalidOperationException_If_Given_ServiceId_Is_Not_Supported()
        {
            MockSession mockSession = MockSession.Create();
            IServiceMetadata mockServiceMetadata = new MockServiceMetadata();

            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>())
                .Returns(() => mockServiceMetadata);
            mockSession.ConfigMock
                .Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>()))
                .Returns(() => null);

            Assert.Throws<InvalidOperationException>(() => mockSession.CreateClient<MockAmazonServiceClient>());

            mockSession.ConfigMock
                .Verify(
                    config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId =>
                        serviceId == mockServiceMetadata.ServiceId)), Times.Once);
        }

        [Fact]
        public void
            CreateClient_Should_Create_SessionAWSCredentials_With_AwsAccessKeyId_And_AwsAccessKey_And_AwsSessionToken()
        {
            MockSession mockSession = MockSession.Create();
            IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
            var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
            var mockClientConfig = new MockClientConfig();
            const string awsAccessKeyId = "AwsAccessKeyId";
            const string awsAccessKey = "AwsAccessKey";
            const string awsSessionToken = "AwsSessionToken";


            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKeyId).Returns(awsAccessKeyId);
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKey).Returns(awsAccessKey);
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsSessionToken).Returns(awsSessionToken);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>())
                .Returns(() => mockServiceMetadata);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.CreateClientConfig<MockAmazonServiceClient>())
                .Returns(() => mockClientConfig);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true))
                .Returns(() => true);
            mockSession.ConfigMock
                .Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>()))
                .Returns(() => mockAwsServiceEndpoint);

            var mockAmazonServiceClient = mockSession.CreateClient<MockAmazonServiceClient>();
            Assert.NotNull(mockAmazonServiceClient);

            AWSCredentials awsCredentials = mockAmazonServiceClient.AwsCredentials;
            Assert.NotNull(awsCredentials);
            Assert.IsType<SessionAWSCredentials>(awsCredentials);

            var sessionAwsCredentials = (SessionAWSCredentials)awsCredentials;
            ImmutableCredentials immutableCredentials = sessionAwsCredentials.GetCredentials();
            Assert.Equal(awsAccessKeyId, immutableCredentials.AccessKey);
            Assert.Equal(awsAccessKey, immutableCredentials.SecretKey);
            Assert.Equal(awsSessionToken, immutableCredentials.Token);
        }

        [Fact]
        public void CreateClient_Should_Create_ClientConfig_With_ServiceURL_UseHttp_ProxyHost_ProxyPort()
        {
            MockSession mockSession = MockSession.Create();
            IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
            var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
            var mockClientConfig = new MockClientConfig();

            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKeyId).Returns("AwsAccessKeyId");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKey).Returns("AwsAccessKey");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsSessionToken).Returns("AwsSessionToken");
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>())
                .Returns(() => mockServiceMetadata);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.CreateClientConfig<MockAmazonServiceClient>())
                .Returns(() => mockClientConfig);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true))
                .Returns(() => true);
            mockSession.ConfigMock
                .Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>()))
                .Returns(() => mockAwsServiceEndpoint);

            var mockAmazonServiceClient = mockSession.CreateClient<MockAmazonServiceClient>();
            Assert.NotNull(mockAmazonServiceClient);

            IClientConfig clientConfig = mockAmazonServiceClient.Config;
            Assert.Equal(mockAwsServiceEndpoint.ServiceUrl, clientConfig.ServiceURL);
            Assert.True(clientConfig.UseHttp);
            Assert.Equal(mockAwsServiceEndpoint.Host, clientConfig.ProxyHost);
            Assert.Equal(mockAwsServiceEndpoint.Port, clientConfig.ProxyPort);
        }

        [Fact]
        public void CreateClient_Should_Pass_The_ClientConfig_To_SetForcePathStyle()
        {
            MockSession mockSession = MockSession.Create();
            IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
            var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
            var mockClientConfig = new MockClientConfig();

            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKeyId).Returns("AwsAccessKeyId");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKey).Returns("AwsAccessKey");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsSessionToken).Returns("AwsSessionToken");
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>())
                .Returns(() => mockServiceMetadata);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.CreateClientConfig<MockAmazonServiceClient>())
                .Returns(() => mockClientConfig);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true))
                .Returns(() => true);
            mockSession.ConfigMock
                .Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>()))
                .Returns(() => mockAwsServiceEndpoint);

            mockSession.CreateClient<MockAmazonServiceClient>();

            mockSession.SessionReflectionMock
                .Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
        }

        [Fact]
        public void CreateClient_Should_Create_AmazonServiceClient_By_Given_Generic_Type()
        {
            MockSession mockSession = MockSession.Create();
            IServiceMetadata mockServiceMetadata = new MockServiceMetadata();
            var mockAwsServiceEndpoint = new MockAwsServiceEndpoint();
            var mockClientConfig = new MockClientConfig();

            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKeyId).Returns("AwsAccessKeyId");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKey).Returns("AwsAccessKey");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsSessionToken).Returns("AwsSessionToken");

            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKeyId).Returns("AwsAccessKeyId");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsAccessKey).Returns("AwsAccessKey");
            mockSession.SessionOptionsMock.SetupGet(options => options.AwsSessionToken).Returns("AwsSessionToken");
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>())
                .Returns(() => mockServiceMetadata);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.CreateClientConfig<MockAmazonServiceClient>())
                .Returns(() => mockClientConfig);
            mockSession.SessionReflectionMock
                .Setup(reflection => reflection.SetForcePathStyle(mockClientConfig, true))
                .Returns(() => true);
            mockSession.ConfigMock
                .Setup(config => config.GetAwsServiceEndpoint(It.IsAny<string>()))
                .Returns(() => mockAwsServiceEndpoint);

            var mockAmazonServiceClient = mockSession.CreateClient<MockAmazonServiceClient>();
            Assert.NotNull(mockAmazonServiceClient);

            mockSession.ConfigMock
                .Verify(config => config.GetAwsServiceEndpoint(It.Is<string>(serviceId =>serviceId == mockServiceMetadata.ServiceId)), Times.Once);
            mockSession.SessionReflectionMock
                .Verify(reflection => reflection.ExtractServiceMetadata<MockAmazonServiceClient>(), Times.Once);
            mockSession.SessionReflectionMock
                .Verify(reflection => reflection.CreateClientConfig<MockAmazonServiceClient>(), Times.Once);
            mockSession.SessionReflectionMock
                .Verify(reflection => reflection.SetForcePathStyle(It.Is<ClientConfig>(config => config == mockClientConfig), true), Times.Once);
        }
    }
}
