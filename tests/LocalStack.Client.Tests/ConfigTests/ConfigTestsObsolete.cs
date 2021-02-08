using System;
using System.Collections.Generic;
using System.Linq;

using LocalStack.Client.Enums;
using LocalStack.Client.Models;
using LocalStack.Client.Options;

using Xunit;

namespace LocalStack.Client.Tests.ConfigTests
{
    [Obsolete("Will be removed in next version")]
    public class ConfigTestsObsolete
    {
        [Fact]
        public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Host_Property_Equals_To_Given_LocalStackHost_Parameter()
        {
            const string localStackHost = "myLocalHost";

            var config = new Config(localStackHost);
            AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(localStackHost, awsServiceEndpoint.Host);

            awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(localStackHost, awsServiceEndpoint.Host);
        }

        [Fact]
        public void GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Host_Property_Equals_To_Setted_LocalStackHost_Environment_Variable()
        {
            const string localStackHost = "myLocalHost";
            Environment.SetEnvironmentVariable("LOCALSTACK_HOST", localStackHost);

            var config = new Config(localStackHost);
            AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(localStackHost, awsServiceEndpoint.Host);

            awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(localStackHost, awsServiceEndpoint.Host);
        }

        [Fact]
        public void GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Host_Property_Of_Every_Item_Equals_To_Given_LocalStackHost_Parameter()
        {
            const string localStackHost = "myLocalHost";

            var config = new Config(localStackHost);
            IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

            Assert.NotNull(awsServiceEndpoints);
            Assert.NotEmpty(awsServiceEndpoints);
            Assert.All(awsServiceEndpoints, endpoint => Assert.Equal(localStackHost, endpoint.Host));
        }

        [Fact]
        public void GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Host_Property_Of_Every_Item_Equals_To_Setted_LocalStackHost_Environment_Variable()
        {
            const string localStackHost = "myLocalHost";
            Environment.SetEnvironmentVariable("LOCALSTACK_HOST", localStackHost);

            var config = new Config(new ConfigOptions(localStackHost));
            IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

            Assert.NotNull(awsServiceEndpoints);
            Assert.NotEmpty(awsServiceEndpoints);
            Assert.All(awsServiceEndpoints, endpoint => Assert.Equal(localStackHost, endpoint.Host));
        }

        [Theory, InlineData("true", "https:"), InlineData("1", "https:"), InlineData("false", "http:"), InlineData("0", "http:"), InlineData(null, "http:")]
        public void
            GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Protocol_Property_Equals_To_Https_Or_Http_If_Given_UseSsl_Environment_Variable_Equals_1_Or_True_Or_0_False(
                string envValue, string expectedProtocol)
        {
            Environment.SetEnvironmentVariable("USE_SSL", envValue);

            var config = new Config(default(string));
            AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

            Assert.NotNull(awsServiceEndpoint);
            Assert.StartsWith(expectedProtocol, awsServiceEndpoint.ServiceUrl);

            awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

            Assert.NotNull(awsServiceEndpoint);
            Assert.StartsWith(expectedProtocol, awsServiceEndpoint.ServiceUrl);
        }

        [Theory, InlineData("true", "https:"), InlineData("1", "https:"), InlineData("false", "http:"), InlineData("0", "http:")]
        public void
            GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Protocol_Property_Of_Every_Item_Equals_To_Https_Or_Http_If_Given_UseSsl_Environment_Variable_Equals_1_Or_True_Or_0_Or_False(
                string envUseSsl, string expectedProtocol)
        {
            Environment.SetEnvironmentVariable("USE_SSL", envUseSsl);

            var config = new Config(default(string));
            IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

            Assert.NotNull(awsServiceEndpoints);
            Assert.NotEmpty(awsServiceEndpoints);
            Assert.All(awsServiceEndpoints, endpoint => Assert.StartsWith(expectedProtocol, endpoint.ServiceUrl));
        }

        [Theory, InlineData("false", "1234", 1234), InlineData("0", "4564", 4564), InlineData("false", null, Constants.EdgePort), InlineData("0", null, Constants.EdgePort)]
        public void
            GetAwsServiceEndpoint_Should_Return_AwsServiceEndpoint_That_Port_Property_Equals_To_Given_EdgePort_Environment_Variable_If_UseLegacyPorts_Environment_Variable_Equals_0_Or_False(
                string envUseLegacyPorts, string envPort, int expectedPort)
        {
            Environment.SetEnvironmentVariable("USE_LEGACY_PORTS", envUseLegacyPorts);
            Environment.SetEnvironmentVariable("EDGE_PORT", envPort);

            var config = new Config(default(string));
            AwsServiceEndpoint awsServiceEndpoint = config.GetAwsServiceEndpoint(AwsServiceEnum.ApiGateway);

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(expectedPort, awsServiceEndpoint.Port);

            awsServiceEndpoint = config.GetAwsServiceEndpoint("ApiGatewayV2");

            Assert.NotNull(awsServiceEndpoint);
            Assert.Equal(expectedPort, awsServiceEndpoint.Port);
        }

        [Theory, InlineData("false", "1234", 1234), InlineData("0", "4564", 4564), InlineData("false", null, Constants.EdgePort), InlineData("0", null, Constants.EdgePort)]
        public void
            GetAwsServiceEndpoints_Should_Return_List_Of_AwsServiceEndpoint_That_Port_Property_Of_Every_Item_Equals_To_Given_EdgePort_Environment_Variable_If_UseLegacyPorts_Environment_Variable_Equals_0_Or_False(
                string envUseLegacyPorts, string envPort, int expectedPort)
        {
            Environment.SetEnvironmentVariable("USE_LEGACY_PORTS", envUseLegacyPorts);
            Environment.SetEnvironmentVariable("EDGE_PORT", envPort);

            var config = new Config(default(string));
            IList<AwsServiceEndpoint> awsServiceEndpoints = config.GetAwsServiceEndpoints().ToList();

            Assert.NotNull(awsServiceEndpoints);
            Assert.NotEmpty(awsServiceEndpoints);
            Assert.All(awsServiceEndpoints, endpoint => Assert.Equal(expectedPort, endpoint.Port));
        }
    }
}
