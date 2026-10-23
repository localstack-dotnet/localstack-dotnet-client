# Maintenance playbook

Date: 2026-10-24

This living procedure records the maintainer's preferred maintenance flow adopted on this date. Use it for a routine refresh or select the applicable steps for a focused repair. The order below is present guidance; the representative Git history at the end does not establish a universal historical sequence.

Current behavior remains owned by [client runtime](../architecture/client-runtime.md), [Extensions and composition](../architecture/extensions-and-composition.md), and [platform and packaging](../architecture/platform-and-packaging.md). Follow the [deviation protocol](deviation-protocol.md) when source, documentation, or a proposed support claim disagrees.

## 1. Refresh dependencies deliberately

- Start with the checked-in revision and a clean understanding of existing local changes. Read versions from [Directory.Packages.props](../../Directory.Packages.props), [Directory.Build.props](../../Directory.Build.props), [global.json](../../global.json), and the affected project files.
- During a full dependency refresh, update every existing NuGet package ID except **AWSSDK.Core** to its latest stable release, including central versions used by tests and sandboxes and build, test, and analyzer dependencies. Preserve intentional `VersionOverride` values and target-framework conditions that define a library's consumer minimums; a general dependency refresh does not authorize raising or removing them. Changing those minimums requires a separate maintainer decision supported by compatibility and package evidence. Enumerate any unresolved exception in the refresh record instead of claiming that every package was updated.
- For **AWSSDK.Core**, prefer a recent, widely downloaded compatible release, while comparing recent stable candidates using version-level download counts. Record the package-feed source, observation date, measurement window, and reason for the selection. The latest release is not mandatory, and there is no fixed waiting period or download threshold. Newer releases have had less time to accumulate downloads; popularity is a selection signal, not proof of compatibility or security.
- Keep three facts distinct: the declared **AWSSDK.Core package minimum**, the Core-only version baseline exercised by tests, and the exact Core versions resolved by Extensions and service-package consumers. A newer Setup or service package may require a higher resolved Core version; that alone does not prove the library's declared minimum must rise. Do not force a lower resolution against actual dependency ranges. Record the resolved graphs and the tests run for them.
- Prefer a servicing or otherwise compatible Core candidate when actual regression or security evidence contradicts popularity.
- Keep the normal pinned dependency track distinct from the floating AWS SDK canary. Inspect [aws-sdk-canary.yml](../../.github/workflows/aws-sdk-canary.yml) and the central-version conditions before interpreting its result. A canary detects upstream drift; it neither selects the release baseline nor proves publication or functional coverage.
- Record exact resolved versions for reproducibility. Framework or SDK upgrades that exceed the refresh's scope can remain separate work.

## 2. Reconcile the service catalog and its tests

