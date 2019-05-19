using Amazon.APIGateway;
using Amazon.CloudFormation;
using Amazon.CloudSearch;
using Amazon.CloudWatch;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchLogs;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.ElasticLoadBalancing;
using Amazon.Elasticsearch;
using Amazon.IdentityManagement;
using Amazon.IoT;
using Amazon.KinesisFirehose;
using Amazon.Lambda;
using Amazon.RDS;
using Amazon.Redshift;
using Amazon.Route53;
using Amazon.S3;
using Amazon.SecretsManager;
using Amazon.SecurityToken;
using Amazon.SimpleEmail;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleWorkflow;
using Amazon.SQS;
using Amazon.StepFunctions;
using LocalStack.Client.Contracts;
using Xunit;

namespace LocalStack.Client.Integration.Tests
{
    public class ClientCreationTests
    {
        private static readonly ISession Session;

        static ClientCreationTests()
        {
            Session = SessionStandalone
                .Init()
                .Create();
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAPIGatewayClient()
        {
            var amazonApiGatewayClient = Session.CreateClient<AmazonAPIGatewayClient>();

            Assert.NotNull(amazonApiGatewayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonS3Client()
        {
            var amazonS3Client = Session.CreateClient<AmazonS3Client>();

            Assert.NotNull(amazonS3Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonDynamoDBClient()
        {
            var amazonDynamoDbClient = Session.CreateClient<AmazonDynamoDBClient>();

            Assert.NotNull(amazonDynamoDbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticsearchClient()
        {
            var amazonElasticsearchClient = Session.CreateClient<AmazonElasticsearchClient>();

            Assert.NotNull(amazonElasticsearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKinesisFirehoseClient()
        {
            var amazonKinesisFirehoseClient = Session.CreateClient<AmazonKinesisFirehoseClient>();

            Assert.NotNull(amazonKinesisFirehoseClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonLambdaClient()
        {
            var amazonLambdaClient = Session.CreateClient<AmazonLambdaClient>();

            Assert.NotNull(amazonLambdaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleNotificationServiceClient()
        {
            var amazonSimpleNotificationServiceClient = Session.CreateClient<AmazonSimpleNotificationServiceClient>();

            Assert.NotNull(amazonSimpleNotificationServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSQSClient()
        {
            var amazonSqsClient = Session.CreateClient<AmazonSQSClient>();

            Assert.NotNull(amazonSqsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRedshiftClient()
        {
            var amazonRedshiftClient = Session.CreateClient<AmazonRedshiftClient>();

            Assert.NotNull(amazonRedshiftClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleEmailServiceClient()
        {
            var amazonSimpleEmailServiceClient = Session.CreateClient<AmazonSimpleEmailServiceClient>();

            Assert.NotNull(amazonSimpleEmailServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRoute53Client()
        {
            var amazonRoute53Client = Session.CreateClient<AmazonRoute53Client>();

            Assert.NotNull(amazonRoute53Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFormationClient()
        {
            var amazonCloudFormationClient = Session.CreateClient<AmazonCloudFormationClient>();

            Assert.NotNull(amazonCloudFormationClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchClient()
        {
            var amazonCloudWatchClient = Session.CreateClient<AmazonCloudWatchClient>();

            Assert.NotNull(amazonCloudWatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleSystemsManagementClient()
        {
            var amazonSimpleSystemsManagementClient = Session.CreateClient<AmazonSimpleSystemsManagementClient>();

            Assert.NotNull(amazonSimpleSystemsManagementClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecretsManagerClient()
        {
            var amazonSecretsManagerClient = Session.CreateClient<AmazonSecretsManagerClient>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonStepFunctionsClient()
        {
            var amazonSecretsManagerClient = Session.CreateClient<AmazonStepFunctionsClient>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchLogsClient()
        {
            var amazonCloudWatchLogsClient = Session.CreateClient<AmazonCloudWatchLogsClient>();

            Assert.NotNull(amazonCloudWatchLogsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchEventsClient()
        {
            var amazonCloudWatchEventsClient = Session.CreateClient<AmazonCloudWatchEventsClient>();

            Assert.NotNull(amazonCloudWatchEventsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticLoadBalancingClient()
        {
            var amazonElasticLoadBalancingClient = Session.CreateClient<AmazonElasticLoadBalancingClient>();

            Assert.NotNull(amazonElasticLoadBalancingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIoTClient()
        {
            var amazonIoTClient = Session.CreateClient<AmazonIoTClient>();

            Assert.NotNull(amazonIoTClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityProviderClient()
        {
            var amazonCognitoIdentityProviderClient = Session.CreateClient<AmazonCognitoIdentityProviderClient>();

            Assert.NotNull(amazonCognitoIdentityProviderClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityClient()
        {
            var amazonCognitoIdentityClient = Session.CreateClient<AmazonCognitoIdentityClient>();

            Assert.NotNull(amazonCognitoIdentityClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecurityTokenServiceClient()
        {
            var amazonSecurityTokenServiceClient = Session.CreateClient<AmazonSecurityTokenServiceClient>();

            Assert.NotNull(amazonSecurityTokenServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIdentityManagementServiceClient()
        {
            var amazonIdentityManagementServiceClient = Session.CreateClient<AmazonIdentityManagementServiceClient>();

            Assert.NotNull(amazonIdentityManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRDSClient()
        {
            var amazonRdsClient = Session.CreateClient<AmazonRDSClient>();

            Assert.NotNull(amazonRdsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudSearchClient()
        {
            var amazonCloudSearchClient = Session.CreateClient<AmazonCloudSearchClient>();

            Assert.NotNull(amazonCloudSearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleWorkflowClient()
        {
            var amazonSimpleWorkflowClient = Session.CreateClient<AmazonSimpleWorkflowClient>();

            Assert.NotNull(amazonSimpleWorkflowClient);
        }
    }
}