![Nuget](https://img.shields.io/nuget/dt/LocalStack.Client) [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/) [![Space Metric](https://localstack-dotnet.testspace.com/spaces/232580/badge?token=bc6aa170f4388c662b791244948f6d2b14f16983)](https://localstack-dotnet.testspace.com/spaces/232580?utm_campaign=metric&utm_medium=referral&utm_source=badge "Test Cases")

# LocalStack .Net Core and .Net Framework Client

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

This is an easy-to-use .NET client for [LocalStack](https://github.com/localstack/localstack).
The client library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net) which
automatically configures the target endpoints to use LocalStack for your local cloud
application development.

## Continuous integration

| Build server   | Platform | Build status                                                                                                                                                                                                                       |
| -------------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Github Actions | Ubuntu   | [![build-ubuntu](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-ubuntu.yml)    |
| Github Actions | Windows  | [![build-windows](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-windows.yml) |
| Github Actions | macOS    | [![build-macos](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/build-macos.yml)       |

## Table of Contents

1. [LocalStack .Net Core and .Net Framework Client](#localstack-net-core-and-net-framework-client)
2. [Continuous Integration](#continuous-integration)
3. [Supported Platforms](#supported-platforms)
4. [Why LocalStack.NET Client?](#why-localstacknet-client)
5. [Prerequisites](#prerequisites)
6. [Installation](#installation)
   - [Recommended: LocalStack.Client.Extensions](#recommended-localstackclientextensions)
   - [Base Library: LocalStack.Client](#base-library-localstackclient)
7. [Packages Overview](#packages-overview)
8. [Usage](#usage)
   - [Configuration](#configuration)
   - [Integrating with Dependency Injection](#integrating-with-dependency-injection)
9. [Developing](#developing)
   - [Building the Project](#building-the-project)
   - [Sandbox Applications](#sandbox-applications)
   - [Running Tests](#running-tests)
10. [Changelog](#changelog)
11. [License](#license)

## Supported Platforms

- [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET 4.6.1 and Above](https://dotnet.microsoft.com/download/dotnet-framework)

## Why LocalStack.NET Client?

- **Consistent Client Configuration:** LocalStack.NET eliminates the need for manual endpoint configuration, providing a standardized and familiar approach to initialize clients.

- **Adaptable Environment Transition:** LocalStack.NET makes it easy to switch between LocalStack and actual AWS services with minimal configuration changes.

- **Versatile .NET Compatibility:** LocalStack.NET supports a broad spectrum of .NET versions, from .NET 7.0 and .NET Standard 2.0, to .NET Framework 4.6.1 and above.

- **Reduced Learning Curve:** LocalStack.NET offers a familiar interface tailored for LocalStack, making it easier for developers already acquainted with the AWS SDK for .NET.

- **Enhanced Development Speed:** LocalStack.NET reduces boilerplate and manual configurations, speeding up the development process.

## Prerequisites

To utilize this library, you need to have LocalStack running. While LocalStack can be installed directly on your machine and accessed via the `localstack` cli, the recommended approach is to run LocalStack using Docker or `docker-compose`.

For detailed installation and setup instructions, please refer to the [official LocalStack installation guide](https://docs.localstack.cloud/getting-started/installation/).

## Installation

### Recommended: LocalStack.Client.Extensions

[`LocalStack.Client.Extensions`](https://www.nuget.org/packages/LocalStack.Client.Extensions) is the recommended package for most modern .NET environments. It integrates with .NET [configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration) and [dependency injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) frameworks and provides a wrapper around [`AWSSDK.Extensions.NETCore.Setup`](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html). This allows you to use both LocalStack and AWS side-by-side seamlessly.

This approach is especially recommended for projects using .NET Core, .NET 6, or .NET 7 etc., given the popularity and best practices associated with `AWSSDK.Extensions.NETCore.Setup`.

To install, use `nuget`:

```
Install-Package LocalStack.Client.Extensions
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client.Extensions
```

**Note**: Installing `LocalStack.Client.Extensions` will also install the base `LocalStack.Client` library.

### Base Library: LocalStack.Client

For specific scenarios, such as using the legacy .NET Framework, or employing a different DI framework like Autofac, or using the library standalone without DI, you might opt for the base [`LocalStack.Client`](https://www.nuget.org/packages/LocalStack.Client) library.

To install, use `nuget`:

```
Install-Package LocalStack.Client
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client
```

## Packages Overview

| Package                      | Stable                                                                                                                                    | Nightly                                                                                                                                                                                                          |
| ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LocalStack.Client            | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/)                       | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client)                       |
| LocalStack.Client.Extensions | [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.Extensions.svg)](https://www.nuget.org/packages/LocalStack.Client.Extensions/) | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.Extensions.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client.Extensions) |

## Usage

`LocalStack.NET` is a library that provides a wrapper around the [aws-sdk-net](https://github.com/aws/aws-sdk-net). This means you can use it in a similar way to the `AWS SDK for .NET` and to [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) with a few differences. For more on how to use the AWS SDK for .NET, see [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html).

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

The `RegionName` is important as LocalStack creates resources based on the specified region. For more advanced configurations and understanding how LocalStack.NET operates with LocalStack, refer to the upcoming detailed documentation.

### Integrating with Dependency Injection

Here's a basic example of how to integrate `LocalStack.NET` with the .NET dependency injection:

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

<e><b>(Alternatively, `AddAWSServiceLocalStack` method can be used to prevent mix-up with `AddAWSService`.)</b><e>

`AddLocalStack` extension method is responsible for both configurations and adding of `LocalStack.Client` dependencies to service collection.

For services where the `RegionEndpoint` is not applicable, such as AWS MediaStore or IoT, you can use the `useServiceUrl` parameter:

```csharp
services.AddAwsService<IAmazonMediaStoreData>(useServiceUrl: true);
services.AddAwsService<IAmazonIoTJobsDataPlane>(useServiceUrl: true);

```

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

The LocalStack .NET repository includes several sandbox console applications located in [tests/sandboxes](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes)
. These applications serve both as testing tools and as examples.

These sandbox applications showcase various initialization methods for `LocalStack.Client` and `LocalStack.Client.Extensions` (refer to the [Usage](#usage)) and demonstrate common AWS applications. If you're looking to contribute or experiment with the library, these sandbox applications provide a safe environment to do so.

To interact with a LocalStack container, use the [LocalStack.Container](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes/LocalStack.Container) console application. This application leverages [testcontainers-dotnet](https://github.com/testcontainers/testcontainers-dotnets) to initialize LocalStack. Start the LocalStack.Container application first, then run any of the sandbox applications to experiment.

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
