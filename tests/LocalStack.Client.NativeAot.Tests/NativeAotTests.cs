using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using LocalStack.Client.Contracts;
using LocalStack.Client.Enums;
using LocalStack.Client.Extensions;
using LocalStack.Client.Options;
using LocalStack.Client.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LocalStack.Client.NativeAot.Tests;

public sealed class NativeAotTests
{
    // The Type-based overloads are part of the public contract under test.
    private static readonly Type S3Interface = typeof(IAmazonS3);
    private static readonly Type S3Implementation = typeof(AmazonS3Client);
    private static readonly Type SqsInterface = typeof(IAmazonSQS);
    private static readonly Type SqsImplementation = typeof(AmazonSQSClient);

    [Fact]
    public void AddAwsService_Should_Configure_S3_For_LocalStack_With_Existing_Consumer_Calls()
    {
        using ConfigurationRoot configuration = LoadConfiguration();
        var services = new ServiceCollection();
        services.AddLocalStack(configuration);
        services.AddAwsService<IAmazonS3>();
        using ServiceProvider provider = services.BuildServiceProvider();
        var s3 = provider.GetRequiredService<IAmazonS3>();

        var client = Assert.IsType<AmazonS3Client>(s3);
        AssertClient(client, provider.GetRequiredService<IConfig>(), AwsService.S3, useServiceUrl: false);
        Assert.Equal("eu-central-1", s3.Config.RegionEndpoint.SystemName);
    }

    [Theory]
    [InlineData("add", false)]
    [InlineData("add", true)]
    [InlineData("add-options", false)]
    [InlineData("add-options", true)]
    [InlineData("alias", false)]
    [InlineData("alias", true)]
    [InlineData("alias-options", false)]
    [InlineData("alias-options", true)]
    [InlineData("try-add", false)]
    [InlineData("try-add", true)]
    [InlineData("try-add-options", false)]
    [InlineData("try-add-options", true)]
    [InlineData("try-alias", false)]
    [InlineData("try-alias", true)]
    [InlineData("try-alias-options", false)]
    [InlineData("try-alias-options", true)]
    public static void Registration_Overloads_Should_Configure_S3_For_LocalStack(string overload, bool useServiceUrl)
    {
        using ConfigurationRoot configuration = LoadConfiguration();
        var services = new ServiceCollection();
        services.AddLocalStack(configuration);
        var options = new AWSOptions { Region = RegionEndpoint.USWest2, Credentials = new AnonymousAWSCredentials() };
        const ServiceLifetime lifetime = ServiceLifetime.Transient;

        switch (overload)
        {
            case "add":
                services.AddAwsService<IAmazonS3>(lifetime, useServiceUrl);
                break;
            case "add-options":
                services.AddAwsService<IAmazonS3>(options, lifetime, useServiceUrl);
                break;
            case "alias":
                services.AddAWSServiceLocalStack<IAmazonS3>(lifetime, useServiceUrl);
                break;
            case "alias-options":
                services.AddAWSServiceLocalStack<IAmazonS3>(options, lifetime, useServiceUrl);
                break;
            case "try-add":
                services.TryAddAwsService<IAmazonS3>(lifetime, useServiceUrl);
                break;
            case "try-add-options":
                services.TryAddAwsService<IAmazonS3>(options, lifetime, useServiceUrl);
                break;
            case "try-alias":
                services.TryAddAWSServiceLocalStack<IAmazonS3>(lifetime, useServiceUrl);
                break;
            case "try-alias-options":
                services.TryAddAWSServiceLocalStack<IAmazonS3>(options, lifetime, useServiceUrl);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(overload));
        }

