# LocalStack .NET Client ![Nuget](https://img.shields.io/nuget/dt/LocalStack.Client) [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/) [![Space Metric](https://localstack-dotnet.testspace.com/spaces/232580/badge?token=bc6aa170f4388c662b791244948f6d2b14f16983)](https://localstack-dotnet.testspace.com/spaces/232580?utm_campaign=metric&utm_medium=referral&utm_source=badge "Test Cases")

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

Localstack.NET is an easy-to-use .NET client for [LocalStack](https://github.com/localstack/localstack), a fully functional local AWS cloud stack. The client library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net) which automatically configures the target endpoints to use LocalStack for your local cloud application development.

| Package                      | Stable                                                                                                                                    | Nightly                                                                                                                                                                                                          |
| ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LocalStack.Client            | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/)                       | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client)                       |
| LocalStack.Client.Extensions | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.Extensions.svg)](https://www.nuget.org/packages/LocalStack.Client.Extensions/) | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.Extensions.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client.Extensions) |

## Continuous Integration

| Build server   | Platform | Build status                                                                                                                                                                                                                       |
| -------------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Github Actions | Ubuntu   | [![build-ubuntu](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml)    |
| Github Actions | Windows  | [![build-windows](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml) |
| Github Actions | macOS    | [![build-macos](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml)       |

## Table of Contents

1. [Supported Platforms](#supported-platforms)
2. [Why LocalStack.NET Client?](#why-localstacknet-client)
3. [Prerequisites](#prerequisites)
4. [Getting Started](#getting-started)
   - [Setup](#setup)
   - [Configuration](#configuration)
5. [Known Issues](#known-issues)
6. [Developing](#developing)
   - [Building the Project](#building-the-project)
   - [Sandbox Applications](#sandbox-applications)
   - [Running Tests](#running-tests)
7. [Changelog](#changelog)
8. [License](#license)

## Supported Platforms

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET 4.6.2 and Above](https://dotnet.microsoft.com/download/dotnet-framework)

## Why LocalStack.NET Client?

- **Consistent Client Configuration:** LocalStack.NET eliminates the need for manual endpoint configuration, providing a standardized and familiar approach to initializing clients.

- **Adaptable Environment Transition:** Easily switch between LocalStack and actual AWS services with minimal configuration changes.

- **Versatile .NET Compatibility:** Supports a broad spectrum of .NET versions, from .NET 8.0 and .NET Standard 2.0 to .NET Framework 4.6.2 and above.

- **Reduced Learning Curve:** Offers a familiar interface tailored for LocalStack, ideal for developers acquainted with the AWS SDK for .NET.

- **Enhanced Development Speed:**  Reduces boilerplate and manual configurations, speeding up the development process.

## Prerequisites

To utilize this library, you need to have LocalStack running. While LocalStack can be installed directly on your machine and accessed via the localstack cli, the recommended approach is to run LocalStack using [Docker](https://docs.docker.com/get-docker/) or [docker-compose](https://docs.docker.com/compose/install/).

For detailed installation and setup instructions, please refer to the [official LocalStack installation guide](https://docs.localstack.cloud/getting-started/installation/).

## Getting Started

LocalStack.NET is installed from NuGet. To work with LocalStack in your .NET applications, you'll need the main library and its extensions. Here's how you can install them:

```bash
dotnet add package LocalStack.Client
dotnet add package LocalStack.Client.Extensions
```

Refer to [documentation](https://github.com/localstack-dotnet/localstack-dotnet-client/wiki/Getting-Started#installation) for more information on how to install LocalStack.NET.

`LocalStack.NET` is a library that provides a wrapper around the [aws-sdk-net](https://github.com/aws/aws-sdk-net). This means you can use it in a similar way to the `AWS SDK for .NET` and to [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) with a few differences. For more on how to use the AWS SDK for .NET, see [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html).

### Setup

Here's a basic example of how to setup `LocalStack.NET`:

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

The `AddLocalStack` method integrates LocalStack.NET into your application, and the `AddAwsService` method allows you to specify which AWS services you want to use with LocalStack.

<e><b>(Alternatively, `AddAWSServiceLocalStack` method can be used to prevent mix-up with `AddAWSService`.)</b></e>

`AddLocalStack` extension method is responsible for both configurations and adding of `LocalStack.Client` dependencies to service collection.

### Configuration

To configure LocalStack.NET, you can use entries in the appsettings.json files. Here's a basic example for different environments:

`appsettings.Development.json`

```json
"LocalStack": {
    "UseLocalStack": true,
    "Session": {
        "RegionName": "eu-central-1"
    },
    "Config": {
        "LocalStackHost": "localhost.localstack.cloud", // or "localhost",
        "EdgePort": 4566
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

The `RegionName` is important as LocalStack creates resources based on the specified region. For more advanced configurations and understanding how LocalStack.NET operates with LocalStack, refer to [documentation](https://github.com/localstack-dotnet/localstack-dotnet-client/wiki/Setup#configuration).

## Known Issues

- **SNS with LocalStack v3.7.2 and v3.8.0:** During development on the new version, it was discovered that SNS functional tests are not working in LocalStack versions v3.7.2 and v3.8.0. This issue was reported in LocalStack [issue #11652](https://github.com/localstack/localstack/issues/11652). The LocalStack team identified a bug related to handling SNS URIs and resolved it in [PR #11653](https://github.com/localstack/localstack/pull/11653). The fix will be included in an upcoming release of LocalStack. In the meantime, if you're using SNS, it is recommended to stick to version v3.7.1 of LocalStack until the fix is available.

- **LocalStack Versions v2.0.1 - v2.2:** In versions v2.0.1 through v2.2 of LocalStack, the URL routing logic was changed, causing issues with SQS and S3 operations. Two issues were opened in LocalStack regarding this: [issue #8928](https://github.com/localstack/localstack/issues/8928) and [issue #8924](https://github.com/localstack/localstack/issues/8924). LocalStack addressed this problem with [PR #8962](https://github.com/localstack/localstack/pull/8962). Therefore, when using LocalStack.NET, either use version v2.0 of LocalStack (there are no issues with the v1 series as well) or the upcoming v2.3 version, or use the latest v3 series container from Docker Hub.

- **AWS_SERVICE_URL Environment Variable:** Unexpected behaviors might occur in LocalStack.NET when the `AWS_SERVICE_URL` environment variable is set. This environment variable is typically set by LocalStack in the container when using AWS Lambda, and AWS also uses this environment variable in the live environment. Soon, just like in LocalStack's official Python library, this environment variable will be prioritized by LocalStack.NET when configuring the LocalStack host, and there will be a general simplification in the configuration. You can follow this in the issues [issue #27](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/27) and [issue #32](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/32). You set the `AWS_SERVICE_URL` to empty string until this issue is resolved.

```csharp
Environment.SetEnvironmentVariable("AWS_SERVICE_URL", string.Empty);
```

- **IAmazonLambda Operations:** There's a general issue with `IAmazonLambda` operations. This matter is currently under investigation.

- **AWSSDK.SQS Compatibility:** Starting from version `3.7.300.*` of `AWSSDK.SQS`, there are compatibility issues with LocalStack v1 and v2 series versions. The [v3](https://hub.docker.com/r/localstack/localstack/tags?page=&page_size=&ordering=&name=3.4) series of LocalStack does not have these issues. Therefore, it is recommended to either update your LocalStack container to the v3 series or downgrade your `AWSSDK.SQS` to version `3.7.200.*` if you are using LocalStack v1 or v2 series containers. It is important to note that this is not a problem related to LocalStack.NET, but rather an issue with the LocalStack container and the AWS SDK for .NET.

## Developing

We appreciate contributions in the form of feedback, bug reports, and pull requests.

### Building the Project

To build the project, use the following commands based on your operating system:

Windows

```
build.ps1
```

Linux

```
./build.sh
```

### Sandbox Applications

The LocalStack .NET repository includes several sandbox console applications located in [tests/sandboxes](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes). These applications serve both as testing tools and as examples. Refer to [the documentation](https://github.com/localstack-dotnet/localstack-dotnet-client/wiki/Developing#sandbox-applications) for more information

### Running Tests

To execute the tests, use the commands below:

Windows

```
build.ps1 --target=tests
```

Linux

```
./build.sh --target=tests
```

## Changelog

Please refer to [`CHANGELOG.md`](CHANGELOG.md) to see the complete list of changes for each release.

## License

Licensed under MIT, see [LICENSE](LICENSE) for the full text.
