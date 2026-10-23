# Platform and packaging boundary

Date: 2026-10-23

The source of package versions, target frameworks, and dependencies is the project configuration, not a prose copy of their values:

| Question | Owning file |
| --- | --- |
| Core and Extensions package versions | [`Directory.Build.props`](../../Directory.Build.props) |
| AWS SDK and other package versions; pinned versus floating canary track | [`Directory.Packages.props`](../../Directory.Packages.props) |
| Core target frameworks | [`LocalStack.Client.csproj`](../../src/LocalStack.Client/LocalStack.Client.csproj) |
| Extensions target frameworks and dependency on Core | [`LocalStack.Client.Extensions.csproj`](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj) |
| Development SDK selection | [`global.json`](../../global.json) |
| Publication triggers and credentials mechanism | [GitHub Actions workflows](../../.github/workflows/) |

For Extensions' three target frameworks (`netstandard2.0`, `net10.0`, and `net11.0`), the conditional references and `VersionOverride` values in [LocalStack.Client.Extensions.csproj](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj), together with the central `Microsoft.Extensions.Options` version, define these consumer minimums:

| Package | Declared minimum | Target frameworks |
| --- | --- | --- |
| `Microsoft.Extensions.Configuration.Abstractions` | 3.1.32 | All three |
| `Microsoft.Extensions.Configuration.Binder` | 3.1.32 | All three |
| `Microsoft.Extensions.Logging.Abstractions` | 3.1.32 | All three |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | 3.1.32 | All three |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 3.1.32 | `netstandard2.0` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 8.0.0 | `net10.0`, `net11.0` |
| `Microsoft.Extensions.Options` | 8.0.2 | `net10.0`, `net11.0` |

These are this package's consumer minimums, not a copy of the selected [`AWSSDK.Extensions.NETCore.Setup` 4.0.102.4 package ranges](https://www.nuget.org/packages/AWSSDK.Extensions.NETCore.Setup/4.0.102.4). Setup 4.0.102.4 permits older 2.x configuration, logging, and legacy dependency-injection packages, and a `net8.0` dependency-injection package at 8.0.0; those permissive ranges do not lower or define Extensions' own contract. Central versions in [Directory.Packages.props](../../Directory.Packages.props) govern test and sandbox consumers separately. A consumer's resolved graph can select a higher version when another dependency requires it.

The modern-only Options 8.0.2 reference is an unreleased Native AOT fix, not a blanket dependency refresh. It also raises the transitive `Microsoft.Extensions.Primitives` minimum to 8.0.0 on those assets. The `netstandard2.0` dependency floor is unchanged. The centrally managed 10.0.13 versions of other Microsoft.Extensions packages used by tests and sandboxes are not consumer minimums; successful execution with a higher resolved graph does not qualify every declared minimum-version combination.

Core explicitly targets `netstandard2.0`, `net472`, `net10.0`, and `net11.0`; Extensions targets `netstandard2.0`, `net10.0`, and `net11.0`. .NET 8/9 apps select `netstandard2.0` under JIT, without Native AOT; both runtimes reach end of support on 2026-11-10. The .NET 11 RC2 SDK pinned in [global.json](../../global.json) builds `net11.0` until .NET 11 GA on 2026-11-10. Both packages are planned as **3.0.0, unreleased**. The 2.x line is superseded with no further fixes, and 1.x (AWS SDK v3, `sdkv3-lts`) is unmaintained; the [security policy](../../.github/SECURITY.md) owns support windows. Consult the actual package feed before claiming publication.

## Native AOT and trimming

The **unreleased 3.0.0 implementation** supports Native AOT and trimming through the existing entry points on `net10.0` and `net11.0`; both package projects set `IsAotCompatible` on those targets. LocalStack.NET's own supported client/configuration paths use direct operations without runtime reflection, and Extensions uses fixed-schema options binding. The AWS SDK, DI, and BCL internals have their own status; these statements do not qualify those internals or LocalStack's service coverage.

