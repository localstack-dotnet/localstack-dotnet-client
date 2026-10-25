# Quality gates and safe commands

Date: 2026-10-23

The project uses xUnit tests and Cake Frosting build tasks. A command's success is evidence only for the projects, target frameworks, and dependencies it actually exercised. The [workflow YAML](../../.github/workflows/) owns CI triggers; [project files](../../tests/) and [Cake tasks](../../build/LocalStack.Build/CakeTasks/) own the runner mechanics.

| Test layer | What it checks | Infrastructure |
| --- | --- | --- |
| Core and Extensions unit tests | Configuration, session and DI behavior | No LocalStack container |
| Integration tests | AWS SDK client construction and configuration across many services | No LocalStack API calls in the broad client-creation checks |
| Functional tests | Selected S3, DynamoDB, SQS, SNS, and CloudFormation operations | Testcontainers using two LocalStack images pinned in [`TestConstants`](../../tests/LocalStack.Client.Functional.Tests/TestConstants.cs): the last release that starts without an auth token, and a current release that needs one ([image policy](maintenance-playbook.md#3-select-the-image-and-authentication-matrix)) |
| Native AOT tests | `AddLocalStack` + `AddAwsService<IAmazonS3>`, all eight registration overloads, Core generic/`Type` paths and `SessionStandalone` | [xUnit v3 AOT executable](../../tests/LocalStack.Client.NativeAot.Tests/), `net10.0`/`net11.0`; live S3/SQS via `LOCALSTACK_NATIVE_AOT_HOST` |

## Before running a task

- The Cake default target depends on `tests`, and `BuildContext.SkipFunctionalTest` defaults to `true`. The usual build/test scripts therefore **do not run the functional suite** unless explicitly opted in; do not label an unqualified run “all tests.”
- With the .NET 11 SDK, VSTest runs `net472` tests on Windows only; [Cake](../../build/LocalStack.Build/CakeTasks/TestTask.cs) warns and skips them elsewhere. CI no longer installs macOS Mono.
- Cake skips test projects that set `LocalStackTestRunsAsPublishedExecutable` (the Native AOT lane and the package consumer); `dotnet test` does not run them. Use CI's `dotnet publish` (`PublishAot`), then execute the published binary. Cake reads target frameworks through MSBuild, so the shared properties in `Directory.Build.props` apply to it too.
- Functional tests start LocalStack containers through Testcontainers, so they need a reachable Docker engine. [`TestContainers`](../../tests/LocalStack.Client.Functional.Tests/TestContainers.cs) gives each container a unique name and enables Testcontainers cleanup, so its resource reaper removes the containers a run started. Neither the Cake path nor a direct test run removes other containers.
- The current-image scenario classes carry the xUnit trait `LocalStackAuth=Required`. Their collection fixture reads `LOCALSTACK_AUTH_TOKEN` from the environment and passes it to the container. When the variable is missing or blank, the fixture throws a message that names the variable, so those tests fail; they never pass without a token. To run without a token, exclude them: `--filter "LocalStackAuth!=Required"` on `dotnet test`, or `--skipAuthenticatedFunctionalTest true` on the Cake `tests` target, which applies the same filter and prints a warning. Report excluded tests as not run.
- Set the token only in the environment of the run. Never put it in a tracked file, a command-line argument, or a log.
- `nuget-prepare-extensions` removes the Extensions project reference and adds a Core package reference to the tracked `.csproj`. Do not run it as a packaging dry run. An isolated `nuget-pack` task does not invoke that preparation automatically. The library projects' `PreBuild` targets also copy the root README and LICENSE into their package folders.

## Evidence and CI

The repository-wide [`NuGetAuditMode`](../../Directory.Build.props) setting is `all`, so restore audits transitive packages in every project and target framework. `TreatWarningsAsErrors` in that file means audit findings can fail restore. `dotnet list package --vulnerable --include-transitive` can report vulnerable packages and still exit with code 0; read the report instead of treating that exit code as a clean audit.

The [main CI workflow](../../.github/workflows/ci-cd.yml) ignores Markdown-only changes on its master push and eligible pull-request triggers. The [dependency-review workflow](../../.github/workflows/dependency-review.yml) still triggers for eligible pull requests, so “all CI skips Markdown” is false. Documentation changes need local link, source-claim, and copy-consistency checks even when the main build workflow does not run.

In the main CI workflow only the Linux leg runs functional tests. Its test step receives the Actions secret `LOCALSTACK_AUTH_TOKEN`, an organization secret shared with this repository. A pull-request run that cannot read that secret (a fork, a Dependabot run without its own copy, or any run before the secret exists) excludes the token-gated tests through the Cake flag and writes a workflow warning. A push run without the secret still runs them, so it fails at the fixture instead of passing. LocalStack expects a workspace CI Auth Token in CI, not a developer's personal token.

CI's separate `native-aot-tests` job publishes/runs both targets on `windows-latest` (`win-x64`) and `ubuntu-22.04` (`linux-x64`). Linux waits for LocalStack 4.14.0 health and sets `LOCALSTACK_NATIVE_AOT_HOST`; live cases otherwise skip. The [project](../../tests/LocalStack.Client.NativeAot.Tests/LocalStack.Client.NativeAot.Tests.csproj) and pinned xUnit v3 AOT package define the runner; this lane qualifies the [Native AOT boundary](../architecture/platform-and-packaging.md#native-aot-and-trimming), not every service/deployment. The same job then packs both libraries into `artifacts/packages` and publishes [`LocalStack.Client.PackageConsumer.Tests`](../../tests/LocalStack.Client.PackageConsumer.Tests/) from those packages for every modern target framework with a fresh NuGet cache; that project is outside the solution and isolated from the repository's build props, so it proves the generator reaches a consumer through the package's `buildTransitive` target on both operating systems.

On pushes to `master`, each `build-and-test` leg posts its counts to [BadgeSmith](https://github.com/localstack-dotnet/badge-smith) (`api.localstackfor.net`), signed with the organization's `TESTDATASECRET`; the step is `continue-on-error`, so a rejected post does not fail CI. Neither a badge nor a fixed test count in prose proves that the current revision passed. Report the actual command, target frameworks, result, and any tests deliberately not run. Do not infer NuGet publication, Native AOT support, or full LocalStack service behavior from a successful build or client-construction test.
