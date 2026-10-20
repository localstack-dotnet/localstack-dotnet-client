# Use the LocalStack edge port by default and retain legacy ports

`ConfigOptions` defaults to the shared LocalStack edge endpoint on port 4566 with `UseLegacyPorts=false`. Keep `UseLegacyPorts` as an explicit choice for consumers that need the per-service port mappings, so the standard configuration uses one endpoint while older integrations can retain their routing.

The selected route comes from `ConfigOptions`; a hostname choice does not select between edge and per-service ports. The [client runtime](../architecture/client-runtime.md) owns endpoint mapping behavior.

**References:** [Configuration options](../../src/LocalStack.Client/Options/ConfigOptions.cs), [endpoint configuration](../../src/LocalStack.Client/Config.cs), and [maintainer explanation](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/5#issuecomment-775340238).
