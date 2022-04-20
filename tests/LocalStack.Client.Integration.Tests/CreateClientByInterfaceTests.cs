namespace LocalStack.Client.Integration.Tests;

public class CreateClientByInterfaceTests
{
	private static readonly ISession Session;

	static CreateClientByInterfaceTests()
	{
		Session = SessionStandalone.Init()

								   .WithSessionOptions(new SessionOptions(regionName: AssertAmazonClient.TestAwsRegion))
								   .Create();
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAPIGatewayClient()
	{
		AmazonServiceClient amazonApiGatewayClient = Session.CreateClientByInterface<IAmazonAPIGateway>();

		Assert.NotNull(amazonApiGatewayClient);
		AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonApiGatewayV2Client()
	{
		AmazonServiceClient amazonApiGatewayV2Client = Session.CreateClientByInterface<IAmazonApiGatewayV2>();

		Assert.NotNull(amazonApiGatewayV2Client);
		AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayV2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonS3Client()
	{
		AmazonServiceClient amazonS3Client = Session.CreateClientByInterface<IAmazonS3>();

		Assert.NotNull(amazonS3Client);
		AssertAmazonClient.AssertClientConfiguration(amazonS3Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonDynamoDBClient()
	{
		AmazonServiceClient amazonDynamoDbClient = Session.CreateClientByInterface<IAmazonDynamoDB>();

		Assert.NotNull(amazonDynamoDbClient);
		AssertAmazonClient.AssertClientConfiguration(amazonDynamoDbClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticsearchClient()
	{
		AmazonServiceClient amazonElasticsearchClient = Session.CreateClientByInterface<IAmazonElasticsearch>();

		Assert.NotNull(amazonElasticsearchClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticsearchClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonOpenSearchServiceClient()
	{
		AmazonServiceClient amazonOpenSearchServiceClient = Session.CreateClientByInterface<IAmazonOpenSearchService>();

		Assert.NotNull(amazonOpenSearchServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonOpenSearchServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonEventBridgeClient()
	{
		AmazonServiceClient amazonEventBridgeClient = Session.CreateClientByInterface<IAmazonEventBridge>();

		Assert.NotNull(amazonEventBridgeClient);
		AssertAmazonClient.AssertClientConfiguration(amazonEventBridgeClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonKinesisFirehoseClient()
	{
		AmazonServiceClient amazonKinesisFirehoseClient = Session.CreateClientByInterface<IAmazonKinesisFirehose>();

		Assert.NotNull(amazonKinesisFirehoseClient);
		AssertAmazonClient.AssertClientConfiguration(amazonKinesisFirehoseClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonLambdaClient()
	{
		AmazonServiceClient amazonLambdaClient = Session.CreateClientByInterface<IAmazonLambda>();

		Assert.NotNull(amazonLambdaClient);
		AssertAmazonClient.AssertClientConfiguration(amazonLambdaClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSimpleNotificationServiceClient()
	{
		AmazonServiceClient amazonSimpleNotificationServiceClient = Session.CreateClientByInterface<IAmazonSimpleNotificationService>();

		Assert.NotNull(amazonSimpleNotificationServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSimpleNotificationServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSQSClient()
	{
		AmazonServiceClient amazonSqsClient = Session.CreateClientByInterface<IAmazonSQS>();

		Assert.NotNull(amazonSqsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSqsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRedshiftClient()
	{
		AmazonServiceClient amazonRedshiftClient = Session.CreateClientByInterface<IAmazonRedshift>();

		Assert.NotNull(amazonRedshiftClient);
		AssertAmazonClient.AssertClientConfiguration(amazonRedshiftClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSimpleEmailServiceClient()
	{
		AmazonServiceClient amazonSimpleEmailServiceClient = Session.CreateClientByInterface<IAmazonSimpleEmailService>();

		Assert.NotNull(amazonSimpleEmailServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSimpleEmailServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSimpleEmailServiceV2Client()
	{
		AmazonServiceClient simpleEmailServiceV2Client = Session.CreateClientByInterface<IAmazonSimpleEmailServiceV2>();

		Assert.NotNull(simpleEmailServiceV2Client);
		AssertAmazonClient.AssertClientConfiguration(simpleEmailServiceV2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRoute53Client()
	{
		AmazonServiceClient amazonRoute53Client = Session.CreateClientByInterface<IAmazonRoute53>();

		Assert.NotNull(amazonRoute53Client);
		AssertAmazonClient.AssertClientConfiguration(amazonRoute53Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRoute53ResolverClient()
	{
		AmazonServiceClient amazonRoute53ResolverClient = Session.CreateClientByInterface<IAmazonRoute53Resolver>();

		Assert.NotNull(amazonRoute53ResolverClient);
		AssertAmazonClient.AssertClientConfiguration(amazonRoute53ResolverClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudFormationClient()
	{
		AmazonServiceClient amazonCloudFormationClient = Session.CreateClientByInterface<IAmazonCloudFormation>();

		Assert.NotNull(amazonCloudFormationClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudFormationClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudWatchClient()
	{
		AmazonServiceClient amazonCloudWatchClient = Session.CreateClientByInterface<IAmazonCloudWatch>();

		Assert.NotNull(amazonCloudWatchClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSimpleSystemsManagementClient()
	{
		AmazonServiceClient amazonSimpleSystemsManagementClient = Session.CreateClientByInterface<IAmazonSimpleSystemsManagement>();

		Assert.NotNull(amazonSimpleSystemsManagementClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSimpleSystemsManagementClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSecretsManagerClient()
	{
		AmazonServiceClient amazonSecretsManagerClient = Session.CreateClientByInterface<IAmazonSecretsManager>();

		Assert.NotNull(amazonSecretsManagerClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSecretsManagerClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonStepFunctionsClient()
	{
		AmazonServiceClient amazonSecretsManagerClient = Session.CreateClientByInterface<IAmazonStepFunctions>();

		Assert.NotNull(amazonSecretsManagerClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSecretsManagerClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudWatchLogsClient()
	{
		AmazonServiceClient amazonCloudWatchLogsClient = Session.CreateClientByInterface<IAmazonCloudWatchLogs>();

		Assert.NotNull(amazonCloudWatchLogsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchLogsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudWatchEventsClient()
	{
		AmazonServiceClient amazonCloudWatchEventsClient = Session.CreateClientByInterface<IAmazonCloudWatchEvents>();

		Assert.NotNull(amazonCloudWatchEventsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudWatchEventsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticLoadBalancingClient()
	{
		AmazonServiceClient amazonElasticLoadBalancingClient = Session.CreateClientByInterface<IAmazonElasticLoadBalancing>();

		Assert.NotNull(amazonElasticLoadBalancingClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticLoadBalancingClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTClient()
	{
		AmazonServiceClient amazonIoTClient = Session.CreateClientByInterface<IAmazonIoT>();

		Assert.NotNull(amazonIoTClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTAnalyticsClient()
	{
		AmazonServiceClient amazonIoTAnalyticsClient = Session.CreateClientByInterface<IAmazonIoTAnalytics>();

		Assert.NotNull(amazonIoTAnalyticsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTAnalyticsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTEventsClient()
	{
		AmazonServiceClient amazonIoTEventsClient = Session.CreateClientByInterface<IAmazonIoTEvents>();

		Assert.NotNull(amazonIoTEventsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTEventsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTEventsDataClient()
	{
		AmazonServiceClient amazonIoTEventsDataClient = Session.CreateClientByInterface<IAmazonIoTEventsData>();

		Assert.NotNull(amazonIoTEventsDataClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTEventsDataClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTWirelessClient()
	{
		AmazonServiceClient amazonIoTWirelessClient = Session.CreateClientByInterface<IAmazonIoTWireless>();

		Assert.NotNull(amazonIoTWirelessClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTWirelessClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTDataClient_With_ServiceUrl()
	{
		AmazonServiceClient amazonIoTDataClient = Session.CreateClientByInterface<IAmazonIotData>(useServiceUrl:true);

		Assert.NotNull(amazonIoTDataClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTDataClient);
	}

	[Fact]
	public void Should_Throw_AmazonClientException_When_Creating_AmazonIoTDataClient_If_RegionEndpoint_Used()
	{
		try
		{
			Session.CreateClientByInterface<IAmazonIotData>();
		}
		catch (Exception e)
		{
			Exception ex = e;

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
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIoTJobsDataPlaneClient_With_ServiceUrl()
	{
		AmazonServiceClient amazonIoTJobsDataPlaneClient = Session.CreateClientByInterface<IAmazonIoTJobsDataPlane>(useServiceUrl:true);

		Assert.NotNull(amazonIoTJobsDataPlaneClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIoTJobsDataPlaneClient);
	}

	[Fact]
	public void Should_Throw_AmazonClientException_When_Creating_AmazonIoTJobsDataPlaneClient_If_RegionEndpoint_Used()
	{
		try
		{
			Session.CreateClientByInterface<IAmazonIoTJobsDataPlane>();
		}
		catch (Exception e)
		{
			Exception ex = e;

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
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCognitoIdentityProviderClient()
	{
		AmazonServiceClient amazonCognitoIdentityProviderClient = Session.CreateClientByInterface<IAmazonCognitoIdentityProvider>();

		Assert.NotNull(amazonCognitoIdentityProviderClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCognitoIdentityProviderClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCognitoIdentityClient()
	{
		AmazonServiceClient amazonCognitoIdentityClient = Session.CreateClientByInterface<IAmazonCognitoIdentity>();

		Assert.NotNull(amazonCognitoIdentityClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCognitoIdentityClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSecurityTokenServiceClient()
	{
		AmazonServiceClient amazonSecurityTokenServiceClient = Session.CreateClientByInterface<IAmazonSecurityTokenService>();

		Assert.NotNull(amazonSecurityTokenServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSecurityTokenServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonIdentityManagementServiceClient()
	{
		AmazonServiceClient amazonIdentityManagementServiceClient = Session.CreateClientByInterface<IAmazonIdentityManagementService>();

		Assert.NotNull(amazonIdentityManagementServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonIdentityManagementServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRDSClient()
	{
		AmazonServiceClient amazonRdsClient = Session.CreateClientByInterface<IAmazonRDS>();

		Assert.NotNull(amazonRdsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonRdsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRDSDataServiceClient()
	{
		AmazonServiceClient amazonRdsDataServiceClient = Session.CreateClientByInterface<IAmazonRDSDataService>();

		Assert.NotNull(amazonRdsDataServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonRdsDataServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudSearchClient()
	{
		AmazonServiceClient amazonCloudSearchClient = Session.CreateClientByInterface<IAmazonCloudSearch>();

		Assert.NotNull(amazonCloudSearchClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudSearchClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSimpleWorkflowClient()
	{
		AmazonServiceClient amazonSimpleWorkflowClient = Session.CreateClientByInterface<IAmazonSimpleWorkflow>();

		Assert.NotNull(amazonSimpleWorkflowClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSimpleWorkflowClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonEC2Client()
	{
		AmazonServiceClient amazonEc2Client = Session.CreateClientByInterface<IAmazonEC2>();

		Assert.NotNull(amazonEc2Client);
		AssertAmazonClient.AssertClientConfiguration(amazonEc2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElastiCacheClient()
	{
		AmazonServiceClient amazonElastiCacheClient = Session.CreateClientByInterface<IAmazonElastiCache>();

		Assert.NotNull(amazonElastiCacheClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElastiCacheClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonKeyManagementServiceClient()
	{
		AmazonServiceClient amazonKeyManagementServiceClient = Session.CreateClientByInterface<IAmazonKeyManagementService>();

		Assert.NotNull(amazonKeyManagementServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonKeyManagementServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticMapReduceClient()
	{
		AmazonServiceClient amazonElasticMapReduceClient = Session.CreateClientByInterface<IAmazonElasticMapReduce>();

		Assert.NotNull(amazonElasticMapReduceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticMapReduceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonECSClient()
	{
		AmazonServiceClient amazonEcsClient = Session.CreateClientByInterface<IAmazonECS>();

		Assert.NotNull(amazonEcsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonEcsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonEKSClient()
	{
		AmazonServiceClient amazonEksClient = Session.CreateClientByInterface<IAmazonEKS>();

		Assert.NotNull(amazonEksClient);
		AssertAmazonClient.AssertClientConfiguration(amazonEksClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonXRayClient()
	{
		AmazonServiceClient amazonXRayClient = Session.CreateClientByInterface<IAmazonXRay>();

		Assert.NotNull(amazonXRayClient);
		AssertAmazonClient.AssertClientConfiguration(amazonXRayClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticBeanstalkClient()
	{
		AmazonServiceClient amazonElasticBeanstalkClient = Session.CreateClientByInterface<IAmazonElasticBeanstalk>();

		Assert.NotNull(amazonElasticBeanstalkClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticBeanstalkClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAppSyncClient()
	{
		AmazonServiceClient amazonAppSyncClient = Session.CreateClientByInterface<IAmazonAppSync>();

		Assert.NotNull(amazonAppSyncClient);
		AssertAmazonClient.AssertClientConfiguration(amazonAppSyncClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCloudFrontClient()
	{
		AmazonServiceClient amazonCloudFrontClient = Session.CreateClientByInterface<IAmazonCloudFront>();

		Assert.NotNull(amazonCloudFrontClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudFrontClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAthenaClient()
	{
		AmazonServiceClient amazonAthenaClient = Session.CreateClientByInterface<IAmazonAthena>();

		Assert.NotNull(amazonAthenaClient);
		AssertAmazonClient.AssertClientConfiguration(amazonAthenaClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonGlueClient()
	{
		AmazonServiceClient amazonGlueClient = Session.CreateClientByInterface<IAmazonGlue>();

		Assert.NotNull(amazonGlueClient);
		AssertAmazonClient.AssertClientConfiguration(amazonGlueClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSageMakerClient()
	{
		AmazonServiceClient amazonSageMakerClient = Session.CreateClientByInterface<IAmazonSageMaker>();

		Assert.NotNull(amazonSageMakerClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSageMakerClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSageMakerRuntimeClient()
	{
		AmazonServiceClient amazonSageMakerRuntimeClient = Session.CreateClientByInterface<IAmazonSageMakerRuntime>();

		Assert.NotNull(amazonSageMakerRuntimeClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSageMakerRuntimeClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonECRClient()
	{
		AmazonServiceClient amazonEcrClient = Session.CreateClientByInterface<IAmazonECR>();

		Assert.NotNull(amazonEcrClient);
		AssertAmazonClient.AssertClientConfiguration(amazonEcrClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonQLDBClient()
	{
		AmazonServiceClient amazonQldbClient = Session.CreateClientByInterface<IAmazonQLDB>();

		Assert.NotNull(amazonQldbClient);
		AssertAmazonClient.AssertClientConfiguration(amazonQldbClient);
	}

	// [Fact]
	// public void Should_Able_To_Create_AmazonQLDBSessionClient()
	// {
	//     AmazonServiceClient amazonQldbSessionClient = Session.CreateClientByInterface<IAmazonQLDBSession>();
	//
	//     Assert.NotNull(amazonQldbSessionClient);
	//     AssertAmazonClient.AssertClientConfiguration(amazonQldbSessionClient);
	// }

	[Fact]
	public void Should_Able_To_Create_AmazonCloudTrailClient()
	{
		AmazonServiceClient amazonCloudTrailClient = Session.CreateClientByInterface<IAmazonCloudTrail>();

		Assert.NotNull(amazonCloudTrailClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCloudTrailClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonBatchClientClient()
	{
		AmazonServiceClient amazonBatchClient = Session.CreateClientByInterface<IAmazonBatch>();

		Assert.NotNull(amazonBatchClient);
		AssertAmazonClient.AssertClientConfiguration(amazonBatchClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonOrganizationsClient()
	{
		AmazonServiceClient amazonOrganizationsClient = Session.CreateClientByInterface<IAmazonOrganizations>();

		Assert.NotNull(amazonOrganizationsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonOrganizationsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAutoScalingClient()
	{
		AmazonServiceClient amazonAutoScalingClient = Session.CreateClientByInterface<IAmazonAutoScaling>();

		Assert.NotNull(amazonAutoScalingClient);
		AssertAmazonClient.AssertClientConfiguration(amazonAutoScalingClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonMediaStoreClient()
	{
		AmazonServiceClient amazonMediaStoreClient = Session.CreateClientByInterface<IAmazonMediaStore>();

		Assert.NotNull(amazonMediaStoreClient);
		AssertAmazonClient.AssertClientConfiguration(amazonMediaStoreClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonMediaStoreDataClient_With_ServiceUrl()
	{
		AmazonServiceClient amazonMediaStoreDataClient = Session.CreateClientByInterface<IAmazonMediaStoreData>(useServiceUrl:true);

		Assert.NotNull(amazonMediaStoreDataClient);
		AssertAmazonClient.AssertClientConfiguration(amazonMediaStoreDataClient);
	}

	[Fact]
	public void Should_Throw_AmazonClientException_When_Creating_AmazonMediaStoreDataClient_If_RegionEndpoint_Used()
	{
		try
		{
			Session.CreateClientByInterface<IAmazonMediaStoreData>();
		}
		catch (Exception e)
		{
			Exception ex = e;

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
	}

	[Fact]
	public void Should_Able_To_Create_AmazonTransferClient()
	{
		AmazonServiceClient amazonTransferClient = Session.CreateClientByInterface<IAmazonTransfer>();

		Assert.NotNull(amazonTransferClient);
		AssertAmazonClient.AssertClientConfiguration(amazonTransferClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCertificateManagerClient()
	{
		AmazonServiceClient amazonCertificateManagerClient = Session.CreateClientByInterface<IAmazonCertificateManager>();

		Assert.NotNull(amazonCertificateManagerClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCertificateManagerClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCodeCommitClient()
	{
		AmazonServiceClient amazonCodeCommitClient = Session.CreateClientByInterface<IAmazonCodeCommit>();

		Assert.NotNull(amazonCodeCommitClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCodeCommitClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonKinesisAnalyticsClient()
	{
		AmazonServiceClient amazonKinesisAnalyticsClient = Session.CreateClientByInterface<IAmazonKinesisAnalytics>();

		Assert.NotNull(amazonKinesisAnalyticsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonKinesisAnalyticsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonKinesisAnalyticsV2Client()
	{
		AmazonServiceClient kinesisAnalyticsV2Client = Session.CreateClientByInterface<IAmazonKinesisAnalyticsV2>();

		Assert.NotNull(kinesisAnalyticsV2Client);
		AssertAmazonClient.AssertClientConfiguration(kinesisAnalyticsV2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAmplifyClient()
	{
		AmazonServiceClient amazonAmplifyClient = Session.CreateClientByInterface<IAmazonAmplify>();

		Assert.NotNull(amazonAmplifyClient);
		AssertAmazonClient.AssertClientConfiguration(amazonAmplifyClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonKafkaClient()
	{
		AmazonServiceClient amazonKafkaClient = Session.CreateClientByInterface<IAmazonKafka>();

		Assert.NotNull(amazonKafkaClient);
		AssertAmazonClient.AssertClientConfiguration(amazonKafkaClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonRedshiftDataAPIServiceClient()
	{
		AmazonServiceClient amazonRedshiftDataApiServiceClient = Session.CreateClientByInterface<IAmazonRedshiftDataAPIService>();

		Assert.NotNull(amazonRedshiftDataApiServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonRedshiftDataApiServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonApiGatewayManagementApiClient()
	{
		AmazonServiceClient amazonApiGatewayManagementApiClient = Session.CreateClientByInterface<IAmazonApiGatewayManagementApi>();

		Assert.NotNull(amazonApiGatewayManagementApiClient);
		AssertAmazonClient.AssertClientConfiguration(amazonApiGatewayManagementApiClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonTimestreamQueryClient()
	{
		AmazonServiceClient amazonTimestreamQueryClient = Session.CreateClientByInterface<IAmazonTimestreamQuery>();

		Assert.NotNull(amazonTimestreamQueryClient);
		AssertAmazonClient.AssertClientConfiguration(amazonTimestreamQueryClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonTimestreamWriteClient()
	{
		AmazonServiceClient amazonTimestreamWriteClient = Session.CreateClientByInterface<IAmazonTimestreamWrite>();

		Assert.NotNull(amazonTimestreamWriteClient);
		AssertAmazonClient.AssertClientConfiguration(amazonTimestreamWriteClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonS3ControlClient()
	{
		AmazonServiceClient amazonS3ControlClient = Session.CreateClientByInterface<IAmazonS3Control>();

		Assert.NotNull(amazonS3ControlClient);
		AssertAmazonClient.AssertClientConfiguration(amazonS3ControlClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticLoadBalancingV2Client()
	{
		AmazonServiceClient amazonElasticLoadBalancingV2Client = Session.CreateClientByInterface<IAmazonElasticLoadBalancingV2>();

		Assert.NotNull(amazonElasticLoadBalancingV2Client);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticLoadBalancingV2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonSupportClient()
	{
		AmazonServiceClient amazonSupportClient = Session.CreateClientByInterface<IAmazonAWSSupport>();

		Assert.NotNull(amazonSupportClient);
		AssertAmazonClient.AssertClientConfiguration(amazonSupportClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonNeptuneClient()
	{
		AmazonServiceClient amazonNeptuneClient = Session.CreateClientByInterface<IAmazonNeptune>();

		Assert.NotNull(amazonNeptuneClient);
		AssertAmazonClient.AssertClientConfiguration(amazonNeptuneClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonDocDBClient()
	{
		AmazonServiceClient amazonDocDbClient = Session.CreateClientByInterface<IAmazonDocDB>();

		Assert.NotNull(amazonDocDbClient);
		AssertAmazonClient.AssertClientConfiguration(amazonDocDbClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonServiceDiscoveryClient()
	{
		AmazonServiceClient amazonServiceDiscoveryClient = Session.CreateClientByInterface<IAmazonServiceDiscovery>();

		Assert.NotNull(amazonServiceDiscoveryClient);
		AssertAmazonClient.AssertClientConfiguration(amazonServiceDiscoveryClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonServerlessApplicationRepositoryClient()
	{
		AmazonServiceClient amazonServiceServerlessApplicationRepositoryClient = Session.CreateClientByInterface<IAmazonServerlessApplicationRepository>();

		Assert.NotNull(amazonServiceServerlessApplicationRepositoryClient);
		AssertAmazonClient.AssertClientConfiguration(amazonServiceServerlessApplicationRepositoryClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonAppConfigClient()
	{
		AmazonServiceClient amazonAppConfigClient = Session.CreateClientByInterface<IAmazonAppConfig>();

		Assert.NotNull(amazonAppConfigClient);
		AssertAmazonClient.AssertClientConfiguration(amazonAppConfigClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonCostExplorerClient()
	{
		AmazonServiceClient amazonCostExplorerClient = Session.CreateClientByInterface<IAmazonCostExplorer>();

		Assert.NotNull(amazonCostExplorerClient);
		AssertAmazonClient.AssertClientConfiguration(amazonCostExplorerClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonMediaConvertClient()
	{
		AmazonServiceClient amazonMediaConvertClient = Session.CreateClientByInterface<IAmazonMediaConvert>();

		Assert.NotNull(amazonMediaConvertClient);
		AssertAmazonClient.AssertClientConfiguration(amazonMediaConvertClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonResourceGroupsTaggingAPIClient()
	{
		AmazonServiceClient amazonResourceGroupsTaggingApiClient = Session.CreateClientByInterface<IAmazonResourceGroupsTaggingAPI>();

		Assert.NotNull(amazonResourceGroupsTaggingApiClient);
		AssertAmazonClient.AssertClientConfiguration(amazonResourceGroupsTaggingApiClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonResourceGroupsClient()
	{
		AmazonServiceClient amazonResourceGroupsClient = Session.CreateClientByInterface<IAmazonResourceGroups>();

		Assert.NotNull(amazonResourceGroupsClient);
		AssertAmazonClient.AssertClientConfiguration(amazonResourceGroupsClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonElasticFileSystemClient()
	{
		AmazonServiceClient amazonElasticFileSystemClient = Session.CreateClientByInterface<IAmazonElasticFileSystem>();

		Assert.NotNull(amazonElasticFileSystemClient);
		AssertAmazonClient.AssertClientConfiguration(amazonElasticFileSystemClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonBackupClient()
	{
		AmazonServiceClient amazonBackupClient = Session.CreateClientByInterface<IAmazonBackup>();

		Assert.NotNull(amazonBackupClient);
		AssertAmazonClient.AssertClientConfiguration(amazonBackupClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonLakeFormationClient()
	{
		AmazonServiceClient amazonLakeFormationClient = Session.CreateClientByInterface<IAmazonLakeFormation>();

		Assert.NotNull(amazonLakeFormationClient);
		AssertAmazonClient.AssertClientConfiguration(amazonLakeFormationClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonWAFClient()
	{
		AmazonServiceClient amazonWafClient = Session.CreateClientByInterface<IAmazonWAF>();

		Assert.NotNull(amazonWafClient);
		AssertAmazonClient.AssertClientConfiguration(amazonWafClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonWAFV2Client()
	{
		AmazonServiceClient amazonWafV2Client = Session.CreateClientByInterface<IAmazonWAFV2>();

		Assert.NotNull(amazonWafV2Client);
		AssertAmazonClient.AssertClientConfiguration(amazonWafV2Client);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonConfigServiceClient()
	{
		AmazonServiceClient amazonConfigServiceClient = Session.CreateClientByInterface<IAmazonConfigService>();

		Assert.NotNull(amazonConfigServiceClient);
		AssertAmazonClient.AssertClientConfiguration(amazonConfigServiceClient);
	}

	[Fact]
	public void Should_Able_To_Create_AmazonMWAAClient()
	{
		AmazonServiceClient amazonMwaaClient = Session.CreateClientByInterface<IAmazonMWAA>();

		Assert.NotNull(amazonMwaaClient);
		AssertAmazonClient.AssertClientConfiguration(amazonMwaaClient);
	}
}
