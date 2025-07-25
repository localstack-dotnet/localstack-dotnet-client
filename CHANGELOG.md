# LocalStack .NET Client v2.x Change Log

This document outlines the changes, updates, and important notes for the LocalStack .NET Client v2.x series, including the latest preview release.

See v1.x change log for previous versions: [CHANGELOG.md](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/sdkv3-lts/CHANGELOG.md)

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
