# Quality gates and safe commands

Date: 2026-10-19

The project uses xUnit tests and Cake Frosting build tasks. A command's success is evidence only for the projects, target frameworks, and dependencies it actually exercised. The [workflow YAML](../../.github/workflows/) owns CI triggers; [project files](../../tests/) and [Cake tasks](../../build/LocalStack.Build/CakeTasks/) own the runner mechanics.

| Test layer | What it checks | Infrastructure |
| --- | --- | --- |
| Core and Extensions unit tests | Configuration, session and DI behavior | No LocalStack container |
| Integration tests | AWS SDK client construction and configuration across many services | No LocalStack API calls in the broad client-creation checks |
| Functional tests | Selected S3, DynamoDB, SQS, SNS, and CloudFormation operations | Testcontainers using LocalStack 3.7.1 and 4.6.0 |

## Before running a task

- The Cake default target depends on `tests`, and `BuildContext.SkipFunctionalTest` defaults to `true`. The usual build/test scripts therefore **do not run the functional suite** unless explicitly opted in; do not label an unqualified run “all tests.”
- Functional tests start LocalStack containers through Testcontainers, so they need a reachable Docker engine. [`TestContainers`](../../tests/LocalStack.Client.Functional.Tests/TestContainers.cs) gives each container a unique name and enables Testcontainers cleanup, so its resource reaper removes the containers a run started. Neither the Cake path nor a direct test run removes other containers.
- `nuget-prepare-extensions` removes the Extensions project reference and adds a Core package reference to the tracked `.csproj`. Do not run it as a packaging dry run. An isolated `nuget-pack` task does not invoke that preparation automatically. The library projects' `PreBuild` targets also copy the root README and LICENSE into their package folders.

## Evidence and CI

The repository-wide [`NuGetAuditMode`](../../Directory.Build.props) setting is `all`, so restore audits transitive packages in every project and target framework. `TreatWarningsAsErrors` in that file means audit findings can fail restore. `dotnet list package --vulnerable --include-transitive` can report vulnerable packages and still exit with code 0; read the report instead of treating that exit code as a clean audit.

The [main CI workflow](../../.github/workflows/ci-cd.yml) ignores Markdown-only changes on its master push and eligible pull-request triggers. The [dependency-review workflow](../../.github/workflows/dependency-review.yml) still triggers for eligible pull requests, so “all CI skips Markdown” is false. Documentation changes need local link, source-claim, and copy-consistency checks even when the main build workflow does not run.

The badge-update step is `continue-on-error`, and its action logs a non-200 Gist response without turning that response into a failed step. Neither a badge nor a fixed test count in prose proves that the current revision passed. Report the actual command, target frameworks, result, and any tests deliberately not run. Do not infer NuGet publication, Native AOT support, or full LocalStack service behavior from a successful build or client-construction test.
