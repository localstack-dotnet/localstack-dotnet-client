using System;

using Amazon.APIGateway;
using Amazon.ApiGatewayV2;
using Amazon.AppSync;
using Amazon.Athena;
using Amazon.AutoScaling;
using Amazon.Batch;
using Amazon.CloudFormation;
using Amazon.CloudFront;
using Amazon.CloudSearch;
using Amazon.CloudTrail;
using Amazon.CloudWatch;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchLogs;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.EC2;
using Amazon.ECR;
using Amazon.ECS;
using Amazon.EKS;
using Amazon.ElastiCache;
using Amazon.ElasticBeanstalk;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticMapReduce;
using Amazon.Elasticsearch;
using Amazon.Glue;
using Amazon.IdentityManagement;
using Amazon.IoT;
using Amazon.KeyManagementService;
using Amazon.KinesisFirehose;
using Amazon.Lambda;
using Amazon.MediaStore;
using Amazon.MediaStoreData;
using Amazon.Organizations;
using Amazon.QLDB;
using Amazon.RDS;
using Amazon.RDSDataService;
using Amazon.Redshift;
using Amazon.Route53;
using Amazon.S3;
using Amazon.SageMaker;
using Amazon.SageMakerRuntime;
using Amazon.SecretsManager;
using Amazon.SecurityToken;
using Amazon.SimpleEmail;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleWorkflow;
using Amazon.SQS;
using Amazon.StepFunctions;
using Amazon.XRay;

using LocalStack.Client.Contracts;

using Xunit;

namespace LocalStack.Client.Integration.Tests
{
    [Obsolete("Will be removed in next version")]
    public class ClientCreationTestsObsolete
    {
        private static readonly ISession Session;

        static ClientCreationTestsObsolete()
        {
            Session = SessionStandalone.Init().Create();
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAPIGatewayClient()
        {
            var amazonApiGatewayClient = Session.CreateClient<AmazonAPIGatewayClient>();

            Assert.NotNull(amazonApiGatewayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonApiGatewayV2Client()
        {
            var amazonApiGatewayV2Client = Session.CreateClient<AmazonApiGatewayV2Client>();

            Assert.NotNull(amazonApiGatewayV2Client);
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
        public void Should_Able_To_Create_AmazonRDSDataServiceClient()
        {
            var amazonRdsDataServiceClient = Session.CreateClient<AmazonRDSDataServiceClient>();

            Assert.NotNull(amazonRdsDataServiceClient);
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

        [Fact]
        public void Should_Able_To_Create_AmazonEC2Client()
        {
            var amazonEc2Client = Session.CreateClient<AmazonEC2Client>();

            Assert.NotNull(amazonEc2Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElastiCacheClient()
        {
            var amazonElastiCacheClient = Session.CreateClient<AmazonElastiCacheClient>();

            Assert.NotNull(amazonElastiCacheClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKeyManagementServiceClient()
        {
            var amazonKeyManagementServiceClient = Session.CreateClient<AmazonKeyManagementServiceClient>();

            Assert.NotNull(amazonKeyManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticMapReduceClient()
        {
            var amazonElasticMapReduceClient = Session.CreateClient<AmazonElasticMapReduceClient>();

            Assert.NotNull(amazonElasticMapReduceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECSClient()
        {
            var amazonEcsClient = Session.CreateClient<AmazonECSClient>();

            Assert.NotNull(amazonEcsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonEKSClient()
        {
            var amazonEksClient = Session.CreateClient<AmazonEKSClient>();

            Assert.NotNull(amazonEksClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonXRayClient()
        {
            var amazonXRayClient = Session.CreateClient<AmazonXRayClient>();

            Assert.NotNull(amazonXRayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticBeanstalkClient()
        {
            var amazonElasticBeanstalkClient = Session.CreateClient<AmazonElasticBeanstalkClient>();

            Assert.NotNull(amazonElasticBeanstalkClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAppSyncClient()
        {
            var amazonAppSyncClient = Session.CreateClient<AmazonAppSyncClient>();

            Assert.NotNull(amazonAppSyncClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFrontClient()
        {
            var amazonCloudFrontClient = Session.CreateClient<AmazonCloudFrontClient>();

            Assert.NotNull(amazonCloudFrontClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAthenaClient()
        {
            var amazonAthenaClient = Session.CreateClient<AmazonAthenaClient>();

            Assert.NotNull(amazonAthenaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonGlueClient()
        {
            var amazonGlueClient = Session.CreateClient<AmazonGlueClient>();

            Assert.NotNull(amazonGlueClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerClient()
        {
            var amazonSageMakerClient = Session.CreateClient<AmazonSageMakerClient>();

            Assert.NotNull(amazonSageMakerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerRuntimeClient()
        {
            var amazonSageMakerRuntimeClient = Session.CreateClient<AmazonSageMakerRuntimeClient>();

            Assert.NotNull(amazonSageMakerRuntimeClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECRClient()
        {
            var amazonEcrClient = Session.CreateClient<AmazonECRClient>();

            Assert.NotNull(amazonEcrClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonQLDBClient()
        {
            var amazonQldbClient = Session.CreateClient<AmazonQLDBClient>();

            Assert.NotNull(amazonQldbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudTrailClient()
        {
            var amazonCloudTrailClient = Session.CreateClient<AmazonCloudTrailClient>();

            Assert.NotNull(amazonCloudTrailClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonBatchClientClient()
        {
            var amazonBatchClient = Session.CreateClient<AmazonBatchClient>();

            Assert.NotNull(amazonBatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonOrganizationsClient()
        {
            var amazonOrganizationsClient = Session.CreateClient<AmazonOrganizationsClient>();

            Assert.NotNull(amazonOrganizationsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAutoScalingClient()
        {
            var amazonAutoScalingClient = Session.CreateClient<AmazonAutoScalingClient>();

            Assert.NotNull(amazonAutoScalingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreClient()
        {
            var amazonMediaStoreClient = Session.CreateClient<AmazonMediaStoreClient>();

            Assert.NotNull(amazonMediaStoreClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreDataClient()
        {
            var amazonMediaStoreDataClient = Session.CreateClient<AmazonMediaStoreDataClient>();

            Assert.NotNull(amazonMediaStoreDataClient);
        }
    }
}