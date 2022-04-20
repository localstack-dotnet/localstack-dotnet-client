namespace LocalStack.Client.Enums;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class AwsServiceEndpointMetadata
{
    private const string CommonEndpointPattern = "{0}://{1}:{2}";

    public static readonly AwsServiceEndpointMetadata ApiGateway = new("API Gateway", "apigateway", CommonEndpointPattern, 4567, AwsServiceEnum.ApiGateway);
    public static readonly AwsServiceEndpointMetadata ApiGatewayV2 = new("ApiGatewayV2", "apigatewayv2", CommonEndpointPattern, 4567, AwsServiceEnum.ApiGatewayV2);
    public static readonly AwsServiceEndpointMetadata Kinesis = new("Kinesis", "kinesis", CommonEndpointPattern, 4568, AwsServiceEnum.Kinesis);
    public static readonly AwsServiceEndpointMetadata DynamoDb = new("DynamoDB", "dynamodb", CommonEndpointPattern, 4569, AwsServiceEnum.DynamoDb);
    public static readonly AwsServiceEndpointMetadata DynamoDbStreams = new("DynamoDB Streams", "dynamodbstreams", CommonEndpointPattern, 4570, AwsServiceEnum.DynamoDbStreams);
    public static readonly AwsServiceEndpointMetadata ElasticSearch = new("Elasticsearch Service", "elasticsearch", CommonEndpointPattern, 4571, AwsServiceEnum.ElasticSearch);
    public static readonly AwsServiceEndpointMetadata OpenSearch = new("OpenSearch", "opensearch", CommonEndpointPattern, 4571, AwsServiceEnum.OpenSearch);
    public static readonly AwsServiceEndpointMetadata S3 = new("S3", "s3", CommonEndpointPattern, 4572, AwsServiceEnum.S3);
    public static readonly AwsServiceEndpointMetadata Firehose = new("Firehose", "firehose", CommonEndpointPattern, 4573, AwsServiceEnum.Firehose);
    public static readonly AwsServiceEndpointMetadata Lambda = new("Lambda", "lambda", CommonEndpointPattern, 4574, AwsServiceEnum.Lambda);
    public static readonly AwsServiceEndpointMetadata Sns = new("SNS", "sns", CommonEndpointPattern, 4575, AwsServiceEnum.Sns);
    public static readonly AwsServiceEndpointMetadata Sqs = new("SQS", "sqs", CommonEndpointPattern, 4576, AwsServiceEnum.Sqs);
    public static readonly AwsServiceEndpointMetadata Redshift = new("Redshift", "redshift", CommonEndpointPattern, 4577, AwsServiceEnum.Redshift);
    public static readonly AwsServiceEndpointMetadata RedshiftData = new("Redshift Data", "redshift-data", CommonEndpointPattern, 4577, AwsServiceEnum.RedshiftData);
    public static readonly AwsServiceEndpointMetadata Es = new("ES", "es", CommonEndpointPattern, 4578, AwsServiceEnum.Es);
    public static readonly AwsServiceEndpointMetadata Ses = new("SES", "ses", CommonEndpointPattern, 4579, AwsServiceEnum.Ses);
    public static readonly AwsServiceEndpointMetadata Sesv2 = new("SESv2", "sesv2", CommonEndpointPattern, 4579, AwsServiceEnum.Sesv2);
    public static readonly AwsServiceEndpointMetadata Route53 = new("Route 53", "route53", CommonEndpointPattern, 4580, AwsServiceEnum.Route53);
    public static readonly AwsServiceEndpointMetadata Route53Resolver = new("Route53Resolver", "route53resolver", CommonEndpointPattern, 4580, AwsServiceEnum.Route53Resolver);
    public static readonly AwsServiceEndpointMetadata CloudFormation = new("CloudFormation", "cloudformation", CommonEndpointPattern, 4581, AwsServiceEnum.CloudFormation);
    public static readonly AwsServiceEndpointMetadata CloudWatch = new("CloudWatch", "cloudwatch", CommonEndpointPattern, 4582, AwsServiceEnum.CloudWatch);
    public static readonly AwsServiceEndpointMetadata Ssm = new("SSM", "ssm", CommonEndpointPattern, 4583, AwsServiceEnum.Ssm);
    public static readonly AwsServiceEndpointMetadata SecretsManager = new("Secrets Manager", "secretsmanager", CommonEndpointPattern, 4584, AwsServiceEnum.SecretsManager);
    public static readonly AwsServiceEndpointMetadata StepFunctions = new("SFN", "stepfunctions", CommonEndpointPattern, 4585, AwsServiceEnum.StepFunctions);
    public static readonly AwsServiceEndpointMetadata Logs = new("CloudWatch Logs", "logs", CommonEndpointPattern, 4586, AwsServiceEnum.Logs);
    public static readonly AwsServiceEndpointMetadata Events = new("CloudWatch Events", "events", CommonEndpointPattern, 4587, AwsServiceEnum.Events);
    public static readonly AwsServiceEndpointMetadata Elb = new("Elastic Load Balancing", "elb", CommonEndpointPattern, 4588, AwsServiceEnum.Elb);
    public static readonly AwsServiceEndpointMetadata Iot = new("IoT", "iot", CommonEndpointPattern, 4589, AwsServiceEnum.Iot);
    public static readonly AwsServiceEndpointMetadata IoTAnalytics = new("IoTAnalytics", "iotanalytics", CommonEndpointPattern, 4589, AwsServiceEnum.IoTAnalytics);
    public static readonly AwsServiceEndpointMetadata IoTEvents = new("IoT Events", "iotevents", CommonEndpointPattern, 4589, AwsServiceEnum.IoTEvents);
    public static readonly AwsServiceEndpointMetadata IoTEventsData = new("IoT Events Data", "iotevents-data", CommonEndpointPattern, 4589, AwsServiceEnum.IoTEventsData);
    public static readonly AwsServiceEndpointMetadata IoTWireless = new("IoT Wireless", "iotwireless", CommonEndpointPattern, 4589, AwsServiceEnum.IoTWireless);
    public static readonly AwsServiceEndpointMetadata IoTDataPlane = new("IoT Data Plane", "iot-data", CommonEndpointPattern, 4589, AwsServiceEnum.IoTDataPlane);
    public static readonly AwsServiceEndpointMetadata IoTJobsDataPlane = new("IoT Jobs Data Plane", "iot-jobs-data", CommonEndpointPattern, 4589, AwsServiceEnum.IoTJobsDataPlane);
    public static readonly AwsServiceEndpointMetadata CognitoIdp = new("Cognito Identity Provider", "cognito-idp", CommonEndpointPattern, 4590, AwsServiceEnum.CognitoIdp);
    public static readonly AwsServiceEndpointMetadata CognitoIdentity = new("Cognito Identity", "cognito-identity", CommonEndpointPattern, 4591, AwsServiceEnum.CognitoIdentity);
    public static readonly AwsServiceEndpointMetadata Sts = new("STS", "sts", CommonEndpointPattern, 4592, AwsServiceEnum.Sts);
    public static readonly AwsServiceEndpointMetadata Iam = new("IAM", "iam", CommonEndpointPattern, 4593, AwsServiceEnum.Iam);
    public static readonly AwsServiceEndpointMetadata Rds = new("RDS", "rds", CommonEndpointPattern, 4594, AwsServiceEnum.Rds);
    public static readonly AwsServiceEndpointMetadata RdsData = new("RDS Data", "rds-data", CommonEndpointPattern, 4594, AwsServiceEnum.RdsData);
    public static readonly AwsServiceEndpointMetadata CloudSearch = new("CloudSearch", "cloudsearch", CommonEndpointPattern, 4595, AwsServiceEnum.CloudSearch);
    public static readonly AwsServiceEndpointMetadata Swf = new("SWF", "swf", CommonEndpointPattern, 4596, AwsServiceEnum.Swf);
    public static readonly AwsServiceEndpointMetadata Ec2 = new("EC2", "ec2", CommonEndpointPattern, 4597, AwsServiceEnum.Ec2);
    public static readonly AwsServiceEndpointMetadata ElastiCache = new("ElastiCache", "elasticache", CommonEndpointPattern, 4598, AwsServiceEnum.ElastiCache);
    public static readonly AwsServiceEndpointMetadata Kms = new("KMS", "kms", CommonEndpointPattern, 4599, AwsServiceEnum.Kms);
    public static readonly AwsServiceEndpointMetadata Emr = new("EMR", "emr", CommonEndpointPattern, 4600, AwsServiceEnum.Emr);
    public static readonly AwsServiceEndpointMetadata Ecs = new("ECS", "ecs", CommonEndpointPattern, 4601, AwsServiceEnum.Ecs);
    public static readonly AwsServiceEndpointMetadata Eks = new("EKS", "eks", CommonEndpointPattern, 4602, AwsServiceEnum.Eks);
    public static readonly AwsServiceEndpointMetadata XRay = new("XRay", "xray", CommonEndpointPattern, 4603, AwsServiceEnum.XRay);
    public static readonly AwsServiceEndpointMetadata ElasticBeanstalk = new("Elastic Beanstalk", "elasticbeanstalk", CommonEndpointPattern, 4604, AwsServiceEnum.ElasticBeanstalk);
    public static readonly AwsServiceEndpointMetadata AppSync = new("AppSync", "appsync", CommonEndpointPattern, 4605, AwsServiceEnum.AppSync);
    public static readonly AwsServiceEndpointMetadata CloudFront = new("CloudFront", "cloudfront", CommonEndpointPattern, 4606, AwsServiceEnum.CloudFront);
    public static readonly AwsServiceEndpointMetadata Athena = new("Athena", "athena", CommonEndpointPattern, 4607, AwsServiceEnum.Athena);
    public static readonly AwsServiceEndpointMetadata Glue = new("Glue", "glue", CommonEndpointPattern, 4608, AwsServiceEnum.Glue);
    public static readonly AwsServiceEndpointMetadata SageMaker = new("SageMaker", "sagemaker", CommonEndpointPattern, 4609, AwsServiceEnum.SageMaker);
    public static readonly AwsServiceEndpointMetadata SageMakerRuntime = new("SageMaker Runtime", "sagemaker-runtime", CommonEndpointPattern, 4609, AwsServiceEnum.SageMakerRuntime);
    public static readonly AwsServiceEndpointMetadata Ecr = new("ECR", "ecr", CommonEndpointPattern, 4610, AwsServiceEnum.Ecr);
    public static readonly AwsServiceEndpointMetadata Qldb = new("QLDB", "qldb", CommonEndpointPattern, 4611, AwsServiceEnum.Qldb);
    public static readonly AwsServiceEndpointMetadata QldbSession = new("QLDB Session", "qldb-session", CommonEndpointPattern, 4611, AwsServiceEnum.QldbSession);
    public static readonly AwsServiceEndpointMetadata CloudTrail = new("CloudTrail", "cloudtrail", CommonEndpointPattern, 4612, AwsServiceEnum.CloudTrail);
    public static readonly AwsServiceEndpointMetadata Glacier = new("Glacier", "glacier", CommonEndpointPattern, 4613, AwsServiceEnum.Glacier);
    public static readonly AwsServiceEndpointMetadata Batch = new("Batch", "batch", CommonEndpointPattern, 4614, AwsServiceEnum.Batch);
    public static readonly AwsServiceEndpointMetadata Organizations = new("Organizations", "organizations", CommonEndpointPattern, 4615, AwsServiceEnum.Organizations);
    public static readonly AwsServiceEndpointMetadata AutoScaling = new("Auto Scaling", "autoscaling", CommonEndpointPattern, 4616, AwsServiceEnum.AutoScaling);
    public static readonly AwsServiceEndpointMetadata MediaStore = new("MediaStore", "mediastore", CommonEndpointPattern, 4617, AwsServiceEnum.MediaStore);
    public static readonly AwsServiceEndpointMetadata MediaStoreData = new("MediaStore Data", "mediastore-data", CommonEndpointPattern, 4617, AwsServiceEnum.MediaStoreData);
    public static readonly AwsServiceEndpointMetadata Transfer = new("Transfer", "transfer", CommonEndpointPattern, 4618, AwsServiceEnum.Transfer);
    public static readonly AwsServiceEndpointMetadata Acm = new("ACM", "acm", CommonEndpointPattern, 4619, AwsServiceEnum.Acm);
    public static readonly AwsServiceEndpointMetadata CodeCommit = new("CodeCommit", "codecommit", CommonEndpointPattern, 4620, AwsServiceEnum.CodeCommit);
    public static readonly AwsServiceEndpointMetadata KinesisAnalytics = new("Kinesis Analytics", "kinesisanalytics", CommonEndpointPattern, 4621, AwsServiceEnum.KinesisAnalytics);
    public static readonly AwsServiceEndpointMetadata KinesisAnalyticsV2 = new("Kinesis Analytics V2", "kinesisanalyticsv2", CommonEndpointPattern, 4621, AwsServiceEnum.KinesisAnalyticsV2);
    public static readonly AwsServiceEndpointMetadata Amplify = new("Amplify", "amplify", CommonEndpointPattern, 4622, AwsServiceEnum.Amplify);
    public static readonly AwsServiceEndpointMetadata ApplicationAutoscaling = new("Application Auto Scaling", "application-autoscaling", CommonEndpointPattern, 4623, AwsServiceEnum.ApplicationAutoscaling);
    public static readonly AwsServiceEndpointMetadata Kafka = new("Kafka", "kafka", CommonEndpointPattern, 4624, AwsServiceEnum.Kafka);
    public static readonly AwsServiceEndpointMetadata ApiGatewayManagementApi = new("ApiGatewayManagementApi", "apigatewaymanagementapi", CommonEndpointPattern, 4625, AwsServiceEnum.ApiGatewayManagementApi);
    public static readonly AwsServiceEndpointMetadata TimeStreamQuery = new("Timestream Query", "timestream-query", CommonEndpointPattern, 4626, AwsServiceEnum.TimeStreamQuery);
    public static readonly AwsServiceEndpointMetadata TimeStreamWrite = new("Timestream Write", "timestream-write", CommonEndpointPattern, 4626, AwsServiceEnum.TimeStreamWrite);
    public static readonly AwsServiceEndpointMetadata S3Control = new("S3 Control", "s3control", CommonEndpointPattern, 4627, AwsServiceEnum.S3Control);
    public static readonly AwsServiceEndpointMetadata ElbV2 = new("Elastic Load Balancing v2", "elbv2", CommonEndpointPattern, 4628, AwsServiceEnum.ElbV2);
    public static readonly AwsServiceEndpointMetadata Support = new("Support", "support", CommonEndpointPattern, 4629, AwsServiceEnum.Support);
    public static readonly AwsServiceEndpointMetadata Neptune = new("Neptune", "neptune", CommonEndpointPattern, 4594, AwsServiceEnum.Neptune);
    public static readonly AwsServiceEndpointMetadata DocDb = new("DocDB", "docdb", CommonEndpointPattern, 4594, AwsServiceEnum.DocDb);
    public static readonly AwsServiceEndpointMetadata ServiceDiscovery = new("ServiceDiscovery", "servicediscovery", CommonEndpointPattern, 4630, AwsServiceEnum.ServiceDiscovery);
    public static readonly AwsServiceEndpointMetadata ServerlessApplicationRepository = new("ServerlessApplicationRepository", "serverlessrepo", CommonEndpointPattern, 4631, AwsServiceEnum.ServerlessApplicationRepository);
    public static readonly AwsServiceEndpointMetadata AppConfig = new("AppConfig", "appconfig", CommonEndpointPattern, 4632, AwsServiceEnum.AppConfig);
    public static readonly AwsServiceEndpointMetadata CostExplorer = new("Cost Explorer", "ce", CommonEndpointPattern, 4633, AwsServiceEnum.CostExplorer);
    public static readonly AwsServiceEndpointMetadata MediaConvert = new("MediaConvert", "mediaconvert", CommonEndpointPattern, 4634, AwsServiceEnum.MediaConvert);
    public static readonly AwsServiceEndpointMetadata ResourceGroupsTaggingApi = new("Resource Groups Tagging API", "resourcegroupstaggingapi", CommonEndpointPattern, 4635, AwsServiceEnum.ResourceGroupsTaggingApi);
    public static readonly AwsServiceEndpointMetadata ResourceGroups = new("Resource Groups", "resource-groups", CommonEndpointPattern, 4636, AwsServiceEnum.ResourceGroups);
    public static readonly AwsServiceEndpointMetadata Efs = new("EFS", "efs", CommonEndpointPattern, 4637, AwsServiceEnum.Efs);
    public static readonly AwsServiceEndpointMetadata Backup = new("Backup", "backup", CommonEndpointPattern, 4638, AwsServiceEnum.Backup);
    public static readonly AwsServiceEndpointMetadata LakeFormation = new("LakeFormation", "lakeformation", CommonEndpointPattern, 4639, AwsServiceEnum.LakeFormation);
    public static readonly AwsServiceEndpointMetadata Waf = new("WAF", "waf", CommonEndpointPattern, 4640, AwsServiceEnum.Waf);
    public static readonly AwsServiceEndpointMetadata WafV2 = new("WAFV2", "wafv2", CommonEndpointPattern, 4640, AwsServiceEnum.WafV2);
    public static readonly AwsServiceEndpointMetadata ConfigService = new("Config Service", "config", CommonEndpointPattern, 4641, AwsServiceEnum.ConfigService);
    public static readonly AwsServiceEndpointMetadata Mwaa = new("MWAA", "mwaa", CommonEndpointPattern, 4642, AwsServiceEnum.Mwaa);
    public static readonly AwsServiceEndpointMetadata EventBridge = new("EventBridge", "eventbridge", CommonEndpointPattern, 4587, AwsServiceEnum.EventBridge);


    public static readonly AwsServiceEndpointMetadata[] All =
    {
        ApiGateway, ApiGatewayV2, Kinesis, DynamoDb, DynamoDbStreams, ElasticSearch, OpenSearch, S3, Firehose, Lambda, Sns, Sqs, Redshift, RedshiftData, Es, Ses, Sesv2, Route53, Route53Resolver, CloudFormation, 
        CloudWatch, Ssm, SecretsManager, StepFunctions, Logs, Events, Elb, Iot, IoTAnalytics, IoTEvents, IoTEventsData, IoTWireless, IoTDataPlane, IoTJobsDataPlane, CognitoIdp, CognitoIdentity, Sts, 
        Iam, Rds, RdsData, CloudSearch, Swf, Ec2, ElastiCache, Kms, Emr, Ecs, Eks, XRay, ElasticBeanstalk, AppSync, CloudFront, Athena, Glue, SageMaker, SageMakerRuntime, Ecr, Qldb, QldbSession, 
        CloudTrail, Glacier, Batch, Organizations, AutoScaling, MediaStore, MediaStoreData, Transfer, Acm, CodeCommit, KinesisAnalytics, KinesisAnalyticsV2, Amplify, ApplicationAutoscaling, Kafka, ApiGatewayManagementApi, 
        TimeStreamQuery, TimeStreamWrite, S3Control, ElbV2, Support, Neptune, DocDb, ServiceDiscovery, ServerlessApplicationRepository, AppConfig, CostExplorer, MediaConvert, ResourceGroupsTaggingApi,
        ResourceGroups, Efs, Backup, LakeFormation, Waf, WafV2, ConfigService, Mwaa, EventBridge
    };

    private AwsServiceEndpointMetadata()
    {
    }

    private AwsServiceEndpointMetadata(string serviceId, string cliName, string endPointPattern, int port, AwsServiceEnum @enum)
    {
        ServiceId = serviceId;
        CliName = cliName;
        EndPointPattern = endPointPattern;
        Enum = @enum;
        Port = port;
    }

    public string ServiceId { get; }

    public string CliName { get; }

    public string EndPointPattern { get; }

    public int Port { get; }

    public AwsServiceEnum Enum { get; }

    public static AwsServiceEndpointMetadata ByName(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return All.SingleOrDefault(service => service.ServiceId == name);
    }

    public static AwsServiceEndpointMetadata ByEnum(AwsServiceEnum @enum)
    {
        return All.SingleOrDefault(service => service.Enum == @enum);
    }

    public static AwsServiceEndpointMetadata ByPort(int port)
    {
        if (port <= 0)
        {
            throw new ArgumentException("Your port number must be greater than 0", nameof(port));
        }

        return All.SingleOrDefault(service => service.Port == port);
    }

    public string GetServiceUrl(string proto, string host, int? port = null)
    {
        return proto == null || host == null
                   ? throw new ArgumentNullException(proto == null ? nameof(proto) : nameof(host))
                   : string.Format(EndPointPattern, proto, host, port ?? Port);
    }

    public override string ToString()
    {
        return $"{ServiceId} - {CliName} - {Port}";
    }
}