# Use the LocalStack edge port by default and retain legacy ports

Date: 2021-02-08

LocalStack introduced a shared edge endpoint, while existing client configurations still used per-service ports. The project made port 4566 and non-legacy routing the `ConfigOptions` defaults, but retained `UseLegacyPorts` as an explicit choice for older setups rather than removing their port mapping during the transition. This changed the usual network destination without forcing every existing configuration to change at once.

**Scope:** The 2021 obsolete configuration constructor still had a legacy-port default; this record does not claim all historical constructors used edge routing. The 2023 change from `localhost` to `localhost.localstack.cloud` was a separate hostname change. [Issue #26](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/26) proposes removing legacy ports, but an open proposal is not a reversal of this accepted decision.

**Evidence:** [Merged PR #7](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/7), [maintainer explanation in issue #5](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/5#issuecomment-775340238), and the [current endpoint configuration](../../src/LocalStack.Client/Config.cs).
