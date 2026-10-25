# LocalStack.NET

[![LocalStack.Client](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client%3Fsource%3Dnuget%26label%3DLocalStack.Client)](https://www.nuget.org/packages/LocalStack.Client/) [![LocalStack.Client.Extensions](https://img.shields.io/endpoint?url=https%3A%2F%2Fyvfdbfas85.execute-api.eu-central-1.amazonaws.com%2Flive%2Fbadge%2Fpackages%2Flocalstack.client.extensions%3Fsource%3Dnuget%26label%3DLocalStack.Client.Extensions)](https://www.nuget.org/packages/LocalStack.Client.Extensions/) [![Downloads](https://img.shields.io/nuget/dt/LocalStack.Client)](https://www.nuget.org/packages/LocalStack.Client/) [![CI/CD Pipeline](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml) [![Security](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/github-code-scanning/codeql) [![Tests](https://img.shields.io/endpoint?url=https://yvfdbfas85.execute-api.eu-central-1.amazonaws.com/live/badge/tests/linux?label=Tests)](https://github.com/localstack-dotnet/localstack-dotnet-client/actions/workflows/ci-cd.yml)

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

[LocalStack](https://github.com/localstack/localstack) is a local AWS cloud emulator. LocalStack.NET wires AWS SDK for .NET v4 clients to LocalStack and lets you switch between LocalStack and real AWS with one configuration setting. LocalStack's [.NET guide](https://docs.localstack.cloud/aws/connecting/aws-sdks/dotnet/) lists LocalStack.NET as an alternative to configuring endpoints by hand.

## Features

- Create LocalStack-configured AWS clients without manual endpoint code.
- Register clients through dependency injection with APIs that mirror `AWSSDK.Extensions.NETCore.Setup`.
- Switch client creation between LocalStack and AWS with `LocalStack:UseLocalStack`.
- Use Native AOT and trimming on `net10.0` and `net11.0` without changing consumer code.
- Run on .NET Framework 4.7.2 and later, and on other `netstandard2.0` runtimes.

## Packages

| Package | Use it when | NuGet link |
| --- | --- | --- |
| `LocalStack.Client` (Core) | You create clients directly through a session. | [Core](https://www.nuget.org/packages/LocalStack.Client/) |
| `LocalStack.Client.Extensions` (Extensions) | You use .NET configuration and dependency injection. | [Extensions](https://www.nuget.org/packages/LocalStack.Client.Extensions/) |

Install one package, plus the AWS SDK v4 service packages your app uses (for example, `AWSSDK.S3`). Extensions already depends on Core.

```bash
dotnet add package LocalStack.Client.Extensions   # configuration and dependency injection
dotnet add package LocalStack.Client              # Core only
```

### Prerelease builds

Development builds are published to GitHub Packages at the feed shown below.
Authentication requires a GitHub personal access token with `read:packages` permission.
Replace the username and token placeholders below with your GitHub credentials.

```bash
dotnet nuget add source https://nuget.pkg.github.com/localstack-dotnet/index.json \
  --name github-localstack --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
dotnet add package LocalStack.Client.Extensions --prerelease --source github-localstack
```

## Supported frameworks

| TFM | Core | Extensions | Native AOT |
| --- | --- | --- | --- |
| `netstandard2.0` | Yes | Yes | No |
| `net472` | Yes (.NET Framework 4.7.2+) | Uses `netstandard2.0` | No |
| `net10.0` | Yes | Yes | Yes |
| `net11.0` (preview) | Yes | Yes | Yes |

.NET 11 is not generally available yet; the `net11.0` assets are built with a .NET 11 preview SDK.
.NET 8 and .NET 9 apps use both packages' `netstandard2.0` assets under JIT, without Native AOT; both runtimes reach [end of support](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) on November 10, 2026.

## Prerequisites

Run LocalStack with Docker or docker-compose; follow the [installation guide](https://docs.localstack.cloud/aws/getting-started/installation/).

From 2026.03.0 on, every LocalStack for AWS image (including `localstack/localstack:latest`) needs `LOCALSTACK_AUTH_TOKEN`, even on the Hobby plan, which is free for non-commercial use.
The token belongs to the container: LocalStack.NET does not read it, and it is not an AWS credential.
`localstack/localstack:4.14.0` is the last image that starts without a token; it receives no further updates.
See the official [auth-token documentation](https://docs.localstack.cloud/aws/getting-started/auth-token/) and [CI guidance](https://docs.localstack.cloud/aws/ci-pipelines/).

## Quick start

In a minimal-hosting `Program.cs`, register LocalStack and your AWS service interfaces:

```csharp
using Amazon.S3;
using LocalStack.Client.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalStack(builder.Configuration);
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAwsService<IAmazonS3>();

var app = builder.Build();
app.Run();
```

`appsettings.Development.json` selects LocalStack:

```json
{
  "LocalStack": {
    "UseLocalStack": true,
    "Session": {
      "RegionName": "eu-central-1"
    },
    "Config": {
      "LocalStackHost": "localhost.localstack.cloud",
      "EdgePort": 4566
    }
  }
}
```

`appsettings.Production.json` selects the ordinary AWS SDK factory:

```json
{
  "LocalStack": {
    "UseLocalStack": false
  },
  "AWS": {
    "Profile": "<your aws profile>",
    "Region": "eu-central-1"
  }
}
```

## Configuration

| Key | Default |
| --- | --- |
| `LocalStack:UseLocalStack` | `false` |
| `LocalStack:Session:AwsAccessKeyId` | `accessKey` |
| `LocalStack:Session:AwsAccessKey` | `secretKey` |
| `LocalStack:Session:AwsSessionToken` | `token` |
| `LocalStack:Session:RegionName` | `us-east-1` |
| `LocalStack:Config:LocalStackHost` | `localhost.localstack.cloud` |
| `LocalStack:Config:UseSsl` | `false` |
| `LocalStack:Config:UseLegacyPorts` | `false` (only for LocalStack before 0.11.5) |
| `LocalStack:Config:EdgePort` | `4566` |

LocalStack does not validate the dummy session credentials.
When `UseLocalStack` is `false`, the ordinary `AWS` section and `AddDefaultAWSOptions(configuration.GetAWSOptions())` from `AWSSDK.Extensions.NETCore.Setup` apply.

## Registering clients

`AddAwsService<TService>`, `TryAddAwsService<TService>`, `AddAWSServiceLocalStack<TService>`, and `TryAddAWSServiceLocalStack<TService>` each have two overloads:

- `(ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)`
- `(AWSOptions? options, ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)`

The `Try*` methods register a client only if its service type is not registered yet. `AddAWSServiceLocalStack` is an alias of `AddAwsService` that avoids confusion with the AWS SDK's `AddAWSService`.
`AddDefaultAwsOptions(AWSOptions)` delegates to the SDK's `AddDefaultAWSOptions`.

Every LocalStack client gets the configured session credentials (dummy values by default), `ProxyHost`/`ProxyPort` pointing at LocalStack, HTTP unless `UseSsl` is `true`, and S3 path-style addressing.
With `useServiceUrl: false`, the client also gets `RegionEndpoint` from `RegionName`, which must be set; `useServiceUrl: true` sets `ServiceURL` to the service's LocalStack URL instead.
This option configures a LocalStack client; `LocalStack:UseLocalStack` selects LocalStack or AWS client creation.

```csharp
builder.Services.TryAddAwsService<IAmazonS3>(
    lifetime: ServiceLifetime.Singleton, useServiceUrl: true);
```

## Using LocalStack.Client without dependency injection

Create a session with `SessionStandalone` and choose an implementation-based or interface-based client:

```csharp
using Amazon.S3;
using LocalStack.Client;
using LocalStack.Client.Options;

var session = SessionStandalone.Init()
    .WithSessionOptions(new SessionOptions())
    .WithConfigurationOptions(new ConfigOptions())
    .Create();

using var s3 = session.CreateClientByImplementation<AmazonS3Client>();
using var s3ByInterface = session.CreateClientByInterface<IAmazonS3>();
```

The `Type`-based overloads are `CreateClientByImplementation(Type implType, bool useServiceUrl = false)` and `CreateClientByInterface(Type serviceInterfaceType, bool useServiceUrl = false)`.

## Native AOT and trimming

On `net10.0` and `net11.0`, both packages support Native AOT and trimming with no consumer code changes, zero trim/AOT warnings and no suppressions. They use no runtime reflection for AWS SDK service clients and configs, or for custom clients that follow the SDK's shape; other client types keep a reflection fallback that runs only under JIT. The `Type`-based APIs rely on a source generator that runs in C# projects, so F# and Visual Basic apps should use the generic APIs under Native AOT. These statements cover LocalStack.NET's own code; the AWS SDK and LocalStack's service coverage have their own status. See [platform and packaging](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/docs/architecture/platform-and-packaging.md#native-aot-and-trimming) for the exact boundary.

## Versions and support

3.x (AWS SDK v4) is the supported line, developed on `master`; earlier versions are not supported. .NET 8 and .NET 9 apps use the `netstandard2.0` assets under JIT, without Native AOT. If you pin AWS packages yourself, use at least `AWSSDK.Core` 4.0.102.6 with Core and 4.0.102.11 with Extensions, and `AWSSDK.Extensions.NETCore.Setup` 4.0.102.4; lower pins fail restore with `NU1605`. See the [changelog](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/CHANGELOG.md) for release details and the [security policy](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/.github/SECURITY.md) for the patch policy.

## Known issues

- **SNS with LocalStack v3.7.2 through v3.8.1:** SNS operations failed on LocalStack v3.7.2 and v3.8.0 because of a request-forwarding bug ([issue #11652](https://github.com/localstack/localstack/issues/11652)). The fix ([PR #11653](https://github.com/localstack/localstack/pull/11653)) was merged after v3.8.1 shipped and first appears in **v4.0.0**; no 3.x release contains it. Use LocalStack v4.0.0 or later, or v3.7.1 if you must stay on the 3.x line.
- **Endpoint environment variables:** With `UseLocalStack: true`, the LocalStack proxy settings took precedence over `AWS_ENDPOINT_URL` in a reported SQS case; with `UseLocalStack: false`, `AWS_ENDPOINT_URL` applied. The library defines no general precedence rule for `AWS_ENDPOINT_URL` or `AWS_SERVICE_URL`, so check the actual SDK configuration before relying on either. See [#27](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/27#issuecomment-4937111791).

## Contributing

See the [contribution guide](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/.github/CONTRIBUTING.md) for build, sandbox, and test instructions. Ask questions and share ideas in [Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions).

## Changelog

Read the [changelog](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/CHANGELOG.md) for changes in each release.

## License

LocalStack.NET uses the [MIT License](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/LICENSE).
