namespace LocalStack.Client.Integration.Tests;

public class CreateClientByImplementationTests
{
    private static readonly ISession Session = SessionStandalone.Init()
                                                                .WithSessionOptions(new SessionOptions(regionName: AssertAmazonClient.TestAwsRegion))
                                                                .WithConfigurationOptions(new ConfigOptions(useSsl: AssertAmazonClient.UseSsl))
                                                                .Create();

    static CreateClientByImplementationTests()
    {
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAPIGatewayClient()
    {
        var amazonApiGatewayClient = Session.CreateClientByImplementation<AmazonAPIGatewayClient>();

        Assert.NotNull(amazonApiGatewayClient);
        AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonApiGatewayV2Client()
    {
        var amazonApiGatewayV2Client = Session.CreateClientByImplementation<AmazonApiGatewayV2Client>();

        Assert.NotNull(amazonApiGatewayV2Client);
        AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayV2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonS3Client()
    {
        var amazonS3Client = Session.CreateClientByImplementation<AmazonS3Client>();

        Assert.NotNull(amazonS3Client);
        AssertAmazonClient.AssertClientConfiguration(amazonS3Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonDynamoDBClient()
    {
        var amazonDynamoDbClient = Session.CreateClientByImplementation<AmazonDynamoDBClient>();

        Assert.NotNull(amazonDynamoDbClient);
        AssertAmazonClient.AssertClientConfiguration(amazonDynamoDbClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticsearchClient()
    {
        var amazonElasticsearchClient = Session.CreateClientByImplementation<AmazonElasticsearchClient>();

        Assert.NotNull(amazonElasticsearchClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticsearchClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonOpenSearchServiceClient()
    {
        var amazonOpenSearchServiceClient = Session.CreateClientByImplementation<AmazonOpenSearchServiceClient>();

        Assert.NotNull(amazonOpenSearchServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonOpenSearchServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonEventBridgeClient()
    {
        var amazonEventBridgeClient = Session.CreateClientByImplementation<AmazonEventBridgeClient>();

        Assert.NotNull(amazonEventBridgeClient);
        AssertAmazonClient.AssertClientConfiguration(amazonEventBridgeClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKinesisFirehoseClient()
    {
        var amazonKinesisFirehoseClient = Session.CreateClientByImplementation<AmazonKinesisFirehoseClient>();

        Assert.NotNull(amazonKinesisFirehoseClient);
        AssertAmazonClient.AssertClientConfiguration(amazonKinesisFirehoseClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonLambdaClient()
    {
        var amazonLambdaClient = Session.CreateClientByImplementation<AmazonLambdaClient>();

        Assert.NotNull(amazonLambdaClient);
        AssertAmazonClient.AssertClientConfiguration(amazonLambdaClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSimpleNotificationServiceClient()
    {
        var amazonSimpleNotificationServiceClient = Session.CreateClientByImplementation<AmazonSimpleNotificationServiceClient>();

        Assert.NotNull(amazonSimpleNotificationServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSimpleNotificationServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSQSClient()
    {
        var amazonSqsClient = Session.CreateClientByImplementation<AmazonSQSClient>();

        Assert.NotNull(amazonSqsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSqsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRedshiftClient()
    {
        var amazonRedshiftClient = Session.CreateClientByImplementation<AmazonRedshiftClient>();

        Assert.NotNull(amazonRedshiftClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRedshiftClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSimpleEmailServiceClient()
    {
        var amazonSimpleEmailServiceClient = Session.CreateClientByImplementation<AmazonSimpleEmailServiceClient>();

        Assert.NotNull(amazonSimpleEmailServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSimpleEmailServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSimpleEmailServiceV2Client()
    {
        var simpleEmailServiceV2Client = Session.CreateClientByImplementation<AmazonSimpleEmailServiceV2Client>();

        Assert.NotNull(simpleEmailServiceV2Client);
        AssertAmazonClient.AssertClientConfiguration(simpleEmailServiceV2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRoute53Client()
    {
        var amazonRoute53Client = Session.CreateClientByImplementation<AmazonRoute53Client>();

        Assert.NotNull(amazonRoute53Client);
        AssertAmazonClient.AssertClientConfiguration(amazonRoute53Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRoute53ResolverClient()
    {
        var amazonRoute53ResolverClient = Session.CreateClientByImplementation<AmazonRoute53ResolverClient>();

        Assert.NotNull(amazonRoute53ResolverClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRoute53ResolverClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudFormationClient()
    {
        var amazonCloudFormationClient = Session.CreateClientByImplementation<AmazonCloudFormationClient>();

        Assert.NotNull(amazonCloudFormationClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudFormationClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudWatchClient()
    {
        var amazonCloudWatchClient = Session.CreateClientByImplementation<AmazonCloudWatchClient>();

        Assert.NotNull(amazonCloudWatchClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSimpleSystemsManagementClient()
    {
        var amazonSimpleSystemsManagementClient = Session.CreateClientByImplementation<AmazonSimpleSystemsManagementClient>();

        Assert.NotNull(amazonSimpleSystemsManagementClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSimpleSystemsManagementClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSecretsManagerClient()
    {
        var amazonSecretsManagerClient = Session.CreateClientByImplementation<AmazonSecretsManagerClient>();

        Assert.NotNull(amazonSecretsManagerClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSecretsManagerClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonStepFunctionsClient()
    {
        var amazonStepFunctionsClient = Session.CreateClientByImplementation<AmazonStepFunctionsClient>();

        Assert.NotNull(amazonStepFunctionsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonStepFunctionsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudWatchLogsClient()
    {
        var amazonCloudWatchLogsClient = Session.CreateClientByImplementation<AmazonCloudWatchLogsClient>();

        Assert.NotNull(amazonCloudWatchLogsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchLogsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudWatchEventsClient()
    {
        var amazonCloudWatchEventsClient = Session.CreateClientByImplementation<AmazonCloudWatchEventsClient>();

        Assert.NotNull(amazonCloudWatchEventsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchEventsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticLoadBalancingClient()
    {
        var amazonElasticLoadBalancingClient = Session.CreateClientByImplementation<AmazonElasticLoadBalancingClient>();

        Assert.NotNull(amazonElasticLoadBalancingClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticLoadBalancingClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTClient()
    {
        var amazonIoTClient = Session.CreateClientByImplementation<AmazonIoTClient>();

        Assert.NotNull(amazonIoTClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTAnalyticsClient()
    {
        var amazonIoTAnalyticsClient = Session.CreateClientByImplementation<AmazonIoTAnalyticsClient>();

        Assert.NotNull(amazonIoTAnalyticsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTAnalyticsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTEventsClient()
    {
        var amazonIoTEventsClient = Session.CreateClientByImplementation<AmazonIoTEventsClient>();

        Assert.NotNull(amazonIoTEventsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTEventsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTEventsDataClient()
    {
        var amazonIoTEventsDataClient = Session.CreateClientByImplementation<AmazonIoTEventsDataClient>();

        Assert.NotNull(amazonIoTEventsDataClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTEventsDataClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTWirelessClient()
    {
        var amazonIoTWirelessClient = Session.CreateClientByImplementation<AmazonIoTWirelessClient>();

        Assert.NotNull(amazonIoTWirelessClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTWirelessClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTDataClient_With_ServiceUrl()
    {
        var amazonIoTDataClient = Session.CreateClientByImplementation<AmazonIotDataClient>(useServiceUrl: true);

        Assert.NotNull(amazonIoTDataClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTDataClient);
    }

    [Fact]
    public void Should_Throw_AmazonClientException_When_Creating_AmazonIoTDataClient_If_RegionEndpoint_Used()
    {
        try
        {
            Session.CreateClientByImplementation<AmazonIotDataClient>();
        }
        catch (Exception e)
        {
            Exception? ex = e;

            while (ex != null)
            {
                if (ex is AmazonClientException)
                {
                    return;
                }

                ex = ex.InnerException;
            }

            throw;
        }

        Assert.Fail("Exception has not thrown");
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIoTJobsDataPlaneClient_With_ServiceUr()
    {
        var amazonIoTJobsDataPlaneClient = Session.CreateClientByImplementation<AmazonIoTJobsDataPlaneClient>(useServiceUrl: true);

        Assert.NotNull(amazonIoTJobsDataPlaneClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIoTJobsDataPlaneClient);
    }

    [Fact]
    public void Should_Throw_AmazonClientException_When_Creating_AmazonIoTJobsDataPlaneClient_If_RegionEndpoint_Used()
    {
        try
        {
            Session.CreateClientByImplementation<AmazonIoTJobsDataPlaneClient>();
        }
        catch (Exception e)
        {
            Exception? ex = e;

            while (ex != null)
            {
                if (ex is AmazonClientException)
                {
                    return;
                }

                ex = ex.InnerException;
            }

            throw;
        }

        Assert.Fail("Exception has not thrown");
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCognitoIdentityProviderClient()
    {
        var amazonCognitoIdentityProviderClient = Session.CreateClientByImplementation<AmazonCognitoIdentityProviderClient>();

        Assert.NotNull(amazonCognitoIdentityProviderClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCognitoIdentityProviderClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCognitoIdentityClient()
    {
        var amazonCognitoIdentityClient = Session.CreateClientByImplementation<AmazonCognitoIdentityClient>();

        Assert.NotNull(amazonCognitoIdentityClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCognitoIdentityClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSecurityTokenServiceClient()
    {
        var amazonSecurityTokenServiceClient = Session.CreateClientByImplementation<AmazonSecurityTokenServiceClient>();

        Assert.NotNull(amazonSecurityTokenServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSecurityTokenServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonIdentityManagementServiceClient()
    {
        var amazonIdentityManagementServiceClient = Session.CreateClientByImplementation<AmazonIdentityManagementServiceClient>();

        Assert.NotNull(amazonIdentityManagementServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonIdentityManagementServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRDSClient()
    {
        var amazonRdsClient = Session.CreateClientByImplementation<AmazonRDSClient>();

        Assert.NotNull(amazonRdsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRdsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRDSDataServiceClient()
    {
        var amazonRdsDataServiceClient = Session.CreateClientByImplementation<AmazonRDSDataServiceClient>();

        Assert.NotNull(amazonRdsDataServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRdsDataServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudSearchClient()
    {
        var amazonCloudSearchClient = Session.CreateClientByImplementation<AmazonCloudSearchClient>();

        Assert.NotNull(amazonCloudSearchClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudSearchClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSimpleWorkflowClient()
    {
        var amazonSimpleWorkflowClient = Session.CreateClientByImplementation<AmazonSimpleWorkflowClient>();

        Assert.NotNull(amazonSimpleWorkflowClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSimpleWorkflowClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonEC2Client()
    {
        var amazonEc2Client = Session.CreateClientByImplementation<AmazonEC2Client>();

        Assert.NotNull(amazonEc2Client);
        AssertAmazonClient.AssertClientConfiguration(amazonEc2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElastiCacheClient()
    {
        var amazonElastiCacheClient = Session.CreateClientByImplementation<AmazonElastiCacheClient>();

        Assert.NotNull(amazonElastiCacheClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElastiCacheClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKeyManagementServiceClient()
    {
        var amazonKeyManagementServiceClient = Session.CreateClientByImplementation<AmazonKeyManagementServiceClient>();

        Assert.NotNull(amazonKeyManagementServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonKeyManagementServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticMapReduceClient()
    {
        var amazonElasticMapReduceClient = Session.CreateClientByImplementation<AmazonElasticMapReduceClient>();

        Assert.NotNull(amazonElasticMapReduceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticMapReduceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonECSClient()
    {
        var amazonEcsClient = Session.CreateClientByImplementation<AmazonECSClient>();

        Assert.NotNull(amazonEcsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonEcsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonEKSClient()
    {
        var amazonEksClient = Session.CreateClientByImplementation<AmazonEKSClient>();

        Assert.NotNull(amazonEksClient);
        AssertAmazonClient.AssertClientConfiguration(amazonEksClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonXRayClient()
    {
        var amazonXRayClient = Session.CreateClientByImplementation<AmazonXRayClient>();

        Assert.NotNull(amazonXRayClient);
        AssertAmazonClient.AssertClientConfiguration(amazonXRayClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticBeanstalkClient()
    {
        var amazonElasticBeanstalkClient = Session.CreateClientByImplementation<AmazonElasticBeanstalkClient>();

        Assert.NotNull(amazonElasticBeanstalkClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticBeanstalkClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAppSyncClient()
    {
        var amazonAppSyncClient = Session.CreateClientByImplementation<AmazonAppSyncClient>();

        Assert.NotNull(amazonAppSyncClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAppSyncClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudFrontClient()
    {
        var amazonCloudFrontClient = Session.CreateClientByImplementation<AmazonCloudFrontClient>();

        Assert.NotNull(amazonCloudFrontClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudFrontClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAthenaClient()
    {
        var amazonAthenaClient = Session.CreateClientByImplementation<AmazonAthenaClient>();

        Assert.NotNull(amazonAthenaClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAthenaClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonGlueClient()
    {
        var amazonGlueClient = Session.CreateClientByImplementation<AmazonGlueClient>();

        Assert.NotNull(amazonGlueClient);
        AssertAmazonClient.AssertClientConfiguration(amazonGlueClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSageMakerClient()
    {
        var amazonSageMakerClient = Session.CreateClientByImplementation<AmazonSageMakerClient>();

        Assert.NotNull(amazonSageMakerClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSageMakerClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSageMakerRuntimeClient()
    {
        var amazonSageMakerRuntimeClient = Session.CreateClientByImplementation<AmazonSageMakerRuntimeClient>();

        Assert.NotNull(amazonSageMakerRuntimeClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSageMakerRuntimeClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonECRClient()
    {
        var amazonEcrClient = Session.CreateClientByImplementation<AmazonECRClient>();

        Assert.NotNull(amazonEcrClient);
        AssertAmazonClient.AssertClientConfiguration(amazonEcrClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonQLDBClient()
    {
        var amazonQldbClient = Session.CreateClientByImplementation<AmazonQLDBClient>();

        Assert.NotNull(amazonQldbClient);
        AssertAmazonClient.AssertClientConfiguration(amazonQldbClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonQLDBSessionClient()
    {
        var amazonQldbSessionClient = Session.CreateClientByImplementation<AmazonQLDBSessionClient>();

        Assert.NotNull(amazonQldbSessionClient);
        AssertAmazonClient.AssertClientConfiguration(amazonQldbSessionClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudTrailClient()
    {
        var amazonCloudTrailClient = Session.CreateClientByImplementation<AmazonCloudTrailClient>();

        Assert.NotNull(amazonCloudTrailClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudTrailClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonBatchClientClient()
    {
        var amazonBatchClient = Session.CreateClientByImplementation<AmazonBatchClient>();

        Assert.NotNull(amazonBatchClient);
        AssertAmazonClient.AssertClientConfiguration(amazonBatchClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonOrganizationsClient()
    {
        var amazonOrganizationsClient = Session.CreateClientByImplementation<AmazonOrganizationsClient>();

        Assert.NotNull(amazonOrganizationsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonOrganizationsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAutoScalingClient()
    {
        var amazonAutoScalingClient = Session.CreateClientByImplementation<AmazonAutoScalingClient>();

        Assert.NotNull(amazonAutoScalingClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAutoScalingClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMediaStoreClient()
    {
        var amazonMediaStoreClient = Session.CreateClientByImplementation<AmazonMediaStoreClient>();

        Assert.NotNull(amazonMediaStoreClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMediaStoreClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMediaStoreDataClient_With_ServiceUrl()
    {
        var amazonMediaStoreDataClient = Session.CreateClientByImplementation<AmazonMediaStoreDataClient>(useServiceUrl: true);

        Assert.NotNull(amazonMediaStoreDataClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMediaStoreDataClient);
    }

    [Fact]
    public void Should_Throw_AmazonClientException_When_Creating_AmazonMediaStoreDataClient_If_RegionEndpoint_Used()
    {
        try
        {
            Session.CreateClientByImplementation<AmazonMediaStoreDataClient>();
        }
        catch (Exception e)
        {
            Exception? ex = e;

            while (ex != null)
            {
                if (ex is AmazonClientException)
                {
                    return;
                }

                ex = ex.InnerException;
            }

            throw;
        }

        Assert.Fail("Exception has not thrown");
    }

    [Fact]
    public void Should_Able_To_Create_AmazonTransferClient()
    {
        var amazonTransferClient = Session.CreateClientByImplementation<AmazonTransferClient>();

        Assert.NotNull(amazonTransferClient);
        AssertAmazonClient.AssertClientConfiguration(amazonTransferClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCertificateManagerClient()
    {
        var amazonCertificateManagerClient = Session.CreateClientByImplementation<AmazonCertificateManagerClient>();

        Assert.NotNull(amazonCertificateManagerClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCertificateManagerClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCodeCommitClient()
    {
        var amazonCodeCommitClient = Session.CreateClientByImplementation<AmazonCodeCommitClient>();

        Assert.NotNull(amazonCodeCommitClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCodeCommitClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKinesisAnalyticsClient()
    {
        var amazonKinesisAnalyticsClient = Session.CreateClientByImplementation<AmazonKinesisAnalyticsClient>();

        Assert.NotNull(amazonKinesisAnalyticsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonKinesisAnalyticsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKinesisAnalyticsV2Client()
    {
        var kinesisAnalyticsV2Client = Session.CreateClientByImplementation<AmazonKinesisAnalyticsV2Client>();

        Assert.NotNull(kinesisAnalyticsV2Client);
        AssertAmazonClient.AssertClientConfiguration(kinesisAnalyticsV2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAmplifyClient()
    {
        var amazonAmplifyClient = Session.CreateClientByImplementation<AmazonAmplifyClient>();

        Assert.NotNull(amazonAmplifyClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAmplifyClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKafkaClient()
    {
        var amazonKafkaClient = Session.CreateClientByImplementation<AmazonKafkaClient>();

        Assert.NotNull(amazonKafkaClient);
        AssertAmazonClient.AssertClientConfiguration(amazonKafkaClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRedshiftDataAPIServiceClient()
    {
        var amazonRedshiftDataApiServiceClient = Session.CreateClientByImplementation<AmazonRedshiftDataAPIServiceClient>();

        Assert.NotNull(amazonRedshiftDataApiServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRedshiftDataApiServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonApiGatewayManagementApiClient()
    {
        var amazonApiGatewayManagementApiClient = Session.CreateClientByImplementation<AmazonApiGatewayManagementApiClient>();

        Assert.NotNull(amazonApiGatewayManagementApiClient);
        AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayManagementApiClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonTimestreamQueryClient()
    {
        var amazonTimestreamQueryClient = Session.CreateClientByImplementation<AmazonTimestreamQueryClient>();

        Assert.NotNull(amazonTimestreamQueryClient);
        AssertAmazonClient.AssertClientConfiguration(amazonTimestreamQueryClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonTimestreamWriteClient()
    {
        var amazonTimestreamWriteClient = Session.CreateClientByImplementation<AmazonTimestreamWriteClient>();

        Assert.NotNull(amazonTimestreamWriteClient);
        AssertAmazonClient.AssertClientConfiguration(amazonTimestreamWriteClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonS3ControlClient()
    {
        var amazonS3ControlClient = Session.CreateClientByImplementation<AmazonS3ControlClient>();

        Assert.NotNull(amazonS3ControlClient);
        AssertAmazonClient.AssertClientConfiguration(amazonS3ControlClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticLoadBalancingV2Client()
    {
        var amazonElasticLoadBalancingV2Client = Session.CreateClientByImplementation<AmazonElasticLoadBalancingV2Client>();

        Assert.NotNull(amazonElasticLoadBalancingV2Client);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticLoadBalancingV2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSupportClient()
    {
        var amazonSupportClient = Session.CreateClientByImplementation<AmazonAWSSupportClient>();

        Assert.NotNull(amazonSupportClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSupportClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonNeptuneClient()
    {
        var amazonNeptuneClient = Session.CreateClientByImplementation<AmazonNeptuneClient>();

        Assert.NotNull(amazonNeptuneClient);
        AssertAmazonClient.AssertClientConfiguration(amazonNeptuneClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonDocDBClient()
    {
        var amazonDocDbClient = Session.CreateClientByImplementation<AmazonDocDBClient>();

        Assert.NotNull(amazonDocDbClient);
        AssertAmazonClient.AssertClientConfiguration(amazonDocDbClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonServiceDiscoveryClient()
    {
        var amazonServiceDiscoveryClient = Session.CreateClientByImplementation<AmazonServiceDiscoveryClient>();

        Assert.NotNull(amazonServiceDiscoveryClient);
        AssertAmazonClient.AssertClientConfiguration(amazonServiceDiscoveryClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonServerlessApplicationRepositoryClient()
    {
        var amazonServiceServerlessApplicationRepositoryClient = Session.CreateClientByImplementation<AmazonServerlessApplicationRepositoryClient>();

        Assert.NotNull(amazonServiceServerlessApplicationRepositoryClient);
        AssertAmazonClient.AssertClientConfiguration(amazonServiceServerlessApplicationRepositoryClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAppConfigClient()
    {
        var amazonAppConfigClient = Session.CreateClientByImplementation<AmazonAppConfigClient>();

        Assert.NotNull(amazonAppConfigClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAppConfigClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCostExplorerClient()
    {
        var amazonCostExplorerClient = Session.CreateClientByImplementation<AmazonCostExplorerClient>();

        Assert.NotNull(amazonCostExplorerClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCostExplorerClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMediaConvertClient()
    {
        var amazonMediaConvertClient = Session.CreateClientByImplementation<AmazonMediaConvertClient>();

        Assert.NotNull(amazonMediaConvertClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMediaConvertClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonResourceGroupsTaggingAPIClient()
    {
        var amazonResourceGroupsTaggingApiClient = Session.CreateClientByImplementation<AmazonResourceGroupsTaggingAPIClient>();

        Assert.NotNull(amazonResourceGroupsTaggingApiClient);
        AssertAmazonClient.AssertClientConfiguration(amazonResourceGroupsTaggingApiClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonResourceGroupsClient()
    {
        var amazonResourceGroupsClient = Session.CreateClientByImplementation<AmazonResourceGroupsClient>();

        Assert.NotNull(amazonResourceGroupsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonResourceGroupsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticFileSystemClient()
    {
        var amazonElasticFileSystemClient = Session.CreateClientByImplementation<AmazonElasticFileSystemClient>();

        Assert.NotNull(amazonElasticFileSystemClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticFileSystemClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonBackupClient()
    {
        var amazonBackupClient = Session.CreateClientByImplementation<AmazonBackupClient>();

        Assert.NotNull(amazonBackupClient);
        AssertAmazonClient.AssertClientConfiguration(amazonBackupClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonLakeFormationClient()
    {
        var amazonLakeFormationClient = Session.CreateClientByImplementation<AmazonLakeFormationClient>();

        Assert.NotNull(amazonLakeFormationClient);
        AssertAmazonClient.AssertClientConfiguration(amazonLakeFormationClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonWAFClient()
    {
        var amazonWafClient = Session.CreateClientByImplementation<AmazonWAFClient>();

        Assert.NotNull(amazonWafClient);
        AssertAmazonClient.AssertClientConfiguration(amazonWafClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonWAFV2Client()
    {
        var amazonWafV2Client = Session.CreateClientByImplementation<AmazonWAFV2Client>();

        Assert.NotNull(amazonWafV2Client);
        AssertAmazonClient.AssertClientConfiguration(amazonWafV2Client);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonConfigServiceClient()
    {
        var amazonConfigServiceClient = Session.CreateClientByImplementation<AmazonConfigServiceClient>();

        Assert.NotNull(amazonConfigServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonConfigServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMWAAClient()
    {
        var amazonMwaaClient = Session.CreateClientByImplementation<AmazonMWAAClient>();

        Assert.NotNull(amazonMwaaClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMwaaClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonFISClient()
    {
        var amazonFisClient = Session.CreateClientByImplementation<AmazonFISClient>();

        Assert.NotNull(amazonFisClient);
        AssertAmazonClient.AssertClientConfiguration(amazonFisClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAWSMarketplaceMeteringClient()
    {
        var awsMarketplaceMeteringClient = Session.CreateClientByImplementation<AmazonAWSMarketplaceMeteringClient>();

        Assert.NotNull(awsMarketplaceMeteringClient);
        AssertAmazonClient.AssertClientConfiguration(awsMarketplaceMeteringClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonTranscribeServiceClient()
    {
        var amazonTranscribeServiceClient = Session.CreateClientByImplementation<AmazonTranscribeServiceClient>();

        Assert.NotNull(amazonTranscribeServiceClient);
        AssertAmazonClient.AssertClientConfiguration(amazonTranscribeServiceClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMQClient()
    {
        var amazonMqClient = Session.CreateClientByImplementation<AmazonMQClient>();

        Assert.NotNull(amazonMqClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMqClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonEMRServerlessClient()
    {
        var amazonEmrServerlessClient = Session.CreateClientByImplementation<AmazonEMRServerlessClient>();

        Assert.NotNull(amazonEmrServerlessClient);
        AssertAmazonClient.AssertClientConfiguration(amazonEmrServerlessClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAppflowClient()
    {
        var amazonAppflowClient = Session.CreateClientByImplementation<AmazonAppflowClient>();

        Assert.NotNull(amazonAppflowClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAppflowClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRoute53DomainsClient()
    {
        var amazonRoute53DomainsClient = Session.CreateClientByImplementation<AmazonRoute53DomainsClient>();

        Assert.NotNull(amazonRoute53DomainsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRoute53DomainsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonKeyspacesClient()
    {
        var amazonKeyspacesClient = Session.CreateClientByImplementation<AmazonKeyspacesClient>();

        Assert.NotNull(amazonKeyspacesClient);
        AssertAmazonClient.AssertClientConfiguration(amazonKeyspacesClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonSchedulerClient()
    {
        var amazonSchedulerClient = Session.CreateClientByImplementation<AmazonSchedulerClient>();

        Assert.NotNull(amazonSchedulerClient);
        AssertAmazonClient.AssertClientConfiguration(amazonSchedulerClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonRAM()
    {
        var amazonRamClient = Session.CreateClientByImplementation<AmazonRAMClient>();

        Assert.NotNull(amazonRamClient);
        AssertAmazonClient.AssertClientConfiguration(amazonRamClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAppConfigData()
    {
        var amazonAppConfigDataClient = Session.CreateClientByImplementation<AmazonAppConfigDataClient>();

        Assert.NotNull(amazonAppConfigDataClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAppConfigDataClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonPinpoint()
    {
        var amazonPinpointClient = Session.CreateClientByImplementation<AmazonPinpointClient>();

        Assert.NotNull(amazonPinpointClient);
        AssertAmazonClient.AssertClientConfiguration(amazonPinpointClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonPipes()
    {
        var amazonPipesClient = Session.CreateClientByImplementation<AmazonPipesClient>();

        Assert.NotNull(amazonPipesClient);
        AssertAmazonClient.AssertClientConfiguration(amazonPipesClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonAccount()
    {
        var amazonAccountClient = Session.CreateClientByImplementation<AmazonAccountClient>();

        Assert.NotNull(amazonAccountClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAccountClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonACMPCAClient()
    {
        var amazonAcmpcaClient = Session.CreateClientByImplementation<AmazonACMPCAClient>();

        Assert.NotNull(amazonAcmpcaClient);
        AssertAmazonClient.AssertClientConfiguration(amazonAcmpcaClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonBedrockClient()
    {
        var amazonBedrockClient = Session.CreateClientByImplementation<AmazonBedrockClient>();

        Assert.NotNull(amazonBedrockClient);
        AssertAmazonClient.AssertClientConfiguration(amazonBedrockClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCloudControlApiClient()
    {
        var amazonCloudControlApiClient = Session.CreateClientByImplementation<AmazonCloudControlApiClient>();

        Assert.NotNull(amazonCloudControlApiClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCloudControlApiClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCodeBuildClient()
    {
        var amazonCodeBuildClient = Session.CreateClientByImplementation<AmazonCodeBuildClient>();

        Assert.NotNull(amazonCodeBuildClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCodeBuildClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCodeConnectionsClient()
    {
        var amazonCodeConnectionsClient = Session.CreateClientByImplementation<AmazonCodeConnectionsClient>();

        Assert.NotNull(amazonCodeConnectionsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCodeConnectionsClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCodeDeployClient()
    {
        var amazonCodeDeployClient = Session.CreateClientByImplementation<AmazonCodeDeployClient>();

        Assert.NotNull(amazonCodeDeployClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCodeDeployClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonCodePipelineClient()
    {
        var amazonCodePipelineClient = Session.CreateClientByImplementation<AmazonCodePipelineClient>();

        Assert.NotNull(amazonCodePipelineClient);
        AssertAmazonClient.AssertClientConfiguration(amazonCodePipelineClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonElasticTranscoderClient()
    {
        var amazonElasticTranscoderClient = Session.CreateClientByImplementation<AmazonElasticTranscoderClient>();

        Assert.NotNull(amazonElasticTranscoderClient);
        AssertAmazonClient.AssertClientConfiguration(amazonElasticTranscoderClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonMemoryDBClient()
    {
        var amazonMemoryDbClient = Session.CreateClientByImplementation<AmazonMemoryDBClient>();

        Assert.NotNull(amazonMemoryDbClient);
        AssertAmazonClient.AssertClientConfiguration(amazonMemoryDbClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonShieldClient()
    {
        var amazonShieldClient = Session.CreateClientByImplementation<AmazonShieldClient>();

        Assert.NotNull(amazonShieldClient);
        AssertAmazonClient.AssertClientConfiguration(amazonShieldClient);
    }

    [Fact]
    public void Should_Able_To_Create_AmazonVerifiedPermissionsClient()
    {
        var amazonVerifiedPermissionsClient = Session.CreateClientByImplementation<AmazonVerifiedPermissionsClient>();

        Assert.NotNull(amazonVerifiedPermissionsClient);
        AssertAmazonClient.AssertClientConfiguration(amazonVerifiedPermissionsClient);
    }
}