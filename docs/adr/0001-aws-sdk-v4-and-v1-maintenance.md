# Migrate to AWS SDK v4 on the v2 line while retaining a v1 line

Date: 2025-07-16

AWS SDK v4 changed more of the SDK integration surface than the first `ClientFactory<T>` failure exposed. Rather than patch v1 for that one symptom, the project moved the existing `LocalStack.Client` and `LocalStack.Client.Extensions` package identities to a v2 line using AWS SDK v4 on `master`, while retaining AWS SDK v3 on `sdkv3-lts` to give existing users migration time. This changes SDK compatibility and the explicit .NET Framework target from 4.6.2 to 4.7.2.

**Consequences:** Maintaining two lines costs backports. The security policy says v1 was maintained “until July 2026”; the subsequently edited [announcement](https://github.com/orgs/localstack-dotnet/discussions/45) specifies July 31, 2026. This historical record does not establish support after that window or turn the announcement's exact day into the policy file's wording. The first-party record explains the rejected temporary v1 patch, but not why the existing package names were preferred over separate v4-suffixed names.

**Evidence:** [Maintainer announcement (subsequently edited)](https://github.com/orgs/localstack-dotnet/discussions/45), [AWS SDK v4 merge PR #46](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/46), and the [v2 migration changelog](../../CHANGELOG.md). The merge date identifies acceptance of the v2 code; it is not the later GA publication date.
