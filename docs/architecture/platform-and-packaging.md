# Platform and packaging boundary

Date: 2026-10-22

The source of package versions, target frameworks, and dependencies is the project configuration, not a prose copy of their values:

| Question | Owning file |
| --- | --- |
| Core and Extensions package versions | [`Directory.Build.props`](../../Directory.Build.props) |
| AWS SDK and other package versions; pinned versus floating canary track | [`Directory.Packages.props`](../../Directory.Packages.props) |
| Core target frameworks | [`LocalStack.Client.csproj`](../../src/LocalStack.Client/LocalStack.Client.csproj) |
| Extensions target frameworks and dependency on Core | [`LocalStack.Client.Extensions.csproj`](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj) |
| Development SDK selection | [`global.json`](../../global.json) |
| Publication triggers and credentials mechanism | [GitHub Actions workflows](../../.github/workflows/) |

For Extensions' four target frameworks (`netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`), the conditional references and `VersionOverride` values in [LocalStack.Client.Extensions.csproj](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj), together with the central `Microsoft.Extensions.Options` version, define these consumer minimums:

| Package | Declared minimum | Target frameworks |
| --- | --- | --- |
| `Microsoft.Extensions.Configuration.Abstractions` | 3.1.32 | All four |
| `Microsoft.Extensions.Configuration.Binder` | 3.1.32 | All four |
| `Microsoft.Extensions.Logging.Abstractions` | 3.1.32 | All four |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | 3.1.32 | All four |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 3.1.32 | `netstandard2.0` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 8.0.0 | `net8.0`, `net9.0`, `net10.0` |
| `Microsoft.Extensions.Options` | 8.0.2 | `net8.0`, `net9.0`, `net10.0` |

These are this package's consumer minimums, not a copy of the selected [`AWSSDK.Extensions.NETCore.Setup` 4.0.102.4 package ranges](https://www.nuget.org/packages/AWSSDK.Extensions.NETCore.Setup/4.0.102.4). Setup 4.0.102.4 permits older 2.x configuration, logging, and legacy dependency-injection packages, and a `net8.0` dependency-injection package at 8.0.0; those permissive ranges do not lower or define Extensions' own contract. Central versions in [Directory.Packages.props](../../Directory.Packages.props) govern test and sandbox consumers separately. A consumer's resolved graph can select a higher version when another dependency requires it.

The modern-only Options 8.0.2 reference is an unreleased Native AOT fix, not a blanket dependency refresh. It also raises the transitive `Microsoft.Extensions.Primitives` minimum to 8.0.0 on those assets. The `netstandard2.0` dependency floor is unchanged. The centrally managed 10.0.13 versions of other Microsoft.Extensions packages used by tests and sandboxes are not consumer minimums; successful execution with a higher resolved graph does not qualify every declared minimum-version combination.

At this revision Core explicitly targets `netstandard2.0`, `net472`, `net8.0`, `net9.0`, and `net10.0`; Extensions targets `netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`. The AWS SDK v4 migration changed the explicit .NET Framework target from 4.6.2 to 4.7.2. The two packages can have different versions: the [changelog](../../CHANGELOG.md) records the Extensions-only 2.0.1 release. For an installation instruction, consult the actual package feed as well as the declared project version.

## Native AOT and trimming

The **unreleased implementation** preserves existing C# entry points through automatic source generation and fixed-schema options binding. Core packages a compiler-only generator and a [`buildTransitive` target](../../src/LocalStack.Client/buildTransitive/LocalStack.Client.targets) that enables it for .NET 8-compatible targets. It flows to Core-only and Extensions-only package consumers without manual generator enrollment and introduces no runtime compiler dependency. Generated operations root the existing metadata, constructor, and property reflection; this is not a reflection-free or parallel consumer API. The [client runtime](client-runtime.md#client-creation) and [composition](extensions-and-composition.md) documents own those behaviors.

Local-package validation covers .NET 10 Native AOT with its mandatory trimming on `win-x64` and `linux-x64`: unchanged `AddLocalStack(configuration)` / `AddAwsService<IAmazonS3>()` resolution and `ListBucketsAsync`, the eight existing S3 Add/TryAdd/alias registration overloads, and Core `Session` generic/`Type` paths and `SessionStandalone`. Core-only, Extensions-only, and combined consumers publish without trim/AOT warnings or authored suppressions and run against LocalStack. Matching JIT runs verify endpoint, region, credentials, and configured-value parity, while retaining the documented shorter TryAdd overload caveat.

This is not a claim for every cataloged service or operation, other native RIDs or frameworks, non-C# consumer compilers, or arbitrary runtime-loaded types absent from the compiled graph. Modern assets contain the implementation, but no separate trimmed-JIT deployment promise or `IsAotCompatible` declaration is made. Do not suppress trim/AOT warnings to obtain a successful publication.

This capability is **not part of the published Core 2.0.0 / Extensions 2.0.1 releases**; no release version or delivery date is assigned here. The [Native AOT decision](../adr/0004-native-aot-preserves-existing-consumer-code.md) defines the unchanged-consumer contract. Earlier draft generator work is not evidence that a released package meets it.

## Consumer README and release channels

The [root README](../../README.md) is both the GitHub entry point and the source copied into each package project's tracked `README.md` by its `PreBuild` target. Those copies are package content, not independent documentation owners. Edit the root first and keep both copies byte-identical; a build can copy root content into them.

The [CI workflow](../../.github/workflows/ci-cd.yml) publishes development packages to GitHub Packages after its configured matrix succeeds on a master push or eligible feature pull request. The scheduled [AWS SDK canary](../../.github/workflows/aws-sdk-canary.yml) tests newer Core and Setup versions; it does not publish packages. NuGet.org releases use the manually dispatched [publication workflow](../../.github/workflows/publish-nuget.yml), which requests an OIDC-based short-lived key. Workflow configuration describes intended execution, not evidence that a particular run or package publication succeeded.
