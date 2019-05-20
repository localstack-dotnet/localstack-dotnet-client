# LocalStack .Net Core and .Net Framework Client

This is an easy-to-use .NET client for [LocalStack](https://github.com/localstack/localstack).
The client library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net) which
automatically configures the target endpoints to use LocalStack for your local cloud
application development.

## Supported Platforms

* .NET 4.6.1 (Desktop / Server)
* [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

## Prerequisites

To make use of this library, you need to have [LocalStack](https://github.com/localstack/localstack)
installed on your local machine. In particular, the `localstack` command needs to be available.

## Continuous integration

| Build server    	| Platform 	| Build status                                                                                                                                                                                                                                                                         	|
|-----------------	|----------	|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| Azure Pipelines 	| Ubuntu   	| [![Build Status](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_apis/build/status/localstack-dotnet.localstack-dotnet-client?branchName=master) ](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_build/latest?definitionId=8&branchName=master ) 	|



## Installation

The easiest way to install *LocalStack .NET Client* is via `nuget`:

```
Install-Package LocalStack.Client
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client
```

## Usage

This library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net). 
Therefore the usage of this library is same as using `AWS SDK for .NET`.

See [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html)

This library can be used with any DI library, or it can be used as standalone.

### Standalone Initialization

If you do not want to use any DI library, you have to instantiate `SessionStandalone` as follows.

```csharp
var awsAccessKeyId = "Key Id";
var awsAccessKey = "Secret Key";
var awsSessionToken = "Token";
var regionName = "us-west-1";
var localStackHost = "localhost";

ISession session = SessionStandalone
    .Init()
    .WithSessionOptions(awsAccessKeyId, awsAccessKey, awsSessionToken, regionName)
    .WithConfig(localStackHost)
    .Create();
```

All parameters are optional and in case of passed `null`, default values will be setted. Since its workin on local machine, the real aws credantials are not needed for `awsAccessKeyId`, `awsAccessKey`, `awsSessionToken` parameters.

### Microsoft.Extensions.DependencyInjection Initialization

First, you need to install `Microsoft.Extensions.DependencyInjection` nuget package as follows

```
dotnet add package Microsoft.Extensions.DependencyInjection
```

Register necessary dependencies to `ServiceCollection` as follows

```csharp
var collection = new ServiceCollection();

var awsAccessKeyId = "Key Id";
var awsAccessKey = "Secret Key";
var awsSessionToken = "Token";
var regionName = "us-west-1";
var localStackHost = "localhost";

collection
    .AddScoped<ISessionOptions, SessionOptions>(provider =>
        new SessionOptions(awsAccessKeyId, awsAccessKey, awsSessionToken, regionName))
    .AddScoped<IConfig, Config>(provider => new Config(localStackHost))
    .AddScoped<ISessionReflection, SessionReflection>()
    .AddScoped<ISession, Session>();

ServiceProvider serviceProvider = collection.BuildServiceProvider();

var session = serviceProvider.GetRequiredService<ISession>();
```

All parameters are optional and in case of passed `null`, default values will be setted. Since its workin on local machine, the real aws credantials are not needed for `awsAccessKeyId`, `awsAccessKey`, `awsSessionToken` parameters.

### Create and use AWS SDK Clients

For all local services supported by [LocalStack](https://github.com/localstack/localstack#overview), the corresponding [AWSSDK packages](https://www.nuget.org/profiles/awsdotnet) can be use.

The following example shows how to use [AWSSDK.S3](https://www.nuget.org/packages/AWSSDK.S3/) with `LocalStack.NET Client`

```csharp
var amazonS3Client = session.CreateClient<AmazonS3Client>();

ListBucketsResponse listBucketsResponse = await amazonS3Client.ListBucketsAsync();

const string bucketName = "test-bucket-3";

if (!await AmazonS3Util.DoesS3BucketExistAsync(amazonS3Client, bucketName))
{
    PutBucketResponse putBucketResponse = await amazonS3Client.PutBucketAsync(bucketName);
}

var fileTransferUtility = new TransferUtility(amazonS3Client);

await fileTransferUtility.UploadAsync("SampleData.txt", bucketName, "SampleData.txt");
GetObjectResponse getObjectResponse = await amazonS3Client.GetObjectAsync(bucketName, "SampleData.txt");
```

See [sandbox projects](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes) for more examples.

## Developing

We welcome feedback, bug reports, and pull requests!

Use these commands to get you started and test your code:

Windows
```
build.ps1
```

Linux
```
./build.sh
```

<!-- ## Changelog

* v0.8: Add more service endpoint mappings that will be implemented in the near future -->

## License
Licensed under MIT, see [LICENSE](LICENSE) for the full text.
