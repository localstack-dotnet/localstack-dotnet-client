using Amazon.S3;
using Amazon.SQS;
using LocalStack.Client.Contracts;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

if (System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
{
    Console.Error.WriteLine("Expected a Native AOT executable.");
    return 1;
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

// The consumer contract, unchanged from earlier versions.
var services = new ServiceCollection();
services.AddLocalStack(configuration);
services.AddAwsService<IAmazonS3>();
using var provider = services.BuildServiceProvider();
var s3 = provider.GetRequiredService<IAmazonS3>();

// A Type-based API: served by the source generator that arrives through the package.
var session = provider.GetRequiredService<ISession>();
using var sqs = (IAmazonSQS)session.CreateClientByInterface(typeof(IAmazonSQS));
string serviceId = provider.GetRequiredService<ISessionReflection>().ExtractServiceMetadata(typeof(AmazonSQSClient)).ServiceId;

if (!((AmazonS3Config)s3.Config).ForcePathStyle || serviceId != "SQS" || sqs.Config.ProxyPort != 4566)
{
    Console.Error.WriteLine($"Unexpected configuration: pathStyle={((AmazonS3Config)s3.Config).ForcePathStyle} serviceId={serviceId} sqsPort={sqs.Config.ProxyPort}");
    return 1;
}

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOCALSTACK_NATIVE_AOT_HOST")))
{
    await s3.ListBucketsAsync();
    await sqs.ListQueuesAsync(new Amazon.SQS.Model.ListQueuesRequest());
}

Console.WriteLine("Package consumer passed.");
return 0;
