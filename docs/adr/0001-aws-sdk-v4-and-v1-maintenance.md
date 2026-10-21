# Use AWS SDK v4 on the supported v2 package line

The supported `LocalStack.Client` and `LocalStack.Client.Extensions` v2 line uses AWS SDK v4 on `master`. SDK v4 changes the integration surface beyond a single client-factory fix, so it is a major-version integration change rather than a patch to the SDK v3-based v1 line. The existing package identities are retained, with major versions separating SDK compatibility.

The `sdkv3-lts` branch preserves the v1/SDK v3 code for existing consumers, but that line is no longer maintained. Consumers needing supported development or security updates must use v2; the [security policy](../../.github/SECURITY.md) owns the support window, and the [platform boundary](../architecture/platform-and-packaging.md) owns target-framework details.

**Rationale:** [Maintainer explanation](https://github.com/orgs/localstack-dotnet/discussions/45) and [AWS SDK v4 integration](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/46).
