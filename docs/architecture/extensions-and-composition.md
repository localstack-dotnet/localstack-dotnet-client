# Extensions and application composition

Date: 2026-10-17

`LocalStack.Client.Extensions` integrates the core package with .NET configuration and dependency injection. The [client runtime](client-runtime.md) owns endpoint mapping and LocalStack client creation; this document owns application-level composition.

[`AddLocalStack(IConfiguration)`](../../src/LocalStack.Client.Extensions/ServiceCollectionExtensions.cs) binds the `LocalStack` section to `LocalStackOptions`, `SessionOptions`, and `ConfigOptions`, including their non-public setters. It registers `IConfig` as transient and `ISessionReflection`, `ISession`, and `IAwsClientFactoryWrapper` as singletons. Registration prepares both client-creation paths; it does not itself enable LocalStack mode.

`AddAwsService<TService>` registers an AWS service interface (singleton by default). When the configured `UseLocalStack` value is `true`, resolution calls `Session.CreateClientByInterface<TService>` and uses the LocalStack catalog. When it is `false`—the options default—resolution calls [`AwsClientFactoryWrapper`](../../src/LocalStack.Client.Extensions/AwsClientFactoryWrapper.cs) to create an ordinary AWS SDK client. `AddAWSServiceLocalStack<TService>` is an alias for the registration method, **not** an override of the mode switch. `TryAddAwsService<TService>` only adds a registration when one is absent.

The AWS-mode wrapper uses reflection to find the SDK's non-public generic `ClientFactory<T>`. It selects a constructor whose first parameter is `AWSOptions`, preferring fewer parameters, then supplies defaults for the remaining parameters and invokes the SDK factory method. This tolerates the optional constructor parameter introduced in `AWSSDK.Extensions.NETCore.Setup` 4.0.4, but it still depends on internal SDK type and method names. The accepted rationale for the compatibility choice lives in the [decision record](../adr/0002-aws-setup-constructor-compatibility.md).

**Current overload caveat:** the shorter `TryAddAwsService<TService>(lifetime, useServiceUrl)` overload does not forward its `useServiceUrl` argument (`ServiceCollectionExtensions.cs:126-129`). The overload that accepts `AWSOptions?` does forward it. Do not describe URL selection as reliable across both overloads until the implementation changes. The `useServiceUrl` switch itself is distinct from `UseLocalStack`: it selects a client configuration path only after the LocalStack session is chosen.