Inspect the official [LocalStack Python client](https://github.com/localstack/localstack-python-client) at a recorded commit. Compare its catalog with this library and the selected AWS SDK; record relevant additions, removals, renames, and intentional differences. Do not copy an upstream name without checking its meaning.

Record the inspected Python client commit and the added services in [CHANGELOG.md](../../CHANGELOG.md).

For each applicable mapping change, review these locations together:

| Concern | Source to review |
| --- | --- |
| Catalog enum | [src/LocalStack.Client/Enums/AwsService.cs](../../src/LocalStack.Client/Enums/AwsService.cs) |
| SDK service ID, CLI name, catalog key, endpoint metadata, and `All` membership | [src/LocalStack.Client/Enums/AwsServiceEndpointMetadata.cs](../../src/LocalStack.Client/Enums/AwsServiceEndpointMetadata.cs) |
| Central AWS service-package versions | [Directory.Packages.props](../../Directory.Packages.props) |
| Integration package references and test imports | [tests/LocalStack.Client.Integration.Tests/LocalStack.Client.Integration.Tests.csproj](../../tests/LocalStack.Client.Integration.Tests/LocalStack.Client.Integration.Tests.csproj), [GlobalUsings.cs](../../tests/LocalStack.Client.Integration.Tests/GlobalUsings.cs) |
| Interface and concrete client construction | [CreateClientByInterfaceTests.cs](../../tests/LocalStack.Client.Integration.Tests/CreateClientByInterfaceTests.cs), [CreateClientByImplementationTests.cs](../../tests/LocalStack.Client.Integration.Tests/CreateClientByImplementationTests.cs) |
| Relevant emulator operations | [tests/LocalStack.Client.Functional.Tests/Scenarios/](../../tests/LocalStack.Client.Functional.Tests/Scenarios/) and its fixtures/configuration |

Reconcile spelling with the AWS SDK's actual service metadata: service IDs are exact matches and are not interchangeable with CLI names or enum names. Review compatibility effects before removing or renaming public catalog entries. Update both construction suites for applicable clients, plus relevant functional scenarios when operation behavior changes or needs evidence. Check existing gaps rather than assuming every catalog member already has complete test coverage.

Keep three evidence layers explicit: a Python mapping informs the comparison; a .NET mapping and construction test establish client configuration; a functional scenario exercises named operations on a particular emulator. None alone establishes comprehensive LocalStack service support. See [client runtime](../architecture/client-runtime.md) for the mapping contract.

## 3. Select the image and authentication matrix

The functional suite runs every scenario against exactly two LocalStack images:

1. **The last token-free release.** LocalStack's semantic-version line ended at v4.14.0 ([FAQ](https://docs.localstack.cloud/getting-started/faq)); the [4.14.0 announcement](https://blog.localstack.cloud/localstack-for-aws-release-v-4-14-0/) labels that tag Community Edition, and no 4.14.x patch exists on Docker Hub. The [2026.03.0 announcement](https://blog.localstack.cloud/localstack-for-aws-release-2026-03-0/) requires an auth token from that release on, while the vendor's [earlier-image guidance](https://blog.localstack.cloud/the-road-ahead-for-localstack/) keeps prior tags runnable without authentication, locally and in CI, with no further service or security updates. This leg is therefore frozen: it proves that the library still works for consumers who stay on a token-free image, and it stays until the maintainer drops that audience.
2. **The current release.** The newest exact calendar-versioned tag (`YYYY.MM.patch`) on [Docker Hub](https://hub.docker.com/r/localstack/localstack/tags). Move it to the newest exact tag in each maintenance cycle; never pin a moving tag such as `latest`, `stable`, or `YYYY.MM`.

The exact tags live only in [TestConstants.cs](../../tests/LocalStack.Client.Functional.Tests/TestConstants.cs); [TestContainers.cs](../../tests/LocalStack.Client.Functional.Tests/TestContainers.cs) and [LocalStackFixtures.cs](../../tests/LocalStack.Client.Functional.Tests/Fixtures/LocalStackFixtures.cs) turn them into containers, and the [container sandbox](../../tests/sandboxes/LocalStack.Container/Program.cs) pins the token-free image. Adding a third image is a maintainer decision, not a routine refresh. Fixture names, commit titles, and a green build are insufficient evidence of the image used.

Record image repository, exact tag and resolved digest where available, host OS/architecture, target frameworks, resolved SDK packages, configuration mode, services, and scenarios. Pin reproducible inputs; label any moving-image experiment separately. A tested matrix is evidence for that scope, not a new support promise.

### Authentication for the current image

- The token reaches the container only through the `LOCALSTACK_AUTH_TOKEN` environment variable, the name the [Auth Token documentation](https://docs.localstack.cloud/aws/getting-started/auth-token/) and Testcontainers use. Locally, set it in the shell for the run. In CI, the organization Actions secret of the same name feeds the test step; LocalStack requires a workspace CI Auth Token there, not a developer's personal token. Token ownership, the license it consumes, and rotation stay with the maintainer; recheck [current plan and entitlement information](https://www.localstack.cloud/pricing), because this playbook establishes no Hobby, open-source-program, or commercial eligibility for this repository.
- A missing token never passes. The current-image fixture fails with a message that names the variable; a deliberate run without a token excludes those tests by trait. [Quality gates](quality-gates.md) owns the commands and the CI behavior for pull requests that cannot read the secret. Report an excluded leg as not run, with the reason.
- The Testcontainers.LocalStack version in [Directory.Packages.props](../../Directory.Packages.props) refuses to build a container for a tag newer than 4.14 without the variable, but accepts an empty value; the fixture's own blank check covers that gap. Recheck that module's source when you update it. A present token is not proof of validity or entitlement: without a valid token the current image exits at startup with code 55 ("License activation failed").

LocalStack platform authentication activates the emulator. [AWS credentials](https://docs.localstack.cloud/aws/connecting/credentials/) identify/sign AWS-compatible API requests; keep these separate from the platform token and the client's AWS credential fields. The library itself gains no LocalStack authentication API from this test setup. Official authentication and image sources above were checked on 2026-10-23; recheck them when changing the matrix.

## 4. Validate safely and prepare release evidence

Read [quality gates](quality-gates.md) before selecting any build, test, Docker, or packaging command. That document owns task side effects and safe execution prerequisites. Inspect the relevant [Cake tasks](../../build/LocalStack.Build/CakeTasks/) and [workflows](../../.github/workflows/) for the current commands and defaults.

- Validate the changed behavior at the appropriate layers: unit behavior, both SDK client-construction paths, and selected emulator operations. Include relevant Extensions/AWS-mode regression coverage when dependency changes affect that factory, and the separate Native AOT publish/run lane when changes affect its qualified paths; [quality gates](quality-gates.md) owns execution details.
- Report actual commands, revision, frameworks, resolved dependencies, image/architecture/scenarios, outcomes, failures, and omissions. A default run that skips functional tests is not evidence that they passed. Neither badges nor fixed test counts substitute for a run record.
- Select Core and Extensions versions independently in [Directory.Build.props](../../Directory.Build.props), accounting for the actual Extensions-to-Core dependency and change scope. A focused Extensions repair need not include a catalog or image refresh.
- Extensions calls Core option members such as `ApplyConfiguration` through [`InternalsVisibleTo`](../../src/LocalStack.Client/Properties/AssemblyInfo.cs). Those internal members keep their signatures within a major version; a breaking change requires a new major of **both packages released together**. The Extensions-to-Core package dependency remains a minimum version; do not change it to a bounded range.
- Update [CHANGELOG.md](../../CHANGELOG.md) for the release's actual changes. If consumer guidance changes, edit the [root README](../../README.md) and keep [Core's README](../../src/LocalStack.Client/README.md) and [Extensions' README](../../src/LocalStack.Client.Extensions/README.md) byte-identical, following [platform and packaging](../architecture/platform-and-packaging.md).
- Check the actual [Core feed](https://www.nuget.org/packages/LocalStack.Client) and [Extensions feed](https://www.nuget.org/packages/LocalStack.Client.Extensions) before claiming a version is published or asking consumers to install it. Declared versions, tags, workflow configuration, and successful tests do not establish upload success. Preserve stable versus prerelease/development channel distinctions.

For documentation-only work, check links, claims, whitespace, and affected copies; record runtime tests as **not run**. Do not turn a documentation check into a claim of runtime validation.

## 5. Propagate and dogfood through actual consumers

The intended rollout is: a new LocalStack image feeds the [Aspire Hosting integration](https://github.com/localstack-dotnet/dotnet-aspire-for-localstack); integration/client releases then feed BadgeSmith and applicable demos, alongside other Client consumers. Discover each consumer's actual direct and transitive dependencies at maintenance time before deciding which update applies. This is the maintainer's rollout direction, not an audited description of today's cross-repository dependency graph.

Use current compatible published packages in the intended channel. Update consumers that use Client directly as well as those reached through the integration; do not introduce an Aspire dependency into a consumer that has none. Pin package/image inputs for repeatable runs and identify canary experiments separately. Record the consumer revision, actual dependency path, exercised scenario, result, and any consumer or scenario not checked. Feed regressions back to the owning repository with their reproducible inputs.

## Per-cycle checklist and evidence record

Keep a short record in the change or release discussion; links to existing run evidence are enough:

- [ ] Base/change revision, dependency candidates and selected versions, Core selection rationale, Python-client revision and catalog differences.
- [ ] Applicable enum/metadata/`All`/package-reference changes, both construction suites, functional scenarios, and known coverage gaps.
- [ ] Chosen image matrix and auth/entitlement decision where applicable; exact tested scope and any omitted legs.
- [ ] Commands and outcomes at each test layer; safe-command prerequisites checked under quality gates.
- [ ] Independent package versions, changelog, README consistency if edited, and actual feed/channel verification.
- [ ] Actual consumer dependencies discovered; rollout results and untested consumers recorded.

## Representative history and remaining choices

These are sampled accepted commits, not an exhaustive audit or proof of local edit/test/release order, download-based motives, runtime success, or package publication:

| Example | What the patch illustrates |
| --- | --- |
| [a7c796bacf3d4f051cbe692860c2739796d97594](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/a7c796bacf3d4f051cbe692860c2739796d97594) | Explicit Python-client provenance with catalog and construction-test updates. |
| [53d6cb6a7706b4558dc55d086bf815227f10ffd5](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/53d6cb6a7706b4558dc55d086bf815227f10ffd5) | Pipes mapping, central/project package references, and both construction paths; no Pipes API operation is tested by those cases. |
| [31f95c1b288e92bb8d42739c9d99e1a511c7fd49](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/31f95c1b288e92bb8d42739c9d99e1a511c7fd49) | Catalog, dependencies, image fixtures, and release metadata bundled together; inspect the patch rather than deriving completeness from its title. |
| [05ffff1795f92171b744bcea63fd0b3d2de14848](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/05ffff1795f92171b744bcea63fd0b3d2de14848) | A 3.8.0 message/fixture label accompanies a 3.7.1 image constant: owning inputs matter. |
| [f539bf439cfccbff6992643c13d042da085a54b5](https://github.com/localstack-dotnet/localstack-dotnet-client/commit/f539bf439cfccbff6992643c13d042da085a54b5) | Extensions-only repair release preparation with Core's version unchanged. |

The 3.0.0 implementation includes Native AOT support on `net10.0`/`net11.0`, qualified by the separate native test lane. Consult [platform and packaging](../architecture/platform-and-packaging.md) and its linked projects for the exact boundary, legacy JIT assets, and SDK pin; a target list or successful build does not establish native execution or package publication.
