# LocalStack .NET Client v2.x Change Log

This document outlines the changes, updates, and important notes for the LocalStack .NET Client v2.x series, including the latest preview release.

See v1.x change log for previous versions: [CHANGELOG.md](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/sdkv3-lts/CHANGELOG.md)

## [Unreleased]

### Documentation

- The README prerequisites now explain that LocalStack images from 2026.03.0 on, including `latest`, need `LOCALSTACK_AUTH_TOKEN` to start. They show how to pass it with Docker and docker-compose and name 4.14.0 as the last token-free release, which gets no further updates ([#51](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/51)).
- Known Issues now states that the SNS fix first shipped in LocalStack v4.0.0. Removed the LocalStack v2 routing, AWS SDK v3 `AWSSDK.SQS` and untracked `IAmazonLambda` notes; the v1.x README keeps the historical entries.

### General

- Replaced the AWS-mode wrapper's private SDK reflection with the public `AddAWSService<T>` factory, preserving provider options fallback and client lifetimes. Removed its obsolete linker descriptor and reflection/AOT annotations; Core client creation and `AddLocalStack` configuration binding still do not support Native AOT.
- Added explicit `net10.0` target assets to both packages while retaining their existing target frameworks.
- Set the development SDK baseline to 10.0.402 and configured test projects and CI workflows to cover `net10.0`.
- Updated all existing non-Core centrally managed NuGet versions to their latest stable releases, including build, test, and analyzer packages (Extensions' consumer minimums are preserved below). Build-tool updates include Cake.Frosting 6.3.0 and NuGet.Versioning 7.9.0.
- Updated centrally managed Microsoft.Extensions versions to 10.0.13 for test and sandbox consumers. Extensions retains its existing consumer minimums: `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Configuration.Binder`, `Microsoft.Extensions.Logging.Abstractions`, and `Microsoft.Extensions.Options.ConfigurationExtensions` at 3.1.32 on all targets; `Microsoft.Extensions.DependencyInjection.Abstractions` at 3.1.32 on `netstandard2.0` and 8.0.0 on `net8.0`, `net9.0`, and `net10.0`.
- Updated SourceLink to 10.0.402 and Testcontainers plus Testcontainers.LocalStack to 4.16.0; restore resolves Microsoft.Build.Tasks.Git 10.0.402 and SSH.NET 2026.0.0.
- Set the declared `AWSSDK.Core` minimum to 4.0.102.6, the recent compatible stable candidate with the highest version-level downloads among stable AWS SDK v4 releases in a six-week comparison of [NuGet.org releases](https://www.nuget.org/packages/AWSSDK.Core), checked 2026-10-19. Core-only projects resolve 4.0.102.6; Extensions and service-package consumers resolve 4.0.102.11 under their actual dependency ranges. `AWSSDK.Extensions.NETCore.Setup` remains at 4.0.102.4; refreshed 112 existing AWS service SDK test-dependency pins, retained six already-latest entries, and made no catalog additions or removals.
- Updated the build dependency `System.Security.Cryptography.Pkcs` to 10.0.13; repository-wide `NuGetAuditMode=all` remains enabled.
- The Cake build no longer changes the global Git `core.autocrlf` setting on Unix; a repository `.gitattributes` now normalizes line endings. The Cake test task no longer deletes every Docker container before functional tests, because Testcontainers removes the containers each run starts, and the `Cake.Docker` build dependency was removed.

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
