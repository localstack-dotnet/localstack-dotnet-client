![Nuget](https://img.shields.io/nuget/dt/LocalStack.Client) [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/) [![Testspace tests (compact)](https://img.shields.io/testspace/tests/localstack-dotnet/localstack-dotnet:localstack-dotnet-client/master?compact_message)](https://localstack-dotnet.testspace.com/spaces/155695/result_sets)

# LocalStack .Net Core and .Net Framework Client

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

This is an easy-to-use .NET client for [LocalStack](https://github.com/localstack/localstack).
The client library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net) which
automatically configures the target endpoints to use LocalStack for your local cloud
application development.

## Continuous integration

| Build server     | Platform  | Build status                                                                                                                                                                                                                                                                          |
|----------------- |---------- |-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Github Actions  | Ubuntu    | [![build-ubuntu](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml)  |
| Github Actions   | Windows    | [![build-windows](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml)  |
| Github Actions   | macO    | [![build-macos](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml) |

## Table of Contents

1. [Supported Platforms](#supported-platforms)
2. [Prerequisites](#prerequisites)
3. [Installation](#installation)
4. [Usage](#usage)
    - [LocalStack.Client.Extensions (Recommended)](#localstack-client-extensions)
        - [Installation](#extensions-installation)
        - [Usage](#extensions-usage)
        - [About AddAwsService](#extensions-usage-about-addawsservice)
    - [Standalone Initialization](#standalone-initialization)
    - [Microsoft.Extensions.DependencyInjection Initialization](#di)
5. [Developing](#developing)
    - [About Sandbox Applications](#about-sandboxes)
    - [Running Tests](#running-tests)
6. [Changelog](#changelog)
7. [License](#license)

## <a name="supported-platforms"></a> Supported Platforms

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET 4.6.1 and Above](https://dotnet.microsoft.com/download/dotnet-framework)

## <a name="prerequisites"></a> Prerequisites

To make use of this library, you need to have [LocalStack](https://github.com/localstack/localstack)
installed on your local machine. In particular, the `localstack` command needs to be available.

## <a name="installation"></a>  Installation

The easiest way to install *LocalStack .NET Client* is via `nuget`:

```
Install-Package LocalStack.Client
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client
```

| Package                      | Stable | Nightly |
|------------------------------|--------|---------|
| LocalStack.Client            | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/)       | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client)        |
| LocalStack.Client.Extensions | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.Extensions.svg)](https://www.nuget.org/packages/LocalStack.Client.Extensions/)       | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.Extensions.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client.Extensions)        |

## <a name="usage"></a> Usage

This library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net).
Therefore the usage of this library is same as using `AWS SDK for .NET`.

See [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html)

This library can be used with any DI library, [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) or it can be used as standalone.

### <a name="localstack-client-extensions"></a>  LocalStack.Client.Extensions (Recommended)

[LocalStack.Client.Extensions](https://www.nuget.org/packages/LocalStack.Client/) is extensions for the LocalStack.NET Client to integrate with .NET Core configuration and dependency injection frameworks. The extensions also provides wrapper around [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) to use both LocalStack and AWS side-by-side.

This approach is recommended since `AWSSDK.Extensions.NETCore.Setup` is very popular and also it is best practice for using [AWSSDK.NET](https://aws.amazon.com/sdk-for-net/) with .NET Core, .NET 5 or .NET 6

#### <a name="extensions-installation"></a>  Installation

The easiest way to install *LocalStack .NET Client Extensions* is via `nuget`:

```
Install-Package LocalStack.Client.Extensions
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client.Extensions  
```

#### <a name="extensions-usage"></a> Usage

The usage is very similar to `AWSSDK.Extensions.NETCore.Setup` with some differences.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    services.AddLocalStack(Configuration)
    services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
    services.AddAwsService<IAmazonS3>();
    services.AddAwsService<IAmazonDynamoDB>();
}
```

The most important difference is that `AddAwsService` extensions method is used instead of `AddAWSService` used in `AWSSDK.Extensions.NETCore.Setup`. The reason for this will be explained later in this section.

In addition, the `AddLocalStack` extension method is also used.

<e><b>(Alternatively, `AddAWSServiceLocalStack` method can be used to prevent mix-up with `AddAWSService`.)</b><e>

`AddLocalStack` extension method is responsible for both configurations and adding of `LocalStack.Client` dependencies to service collection.

You can configure `LocalStack.Client` by using entries in the `appsettings.json` files, as shown in the following example.

```json
"LocalStack": {
    "UseLocalStack": true,
    "Session": {
        "AwsAccessKeyId": "my-AwsAccessKeyId",
        "AwsAccessKey": "my-AwsAccessKey",
        "AwsSessionToken": "my-AwsSessionToken",
        "RegionName": "eu-central-1"
    },
    "Config": {
        "LocalStackHost": "localhost",
        "UseSsl": false,
        "UseLegacyPorts": false,
        "EdgePort": 4566
    }
}
```

All the entries above are has shown with default values (except `UseLocalStack`, it's `false` by default).
So the above entries do not need to be specified.

What is entered for the aws credential values ​​in the `Session` section does not matter for LocalStack. `RegionName` is important since LocalStack creates resources by spesified region.

`Config` section contains important entries for local development. Starting with LocalStack releases after `v0.11.5`, all services are now exposed via the edge service (port 4566) only! If you are using a version of LocalStack lower than v0.11.5, you should set `UseLegacyPorts` to `true`. Edge port can be set to any available port ([see LocalStack configuration section](https://github.com/localstack/localstack#configurations)). If you have made such a change in LocalStack's configuration, be sure to set the same port value to `EdgePort` in the `Config` section. For `LocalStackHost` and `UseSsl` entries, ​​corresponding to the [LocalStack configuration](https://github.com/localstack/localstack#configurations) should be used.

The following sample setting files can be used to use both `LocalStack.Client` and`AWSSDK.Extensions.NETCore.Setup` in different environments.

`appsettings.Development.json`

```json
"LocalStack": {
    "UseLocalStack": true,
    "Session": {
        ...
    },
    "Config": {
        ...
    }
}
```

`appsettings.Production.json`

```json
"LocalStack": {
    "UseLocalStack": false
},
"AWS": {
    "Profile": "<your aws profile>",
    "Region": "eu-central-1"
}
```

See project [LocalStack.Client.Sandbox.WithGenericHost](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes/LocalStack.Client.Sandbox.WithGenericHost) for a use case.

#### <a name="extensions-usage-about-addawsservice"></a> About AddAwsService

`AddAwsService` is equivalent of `AddAWSService` used in `AWSSDK.Extensions.NETCore.Setup`. It decides which factory to use when resolving any AWS Service. To decide this, it checks the `UseLocalStack` entry.
If the `UseLocalStack` entry is `true`, it uses the [Session](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/src/LocalStack.Client/Session.cs) class of `LocalStack.Client` to create AWS Service. If the `UseLocalStack` entry is `false`, it uses the [ClientFactory](https://github.com/aws/aws-sdk-net/blob/master/extensions/src/AWSSDK.Extensions.NETCore.Setup/ClientFactory.cs) class of `AWSSDK.Extensions.NETCore.Setup` which is also used by original `AddAWSService`.

It is named as `AddAwsService` to avoid name conflict with `AddAWSService`.

<e><b>(Alternatively, `AddAWSServiceLocalStack` method can be used to prevent mix-up with `AddAWSService`.)</b><e>

### <a name="standalone-initialization"></a> Standalone Initialization

If you do not want to use any DI library, you have to instantiate `SessionStandalone` as follows.

```csharp
/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
* ==== Custom Values ====
* var sessionOptions = new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-");
*/
var sessionOptions = new SessionOptions();

/*
* ==== Default Values ====
* LocalStackHost: localhost
* UseSsl: false
* UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
* EdgePort: 4566 (It doesn't matter if use legacy ports)
* ==== Custom Values ====
* var configOptions = new ConfigOptions("mylocalhost", false, false, 4566);
*/
var configOptions = new ConfigOptions();

ISession session = SessionStandalone.Init()
                                .WithSessionOptions(sessionOptions)
                                .WithConfigurationOptions(configOptions).Create();

var amazonS3Client = session.CreateClientByImplementation<AmazonS3Client>();
```

`CreateClientByInterface<TSerice>` method can also be used to create AWS service, as follows

```csharp
var amazonS3Client = session.CreateClientByInterface<IAmazonS3>();
```

### <a name="di"></a>  Microsoft.Extensions.DependencyInjection Initialization

First, you need to install `Microsoft.Extensions.DependencyInjection` nuget package as follows

```
dotnet add package Microsoft.Extensions.DependencyInjection
```

Register necessary dependencies to `ServiceCollection` as follows

```csharp
var collection = new ServiceCollection();

/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
* ==== Custom Values ====
* var sessionOptions = new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-");
*/
var sessionOptions = new SessionOptions();

/*
* ==== Default Values ====
* LocalStackHost: localhost
* UseSsl: false
* UseLegacyPorts: false (Set true if your LocalStack version is 0.11.4 or below)
* EdgePort: 4566 (It doesn't matter if use legacy ports)
* ==== Custom Values ====
* var configOptions = new ConfigOptions("mylocalhost", false, false, 4566);
*/
var configOptions = new ConfigOptions();

collection
    .AddScoped<ISessionOptions, SessionOptions>(provider => sessionOptions)
    .AddScoped<IConfigOptions, ConfigOptions>(provider => configOptions))
    .AddScoped<IConfig, Config>()
    .AddSingleton<ISessionReflection, SessionReflection>()
    .AddSingleton<ISession, Session>()
    .AddTransient<IAmazonS3>(provider =>
    {
        var session = provider.GetRequiredService<ISession>();

        return (IAmazonS3) session.CreateClientByInterface<IAmazonS3>();
    });

ServiceProvider serviceProvider = collection.BuildServiceProvider();

var amazonS3Client = serviceProvider.GetRequiredService<IAmazonS3>();
```

If you want to use it with `ConfigurationBuilder`, you can also choose a usage as below.

```csharp
var collection = new ServiceCollection();
var builder = new ConfigurationBuilder();

builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsettings.json", true);
builder.AddJsonFile("appsettings.Development.json", true);
builder.AddEnvironmentVariables();
builder.AddCommandLine(args);

IConfiguration configuration = builder.Build();

collection.Configure<LocalStackOptions>(options => configuration.GetSection("LocalStack").Bind(options, c => c.BindNonPublicProperties = true));
/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
    */
collection.Configure<SessionOptions>(options => configuration.GetSection("LocalStack")
                                                                .GetSection(nameof(LocalStackOptions.Session))
                                                                .Bind(options, c => c.BindNonPublicProperties = true));
/*
    * ==== Default Values ====
    * LocalStackHost: localhost
    * UseSsl: false
    * UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
    * EdgePort: 4566 (It doesn't matter if use legacy ports)
    */
collection.Configure<ConfigOptions>(options => configuration.GetSection("LocalStack")
                                                            .GetSection(nameof(LocalStackOptions.Config))
                                                            .Bind(options, c => c.BindNonPublicProperties = true));


collection.AddTransient<IConfig, Config>(provider =>
{
    ConfigOptions options = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;

    return new Config(options);
})
.AddSingleton<ISessionReflection, SessionReflection>()
.AddSingleton<ISession, Session>(provider =>
{
    SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
    var config = provider.GetRequiredService<IConfig>();
    var sessionReflection = provider.GetRequiredService<ISessionReflection>();

    return new Session(sessionOptions, config, sessionReflection);
})
.AddTransient<IAmazonS3>(provider =>
{
    var session = provider.GetRequiredService<ISession>();

    return (IAmazonS3) session.CreateClientByInterface<IAmazonS3>();
});

ServiceProvider serviceProvider = collection.BuildServiceProvider();

var amazonS3Client = serviceProvider.GetRequiredService<IAmazonS3>();
```

## <a name="developing"></a> Developing

We welcome feedback, bug reports, and pull requests!

Use commands below to get you started and test your code:

Windows

```
build.ps1
```

Linux

```
./build.sh
```

### <a name="about-sandboxes"></a> About Sandbox Applications

In addition to Unit Tests and Functional Test, LocalStack .Net Repository has various sandbox console applications for both testing and example purposes under [tests/sandboxes](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes)

Sandbox applications include various examples of initialization methods of `LocalStack.Client` (see [Usage](#usage) section) and common AWS applications. They provide a convenient and safe environment for those who want to make developments in the library.

To run sandbox applications with LocalStack container, console application called [LocalStack.Container](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes/LocalStack.Container) has been developed. It uses [Dotnet Testcontainer](https://github.com/HofmeisterAn/dotnet-testcontainers) to bootstrap LocalStack. Experiments can be made by running LocalStack.Container application first and then any sandbox application.

### <a name="running-tests"></a> Running Tests

Use commands below to run tests

Windows

```
build.ps1 --target=tests
```

Linux

```
./build.sh --target=tests
```

## <a name="changelog"></a> Changelog

### [v1.2.3](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.2.3)

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.25 have been added.
  - Config Service
- .NET 6.0 support added.
- AWSSDK.Core set to 3.7.3.15 as the minimum version.
- AWSSDK.Extensions.NETCore.Setup set to 3.7.1 as the minimum version.
- Tested against LocalStack v0.13.0 container.

### [v1.2.2](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.2.2)

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.22 have been added.
  - EFS, Backup, LakeFormation, WAF, WAF V2 and QLDB Session
- AWSSDK.Core set to 3.7.1 as the minimum version.
- Tested against LocalStack v0.12.16 container.

### [v1.2](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.2.0)

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.20 have been added.
  - IoTAnalytics, IoT Events, IoT Events Data, IoT Wireless, IoT Data Plane, IoT Jobs Data Plane, Support, Neptune, DocDB, ServiceDiscovery, ServerlessApplicationRepository, AppConfig, Cost Explorer, MediaConvert, Resource Groups Tagging API, Resource Groups
- AWSSDK.Core set to 3.7.0 as the minimum version.
- Obsolete methods removed.
- New alternate AddAWSServiceLocalStack method added to prevent mix up with AddAWSService (for LocalStack.Client.Extension v1.1.0).
- Tested against LocalStack v0.12.10 container.

### [v1.1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.1.0)

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.10 have been added.
  - Transfer, ACM, CodeCommit, Kinesis Analytics, Amplify, Application Auto Scaling, Kafka, Timestream Query, Timestream Write, Timestream Write, S3 Control, Elastic Load Balancing v2, Redshift Data
- .NET 5.0 support added.
- AWSSDK.Core set to 3.5.0 as the minimum version.
- Tested against LocalStack v0.12.07 container.

### [v1.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.0.0)

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v0.23 have been added.
  - ElastiCache, Kms, Emr, Ecs, Eks, XRay, ElasticBeanstalk, AppSync, CloudFront, Athena, Glue, Api Gateway V2, RdsData, SageMaker, SageMakerRuntime, Ecr, Qldb
- .netcore2.2 support removed since Microsoft depracated it. .netcore3.1 support added.
- AWSSDK.Core set to 3.3.106.5 as the minimum version.

### [v0.8.0.163](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v0.8.0.163)

- First release.

## <a name="license"></a> License

Licensed under MIT, see [LICENSE](LICENSE) for the full text.
