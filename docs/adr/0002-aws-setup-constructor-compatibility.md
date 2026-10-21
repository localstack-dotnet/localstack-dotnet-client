# Use the public AWS registration factory for AWS-mode clients

AWS-mode client creation obtains the factory from the SDK's public `AddAWSService<T>()` registration and invokes it with the original `IServiceProvider`. SDK private constructor and method names can change in patch releases, so the integration depends on the public registration factory while preserving SDK options, configuration and logger fallback.

Direct `AWSOptions.CreateServiceClient<T>()` lacks the provider-aware behavior needed by the existing integration. Keep the public wrapper and interface, their generic constraint, outer registration lifetimes and `TargetInvocationException` boundary. A small collection adapter supports the existing legacy DI abstractions minimum without a new dependency or a second provider.

The public registration descriptor shape remains an upstream dependency, so the pinned tests and floating SDK canary remain necessary. This choice applies to AWS-mode factory composition; the [platform boundary](../architecture/platform-and-packaging.md#native-aot-and-trimming) owns the remaining Native AOT limits.

**References:** [Current wrapper](../../src/LocalStack.Client.Extensions/AwsClientFactoryWrapper.cs), [composition contract](../architecture/extensions-and-composition.md), and [private SDK constructor compatibility failure](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/52).
