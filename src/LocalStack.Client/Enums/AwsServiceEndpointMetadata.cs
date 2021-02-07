using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LocalStack.Client.Enums
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AwsServiceEndpointMetadata
    {
        private const string CommonEndpointPattern = "{0}://{1}:{2}";

        public static readonly AwsServiceEndpointMetadata ApiGateway = new AwsServiceEndpointMetadata("API Gateway", "apigateway", CommonEndpointPattern, 4567, AwsServiceEnum.ApiGateway);
        public static readonly AwsServiceEndpointMetadata ApiGatewayV2 = new AwsServiceEndpointMetadata("ApiGatewayV2", "apigatewayv2", CommonEndpointPattern, 4567, AwsServiceEnum.ApiGatewayV2);
        public static readonly AwsServiceEndpointMetadata Kinesis = new AwsServiceEndpointMetadata("Kinesis", "kinesis", CommonEndpointPattern, 4568, AwsServiceEnum.Kinesis);
        public static readonly AwsServiceEndpointMetadata DynamoDb = new AwsServiceEndpointMetadata("DynamoDB", "dynamodb", CommonEndpointPattern, 4569, AwsServiceEnum.DynamoDb);
        public static readonly AwsServiceEndpointMetadata DynamoDbStreams = new AwsServiceEndpointMetadata("DynamoDB Streams", "dynamodbstreams", CommonEndpointPattern, 4570, AwsServiceEnum.DynamoDbStreams);
        public static readonly AwsServiceEndpointMetadata ElasticSearch = new AwsServiceEndpointMetadata("Elasticsearch Service", "elasticsearch", CommonEndpointPattern, 4571, AwsServiceEnum.ElasticSearch);
        public static readonly AwsServiceEndpointMetadata S3 = new AwsServiceEndpointMetadata("S3", "s3", CommonEndpointPattern, 4572, AwsServiceEnum.S3);
        public static readonly AwsServiceEndpointMetadata Firehose = new AwsServiceEndpointMetadata("Firehose", "firehose", CommonEndpointPattern, 4573, AwsServiceEnum.Firehose);
        public static readonly AwsServiceEndpointMetadata Lambda = new AwsServiceEndpointMetadata("Lambda", "lambda", CommonEndpointPattern, 4574, AwsServiceEnum.Lambda);
        public static readonly AwsServiceEndpointMetadata Sns = new AwsServiceEndpointMetadata("SNS", "sns", CommonEndpointPattern, 4575, AwsServiceEnum.Sns);
        public static readonly AwsServiceEndpointMetadata Sqs = new AwsServiceEndpointMetadata("SQS", "sqs", CommonEndpointPattern, 4576, AwsServiceEnum.Sqs);
        public static readonly AwsServiceEndpointMetadata Redshift = new AwsServiceEndpointMetadata("Redshift", "redshift", CommonEndpointPattern, 4577, AwsServiceEnum.Redshift);
        public static readonly AwsServiceEndpointMetadata RedshiftData = new AwsServiceEndpointMetadata("Redshift Data", "redshift-data", CommonEndpointPattern, 4577, AwsServiceEnum.RedshiftData);
        public static readonly AwsServiceEndpointMetadata Es = new AwsServiceEndpointMetadata("ES", "es", CommonEndpointPattern, 4578, AwsServiceEnum.Es);
        public static readonly AwsServiceEndpointMetadata Ses = new AwsServiceEndpointMetadata("SES", "ses", CommonEndpointPattern, 4579, AwsServiceEnum.Ses);
        public static readonly AwsServiceEndpointMetadata Route53 = new AwsServiceEndpointMetadata("Route 53", "route53", CommonEndpointPattern, 4580, AwsServiceEnum.Route53);
        public static readonly AwsServiceEndpointMetadata CloudFormation = new AwsServiceEndpointMetadata("CloudFormation", "cloudformation", CommonEndpointPattern, 4581, AwsServiceEnum.CloudFormation);
        public static readonly AwsServiceEndpointMetadata CloudWatch = new AwsServiceEndpointMetadata("CloudWatch", "cloudwatch", CommonEndpointPattern, 4582, AwsServiceEnum.CloudWatch);
        public static readonly AwsServiceEndpointMetadata Ssm = new AwsServiceEndpointMetadata("SSM", "ssm", CommonEndpointPattern, 4583, AwsServiceEnum.Ssm);
        public static readonly AwsServiceEndpointMetadata SecretsManager = new AwsServiceEndpointMetadata("Secrets Manager", "secretsmanager", CommonEndpointPattern, 4584, AwsServiceEnum.SecretsManager);
        public static readonly AwsServiceEndpointMetadata StepFunctions = new AwsServiceEndpointMetadata("SFN", "stepfunctions", CommonEndpointPattern, 4585, AwsServiceEnum.StepFunctions);
        public static readonly AwsServiceEndpointMetadata Logs = new AwsServiceEndpointMetadata("CloudWatch Logs", "logs", CommonEndpointPattern, 4586, AwsServiceEnum.Logs);
        public static readonly AwsServiceEndpointMetadata Events = new AwsServiceEndpointMetadata("CloudWatch Events", "events", CommonEndpointPattern, 4587, AwsServiceEnum.Events);
        public static readonly AwsServiceEndpointMetadata Elb = new AwsServiceEndpointMetadata("Elastic Load Balancing", "elb", CommonEndpointPattern, 4588, AwsServiceEnum.Elb);
        public static readonly AwsServiceEndpointMetadata Iot = new AwsServiceEndpointMetadata("IoT", "iot", CommonEndpointPattern, 4589, AwsServiceEnum.Iot);
        public static readonly AwsServiceEndpointMetadata CognitoIdp = new AwsServiceEndpointMetadata("Cognito Identity Provider", "cognito-idp", CommonEndpointPattern, 4590, AwsServiceEnum.CognitoIdp);
        public static readonly AwsServiceEndpointMetadata CognitoIdentity = new AwsServiceEndpointMetadata("Cognito Identity", "cognito-identity", CommonEndpointPattern, 4591, AwsServiceEnum.CognitoIdentity);
        public static readonly AwsServiceEndpointMetadata Sts = new AwsServiceEndpointMetadata("STS", "sts", CommonEndpointPattern, 4592, AwsServiceEnum.Sts);
        public static readonly AwsServiceEndpointMetadata Iam = new AwsServiceEndpointMetadata("IAM", "iam", CommonEndpointPattern, 4593, AwsServiceEnum.Iam);
        public static readonly AwsServiceEndpointMetadata Rds = new AwsServiceEndpointMetadata("RDS", "rds", CommonEndpointPattern, 4594, AwsServiceEnum.Rds);
        public static readonly AwsServiceEndpointMetadata RdsData = new AwsServiceEndpointMetadata("RDS Data", "rds-data", CommonEndpointPattern, 4594, AwsServiceEnum.RdsData);
        public static readonly AwsServiceEndpointMetadata CloudSearch = new AwsServiceEndpointMetadata("CloudSearch", "cloudsearch", CommonEndpointPattern, 4595, AwsServiceEnum.CloudSearch);
        public static readonly AwsServiceEndpointMetadata Swf = new AwsServiceEndpointMetadata("SWF", "swf", CommonEndpointPattern, 4596, AwsServiceEnum.Swf);
        public static readonly AwsServiceEndpointMetadata Ec2 = new AwsServiceEndpointMetadata("EC2", "ec2", CommonEndpointPattern, 4597, AwsServiceEnum.Ec2);
        public static readonly AwsServiceEndpointMetadata ElastiCache = new AwsServiceEndpointMetadata("ElastiCache", "elasticache", CommonEndpointPattern, 4598, AwsServiceEnum.ElastiCache);
        public static readonly AwsServiceEndpointMetadata Kms = new AwsServiceEndpointMetadata("KMS", "kms", CommonEndpointPattern, 4599, AwsServiceEnum.Kms);
        public static readonly AwsServiceEndpointMetadata Emr = new AwsServiceEndpointMetadata("EMR", "emr", CommonEndpointPattern, 4600, AwsServiceEnum.Emr);
        public static readonly AwsServiceEndpointMetadata Ecs = new AwsServiceEndpointMetadata("ECS", "ecs", CommonEndpointPattern, 4601, AwsServiceEnum.Ecs);
        public static readonly AwsServiceEndpointMetadata Eks = new AwsServiceEndpointMetadata("EKS", "eks", CommonEndpointPattern, 4602, AwsServiceEnum.Eks);
        public static readonly AwsServiceEndpointMetadata XRay = new AwsServiceEndpointMetadata("XRay", "xray", CommonEndpointPattern, 4603, AwsServiceEnum.XRay);
        public static readonly AwsServiceEndpointMetadata ElasticBeanstalk = new AwsServiceEndpointMetadata("Elastic Beanstalk", "elasticbeanstalk", CommonEndpointPattern, 4604, AwsServiceEnum.ElasticBeanstalk);
        public static readonly AwsServiceEndpointMetadata AppSync = new AwsServiceEndpointMetadata("AppSync", "appsync", CommonEndpointPattern, 4605, AwsServiceEnum.AppSync);
        public static readonly AwsServiceEndpointMetadata CloudFront = new AwsServiceEndpointMetadata("CloudFront", "cloudfront", CommonEndpointPattern, 4606, AwsServiceEnum.CloudFront);
        public static readonly AwsServiceEndpointMetadata Athena = new AwsServiceEndpointMetadata("Athena", "athena", CommonEndpointPattern, 4607, AwsServiceEnum.Athena);
        public static readonly AwsServiceEndpointMetadata Glue = new AwsServiceEndpointMetadata("Glue", "glue", CommonEndpointPattern, 4608, AwsServiceEnum.Glue);
        public static readonly AwsServiceEndpointMetadata SageMaker = new AwsServiceEndpointMetadata("SageMaker", "sagemaker", CommonEndpointPattern, 4609, AwsServiceEnum.SageMaker);
        public static readonly AwsServiceEndpointMetadata SageMakerRuntime = new AwsServiceEndpointMetadata("SageMaker Runtime", "sagemaker-runtime", CommonEndpointPattern, 4609, AwsServiceEnum.SageMakerRuntime);
        public static readonly AwsServiceEndpointMetadata Ecr = new AwsServiceEndpointMetadata("ECR", "ecr", CommonEndpointPattern, 4610, AwsServiceEnum.Ecr);
        public static readonly AwsServiceEndpointMetadata Qldb = new AwsServiceEndpointMetadata("QLDB", "qldb", CommonEndpointPattern, 4611, AwsServiceEnum.Qldb);
        public static readonly AwsServiceEndpointMetadata CloudTrail = new AwsServiceEndpointMetadata("CloudTrail", "cloudtrail", CommonEndpointPattern, 4612, AwsServiceEnum.CloudTrail);
        public static readonly AwsServiceEndpointMetadata Glacier = new AwsServiceEndpointMetadata("Glacier", "glacier", CommonEndpointPattern, 4613, AwsServiceEnum.Glacier);
        public static readonly AwsServiceEndpointMetadata Batch = new AwsServiceEndpointMetadata("Batch", "batch", CommonEndpointPattern, 4614, AwsServiceEnum.Batch);
        public static readonly AwsServiceEndpointMetadata Organizations = new AwsServiceEndpointMetadata("Organizations", "organizations", CommonEndpointPattern, 4615, AwsServiceEnum.Organizations);
        public static readonly AwsServiceEndpointMetadata AutoScaling = new AwsServiceEndpointMetadata("Auto Scaling", "autoscaling", CommonEndpointPattern, 4616, AwsServiceEnum.AutoScaling);
        public static readonly AwsServiceEndpointMetadata MediaStore = new AwsServiceEndpointMetadata("MediaStore", "mediastore", CommonEndpointPattern, 4617, AwsServiceEnum.MediaStore);
        public static readonly AwsServiceEndpointMetadata MediaStoreData = new AwsServiceEndpointMetadata("MediaStore Data", "mediastore-data", CommonEndpointPattern, 4617, AwsServiceEnum.MediaStoreData);
        public static readonly AwsServiceEndpointMetadata Transfer = new AwsServiceEndpointMetadata("Transfer", "transfer", CommonEndpointPattern, 4618, AwsServiceEnum.Transfer);
        public static readonly AwsServiceEndpointMetadata Acm = new AwsServiceEndpointMetadata("ACM", "acm", CommonEndpointPattern, 4619, AwsServiceEnum.Acm);
        public static readonly AwsServiceEndpointMetadata CodeCommit = new AwsServiceEndpointMetadata("CodeCommit", "codecommit", CommonEndpointPattern, 4620, AwsServiceEnum.CodeCommit);
        public static readonly AwsServiceEndpointMetadata KinesisAnalytics = new AwsServiceEndpointMetadata("Kinesis Analytics", "kinesisanalytics", CommonEndpointPattern, 4621, AwsServiceEnum.KinesisAnalytics);
        public static readonly AwsServiceEndpointMetadata Amplify = new AwsServiceEndpointMetadata("Amplify", "amplify", CommonEndpointPattern, 4622, AwsServiceEnum.Amplify);
        public static readonly AwsServiceEndpointMetadata ApplicationAutoscaling = new AwsServiceEndpointMetadata("Application Auto Scaling", "application-autoscaling", CommonEndpointPattern, 4623, AwsServiceEnum.ApplicationAutoscaling);
        public static readonly AwsServiceEndpointMetadata Kafka = new AwsServiceEndpointMetadata("Kafka", "kafka", CommonEndpointPattern, 4624, AwsServiceEnum.Kafka);
        public static readonly AwsServiceEndpointMetadata ApiGatewayManagementApi = new AwsServiceEndpointMetadata("ApiGatewayManagementApi", "apigatewaymanagementapi", CommonEndpointPattern, 4625, AwsServiceEnum.ApiGatewayManagementApi);
        public static readonly AwsServiceEndpointMetadata TimeStreamQuery = new AwsServiceEndpointMetadata("Timestream Query", "timestream-query", CommonEndpointPattern, 4626, AwsServiceEnum.TimeStreamQuery);
        public static readonly AwsServiceEndpointMetadata TimeStreamWrite = new AwsServiceEndpointMetadata("Timestream Write", "timestream-write", CommonEndpointPattern, 4626, AwsServiceEnum.TimeStreamWrite);
        public static readonly AwsServiceEndpointMetadata S3Control = new AwsServiceEndpointMetadata("S3 Control", "s3control", CommonEndpointPattern, 4627, AwsServiceEnum.S3Control);
        public static readonly AwsServiceEndpointMetadata ElbV2 = new AwsServiceEndpointMetadata("Elastic Load Balancing v2", "elbv2", CommonEndpointPattern, 4628, AwsServiceEnum.ElbV2);

        public static readonly AwsServiceEndpointMetadata[] All =
        {
            ApiGateway, ApiGatewayV2, Kinesis, DynamoDb, DynamoDbStreams, ElasticSearch, S3, Firehose, Lambda, Sns, Sqs, Redshift, RedshiftData, Es, Ses, Route53, CloudFormation, CloudWatch,
            Ssm, SecretsManager, StepFunctions, Logs, Events, Elb, Iot, CognitoIdp, CognitoIdentity, Sts, Iam, Rds, RdsData, CloudSearch, Swf, Ec2, ElastiCache, Kms, Emr, Ecs,
            Eks, XRay, ElasticBeanstalk, AppSync, CloudFront, Athena, Glue, SageMaker, SageMakerRuntime, Ecr, Qldb, CloudTrail, Glacier, Batch, Organizations, AutoScaling, MediaStore, MediaStoreData,
            Transfer, Acm, CodeCommit, KinesisAnalytics, Amplify, ApplicationAutoscaling, Kafka, ApiGatewayManagementApi, TimeStreamQuery, TimeStreamWrite, S3Control, ElbV2
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
}