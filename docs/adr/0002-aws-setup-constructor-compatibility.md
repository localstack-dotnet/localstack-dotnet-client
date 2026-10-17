# Keep the AWS client factory compatible with changed SDK internals

Date: 2026-07-29

`AWSSDK.Extensions.NETCore.Setup` 4.0.4 added an optional parameter to the internal `ClientFactory<T>` constructor, breaking this library's exact-signature reflection lookup when `UseLocalStack` was false. The #52 fix selects a non-public constructor with `AWSOptions` first and prefers the fewest parameters, supplies trailing defaults, and raises the Setup dependency floor to 4.0.100.5 rather than retaining a pre-4.0.4 compatibility leg that the new package floor would make uninstallable. A daily canary floats Core and Setup so another upstream change can surface before pinned builds would find it.

**Consequences:** Consumers pinned below the new dependency floor must update. The wrapper still reflects over internal AWS type and method names; replacing it with the public `AWSOptions.CreateServiceClient<T>()` path was deferred because that path lacks an `IServiceProvider` overload and would require reworking options fallback and logger behavior. The canary is a mitigation, not proof that future SDK versions remain compatible.

**Evidence:** [Bug #52](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/52), [merged PR #53](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/53), [floor/legacy-track decision commit](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/8da194b2c6abb7b06b14128394a3fb5c9338d17a), and [current wrapper](../../src/LocalStack.Client.Extensions/AwsClientFactoryWrapper.cs). The earlier proposal to keep the old floor was not the merged decision.