        using ServiceProvider provider = services.BuildServiceProvider();
        var client = Assert.IsType<AmazonS3Client>(provider.GetRequiredService<IAmazonS3>());
        AssertClient(client, provider.GetRequiredService<IConfig>(), AwsService.S3, useServiceUrl);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public static void Session_Should_Configure_S3_Clients_On_Every_Creation_Path(bool useServiceUrl, bool useSsl, bool standalone)
    {
        var configOptions = new ConfigOptions(localStackHost: "native-aot-host", useSsl: useSsl, edgePort: 4566);
        var config = new Config(configOptions);
        ISession session = CreateSession(configOptions, standalone);

        using AmazonServiceClient genericInterface = session.CreateClientByInterface<IAmazonS3>(useServiceUrl);
        using AmazonServiceClient typeInterface = session.CreateClientByInterface(S3Interface, useServiceUrl);
        using AmazonS3Client genericImplementation = session.CreateClientByImplementation<AmazonS3Client>(useServiceUrl);
        using AmazonServiceClient typeImplementation = session.CreateClientByImplementation(S3Implementation, useServiceUrl);
        Assert.IsType<AmazonS3Client>(genericInterface);
        Assert.IsType<AmazonS3Client>(typeInterface);
        Assert.IsType<AmazonS3Client>(genericImplementation);
        Assert.IsType<AmazonS3Client>(typeImplementation);
        AssertClient(genericInterface, config, AwsService.S3, useServiceUrl);
        AssertClient(typeInterface, config, AwsService.S3, useServiceUrl);
        AssertClient(genericImplementation, config, AwsService.S3, useServiceUrl);
        AssertClient(typeImplementation, config, AwsService.S3, useServiceUrl);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public static void Session_Should_Configure_Sqs_Clients_On_Every_Creation_Path(bool useServiceUrl, bool useSsl, bool standalone)
    {
        var configOptions = new ConfigOptions(localStackHost: "native-aot-host", useSsl: useSsl, edgePort: 4566);
        var config = new Config(configOptions);
        ISession session = CreateSession(configOptions, standalone);

        using AmazonServiceClient genericInterface = session.CreateClientByInterface<IAmazonSQS>(useServiceUrl);
        using AmazonServiceClient typeInterface = session.CreateClientByInterface(SqsInterface, useServiceUrl);
        using AmazonSQSClient genericImplementation = session.CreateClientByImplementation<AmazonSQSClient>(useServiceUrl);
        using AmazonServiceClient typeImplementation = session.CreateClientByImplementation(SqsImplementation, useServiceUrl);
        Assert.IsType<AmazonSQSClient>(genericInterface);
        Assert.IsType<AmazonSQSClient>(typeInterface);
        Assert.IsType<AmazonSQSClient>(genericImplementation);
        Assert.IsType<AmazonSQSClient>(typeImplementation);
        AssertClient(genericInterface, config, AwsService.Sqs, useServiceUrl);
        AssertClient(typeInterface, config, AwsService.Sqs, useServiceUrl);
        AssertClient(genericImplementation, config, AwsService.Sqs, useServiceUrl);
        AssertClient(typeImplementation, config, AwsService.Sqs, useServiceUrl);
    }

    [Fact]
    public async Task S3_Should_Create_And_List_A_Bucket_Against_LocalStack()
    {
        using ConfigurationRoot configuration = LoadLiveConfiguration();
        var services = new ServiceCollection();
        services.AddLocalStack(configuration);
        services.AddAwsService<IAmazonS3>();
        using ServiceProvider provider = services.BuildServiceProvider();
        var s3 = provider.GetRequiredService<IAmazonS3>();
        string bucket = $"native-aot-{Guid.NewGuid():N}";

        await s3.PutBucketAsync(new PutBucketRequest { BucketName = bucket }, TestContext.Current.CancellationToken);
        ListBucketsResponse response = await s3.ListBucketsAsync(new ListBucketsRequest(), TestContext.Current.CancellationToken);
        Assert.Contains(response.Buckets, item => string.Equals(bucket, item.BucketName, StringComparison.Ordinal));
    }

    [Fact]
    public async Task Sqs_Should_Create_And_List_A_Queue_Against_LocalStack()
    {
        using ConfigurationRoot configuration = LoadLiveConfiguration();
        var services = new ServiceCollection();
        services.AddLocalStack(configuration);
        services.AddAwsService<IAmazonSQS>();
        using ServiceProvider provider = services.BuildServiceProvider();
        var sqs = provider.GetRequiredService<IAmazonSQS>();
        string queue = $"native-aot-{Guid.NewGuid():N}";

        CreateQueueResponse created = await sqs.CreateQueueAsync(new CreateQueueRequest { QueueName = queue }, TestContext.Current.CancellationToken);
        ListQueuesResponse response = await sqs.ListQueuesAsync(new ListQueuesRequest { QueueNamePrefix = queue }, TestContext.Current.CancellationToken);
        Assert.EndsWith($"/{queue}", created.QueueUrl, StringComparison.Ordinal);
        Assert.Contains(response.QueueUrls, url => string.Equals(created.QueueUrl, url, StringComparison.Ordinal));
    }

    private static ConfigurationRoot LoadConfiguration() => (ConfigurationRoot)new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

    private static ConfigurationRoot LoadLiveConfiguration()
    {
        string? host = Environment.GetEnvironmentVariable("LOCALSTACK_NATIVE_AOT_HOST");
        if (string.IsNullOrEmpty(host))
        {
            Assert.Skip("LOCALSTACK_NATIVE_AOT_HOST is not set");
        }

        ConfigurationRoot configuration = LoadConfiguration();
        configuration["LocalStack:Config:LocalStackHost"] = host;
        return configuration;
    }

    private static ISession CreateSession(ConfigOptions configOptions, bool standalone)
    {
        var sessionOptions = new SessionOptions("native-aot-id", "native-aot-secret", "native-aot-token", "eu-central-1");
        return standalone
            ? SessionStandalone.Init().WithSessionOptions(sessionOptions).WithConfigurationOptions(configOptions).Create()
            : new Session(sessionOptions, new Config(configOptions), new SessionReflection());
    }

    private static void AssertClient(AmazonServiceClient client, IConfig config, AwsService service, bool useServiceUrl)
    {
        var endpoint = config.GetAwsServiceEndpoint(service);
        Assert.NotNull(endpoint);
        if (useServiceUrl)
        {
            Assert.Equal(endpoint.ServiceUrl.AbsoluteUri, client.Config.ServiceURL);
            Assert.Null(client.Config.RegionEndpoint);
        }
        else
        {
            Assert.Null(client.Config.ServiceURL);
            Assert.Equal("eu-central-1", client.Config.RegionEndpoint.SystemName);
        }

        Assert.Equal(!config.GetConfigOptions().UseSsl, client.Config.UseHttp);
        Assert.Equal(endpoint.Host, client.Config.ProxyHost);
        Assert.Equal(endpoint.Port, client.Config.ProxyPort);
        if (service == AwsService.S3)
        {
            Assert.True(Assert.IsType<AmazonS3Config>(client.Config).ForcePathStyle);
        }
    }
}
