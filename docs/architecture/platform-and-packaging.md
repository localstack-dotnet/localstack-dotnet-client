# Platform and packaging boundary

Date: 2026-10-19

The source of package versions, target frameworks, and dependencies is the project configuration, not a prose copy of their values:

| Question | Owning file |
| --- | --- |
| Core and Extensions package versions | [`Directory.Build.props`](../../Directory.Build.props) |
| AWS SDK and other package versions; pinned versus floating canary track | [`Directory.Packages.props`](../../Directory.Packages.props) |
| Core target frameworks | [`LocalStack.Client.csproj`](../../src/LocalStack.Client/LocalStack.Client.csproj) |
| Extensions target frameworks and dependency on Core | [`LocalStack.Client.Extensions.csproj`](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj) |
| Development SDK selection | [`global.json`](../../global.json) |
| Publication triggers and credentials mechanism | [GitHub Actions workflows](../../.github/workflows/) |

For Extensions' four target frameworks (`netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`), the Microsoft.Extensions `VersionOverride` values in [LocalStack.Client.Extensions.csproj](../../src/LocalStack.Client.Extensions/LocalStack.Client.Extensions.csproj) define these existing consumer minimums:

| Package | Declared minimum | Target frameworks |
| --- | --- | --- |
| `Microsoft.Extensions.Configuration.Abstractions` | 3.1.32 | All four |
| `Microsoft.Extensions.Configuration.Binder` | 3.1.32 | All four |
| `Microsoft.Extensions.Logging.Abstractions` | 3.1.32 | All four |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | 3.1.32 | All four |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 3.1.32 | `netstandard2.0` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 8.0.0 | `net8.0`, `net9.0`, `net10.0` |

These are this package's consumer minimums, not a copy of the selected [`AWSSDK.Extensions.NETCore.Setup` 4.0.102.4 package ranges](https://www.nuget.org/packages/AWSSDK.Extensions.NETCore.Setup/4.0.102.4). Setup 4.0.102.4 permits older 2.x configuration, logging, and legacy dependency-injection packages, and a `net8.0` dependency-injection package at 8.0.0; those permissive ranges do not lower or define Extensions' own contract. Central versions in [Directory.Packages.props](../../Directory.Packages.props) govern test and sandbox consumers separately. A consumer's resolved graph can select a higher version when another dependency requires it.

At this revision Core explicitly targets `netstandard2.0`, `net472`, `net8.0`, `net9.0`, and `net10.0`; Extensions targets `netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`. The AWS SDK v4 migration changed the explicit .NET Framework target from 4.6.2 to 4.7.2. The two packages can have different versions: the [changelog](../../CHANGELOG.md) records the Extensions-only 2.0.1 release. For an installation instruction, consult the actual package feed as well as the declared project version.

## Native AOT and trimming

The current Core and Extensions implementations reflect over AWS SDK internals. Their annotations warn consumers about dynamic code and trimming, and Extensions includes a linker descriptor for the internal AWS factory. Neither an annotation nor that descriptor establishes Native AOT compatibility for an application. Native AOT support is **not** a capability of the current published v2 line; the source-generator/`UnsafeAccessor` work in [draft PR #49](https://github.com/localstack-dotnet/localstack-dotnet-client/pull/49) is not part of this `master` revision. Do not treat a proposed release number as a delivery date.

## Consumer README and release channels

The [root README](../../README.md) is both the GitHub entry point and the source copied into each package project's tracked `README.md` by its `PreBuild` target. Those copies are package content, not independent documentation owners. Edit the root first and keep both copies byte-identical; a build can copy root content into them.

The [CI workflow](../../.github/workflows/ci-cd.yml) publishes development packages to GitHub Packages after its configured matrix succeeds on a master push or eligible feature pull request. The scheduled [AWS SDK canary](../../.github/workflows/aws-sdk-canary.yml) tests newer Core and Setup versions; it does not publish packages. NuGet.org releases use the manually dispatched [publication workflow](../../.github/workflows/publish-nuget.yml), which requests an OIDC-based short-lived key. Workflow configuration describes intended execution, not evidence that a particular run or package publication succeeded.
