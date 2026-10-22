# LocalStack .NET Client

[![Nuget](https://img.shields.io/nuget/dt/LocalStack.Client)](https://www.nuget.org/packages/LocalStack.Client/) [![NuGet v2.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2F%3Fpackage%3Dlocalstack.client%26source%3Dnuget%26track%3D2%26includeprerelease%3Dtrue%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client/) [![NuGet v1.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2F%3Fpackage%3Dlocalstack.client%26source%3Dnuget%26track%3D1%26includeprerelease%3Dtrue%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client/) [![CI/CD Pipeline](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml) [![Security](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql) [![Linux Tests](https://img.shields.io/endpoint?url=https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/badge/tests/linux?label=Tests)](https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/redirect/test-results/linux)

> **🚀 Quick Start**: `dotnet add package LocalStack.Client --version 2.0.0` (AWS SDK v4) | [Package installation](#-package-installation) | [Migration context](https://github.com/orgs/localstack-dotnet/discussions/45)

---

## 🎉 What's New: AWS SDK v4 Support Available

**v2.0.0** introduced AWS SDK for .NET v4 compatibility. Service reachability depends on this library's catalog mapping and the capabilities of the LocalStack release and edition you use.

- 🧪 Unit tests, AWS client-construction tests, and container-backed functional tests cover different parts of the library. See the [test and CI boundaries](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/engineering/quality-gates.md) rather than relying on a fixed pass count.
- ⚠️ **Migration changes** include .NET Framework 4.6.2 → 4.7.2 and AWS SDK v3 → v4; see the [changelog](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/CHANGELOG.md) before upgrading.

**Version Strategy**:

- v2.x (AWS SDK v4) active development on [master branch](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master)
- v1.x (AWS SDK v3) remains available on the [sdkv3-lts branch](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/sdkv3-lts), but support ended in July 2026. Use v2.x for maintained releases; see the [security policy](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/.github/SECURITY.md).

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

LocalStack.NET is a .NET client library for [LocalStack](https://github.com/localstack/localstack), a local AWS cloud emulator. It configures AWS SDK clients for LocalStack when LocalStack mode is enabled; the same application registration can instead create ordinary AWS clients.

## 🚀 Platform Compatibility & Quality Status

### Supported Platforms

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) | [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) | [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET Framework 4.7.2 and Above](https://dotnet.microsoft.com/download/dotnet-framework)

## ⚡ Native AOT & Trimming Status

- **Unreleased:** Native AOT support preserves existing C# calls such as `AddLocalStack(configuration)`, `AddAwsService<IAmazonS3>()`, and Core `Session` calls. Source generation flows automatically through either package; no new registration or factory API is required. It roots existing reflection operations rather than removing reflection.
- Local-package consumers have been validated with Native AOT and its mandatory trimming on .NET 10 for `win-x64` and `linux-x64`, including S3 `ListBucketsAsync` and JIT parity for endpoint, region, credentials, and configured values. This does not establish support for every service, operation, or native target, or for separately trimmed JIT deployments. Do not suppress trim/AOT warnings.
- This implementation is **not part of the published Core 2.0.0 / Extensions 2.0.1 releases**. See the [platform boundary](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/architecture/platform-and-packaging.md#native-aot-and-trimming) for scope and the [Native AOT decision](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/adr/0004-native-aot-preserves-existing-consumer-code.md) for the unchanged-consumer contract.

### Build & Test Matrix

| Category | Platform/Type | Status | Description |
|----------|---------------|--------|-------------|
| **🔧 Build** | Cross-Platform | [![CI/CD Pipeline](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml) | Matrix testing: Windows, Linux, macOS |
| **🔒 Security** | Static Analysis | [![Security](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql) | CodeQL analysis & dependency review |
| **🧪 Tests** | Linux | [![Linux Tests](https://img.shields.io/endpoint?url=https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/badge/tests/linux?label=Tests)](https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/redirect/test-results/linux) | All framework targets |
| **🧪 Tests** | Windows | [![Windows Tests](https://img.shields.io/endpoint?url=https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/badge/tests/windows?label=Tests)](https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/redirect/test-results/windows) | All framework targets |
| **🧪 Tests** | macOS | [![macOS Tests](https://img.shields.io/endpoint?url=https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/badge/tests/macos?label=Tests)](https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/redirect/test-results/macos) | All framework targets |

## Package Status

| Package | NuGet.org | GitHub Packages (development builds) |
|---------|-----------|---------------------------|
| **LocalStack.Client v1.x** | [![NuGet v1.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client%3Fsource%3Dnuget%26track%3D1%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client/) | [![Github v1.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client%3Fsource%3Dgithub%26track%3D1%26includeprerelease%3Dtrue%26label%3Dgithub)](https://github.com/localstack-dotnet/localstack-dotnet-client/pkgs/nuget/LocalStack.Client) |
| **LocalStack.Client v2.x** | [![NuGet v2.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client%3Fsource%3Dnuget%26track%3D2%26includeprerelease%3Dtrue%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client/) | [![Github v2.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client%3Fsource%3Dgithub%26track%3D2%26includeprerelease%3Dtrue%26label%3Dgithub)](https://github.com/localstack-dotnet/localstack-dotnet-client/pkgs/nuget/LocalStack.Client) |
| **LocalStack.Client.Extensions v1.x** | [![NuGet v1.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client.extensions%3Fsource%3Dnuget%26track%3D1%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client.Extensions/) | [![Github v1.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client.extensions%3Fsource%3Dgithub%26track%3D1%26includeprerelease%3Dtrue%26label%3Dgithub)](https://github.com/localstack-dotnet/localstack-dotnet-client/pkgs/nuget/LocalStack.Client.Extensions) |
| **LocalStack.Client.Extensions v2.x** | [![NuGet v2.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client.extensions%3Fsource%3Dnuget%26track%3D2%26includeprerelease%3Dtrue%26label%3Dnuget)](https://www.nuget.org/packages/LocalStack.Client.Extensions/) | [![GitHub Packages v2.x](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2F%3Fpackage%3Dlocalstack.client.extensions%26source%3Dgithub%26track%3D2%26includeprerelease%3Dtrue%26label%3Dgithub)](https://github.com/localstack-dotnet/localstack-dotnet-client/pkgs/nuget/LocalStack.Client.Extensions) |

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

## Why LocalStack.NET Client?

- **Consistent Client Configuration:** LocalStack.NET eliminates the need for manual endpoint configuration, providing a standardized and familiar approach to initializing clients.

- **Adaptable Environment Transition:** Easily switch between LocalStack and actual AWS services with minimal configuration changes.

- **Versatile .NET Compatibility:** Both packages target .NET 8, .NET 9, .NET 10, and .NET Standard 2.0; Core also has an explicit .NET Framework 4.7.2 target. See [platform and packaging](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/architecture/platform-and-packaging.md) for the exact project targets.

- **Reduced Learning Curve:** Offers a familiar interface tailored for LocalStack, ideal for developers acquainted with the AWS SDK for .NET.

- **Enhanced Development Speed:**  Reduces boilerplate and manual configurations, speeding up the development process.

## Prerequisites

To utilize this library, you need to have LocalStack running. While LocalStack can be installed directly on your machine and accessed via the localstack cli, the recommended approach is to run LocalStack using [Docker](https://docs.docker.com/get-docker/) or [docker-compose](https://docs.docker.com/compose/install/).

**LocalStack needs an auth token.** From release 2026.03.0 on, every LocalStack for AWS image, including `localstack/localstack:latest`, requires a `LOCALSTACK_AUTH_TOKEN` to start ([announcement](https://blog.localstack.cloud/localstack-for-aws-release-2026-03-0/)). Without one, the container exits with a "License activation failed" error and LocalStack.NET has nothing to connect to. A free account is available: create it at [localstack.cloud](https://localstack.cloud/), copy your token from the LocalStack Web App, and pass it to the container:

```bash
docker run --rm -it \
  -e LOCALSTACK_AUTH_TOKEN=$LOCALSTACK_AUTH_TOKEN \
  -p 4566:4566 \
  localstack/localstack
```

With docker-compose, add it under the LocalStack service:

```yaml
environment:
  - LOCALSTACK_AUTH_TOKEN=${LOCALSTACK_AUTH_TOKEN}
```

In CI, use a CI Auth Token stored in your CI provider's secret store; see the [Auth Token documentation](https://docs.localstack.cloud/aws/getting-started/auth-token/).

The token is a LocalStack platform credential for the container. LocalStack.NET does not read it, and it does not replace the AWS credentials in the `LocalStack:Session` configuration.

If you cannot use a token, `localstack/localstack:4.14.0` is the last release that starts without one. LocalStack keeps such earlier tags runnable without authentication but gives them no further service or security updates ([details](https://blog.localstack.cloud/the-road-ahead-for-localstack/)).

For detailed installation and setup instructions, please refer to the [official LocalStack installation guide](https://docs.localstack.cloud/aws/getting-started/installation/).

## Getting Started

LocalStack.NET is available through multiple package sources to support different development workflows.

### 📦 Package Installation

To install the latest version of LocalStack.NET with AWS SDK v4 support, use the following command:

```bash
# Install the published v2 releases with AWS SDK v4 support
dotnet add package LocalStack.Client --version 2.0.0
dotnet add package LocalStack.Client.Extensions --version 2.0.1
```

#### Development Builds (GitHub Packages)

For testing latest features and bug fixes:

```bash
# Add GitHub Packages source
dotnet nuget add source https://nuget.pkg.github.com/localstack-dotnet/index.json \
  --name github-localstack \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN

# Install development packages
dotnet add package LocalStack.Client --prerelease --source github-localstack
dotnet add package LocalStack.Client.Extensions --prerelease --source github-localstack
```

> **🔑 GitHub Packages Authentication**: You'll need a GitHub account and [Personal Access Token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token) with `read:packages` permission.

Refer to [documentation](https://github.com/localstack-dotnet/localstack-dotnet-client/wiki/Getting-Started#installation) for more information on how to install LocalStack.NET.

`LocalStack.NET` is a library that provides a wrapper around the [aws-sdk-net](https://github.com/aws/aws-sdk-net). This means you can use it in a similar way to the `AWS SDK for .NET` and to [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) with a few differences. For more on how to use the AWS SDK for .NET, see [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html).

### Setup

Here's a basic example of how to setup `LocalStack.NET`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    services.AddLocalStack(Configuration);
    services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
    services.AddAwsService<IAmazonS3>();
    services.AddAwsService<IAmazonDynamoDB>();
}
```

`AddLocalStack` binds configuration and registers the library's services. `AddAwsService` registers the AWS service interface; `LocalStack:UseLocalStack` selects whether that interface is created through the LocalStack session or the ordinary AWS SDK factory. The default is `false`. See [client runtime and service support](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/architecture/client-runtime.md) for what the endpoint catalog does—and does not—promise.

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

- **`LocalStackClientConfigurationException` on `LocalStack.Client.Extensions` v2.0.0 and earlier:** If `UseLocalStack` is `false` and `AWSSDK.Extensions.NETCore.Setup` resolves to **4.0.4 or newer**, client creation throws `LocalStackClientConfigurationException`. AWS changed an internal `ClientFactory<T>` constructor in 4.0.4; the new parameter is optional, so it shipped as a patch release and is source-compatible on their side, but it is binary-breaking for the reflection lookup used here. Because v2.0.0 declares `AWSSDK.Extensions.NETCore.Setup >= 4.0.2`, NuGet floats new installs straight onto the broken versions. Only the `UseLocalStack: false` path is affected — LocalStack-only usage never reaches this code. **Fixed in v2.0.1**, so upgrading is the recommended fix. If you cannot upgrade yet, pin the AWS package to the last working version, which v2.0.0's range still allows:

  ```xml
  <PackageReference Include="AWSSDK.Extensions.NETCore.Setup" Version="4.0.3.40" />
  ```

  See [issue #52](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/52) for the full analysis.

- **SNS with LocalStack v3.7.2 through v3.8.1:** SNS operations failed on LocalStack v3.7.2 and v3.8.0 because of a request-forwarding bug ([issue #11652](https://github.com/localstack/localstack/issues/11652)). The fix ([PR #11653](https://github.com/localstack/localstack/pull/11653)) was merged after v3.8.1 shipped and first appears in **v4.0.0**; no 3.x release contains it. Use LocalStack v4.0.0 or later, or v3.7.1 if you must stay on the 3.x line.

- **Endpoint environment variables:** [Issue #27](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/27#issuecomment-4937111791) records a 2026-07-10 SQS observation against Extensions 2.0.0. With `UseLocalStack=true`, a LocalStack proxy aimed at A and `AWS_ENDPOINT_URL` aimed at B sent traffic to A. With `UseLocalStack=false`, the proxy was unset and traffic followed B. This is a dated report, not a re-run against every SDK or service; the issue remains open and no general precedence fix is merged. `AWS_SERVICE_URL` can also appear in Lambda environments, but this library does not define a general override rule for it. Diagnose the actual SDK configuration before adopting an environment-variable workaround.

Older notes about LocalStack v1 and v2 releases and AWS SDK v3 packages remain in the [v1.x README](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/sdkv3-lts/README.md#known-issues).

## Developing

We appreciate contributions in the form of feedback, bug reports, and pull requests.

### Building the Project

To build the project, use the following commands based on your operating system:

Windows

```powershell
build.ps1
```

Linux

```bash
./build.sh
```

### Sandbox Applications

The LocalStack .NET repository includes several sandbox console applications located in [tests/sandboxes](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes). These applications serve both as testing tools and as examples. Refer to [the documentation](https://github.com/localstack-dotnet/localstack-dotnet-client/wiki/Developing#sandbox-applications) for more information

### Running Tests

To execute the tests, use the commands below:

Windows

```powershell
build.ps1 --target=tests
```

Linux

```bash
./build.sh --target=tests
```

## Community

Got questions or wild feature ideas?  

👉 Join the conversation in [GitHub Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions).

## Changelog

Please refer to [`CHANGELOG.md`](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/CHANGELOG.md) to see the complete list of changes for each release.

## License

Licensed under MIT, see [LICENSE](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/LICENSE) for the full text.
