# LocalStack .NET Client — agent guide

This public repository owns its current behavior, terminology, and accepted decisions. Read the document for the task; avoid loading historical plans or a full tool inventory for every change.

## Always

- Check Git status before editing. Preserve changes and deletions you did not make.
- Use the checked-in source, projects, workflows, and their pinned versions for implementation facts. Treat locally ignored `artifacts/` files and unmerged branches as dated leads, not current policy.
- Keep the [root README](README.md) and the two tracked package README copies consistent; both packages include that text. Check the actual package feed before claiming a declared version is published.
- Before using Cake functional tests, read [quality gates](docs/engineering/quality-gates.md): their cleanup is not limited to this project's Docker containers. Cake builds on Unix and Extensions package preparation also change state outside the expected test outputs.
- If sources conflict or a public support claim lacks evidence, follow the [deviation protocol](docs/engineering/deviation-protocol.md). Preserve the uncertainty rather than selecting a convenient answer.

## Task routing

| Task | Read first |
| --- | --- |
| Core session, AWS client creation, endpoint catalog, or service-support wording | [Client runtime](docs/architecture/client-runtime.md) and [glossary](GLOSSARY.md) |
| `AddLocalStack`, DI, AWS-mode factory, or Extensions | [Extensions and composition](docs/architecture/extensions-and-composition.md) |
| Target frameworks, package versions, packaging, or Native AOT claims | [Platform and packaging](docs/architecture/platform-and-packaging.md); verify values in the linked project/configuration files |
| Tests, build scripts, Docker, CI, or release-task safety | [Quality gates](docs/engineering/quality-gates.md) and the relevant task/workflow source |
| Document placement, dates, relays, or historical evidence | [Documentation ownership](docs/engineering/documentation.md) |
| An accepted, hard-to-reverse design choice | [ADR eligibility and records](docs/adr/README.md) |
| Consumer setup and package installation | [README](README.md) |
| Contributions and PRs | [Contribution guide](.github/CONTRIBUTING.md) |
| Release changes | [Changelog](CHANGELOG.md) |
| Security disclosure or support window | [Security policy](.github/SECURITY.md) |

Current architecture documents describe `master`; a draft source-generator branch does not establish Native AOT support here. The private research archive, if created, cannot be required to understand this public canon.
