# Documentation ownership

Date: 2026-10-18

The public repository must explain its current behavior and accepted decisions without requiring access to a private archive. Give each current fact one owner. Other documents can briefly describe the fact for their reader and link to its owner; they must not become a second place to maintain the rule.

| Question | Current owner |
| --- | --- |
| What does the core session do, and how is a service mapped? | [Client runtime](../architecture/client-runtime.md) |
| How does application configuration choose a LocalStack or ordinary AWS client? | [Extensions and composition](../architecture/extensions-and-composition.md) |
| What is shipped, on which frameworks, and what is the Native AOT boundary? | [Platform and packaging](../architecture/platform-and-packaging.md) |
| Which words distinguish the library, emulator, service identity, and client? | [Root glossary](../../GLOSSARY.md) |
| What checks and safety constraints apply to builds and tests? | [Quality gates](quality-gates.md); actual commands in the [build tasks](../../build/LocalStack.Build/CakeTasks/) and [workflows](../../.github/workflows/) |
| What happens when a current document conflicts with source or another rule? | [Deviation protocol](deviation-protocol.md) |
| Why was a hard-to-reverse, surprising trade-off accepted? | [Decision records](../adr/README.md) |
| How does a consumer install and configure the packages? | [Root README](../../README.md), also copied into both NuGet packages |
| What changed in a release? | [Changelog](../../CHANGELOG.md) |
| Which releases receive security patches, and how are vulnerabilities reported? | [Security policy](../../.github/SECURITY.md) |
| How does a contributor submit a change? | [Contribution guide](../../.github/CONTRIBUTING.md) and GitHub forms |
| Which package, SDK, framework, and CI values apply now? | `Directory.Build.props`, `Directory.Packages.props`, `global.json`, each `.csproj`, `BannedSymbols.txt`, and the relevant workflow—not a copied prose value |

## Lifecycles

- **Current architecture and engineering** describe the checked-in behavior. They change when the behavior changes. A `Date:` on these living documents is the latest substantive update, not a claim that the source was verified today by a live test.
- **ADRs** record accepted historical reasoning, including alternatives that were actually considered. Their `Date:` is the decision date. A later reversal updates current canon and deliberately supersedes or revises its ADR; Git history keeps the chain.
- **Release history** belongs to `CHANGELOG.md`. An announced future release or a tag is not a test result or proof of a package feed upload.
- **Research, temporary plans, handoffs, and ledgers** are dated evidence, not current policy. An organization-wide private archive now retains individually reviewed historical copies; its access and retention rules live there, while this public repository remains complete without it. Locally ignored `artifacts/` files remain local unless individually admitted and are not a canonical source. Classify and validate each one before considering transfer; never treat a title such as “APPROVED” as acceptance without the corresponding decision event.
- **Open work** has no dedicated tracked roadmap in this revision. GitHub issues and the relevant source are leads; do not create a roadmap by copying checkboxes from an ignored plan.

Useful information survives a documentation move: verify each unique claim, put it under its correct owner or retain it as dated evidence, then replace old current-text copies with short pointers. Shortening alone is not a reason to discard rationale. Public ADRs state their decision and reason in full; when internal evidence becomes available, cite it by date and title rather than linking private paths or numbering. `AGENTS.md` routes agents to the relevant public owner rather than repeating its contents.
