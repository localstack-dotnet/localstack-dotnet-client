# LocalStack .NET Client Change Log

### [v2.0.0-preview1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v2.0.0-preview1)

#### 1. Breaking Changes

- **Framework Support Updates:**
  - **Deprecated** support for **.NET Framework 4.6.2**.
  - **Added** support for **.NET Framework 4.7.2** (required for AWS SDK v4 compatibility).

#### 2. General

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

#### 3. Important Notes

- **Preview Release**: This is a preview release for early adopters and testing. See the [v2.0.0 Roadmap & Migration Guide](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/45) for the complete migration plan.
- **No API Changes**: LocalStack.NET public APIs remain unchanged. All changes are internal to support AWS SDK v4 compatibility.
- **Feedback Welcome**: Please report issues or feedback on [GitHub Issues](https://github.com/localstack-dotnet/localstack-dotnet-client/issues).
- **v2.x series requires AWS SDK v4**: This version is only compatible with AWS SDK for .NET v4.x packages.
- **Migration from v1.x**: Users upgrading from v1.x should ensure their projects reference AWS SDK v4 packages.
- **Framework Requirement**: .NET Framework 4.7.2 or higher is now required (upgrade from 4.6.2).

---

### [v1.6.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.6.0)

#### 1. General

- **Framework Support Updates:**
  - **.NET 9** support added.
  - **Deprecated** support for **.NET 6**.
  - Continued support for **.NET Standard 2.0** to maintain compatibility with older .NET versions.

- **Functional Tests Enhancements:**
  - **Removed** tests for legacy LocalStack versions and versions **v1.3.1** and **v2.3.2**.
    - **Note:** LocalStack.NET no longer guarantees compatibility with v1 and v2 series.
  - **Added** functional test support for LocalStack versions:
    - **v3.7.1**
    - **v4.3.0**

- **Package Updates:**
  - **AWSSDK.Core** minimum version set to **3.7.402**.

- **Testing Compatibility:**
  - Successfully tested against LocalStack versions:
    - **v3.7.1**
    - **v4.3.0**

#### 3. Warnings

- **Breaking Changes Postponed:**
  - The planned breaking changes have been postponed to the next release.
  - **Important:** Users should anticipate some breaking changes in the next release due to the removal of legacy support and configuration updates.

### [v1.5.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.5.0)

#### 1. New Features

- **Added Endpoints from [Localstack Python Client](https://github.com/localstack/localstack-python-client) v2.7:**
  - **RAM**
  - **AppConfigData**
  - **Pinpoint**
  - **EventBridge Pipes**

#### 2. General

- **Framework Support Updates:**
  - **.NET 8** and **.NET 4.6.2** support added.
  - **Deprecated** support for **.NET 7** and **.NET 4.6.1**.
  - Continued support for **.NET Standard 2.0** to maintain compatibility with older .NET versions.
  - **Upcoming Changes:**
    - In the next release, **.NET 6** support will be removed as it reaches end-of-life in November 2024.

- **Functional Tests Enhancements:**
  - **Removed** tests for legacy LocalStack versions and versions **v2.0** and **v2.2**.
    - **Note:** LocalStack.NET no longer guarantees compatibility with these versions.
  - **Added** functional test support for LocalStack versions:
    - **v2.3**
    - **v3.4**
    - **v3.7.1**
  - **New Tests:**
    - Introduced new tests for **CloudFormation**.

- **Package Updates:**
  - **AWSSDK.Core** minimum version set to **3.7.400.30**.

- **Testing Compatibility:**
  - Successfully tested against LocalStack versions:
    - **v1.3.1**
    - **v2.3**
    - **v3.4**
    - **v3.7.1**

#### 3. Warnings

- **Breaking Changes Postponed:**
  - The planned breaking changes have been postponed to the next release.
  - **Important:** Users should anticipate some breaking changes in the next release due to the removal of legacy support and configuration updates.

### [v1.4.1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.4.1)

#### 1. New Features

- **Update Packages and Multi LocalStack Support:**
- New endpoints added from the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v2.3:
  - EMRServerless
  - Appflow
  - Keyspaces
  - Scheduler

#### 2. Bug Fixes and Investigations

- **Investigation and Fixes:**
  - Started investigating issues #23 and #24.
    - Bugs have been fixed with [this PR](https://github.com/localstack/localstack/pull/8962) by LocalStack.
  - Fixed legacy LocalStack container wait strategy for functional tests.

#### 3. General

- **New Solution Standards:**
  - Introduced new solution-wide coding standards with various analyzers.
- **Code Refactoring According to New Standards:**
  - Libraries, sandbox projects, build projects, and test projects have been refactored to adhere to the new coding standards.
  - Moved remaining using directives to GlobalUsings.cs files.
- **Centralized Package Management:**
  - Managed package versions centrally to resolve issue #28.
- **Package Updates:**
  - Updated analyzer packages.
  - Updated test packages.
  - AWSSDK.Core set to 3.7.201 as the minimum version.
- Tested against LocalStack v1.3.1, v2.0, and the latest containers.

#### 4. Warnings

- **Legacy LocalStack Versions:**
  - This version will be the last to support Legacy LocalStack versions.
- **.NET 4.6.1 Support:**
  - .NET 4.6.1 support will be removed in the next release and replaced with .NET 4.6.2.
- **Breaking Changes Ahead:**
  - Users should anticipate some breaking changes in the next release due to the removal of Legacy support and changes in configuration.

### [v1.4.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.4.0)

#### 1. New Features

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.39 have been added.
  - Fault Injection Service (FIS)
  - Marketplace Metering
  - Amazon Transcribe
  - Amazon MQ

#### 2. General

- .NET 7 support added
- .NET 5 ve .NET Core 3.1 runtimes removed from Nuget pack (.netstandard2.0 remains)
- Tested against LocalStack v1.3.1 container.
- AWSSDK.Core set to 3.7.103 as the minimum version.
  - **Warning** In this version, the ServiceURL property of Amazon.Runtime.ClientConfig adds a trailing `/` to every URL set.
    For example, if `http://localhost:1234` is set as the value, it will become `http://localhost:1234/`
- Following depedencies updated from v3.0.0 to v3.1.32 in LocalStack.Client.Extensions for security reasons
  - Microsoft.Extensions.Configuration.Abstractions
  - Microsoft.Extensions.Configuration.Binder
  - Microsoft.Extensions.DependencyInjection.Abstractions
  - Microsoft.Extensions.Logging.Abstractions
  - Microsoft.Extensions.Options.ConfigurationExtensions

#### 3. Bug Fixes

- Write a timestream record using .Net AWSSDK NuGet packages ([#20](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/20))
- Session does not honor UseSsl and always sets UseHttp to true ([#16](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/16))

### [v1.3.1](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.3.1)

#### 1. New Features

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.35 have been added.
  - Route53Resolver
  - KinesisAnalyticsV2
  - OpenSearch
  - Amazon Managed Workflows for Apache Airflow (MWAA)

#### 2. General

- Tested against LocalStack v0.14.2 container.
- AWSSDK.Core set to 3.7.9 as the minimum version.
- AWSSDK.Extensions.NETCore.Setup set to 3.7.2 as the minimum version.

### [v1.3.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.3.0)

#### 1. New Features

- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.27 have been added.
  - SESv2
  - EventBridge ([#14](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/14))
- Tested against LocalStack v0.13.0 container.

#### 2. Enhancements

- `useServiceUrl` parameter added to change client connection behavior. See [useServiceUrl Parameter](#useserviceurl)
- Readme and SourceLink added to Nuget packages

#### 3. Bug Fixes

- Session::RegionName configuration does not honor while creating AWS client ([#15](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/15))

Thanks to [petertownsend](https://github.com/petertownsend) for his contribution

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