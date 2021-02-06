using Amazon.Amplify;
using Amazon.APIGateway;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayV2;
using Amazon.AppSync;
using Amazon.Athena;
using Amazon.AutoScaling;
using Amazon.Batch;
using Amazon.CertificateManager;
using Amazon.CloudFormation;
using Amazon.CloudFront;
using Amazon.CloudSearch;
using Amazon.CloudTrail;
using Amazon.CloudWatch;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchLogs;
using Amazon.CodeCommit;
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
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticMapReduce;
using Amazon.Elasticsearch;
using Amazon.Glue;
using Amazon.IdentityManagement;
using Amazon.IoT;
using Amazon.Kafka;
using Amazon.KeyManagementService;
using Amazon.KinesisAnalytics;
using Amazon.KinesisFirehose;
using Amazon.Lambda;
using Amazon.MediaStore;
using Amazon.MediaStoreData;
using Amazon.Organizations;
using Amazon.QLDB;
using Amazon.RDS;
using Amazon.RDSDataService;
using Amazon.Redshift;
using Amazon.RedshiftDataAPIService;
using Amazon.Route53;
using Amazon.S3;
using Amazon.S3Control;
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
using Amazon.TimestreamQuery;
using Amazon.TimestreamWrite;
using Amazon.Transfer;
using Amazon.XRay;

using LocalStack.Client.Contracts;

using Xunit;

namespace LocalStack.Client.Integration.Tests
{
    public class CreateClientByImplementationTests
    {
        private static readonly ISession Session;

