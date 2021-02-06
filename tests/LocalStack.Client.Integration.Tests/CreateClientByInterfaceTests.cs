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
#if WIN
using Amazon.EC2;
#endif
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
using Amazon.Runtime;
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
    public class CreateClientByInterfaceTests
    {
        private static readonly ISession Session;

        static CreateClientByInterfaceTests()
        {
            Session = SessionStandalone.Init().Create();
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAPIGatewayClient()
        {
            AmazonServiceClient amazonApiGatewayClient = Session.CreateClientByInterface<IAmazonAPIGateway>();

            Assert.NotNull(amazonApiGatewayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonApiGatewayV2Client()
        {
            AmazonServiceClient amazonApiGatewayV2Client = Session.CreateClientByInterface<IAmazonApiGatewayV2>();

            Assert.NotNull(amazonApiGatewayV2Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonS3Client()
        {
            AmazonServiceClient amazonS3Client = Session.CreateClientByInterface<IAmazonS3>();

            Assert.NotNull(amazonS3Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonDynamoDBClient()
        {
            AmazonServiceClient amazonDynamoDbClient = Session.CreateClientByInterface<IAmazonDynamoDB>();

            Assert.NotNull(amazonDynamoDbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticsearchClient()
        {
            AmazonServiceClient amazonElasticsearchClient = Session.CreateClientByInterface<IAmazonElasticsearch>();

            Assert.NotNull(amazonElasticsearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKinesisFirehoseClient()
        {
            AmazonServiceClient amazonKinesisFirehoseClient = Session.CreateClientByInterface<IAmazonKinesisFirehose>();

            Assert.NotNull(amazonKinesisFirehoseClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonLambdaClient()
        {
            AmazonServiceClient amazonLambdaClient = Session.CreateClientByInterface<IAmazonLambda>();

            Assert.NotNull(amazonLambdaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleNotificationServiceClient()
        {
            AmazonServiceClient amazonSimpleNotificationServiceClient = Session.CreateClientByInterface<IAmazonSimpleNotificationService>();

            Assert.NotNull(amazonSimpleNotificationServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSQSClient()
        {
            AmazonServiceClient amazonSqsClient = Session.CreateClientByInterface<IAmazonSQS>();

            Assert.NotNull(amazonSqsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRedshiftClient()
        {
            AmazonServiceClient amazonRedshiftClient = Session.CreateClientByInterface<IAmazonRedshift>();

            Assert.NotNull(amazonRedshiftClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleEmailServiceClient()
        {
            AmazonServiceClient amazonSimpleEmailServiceClient = Session.CreateClientByInterface<IAmazonSimpleEmailService>();

            Assert.NotNull(amazonSimpleEmailServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRoute53Client()
        {
            AmazonServiceClient amazonRoute53Client = Session.CreateClientByInterface<IAmazonRoute53>();

            Assert.NotNull(amazonRoute53Client);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFormationClient()
        {
            AmazonServiceClient amazonCloudFormationClient = Session.CreateClientByInterface<IAmazonCloudFormation>();

            Assert.NotNull(amazonCloudFormationClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchClient()
        {
            AmazonServiceClient amazonCloudWatchClient = Session.CreateClientByInterface<IAmazonCloudWatch>();

            Assert.NotNull(amazonCloudWatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleSystemsManagementClient()
        {
            AmazonServiceClient amazonSimpleSystemsManagementClient = Session.CreateClientByInterface<IAmazonSimpleSystemsManagement>();

            Assert.NotNull(amazonSimpleSystemsManagementClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecretsManagerClient()
        {
            AmazonServiceClient amazonSecretsManagerClient = Session.CreateClientByInterface<IAmazonSecretsManager>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonStepFunctionsClient()
        {
            AmazonServiceClient amazonSecretsManagerClient = Session.CreateClientByInterface<IAmazonStepFunctions>();

            Assert.NotNull(amazonSecretsManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchLogsClient()
        {
            AmazonServiceClient amazonCloudWatchLogsClient = Session.CreateClientByInterface<IAmazonCloudWatchLogs>();

            Assert.NotNull(amazonCloudWatchLogsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudWatchEventsClient()
        {
            AmazonServiceClient amazonCloudWatchEventsClient = Session.CreateClientByInterface<IAmazonCloudWatchEvents>();

            Assert.NotNull(amazonCloudWatchEventsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticLoadBalancingClient()
        {
            AmazonServiceClient amazonElasticLoadBalancingClient = Session.CreateClientByInterface<IAmazonElasticLoadBalancing>();

            Assert.NotNull(amazonElasticLoadBalancingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIoTClient()
        {
            AmazonServiceClient amazonIoTClient = Session.CreateClientByInterface<IAmazonIoT>();

            Assert.NotNull(amazonIoTClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityProviderClient()
        {
            AmazonServiceClient amazonCognitoIdentityProviderClient = Session.CreateClientByInterface<IAmazonCognitoIdentityProvider>();

            Assert.NotNull(amazonCognitoIdentityProviderClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCognitoIdentityClient()
        {
            AmazonServiceClient amazonCognitoIdentityClient = Session.CreateClientByInterface<IAmazonCognitoIdentity>();

            Assert.NotNull(amazonCognitoIdentityClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSecurityTokenServiceClient()
        {
            AmazonServiceClient amazonSecurityTokenServiceClient = Session.CreateClientByInterface<IAmazonSecurityTokenService>();

            Assert.NotNull(amazonSecurityTokenServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonIdentityManagementServiceClient()
        {
            AmazonServiceClient amazonIdentityManagementServiceClient = Session.CreateClientByInterface<IAmazonIdentityManagementService>();

            Assert.NotNull(amazonIdentityManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRDSClient()
        {
            AmazonServiceClient amazonRdsClient = Session.CreateClientByInterface<IAmazonRDS>();

            Assert.NotNull(amazonRdsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRDSDataServiceClient()
        {
            AmazonServiceClient amazonRdsDataServiceClient = Session.CreateClientByInterface<IAmazonRDSDataService>();

            Assert.NotNull(amazonRdsDataServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudSearchClient()
        {
            AmazonServiceClient amazonCloudSearchClient = Session.CreateClientByInterface<IAmazonCloudSearch>();

            Assert.NotNull(amazonCloudSearchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSimpleWorkflowClient()
        {
            AmazonServiceClient amazonSimpleWorkflowClient = Session.CreateClientByInterface<IAmazonSimpleWorkflow>();

            Assert.NotNull(amazonSimpleWorkflowClient);
        }

#if WIN
        [Fact]
        public void Should_Able_To_Create_AmazonEC2Client()
        {
            AmazonServiceClient amazonEc2Client = Session.CreateClientByInterface<IAmazonEC2>();

            Assert.NotNull(amazonEc2Client);
        }
#endif

        [Fact]
        public void Should_Able_To_Create_AmazonElastiCacheClient()
        {
            AmazonServiceClient amazonElastiCacheClient = Session.CreateClientByInterface<IAmazonElastiCache>();

            Assert.NotNull(amazonElastiCacheClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKeyManagementServiceClient()
        {
            AmazonServiceClient amazonKeyManagementServiceClient = Session.CreateClientByInterface<IAmazonKeyManagementService>();

            Assert.NotNull(amazonKeyManagementServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticMapReduceClient()
        {
            AmazonServiceClient amazonElasticMapReduceClient = Session.CreateClientByInterface<IAmazonElasticMapReduce>();

            Assert.NotNull(amazonElasticMapReduceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECSClient()
        {
            AmazonServiceClient amazonEcsClient = Session.CreateClientByInterface<IAmazonECS>();

            Assert.NotNull(amazonEcsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonEKSClient()
        {
            AmazonServiceClient amazonEksClient = Session.CreateClientByInterface<IAmazonEKS>();

            Assert.NotNull(amazonEksClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonXRayClient()
        {
            AmazonServiceClient amazonXRayClient = Session.CreateClientByInterface<IAmazonXRay>();

            Assert.NotNull(amazonXRayClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticBeanstalkClient()
        {
            AmazonServiceClient amazonElasticBeanstalkClient = Session.CreateClientByInterface<IAmazonElasticBeanstalk>();

            Assert.NotNull(amazonElasticBeanstalkClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAppSyncClient()
        {
            AmazonServiceClient amazonAppSyncClient = Session.CreateClientByInterface<IAmazonAppSync>();

            Assert.NotNull(amazonAppSyncClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudFrontClient()
        {
            AmazonServiceClient amazonCloudFrontClient = Session.CreateClientByInterface<IAmazonCloudFront>();

            Assert.NotNull(amazonCloudFrontClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAthenaClient()
        {
            AmazonServiceClient amazonAthenaClient = Session.CreateClientByInterface<IAmazonAthena>();

            Assert.NotNull(amazonAthenaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonGlueClient()
        {
            AmazonServiceClient amazonGlueClient = Session.CreateClientByInterface<IAmazonGlue>();

            Assert.NotNull(amazonGlueClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerClient()
        {
            AmazonServiceClient amazonSageMakerClient = Session.CreateClientByInterface<IAmazonSageMaker>();

            Assert.NotNull(amazonSageMakerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonSageMakerRuntimeClient()
        {
            AmazonServiceClient amazonSageMakerRuntimeClient = Session.CreateClientByInterface<IAmazonSageMakerRuntime>();

            Assert.NotNull(amazonSageMakerRuntimeClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonECRClient()
        {
            AmazonServiceClient amazonEcrClient = Session.CreateClientByInterface<IAmazonECR>();

            Assert.NotNull(amazonEcrClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonQLDBClient()
        {
            AmazonServiceClient amazonQldbClient = Session.CreateClientByInterface<IAmazonQLDB>();

            Assert.NotNull(amazonQldbClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCloudTrailClient()
        {
            AmazonServiceClient amazonCloudTrailClient = Session.CreateClientByInterface<IAmazonCloudTrail>();

            Assert.NotNull(amazonCloudTrailClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonBatchClientClient()
        {
            AmazonServiceClient amazonBatchClient = Session.CreateClientByInterface<IAmazonBatch>();

            Assert.NotNull(amazonBatchClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonOrganizationsClient()
        {
            AmazonServiceClient amazonOrganizationsClient = Session.CreateClientByInterface<IAmazonOrganizations>();

            Assert.NotNull(amazonOrganizationsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAutoScalingClient()
        {
            AmazonServiceClient amazonAutoScalingClient = Session.CreateClientByInterface<IAmazonAutoScaling>();

            Assert.NotNull(amazonAutoScalingClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreClient()
        {
            AmazonServiceClient amazonMediaStoreClient = Session.CreateClientByInterface<IAmazonMediaStore>();

            Assert.NotNull(amazonMediaStoreClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonMediaStoreDataClient()
        {
            AmazonServiceClient amazonMediaStoreDataClient = Session.CreateClientByInterface<IAmazonMediaStoreData>();

            Assert.NotNull(amazonMediaStoreDataClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTransferClient()
        {
            AmazonServiceClient amazonTransferClient = Session.CreateClientByInterface<IAmazonTransfer>();

            Assert.NotNull(amazonTransferClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCertificateManagerClient()
        {
            AmazonServiceClient amazonCertificateManagerClient = Session.CreateClientByInterface<IAmazonCertificateManager>();

            Assert.NotNull(amazonCertificateManagerClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonCodeCommitClient()
        {
            AmazonServiceClient amazonCodeCommitClient = Session.CreateClientByInterface<IAmazonCodeCommit>();

            Assert.NotNull(amazonCodeCommitClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKinesisAnalyticsClient()
        {
            AmazonServiceClient amazonKinesisAnalyticsClient = Session.CreateClientByInterface<IAmazonKinesisAnalytics>();

            Assert.NotNull(amazonKinesisAnalyticsClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonAmplifyClient()
        {
            AmazonServiceClient amazonAmplifyClient = Session.CreateClientByInterface<IAmazonAmplify>();

            Assert.NotNull(amazonAmplifyClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonKafkaClient()
        {
            AmazonServiceClient amazonKafkaClient = Session.CreateClientByInterface<IAmazonKafka>();

            Assert.NotNull(amazonKafkaClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonRedshiftDataAPIServiceClient()
        {
            AmazonServiceClient amazonRedshiftDataApiServiceClient = Session.CreateClientByInterface<IAmazonRedshiftDataAPIService>();

            Assert.NotNull(amazonRedshiftDataApiServiceClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonApiGatewayManagementApiClient()
        {
            AmazonServiceClient amazonApiGatewayManagementApiClient = Session.CreateClientByInterface<IAmazonApiGatewayManagementApi>();

            Assert.NotNull(amazonApiGatewayManagementApiClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTimestreamQueryClient()
        {
            AmazonServiceClient amazonTimestreamQueryClient = Session.CreateClientByInterface<IAmazonTimestreamQuery>();

            Assert.NotNull(amazonTimestreamQueryClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonTimestreamWriteClient()
        {
            AmazonServiceClient amazonTimestreamWriteClient = Session.CreateClientByInterface<IAmazonTimestreamWrite>();

            Assert.NotNull(amazonTimestreamWriteClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonS3ControlClient()
        {
            AmazonServiceClient amazonS3ControlClient = Session.CreateClientByInterface<IAmazonS3Control>();

            Assert.NotNull(amazonS3ControlClient);
        }

        [Fact]
        public void Should_Able_To_Create_AmazonElasticLoadBalancingV2Client()
        {
            AmazonServiceClient amazonElasticLoadBalancingV2Client = Session.CreateClientByInterface<IAmazonElasticLoadBalancingV2>();

            Assert.NotNull(amazonElasticLoadBalancingV2Client);
        }
    }
}
