# LocalStack .NET Client

This glossary distinguishes the library's concepts from the LocalStack emulator and the AWS SDK. It describes language, not package configuration or a service-compatibility matrix.

## Language

**Cataloged AWS service**:
An AWS service with a LocalStack endpoint mapping in this library. Its presence in the catalog does not assert that the emulator implements every operation.
_Avoid_: Supported service, when only catalog membership is known.

**LocalStack-configured AWS service client**:
An AWS SDK service client created through this library's LocalStack session. It is not the emulator service or a guarantee that every later request reaches a particular destination.
_Avoid_: LocalStack service, when referring to the client object.

**AWS service ID**:
The AWS SDK service identifier used to match a service to this library's endpoint catalog. It is distinct from the service's CLI name and the `AwsService` catalog key.
_Avoid_: CLI name, enum name, or an unqualified service name for this identifier.

**Configured LocalStack endpoint**:
The host, scheme, and port this library associates with a cataloged AWS service. Several services can share a port, and the configured endpoint alone does not establish which operations the emulator supports.
_Avoid_: AWS region or service ID as synonyms for an endpoint.

**LocalStack mode / AWS mode**:
The application-level choice between creating an AWS service client through this library's LocalStack session and creating it through the ordinary AWS SDK factory. This choice describes client creation, not an unconditional promise about subsequent network requests.
_Avoid_: LocalStack mode for the separate choice between an explicit service URL and a region-based client configuration.

**LocalStack.NET version line**:
A major-version family of this library's packages: v1.x targets AWS SDK v3, and v2.x targets AWS SDK v4. It is separate from the LocalStack emulator image version and the version of an individual AWS SDK package.
_Avoid_: An unqualified “LocalStack version” when referring to the library.
