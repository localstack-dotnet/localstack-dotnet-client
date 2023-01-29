# LocalStack .NET Client Change Log

### [v1.4.0](https://github.com/localstack-dotnet/localstack-dotnet-client/releases/tag/v1.4.0)

#### 1. New Features
- New endpoints in the official [Localstack Python Client](https://github.com/localstack/localstack-python-client) v1.39 have been added.
  - Fault Injection Service (FIS)
  - Marketplace Metering
  - Amazon Transcribe
  - Amazon MQ

#### 2. General
- Tested against LocalStack v1.3.1 container.
- AWSSDK.Core set to 3.7.103 as the minimum version.
  - **Warning** In this version, the ServiceURL property of Amazon.Runtime.ClientConfig adds a trailing `/` to every URL set.
For example, if `http://localhost:1234` is set as the value, it will become `http://localhost:1234/`

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