# Client runtime and service mapping

Date: 2026-10-17

The core package creates AWS SDK service clients configured for LocalStack. The [glossary](../../GLOSSARY.md) distinguishes an AWS service, an SDK client, and a configured LocalStack endpoint.

## Endpoint catalog

[`AwsServiceEndpointMetadata.All`](../../src/LocalStack.Client/Enums/AwsServiceEndpointMetadata.cs) is the catalog of service IDs, CLI names, catalog keys, and historical per-service ports. [`Config`](../../src/LocalStack.Client/Config.cs) derives an endpoint from that catalog and `ConfigOptions`: a host, HTTP or HTTPS scheme, and either the shared edge port (the default) or a legacy service port. The service ID lookup is an exact match against the AWS SDK's metadata value; a CLI name or enum spelling is not interchangeable with it. Ports are not unique service identifiers.

The catalog is a mapping in **this library**, not a list of all AWS operations available in LocalStack. LocalStack's own service, edition, release, and operation coverage determines emulator capability. The official [LocalStack Python client](https://github.com/localstack/localstack-python-client) is an input when this catalog gains services, but a Python mapping is not itself proof of emulator behavior or of a mapping in this .NET release. A particular .NET release can create a LocalStack-configured client through `Session` for a service only after its own catalog contains the service ID. Until then, `Session` throws `NotSupportedClientException` for the missing mapping; that exception does **not** mean the emulator lacks the service. Functional-test counts do not define product support. Claims about an exact operation on a named LocalStack image or edition need separate evidence.

## Client creation

[`Session`](../../src/LocalStack.Client/Session.cs) accepts session options, an `IConfig` built from the endpoint catalog, and [`SessionReflection`](../../src/LocalStack.Client/Utils/SessionReflection.cs). Its interface-based path locates the AWS SDK implementation type by convention; its implementation-based path starts from the concrete type. Both inspect the SDK's private `serviceMetadata`, construct the corresponding `ClientConfig`, and create an SDK client with the configured credentials.

The client configuration sets `UseHttp`, `ProxyHost`, and `ProxyPort` from the LocalStack settings. Where the configuration exposes `ForcePathStyle`, the reflection helper sets it for S3-compatible addressing. With `useServiceUrl: true`, `Session` sets the explicit `ServiceURL`; otherwise it requires a region name and sets `RegionEndpoint`. These are two configuration paths for a LocalStack-created client, **not** the application-level LocalStack/AWS mode switch. They do not by themselves prove where every later request travels.

[`SessionStandalone`](../../src/LocalStack.Client/SessionStandalone.cs) creates this session without dependency injection. The optional Extensions package owns the application-level mode switch in [Extensions and composition](extensions-and-composition.md). The Core and Extensions reflection paths remain present on current `master`; see [platform and packaging](platform-and-packaging.md) for the Native AOT boundary.
