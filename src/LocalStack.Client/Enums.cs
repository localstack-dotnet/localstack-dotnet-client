using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LocalStack.Client
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AwsServiceEndpointMetadata
    {
        private const string CommonEndpointPattern = "{0}://{1}:{2}";

        public static readonly AwsServiceEndpointMetadata ApiGateway = new AwsServiceEndpointMetadata("API Gateway", "apigateway", CommonEndpointPattern, 4567, AwsServiceEnum.ApiGateway);
        public static readonly AwsServiceEndpointMetadata Kinesis = new AwsServiceEndpointMetadata("Kinesis", "kinesis", CommonEndpointPattern, 4568, AwsServiceEnum.Kinesis);
        public static readonly AwsServiceEndpointMetadata DynamoDb = new AwsServiceEndpointMetadata("DynamoDB", "dynamodb", CommonEndpointPattern, 4569, AwsServiceEnum.DynamoDb);
        public static readonly AwsServiceEndpointMetadata DynamoDbStreams = new AwsServiceEndpointMetadata("DynamoDB Streams", "dynamodbstreams", CommonEndpointPattern, 4570, AwsServiceEnum.DynamoDbStreams);
        public static readonly AwsServiceEndpointMetadata ElasticSearch = new AwsServiceEndpointMetadata("Elasticsearch Service", "elasticsearch", CommonEndpointPattern, 4571, AwsServiceEnum.ElasticSearch);
        public static readonly AwsServiceEndpointMetadata S3 = new AwsServiceEndpointMetadata("S3", "s3", CommonEndpointPattern, 4572, AwsServiceEnum.S3);
        public static readonly AwsServiceEndpointMetadata Firehose = new AwsServiceEndpointMetadata("Firehose","firehose", CommonEndpointPattern, 4573, AwsServiceEnum.Firehose);
        public static readonly AwsServiceEndpointMetadata Lambda = new AwsServiceEndpointMetadata("Lambda","lambda", CommonEndpointPattern, 4574, AwsServiceEnum.Lambda);
        public static readonly AwsServiceEndpointMetadata Sns = new AwsServiceEndpointMetadata("SNS", "sns", CommonEndpointPattern, 4575, AwsServiceEnum.Sns);
        public static readonly AwsServiceEndpointMetadata Sqs = new AwsServiceEndpointMetadata("SQS","sqs", CommonEndpointPattern, 4576, AwsServiceEnum.Sqs);
        public static readonly AwsServiceEndpointMetadata Redshift = new AwsServiceEndpointMetadata("Redshift", "redshift", CommonEndpointPattern, 4577, AwsServiceEnum.Redshift);
        public static readonly AwsServiceEndpointMetadata Es = new AwsServiceEndpointMetadata("ES", "es", CommonEndpointPattern, 4578, AwsServiceEnum.Es);
        public static readonly AwsServiceEndpointMetadata Ses = new AwsServiceEndpointMetadata("SES", "ses", CommonEndpointPattern, 4579, AwsServiceEnum.Ses);
        public static readonly AwsServiceEndpointMetadata Route53 = new AwsServiceEndpointMetadata("Route 53", "route53", CommonEndpointPattern, 4580, AwsServiceEnum.Route53);
        public static readonly AwsServiceEndpointMetadata Cloudformation = new AwsServiceEndpointMetadata("CloudFormation", "cloudformation", CommonEndpointPattern, 4581, AwsServiceEnum.Cloudformation);
        public static readonly AwsServiceEndpointMetadata Cloudwatch = new AwsServiceEndpointMetadata("CloudWatch", "cloudwatch", CommonEndpointPattern, 4582, AwsServiceEnum.Cloudwatch);
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
        public static readonly AwsServiceEndpointMetadata Cloudsearch = new AwsServiceEndpointMetadata("CloudSearch", "cloudsearch", CommonEndpointPattern, 4595, AwsServiceEnum.Cloudsearch);
        public static readonly AwsServiceEndpointMetadata Swf = new AwsServiceEndpointMetadata("SWF", "swf", CommonEndpointPattern, 4596, AwsServiceEnum.Swf);

        public static readonly AwsServiceEndpointMetadata[] All =
        {
            ApiGateway,
            Kinesis,
            DynamoDb,
            DynamoDbStreams,
            ElasticSearch,
            S3,
            Firehose,
            Lambda,
            Sns,
            Sqs,
            Redshift,
            Es,
            Ses,
            Route53,
            Cloudformation,
            Cloudwatch,
            Ssm,
            SecretsManager,
            StepFunctions,
            Logs,
            Events,
            Elb,
            Iot,
            CognitoIdp,
            CognitoIdentity,
            Sts,
            Iam,
            Rds,
            Cloudsearch,
            Swf
        };

        public static AwsServiceEndpointMetadata ByName(string name)
        {
            if (name == null)
            {
                throw  new ArgumentNullException(nameof(name));
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

        public string ToString(string proto, string host) => proto == null || host == null
            ? throw new ArgumentNullException(proto == null ? nameof(proto) : nameof(host))
            : string.Format(EndPointPattern, proto, host, Port);

        public override string ToString() => $"{ServiceId} - {CliName} - {Port}";
    }

    public enum AwsServiceEnum
    {
        ApiGateway,
        Kinesis,
        DynamoDb,
        DynamoDbStreams,
        ElasticSearch,
        S3,
        Firehose,
        Lambda,
        Sns,
        Sqs,
        Redshift,
        Es,
        Ses,
        Route53,
        Cloudformation,
        Cloudwatch,
        Ssm,
        SecretsManager,
        StepFunctions,
        Logs,
        Events,
        Elb,
        Iot,
        CognitoIdp,
        CognitoIdentity,
        Sts,
        Iam,
        Rds,
        Cloudsearch,
        Swf
    }
}
