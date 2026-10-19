# Maintenance playbook

Date: 2026-10-19

This living procedure records the maintainer's preferred maintenance flow adopted on this date. Use it for a routine refresh or select the applicable steps for a focused repair. The order below is present guidance; the representative Git history at the end does not establish a universal historical sequence.

Current behavior remains owned by [client runtime](../architecture/client-runtime.md), [Extensions and composition](../architecture/extensions-and-composition.md), and [platform and packaging](../architecture/platform-and-packaging.md). Follow the [deviation protocol](deviation-protocol.md) when source, documentation, or a proposed support claim disagrees.

## 1. Refresh dependencies deliberately

- Start with the checked-in revision and a clean understanding of existing local changes. Read versions from [Directory.Packages.props](../../Directory.Packages.props), [Directory.Build.props](../../Directory.Build.props), [global.json](../../global.json), and the affected project files.
- Aim for current compatible SDK and library releases. For **AWSSDK.Core**, choose a recent, widely downloaded compatible release rather than automatically taking the highest version. Consult the [package feed](https://www.nuget.org/packages/AWSSDK.Core), record the candidate and selection reason, and check compatibility with Setup and service packages. Popularity is a selection signal, not compatibility proof; no numeric download threshold or mandatory waiting period is prescribed.
- Keep the normal pinned dependency track distinct from the floating AWS SDK canary. Inspect [aws-sdk-canary.yml](../../.github/workflows/aws-sdk-canary.yml) and the central-version conditions before interpreting its result. A canary detects upstream drift; it neither selects the release baseline nor proves publication or functional coverage.
- Record exact resolved versions for reproducibility. Framework or SDK upgrades that exceed the refresh's scope can remain separate work.

## 2. Reconcile the service catalog and its tests

Inspect the official [LocalStack Python client](https://github.com/localstack/localstack-python-client) at a recorded commit. Compare its catalog with this library and the selected AWS SDK; record relevant additions, removals, renames, and intentional differences. Do not copy an upstream name without checking its meaning.

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

Choose image versions deliberately for this cycle: identify the regression baseline, the newer release under evaluation, and why each remains useful. Derive the actual inputs from [TestConstants.cs](../../tests/LocalStack.Client.Functional.Tests/TestConstants.cs), [TestContainers.cs](../../tests/LocalStack.Client.Functional.Tests/TestContainers.cs), [LocalStackFixtures.cs](../../tests/LocalStack.Client.Functional.Tests/Fixtures/LocalStackFixtures.cs), and the [container sandbox](../../tests/sandboxes/LocalStack.Container/Program.cs). Fixture names, commit titles, and a green build are insufficient evidence of the image used.

Record image repository, exact tag and resolved digest where available, host OS/architecture, target frameworks, resolved SDK packages, configuration mode, services, and scenarios. Pin reproducible inputs; label any moving-image experiment separately. A tested matrix is evidence for that scope, not a new support promise.

### Authentication decision for newer images

The official [March 23, 2026 release announcement](https://blog.localstack.cloud/localstack-for-aws-release-2026-03-0/) places the startup auth-token requirement at **LocalStack 2026.03.0** and makes service availability depend on token entitlements. The vendor's [earlier-image guidance](https://blog.localstack.cloud/the-road-ahead-for-localstack/) retains unauthenticated local and CI use for prior Community tags, without future service/security updates. Do not apply the newer-image requirement retroactively to every historical tag or treat older-tag documentation as proof that an image starts on today's host.

Before introducing an authenticated matrix leg, resolve and record:

- The exact image/architecture/scenario scope and the actual account or workspace entitlement needed. Recheck [current plan and entitlement information](https://www.localstack.cloud/pricing); this playbook establishes no Hobby, open-source-program, or commercial eligibility for this repository.
- The appropriate local developer identity and CI workspace/token ownership, license assignment, provisioning/rotation, and runtime injection. Consult the current [Auth Token documentation](https://docs.localstack.cloud/aws/getting-started/auth-token/); a token's presence does not prove validity or entitlement.
- Which checks run without secrets on fork pull requests, which trusted revisions/events can run authenticated checks, and what happens when authentication is unavailable. Report an omitted matrix leg as not run, with the reason.
- The behavior of the selected Testcontainers version, including its image-version parsing and auth validation. Inspect the module source for that dependency version; a builder guard is not a runtime or entitlement test.

LocalStack platform authentication activates the emulator. [AWS credentials](https://docs.localstack.cloud/aws/connecting/credentials/) identify/sign AWS-compatible API requests; keep these separate from the platform token and the client's AWS credential fields. This checkpoint does not choose or implement a CI profile, credential injection mechanism, or new client authentication API. Those remain future implementation choices. Official authentication sources above were checked on 2026-10-19; recheck them when changing the matrix.

## 4. Validate safely and prepare release evidence

Read [quality gates](quality-gates.md) before selecting any build, test, Docker, or packaging command. That document owns task side effects and safe execution prerequisites. Inspect the relevant [Cake tasks](../../build/LocalStack.Build/CakeTasks/) and [workflows](../../.github/workflows/) for the current commands and defaults.

- Validate the changed behavior at the appropriate layers: unit behavior, both SDK client-construction paths, and selected emulator operations. Include relevant Extensions/AWS-mode regression coverage when dependency changes affect that factory.
- Report actual commands, revision, frameworks, resolved dependencies, image/architecture/scenarios, outcomes, failures, and omissions. A default run that skips functional tests is not evidence that they passed. Neither badges nor fixed test counts substitute for a run record.
- Select Core and Extensions versions independently in [Directory.Build.props](../../Directory.Build.props), accounting for the actual Extensions-to-Core dependency and change scope. A focused Extensions repair need not include a catalog or image refresh.
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

Native AOT friendliness, .NET 10/framework modernization, and authenticated CI design remain separate candidate priorities for discussion. They are not delivered capabilities or mandatory additions to every routine release. Core already has an explicit .NET Framework target; consult [platform and packaging](../architecture/platform-and-packaging.md) and its linked projects for the actual target set and Native AOT boundary.
