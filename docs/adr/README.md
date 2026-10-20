# Architecture Decision Records

Each record in this directory is an **evergreen document** describing one current accepted decision: its context, the alternative that matters, the chosen direction, and why. Name distinct decisions `NNNN-slug.md` in sequence. Keep an existing decision's filename stable when its content changes. The directory listing is the index; do not create a second list to maintain.

When the status quo changes, rewrite the existing ADR in place to reflect the current decision and its rationale. Do not add dates, dated entries, amendments, supersession notices, or a replacement ADR for the same decision. Git owns the change history; release changes belong in the changelog.

Current implementation and support details live in [architecture](../architecture/) and [engineering](../engineering/). An ADR explains the current choice and trade-off without duplicating those details. If a record and current source disagree, resolve the conflict with the [deviation protocol](../engineering/deviation-protocol.md) and update the affected documents together.

## When to write an ADR

All three must hold:

1. **Hard to reverse** — changing course later carries real cost.
2. **Surprising without context** — a future reader would wonder "why did they do it this way?"
3. **A real trade-off** — genuine alternatives existed and one was chosen for specific reasons.

If any leg is missing, skip the ADR: current conventions and process rules belong in the relevant `docs/engineering/` home, current architecture in `docs/architecture/`, and dated findings outside current canon. A merged change alone does not supply a reason or prove that an unmerged alternative was rejected.

## Format

```markdown
# Short title of the decision

One to three sentences: context, decision, and why.
```

Add alternatives or consequences only when they explain the current decision beyond that paragraph. Keep useful rationale, but leave incident chronology and obsolete decisions to Git history or separate research. A public ADR must explain its decision without access to private research; cite first-party public evidence directly.
