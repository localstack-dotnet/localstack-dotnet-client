# Changelog

## [v3.0.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v3.0.0)

> Major release of **both packages**: `LocalStack.Client` 3.0.0 and `LocalStack.Client.Extensions` 3.0.0.

### Breaking changes

- Dropped the `net8.0` and `net9.0` assets. .NET 8 and .NET 9 apps still install both packages and use `netstandard2.0` under JIT, without Native AOT; both runtimes reach end of support on November 10, 2026.
- `TryAddAwsService<TService>(lifetime, useServiceUrl)` now honors `useServiceUrl`. Callers that passed `true` now get `ServiceURL` instead of a region endpoint ([#63](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/63)).

### Added

- Added endpoint catalog entries (endpoint mapping and client construction) for DSQL, S3 Tables, Bedrock Runtime, CodeArtifact, Database Migration Service, Identity Store, Managed Blockchain, SSO Admin, Textract, Lambda Web, and S3 Files. Inputs: [LocalStack Python client `localstack/localstack-python-client@75fd327`](https://github.com/localstack/localstack-python-client/blob/75fd327/localstack_client/config.py) for DSQL and S3 Tables, and the [LocalStack service documentation](https://docs.localstack.cloud/aws/services/) for the rest. Most require a LocalStack license tier per that documentation; catalog membership does not establish operation coverage.
- Added `net10.0` and `net11.0` assets to Core (`LocalStack.Client`) and Extensions (`LocalStack.Client.Extensions`). .NET 11 is not generally available yet; the `net11.0` assets are built with the .NET 11 RC2 SDK pinned in `global.json`.
- Both packages support Native AOT and trimming on these targets with zero trim/AOT warnings, no suppressions, and no consumer code changes. This includes `AddLocalStack(configuration)`, all Add/TryAdd/alias registration overloads, Core `Session` / `SessionStandalone`, and the `Type`-based APIs.
- On `net10.0` and `net11.0`, both packages use no runtime reflection for AWS SDK service clients and configs, or for accessible, concrete, non-generic custom clients/configs with constructors and properties eligible for the [generated registry](docs/architecture/platform-and-packaging.md#native-aot-and-trimming). The generic interface APIs (`AddAwsService<T>` and every overload, `CreateClientByInterface<T>()`) use the AWS SDK v4 static factories when the injected helper is exactly the default `SessionReflection` and do not depend on source generation, so they also work in F# and Visual Basic; custom or mocked helpers retain the previous calls and ordering. The `Type`-based APIs and generic implementation API use a build-time source generator that ships in Core and turns itself on for net10-compatible C# projects, with no setup or extra package. Other client shapes (private or generic custom clients, F# and Visual Basic projects for the `Type`-based APIs, assemblies loaded at run time, unusual constructors or properties) keep the previous behavior through a reflection fallback that runs only under JIT; trimmed and Native AOT apps do not include it. These claims cover LocalStack.NET's own code; the AWS SDK, dependency injection and BCL internals have their own AOT status, and LocalStack determines its own service coverage.
- Both packages declare `IsAotCompatible` on `net10.0` and `net11.0`.

### Changed

- With `UseLocalStack: false`, Extensions now creates clients through the AWS SDK's public `AddAWSService<T>` factory instead of private SDK reflection. No consumer code changes are needed.

### Fixed

- With `UseLegacyPorts: true`, Account, ACM PCA, Bedrock, CloudControl, CodeBuild, CodeConnections, CodeDeploy, CodePipeline, Elastic Transcoder, MemoryDB, Shield and Verified Permissions now use port 4566, matching the LocalStack Python client. They used 4567, the legacy API Gateway port. The default edge-port mode is unchanged.

### Documentation

- The README states that LocalStack images from 2026.03.0 on, including `latest`, need `LOCALSTACK_AUTH_TOKEN`, and links the official [auth-token](https://docs.localstack.cloud/aws/getting-started/auth-token/) and [CI documentation](https://docs.localstack.cloud/aws/ci-pipelines/). It names 4.14.0 as the last token-free release, which gets no further updates ([#51](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/51)).
- Known issues now states that the SNS fix first shipped in LocalStack v4.0.0. Removed the LocalStack v2 routing, AWS SDK v3 `AWSSDK.SQS` and untracked `IAmazonLambda` notes.
- The security policy, contribution guide and templates list 3.x as the only supported line.
- Rewrote the README for 3.0.0 with package selection, configuration defaults, registration APIs, standalone usage, and upgrade guidance; moved contributor instructions to the contribution guide.

### Dependencies

- Core: `AWSSDK.Core` >= 4.0.102.6.
- Extensions: `AWSSDK.Extensions.NETCore.Setup` >= 4.0.102.4, which itself needs `AWSSDK.Core` >= 4.0.102.11, so Extensions apps resolve at least that Core version.
- Projects that pin `AWSSDK.Core` or `AWSSDK.Extensions.NETCore.Setup` below these versions (for example in `Directory.Packages.props`) get `NU1605` (package downgrade) on restore and must raise the pins. This showed up when upgrading three consumer repositories; no code changes were needed.
- Extensions: `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Configuration.Binder`, `Microsoft.Extensions.Logging.Abstractions`, and `Microsoft.Extensions.Options.ConfigurationExtensions` >= 3.1.32 on all targets.
- Extensions: `Microsoft.Extensions.DependencyInjection.Abstractions` >= 3.1.32 on `netstandard2.0` and >= 8.0.0 on `net10.0`/`net11.0`.
- Extensions: `Microsoft.Extensions.Options` >= 8.0.2 on `net10.0`/`net11.0` only.

### Build, tests and CI

- The repository now builds with the .NET 11 SDK pinned in `global.json`; test projects and CI cover `net10.0` and `net11.0`.
- Added the xUnit v3 `LocalStack.Client.NativeAot.Tests` lane and the `native-aot-tests` CI job, which publish with `PublishAot` and run native executables on Windows and Linux for both modern targets; Linux also runs live S3/SQS round trips against LocalStack 4.14.0. Cake skips this project. The same job packs both libraries and publishes `LocalStack.Client.PackageConsumer.Tests` from those packages with Native AOT on both operating systems.
- With the .NET 11 SDK, .NET Framework (`net472`) tests run on Windows only; Cake skips them elsewhere with a warning, and CI no longer installs Mono on macOS.
- Refreshed build, test, and analyzer dependencies, including Cake.Frosting 6.3.0, NuGet.Versioning 7.9.0, SourceLink 10.0.402, Testcontainers and Testcontainers.LocalStack 4.16.0, and `System.Security.Cryptography.Pkcs` 10.0.13. Restore resolves Microsoft.Build.Tasks.Git 10.0.402 and SSH.NET 2026.0.0; repository-wide `NuGetAuditMode=all` remains enabled.
- Test and sandbox consumers use centrally managed Microsoft.Extensions 10.0.13. Refreshed 112 AWS service SDK pins and retained six already-latest entries, with no catalog additions or removals. Core-only projects resolve 4.0.102.6; Extensions and service-package consumers resolve 4.0.102.11 under their dependency ranges.
- Cake no longer changes global Git `core.autocrlf` on Unix; `.gitattributes` normalizes line endings. The test task no longer deletes every Docker container; Testcontainers removes the containers each run starts. Removed `Cake.Docker`.
- Functional tests now use LocalStack 4.14.0 and 2026.10.2 instead of 3.7.1 and 4.6.0. The 2026.10.2 tests require `LOCALSTACK_AUTH_TOKEN` and fail with a message naming it when absent; `--filter "LocalStackAuth!=Required"` or Cake's `--skipAuthenticatedFunctionalTest true` excludes them. CI passes the Actions secret to the Linux functional run and skips those tests with a warning on pull requests that cannot read it.

## [v2.0.1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v2.0.1)

> Maintenance release for **`LocalStack.Client.Extensions` only**. `LocalStack.Client` stays at 2.0.0.

### 🐞 Fixes

- **Fixed `LocalStackClientConfigurationException` when `UseLocalStack` is `false`
  ([#52](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/52)).**
  `AWSSDK.Extensions.NETCore.Setup` 4.0.4 added an optional parameter to the internal
  `ClientFactory<T>` constructor, which broke our exact-signature reflection lookup. Resolution now
  matches on the `AWSOptions` parameter and defaults any trailing ones.
- Failure messages now list the constructor signatures actually discovered.

### 🛠️ General

- **Minimum `AWSSDK.Extensions.NETCore.Setup` is now 4.0.100.5** (was 4.0.2). No other dependency
  floor changed.
- **Internal dependency refresh:** `AWSSDK.Core` → 4.0.100.6, 120 AWSSDK service packages, and the
  analyzer/test toolchain. Clears the `NU1901` advisory (GHSA-9cvc-h2w8-phrp) on `AWSSDK.Core`
  4.0.0.15.
- **Added a scheduled AWS SDK canary** that builds against the newest `AWSSDK.Core` and
  `AWSSDK.Extensions.NETCore.Setup`, so upstream changes surface before they reach users.
- **Public API unchanged.**

---

## [v2.0.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v2.0.0)

> **Heads‑up**: Native AOT is not yet supported in GA.  
> Follow  [draft PR #49](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/49) for the reflection‑free path planned for v2.1.

### ✨ New features (since `v2.0.0‑preview1`)

- **Added Endpoints from [Localstack Python Client](https://github.com/localstack/localstack-python-client) v2.9:**
  - **Account Management**
  - **Certificate Manager Private Certificate Authority (ACMPCA)**
  - **Bedrock**
  - **Cloud Control API**
  - **Code Build**
  - **Code Connections**
  - **Code Deploy**
  - **Code Pipeline**
  - **Elastic Transcoder**
  - **MemoryDB for Redis**
  - **Shield**
  - **Verified Permissions**

### 🛠️ General

- **Testing Compatibility:**
  - Successfully tested against LocalStack versions:
    - **v3.7.1**
    - **v4.6.0**

*See [`v2.0.0-preview1`](#v200-preview1) for the complete migration from v1.x and the AWS SDK v4 overhaul.*

---

## [v2.0.0-preview1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v2.0.0-preview1)

### 1. Breaking Changes

- **Framework Support Updates:**
  - **Deprecated** support for **.NET Framework 4.6.2**.
  - **Added** support for **.NET Framework 4.7.2** (required for AWS SDK v4 compatibility).

### 2. General

- **AWS SDK v4 Migration:**
  - **Complete migration** from AWS SDK for .NET v3 to v4.
  - **AWSSDK.Core** minimum version set to **4.0.0.15**.
  - **AWSSDK.Extensions.NETCore.Setup** updated to **4.0.2**.
  - All 70+ AWS SDK service packages updated to v4.x series.

- **Framework Support:**
  - **.NET 9**
  - **.NET 8**  
  - **.NET Standard 2.0**
  - **.NET Framework 4.7.2**

- **Testing Validation:**
  - **1,099 total tests** passing across all target frameworks.
  - Successfully tested with AWS SDK v4 across all supported .NET versions.
  - Tested against following LocalStack versions:
    - **v3.7.1**
    - **v4.3.0**

### 3. Important Notes

- **Preview Release**: This is a preview release for early adopters and testing. See the [v2.0.0 Roadmap & Migration Guide](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/45) for the complete migration plan.
- **No API Changes**: LocalStack.NET public APIs remain unchanged. All changes are internal to support AWS SDK v4 compatibility.
- **Feedback Welcome**: Please report issues or feedback on [GitHub Issues](https://github.com/localstack-dotnet/localstack-dotnet-client/issues).
- **v2.x series requires AWS SDK v4**: This version is only compatible with AWS SDK for .NET v4.x packages.
- **Migration from v1.x**: Users upgrading from v1.x should ensure their projects reference AWS SDK v4 packages.
- **Framework Requirement**: .NET Framework 4.7.2 or higher is now required (upgrade from 4.6.2).

---