`Session.CreateClientByInterface<TClient>()`, used by all eight Add/TryAdd/alias overloads in LocalStack mode, uses AWS SDK v4 static factories when the injected helper is exactly the default `SessionReflection`. It creates configuration with `TClient.CreateDefaultClientConfig()`, looks up the endpoint by `ClientConfig.ServiceId`, sets S3 `ForcePathStyle` through a [string-named `UnsafeAccessor`](../../src/LocalStack.Client/Utils/S3ForcePathStyle.cs), and creates the client with `TClient.CreateDefaultServiceClient(...)`. This path needs no generator and also works in F#/VB. Custom or mocked `ISessionReflection` implementations retain the previous helper calls and ordering.

The `Type` APIs, generic implementation API, and default `ISessionReflection` operations use a registry populated by the [slim generator](../../src/LocalStack.Client.Generators/AwsRegistryGenerator.cs) shipped in Core. Its [`buildTransitive` target](../../src/LocalStack.Client/buildTransitive/LocalStack.Client.targets) enables generation for net10-compatible C# consumers, including Extensions-only package consumers, without enrollment or a runtime compiler dependency. For discoverable, accessible, concrete, non-generic client shapes, it emits `UnsafeAccessor` metadata access, `new TConfig()`, direct constructors, a direct `ForcePathStyle` setter for the simple public bool property shape, and a plain-`RegionEndpoint` flag. Exact discovery and constructor/property eligibility rules live in the generator; [client runtime](client-runtime.md#client-creation) describes their use.

F#/VB projects using the `Type` APIs, runtime-loaded assemblies, private/generic custom clients, and unusual constructor/property shapes retain the previous reflection behavior under JIT. [`ReflectionFallback.IsSupported`](../../src/LocalStack.Client/Utils/ReflectionFallback.cs) is an auto-property with `FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))`; ILLink/ILC treat it as false, removing the fallback from trimmed and Native AOT apps without warnings or suppressions. On a miss without fallback, metadata throws `InvalidOperationException("Invalid service type ...")`, config creation throws `InvalidOperationException("Sequence contains no matching element")`, style returns `false` when neither the S3 accessor nor a generated setter applies, region sets `ClientConfig.RegionEndpoint` directly, client-type lookup returns not found, and implementation creation throws `NotSupportedException`. The `netstandard2.0`/`net472` assets retain the previous reflection paths.

The qualification mechanism is [`LocalStack.Client.NativeAot.Tests`](../../tests/LocalStack.Client.NativeAot.Tests/), an xUnit v3 AOT executable published with `PublishAot` for `net10.0`/`net11.0` and run by CI's `native-aot-tests` job on `win-x64` (`windows-latest`) and `linux-x64` (`ubuntu-22.04`). It checks unchanged `AddLocalStack` + `AddAwsService<IAmazonS3>`, all eight registration overloads, Core generic/`Type` paths, and `SessionStandalone`; Linux also exercises live S3/SQS round trips against LocalStack 4.14.0 via `LOCALSTACK_NATIVE_AOT_HOST`. The gate requires no trim/AOT warnings or suppressions. This is not an all-service, all-operation, arbitrary-type, or other-RID claim. [Quality gates](../engineering/quality-gates.md) owns execution details; [ADR 0004](../adr/0004-native-aot-preserves-existing-consumer-code.md) owns the decision. A build alone does not establish that this lane passed or that 3.0.0 was published.

## Consumer README and release channels

The [root README](../../README.md) is both the GitHub entry point and the source copied into each package project's tracked `README.md` by its `PreBuild` target. Those copies are package content, not independent documentation owners. Edit the root first and keep both copies byte-identical; a build can copy root content into them.

The [CI workflow](../../.github/workflows/ci-cd.yml) publishes development packages to GitHub Packages after its configured matrix succeeds on a master push or eligible feature pull request. The scheduled [AWS SDK canary](../../.github/workflows/aws-sdk-canary.yml) tests newer Core and Setup versions; it does not publish packages. NuGet.org releases use the manually dispatched [publication workflow](../../.github/workflows/publish-nuget.yml), which requests an OIDC-based short-lived key. Workflow configuration describes intended execution, not evidence that a particular run or package publication succeeded.