        static CreateClientByImplementationTests()
        {
            Session = SessionStandalone.Init().Create();
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAPIGatewayClient()
        {
            var amazonApiGatewayClient = Session.CreateClientByImplementation<AmazonAPIGatewayClient>();

            Assert.NotNull(amazonApiGatewayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonApiGatewayV2Client()
        {
            var amazonApiGatewayV2Client = Session.CreateClientByImplementation<AmazonApiGatewayV2Client>();

            Assert.NotNull(amazonApiGatewayV2Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonS3Client()
        {
            var amazonS3Client = Session.CreateClientByImplementation<AmazonS3Client>();

            Assert.NotNull(amazonS3Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonDynamoDBClient()
        {
            var amazonDynamoDbClient = Session.CreateClientByImplementation<AmazonDynamoDBClient>();

            Assert.NotNull(amazonDynamoDbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticsearchClient()
        {
            var amazonElasticsearchClient = Session.CreateClientByImplementation<AmazonElasticsearchClient>();

            Assert.NotNull(amazonElasticsearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKinesisFirehoseClient()
        {
            var amazonKinesisFirehoseClient = Session.CreateClientByImplementation<AmazonKinesisFirehoseClient>();

            Assert.NotNull(amazonKinesisFirehoseClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonLambdaClient()
        {
            var amazonLambdaClient = Session.CreateClientByImplementation<AmazonLambdaClient>();

            Assert.NotNull(amazonLambdaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleNotificationServiceClient()
        {
            var amazonSimpleNotificationServiceClient = Session.CreateClientByImplementation<AmazonSimpleNotificationServiceClient>();

            Assert.NotNull(amazonSimpleNotificationServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSQSClient()
        {
            var amazonSqsClient = Session.CreateClientByImplementation<AmazonSQSClient>();

            Assert.NotNull(amazonSqsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRedshiftClient()
        {
            var amazonRedshiftClient = Session.CreateClientByImplementation<AmazonRedshiftClient>();

            Assert.NotNull(amazonRedshiftClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleEmailServiceClient()
        {
            var amazonSimpleEmailServiceClient = Session.CreateClientByImplementation<AmazonSimpleEmailServiceClient>();

            Assert.NotNull(amazonSimpleEmailServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRoute53Client()
        {
            var amazonRoute53Client = Session.CreateClientByImplementation<AmazonRoute53Client>();

            Assert.NotNull(amazonRoute53Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFormationClient()
        {
            var amazonCloudFormationClient = Session.CreateClientByImplementation<AmazonCloudFormationClient>();

            Assert.NotNull(amazonCloudFormationClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchClient()
        {
            var amazonCloudWatchClient = Session.CreateClientByImplementation<AmazonCloudWatchClient>();

            Assert.NotNull(amazonCloudWatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleSystemsManagementClient()
        {
            var amazonSimpleSystemsManagementClient = Session.CreateClientByImplementation<AmazonSimpleSystemsManagementClient>();

            Assert.NotNull(amazonSimpleSystemsManagementClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecretsManagerClient()
        {
            var amazonSecretsManagerClient = Session.CreateClientByImplementation<AmazonSecretsManagerClient>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonStepFunctionsClient()
        {
            var amazonSecretsManagerClient = Session.CreateClientByImplementation<AmazonStepFunctionsClient>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchLogsClient()
        {
            var amazonCloudWatchLogsClient = Session.CreateClientByImplementation<AmazonCloudWatchLogsClient>();

            Assert.NotNull(amazonCloudWatchLogsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchEventsClient()
        {
            var amazonCloudWatchEventsClient = Session.CreateClientByImplementation<AmazonCloudWatchEventsClient>();

            Assert.NotNull(amazonCloudWatchEventsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticLoadBalancingClient()
        {
            var amazonElasticLoadBalancingClient = Session.CreateClientByImplementation<AmazonElasticLoadBalancingClient>();

            Assert.NotNull(amazonElasticLoadBalancingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIoTClient()
        {
            var amazonIoTClient = Session.CreateClientByImplementation<AmazonIoTClient>();

            Assert.NotNull(amazonIoTClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityProviderClient()
        {
            var amazonCognitoIdentityProviderClient = Session.CreateClientByImplementation<AmazonCognitoIdentityProviderClient>();

            Assert.NotNull(amazonCognitoIdentityProviderClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityClient()
        {
            var amazonCognitoIdentityClient = Session.CreateClientByImplementation<AmazonCognitoIdentityClient>();

            Assert.NotNull(amazonCognitoIdentityClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecurityTokenServiceClient()
        {
            var amazonSecurityTokenServiceClient = Session.CreateClientByImplementation<AmazonSecurityTokenServiceClient>();

            Assert.NotNull(amazonSecurityTokenServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIdentityManagementServiceClient()
        {
            var amazonIdentityManagementServiceClient = Session.CreateClientByImplementation<AmazonIdentityManagementServiceClient>();

            Assert.NotNull(amazonIdentityManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRDSClient()
        {
            var amazonRdsClient = Session.CreateClientByImplementation<AmazonRDSClient>();

            Assert.NotNull(amazonRdsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRDSDataServiceClient()
        {
            var amazonRdsDataServiceClient = Session.CreateClientByImplementation<AmazonRDSDataServiceClient>();

            Assert.NotNull(amazonRdsDataServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudSearchClient()
        {
            var amazonCloudSearchClient = Session.CreateClientByImplementation<AmazonCloudSearchClient>();

            Assert.NotNull(amazonCloudSearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleWorkflowClient()
        {
            var amazonSimpleWorkflowClient = Session.CreateClientByImplementation<AmazonSimpleWorkflowClient>();

            Assert.NotNull(amazonSimpleWorkflowClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonEC2Client()
        {
            var amazonEc2Client = Session.CreateClientByImplementation<AmazonEC2Client>();

            Assert.NotNull(amazonEc2Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElastiCacheClient()
        {
            var amazonElastiCacheClient = Session.CreateClientByImplementation<AmazonElastiCacheClient>();

            Assert.NotNull(amazonElastiCacheClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKeyManagementServiceClient()
        {
            var amazonKeyManagementServiceClient = Session.CreateClientByImplementation<AmazonKeyManagementServiceClient>();

            Assert.NotNull(amazonKeyManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticMapReduceClient()
        {
            var amazonElasticMapReduceClient = Session.CreateClientByImplementation<AmazonElasticMapReduceClient>();

            Assert.NotNull(amazonElasticMapReduceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECSClient()
        {
            var amazonEcsClient = Session.CreateClientByImplementation<AmazonECSClient>();

            Assert.NotNull(amazonEcsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonEKSClient()
        {
            var amazonEksClient = Session.CreateClientByImplementation<AmazonEKSClient>();

            Assert.NotNull(amazonEksClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonXRayClient()
        {
            var amazonXRayClient = Session.CreateClientByImplementation<AmazonXRayClient>();

            Assert.NotNull(amazonXRayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticBeanstalkClient()
        {
            var amazonElasticBeanstalkClient = Session.CreateClientByImplementation<AmazonElasticBeanstalkClient>();

            Assert.NotNull(amazonElasticBeanstalkClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAppSyncClient()
        {
            var amazonAppSyncClient = Session.CreateClientByImplementation<AmazonAppSyncClient>();

            Assert.NotNull(amazonAppSyncClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFrontClient()
        {
            var amazonCloudFrontClient = Session.CreateClientByImplementation<AmazonCloudFrontClient>();

            Assert.NotNull(amazonCloudFrontClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAthenaClient()
        {
            var amazonAthenaClient = Session.CreateClientByImplementation<AmazonAthenaClient>();

            Assert.NotNull(amazonAthenaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonGlueClient()
        {
            var amazonGlueClient = Session.CreateClientByImplementation<AmazonGlueClient>();

            Assert.NotNull(amazonGlueClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerClient()
        {
            var amazonSageMakerClient = Session.CreateClientByImplementation<AmazonSageMakerClient>();

            Assert.NotNull(amazonSageMakerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerRuntimeClient()
        {
            var amazonSageMakerRuntimeClient = Session.CreateClientByImplementation<AmazonSageMakerRuntimeClient>();

            Assert.NotNull(amazonSageMakerRuntimeClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECRClient()
        {
            var amazonEcrClient = Session.CreateClientByImplementation<AmazonECRClient>();

            Assert.NotNull(amazonEcrClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonQLDBClient()
        {
            var amazonQldbClient = Session.CreateClientByImplementation<AmazonQLDBClient>();

            Assert.NotNull(amazonQldbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudTrailClient()
        {
            var amazonCloudTrailClient = Session.CreateClientByImplementation<AmazonCloudTrailClient>();

            Assert.NotNull(amazonCloudTrailClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonBatchClientClient()
        {
            var amazonBatchClient = Session.CreateClientByImplementation<AmazonBatchClient>();

            Assert.NotNull(amazonBatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonOrganizationsClient()
        {
            var amazonOrganizationsClient = Session.CreateClientByImplementation<AmazonOrganizationsClient>();

            Assert.NotNull(amazonOrganizationsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAutoScalingClient()
        {
            var amazonAutoScalingClient = Session.CreateClientByImplementation<AmazonAutoScalingClient>();

            Assert.NotNull(amazonAutoScalingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreClient()
        {
            var amazonMediaStoreClient = Session.CreateClientByImplementation<AmazonMediaStoreClient>();

            Assert.NotNull(amazonMediaStoreClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreDataClient()
        {
            var amazonMediaStoreDataClient = Session.CreateClientByImplementation<AmazonMediaStoreDataClient>();

            Assert.NotNull(amazonMediaStoreDataClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTransferClient()
        {
            var amazonTransferClient = Session.CreateClientByImplementation<AmazonTransferClient>();

            Assert.NotNull(amazonTransferClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCertificateManagerClient()
        {
            var amazonCertificateManagerClient = Session.CreateClientByImplementation<AmazonCertificateManagerClient>();

            Assert.NotNull(amazonCertificateManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCodeCommitClient()
        {
            var amazonCodeCommitClient = Session.CreateClientByImplementation<AmazonCodeCommitClient>();

            Assert.NotNull(amazonCodeCommitClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKinesisAnalyticsClient()
        {
            var amazonKinesisAnalyticsClient = Session.CreateClientByImplementation<AmazonKinesisAnalyticsClient>();

            Assert.NotNull(amazonKinesisAnalyticsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAmplifyClient()
        {
            var amazonAmplifyClient = Session.CreateClientByImplementation<AmazonAmplifyClient>();

            Assert.NotNull(amazonAmplifyClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKafkaClient()
        {
            var amazonKafkaClient = Session.CreateClientByImplementation<AmazonKafkaClient>();

            Assert.NotNull(amazonKafkaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRedshiftDataAPIServiceClient()
        {
            var amazonRedshiftDataApiServiceClient = Session.CreateClientByImplementation<AmazonRedshiftDataAPIServiceClient>();

            Assert.NotNull(amazonRedshiftDataApiServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonApiGatewayManagementApiClient()
        {
            var amazonApiGatewayManagementApiClient = Session.CreateClientByImplementation<AmazonApiGatewayManagementApiClient>();

            Assert.NotNull(amazonApiGatewayManagementApiClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTimestreamQueryClient()
        {
            var amazonTimestreamQueryClient = Session.CreateClientByImplementation<AmazonTimestreamQueryClient>();

            Assert.NotNull(amazonTimestreamQueryClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTimestreamWriteClient()
        {
            var amazonTimestreamWriteClient = Session.CreateClientByImplementation<AmazonTimestreamWriteClient>();

            Assert.NotNull(amazonTimestreamWriteClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonS3ControlClient()
        {
            var amazonS3ControlClient = Session.CreateClientByImplementation<AmazonS3ControlClient>();

            Assert.NotNull(amazonS3ControlClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticLoadBalancingV2Client()
        {
            var amazonElasticLoadBalancingV2Client = Session.CreateClientByImplementation<AmazonElasticLoadBalancingV2Client>();

            Assert.NotNull(amazonElasticLoadBalancingV2Client);
        }
    }
}
