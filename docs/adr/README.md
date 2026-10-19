# Architecture Decision Records

Date: 2026-10-19

Each record in this directory describes one **accepted** decision: its context, the alternative that mattered, the chosen direction, and why. Name records `NNNN-slug.md` in sequence. The directory listing is the index; do not create a second list to maintain.

Current rules live in [architecture](../architecture/) and [engineering](../engineering/). An ADR explains their historical rationale, not an additional current implementation or support policy. If a record and current source appear to disagree, check the decision's date and classify the discrepancy with the [deviation protocol](../engineering/deviation-protocol.md) rather than treating the historical record as newer authority.

## When to write an ADR

All three must hold:

1. **Hard to reverse** — changing course later carries real cost.
2. **Surprising without context** — a future reader would wonder "why did they do it this way?"
3. **A real trade-off** — genuine alternatives existed and one was chosen for specific reasons.

If any leg is missing, skip the ADR: current conventions and process rules belong in the relevant `docs/engineering/` home, current architecture in `docs/architecture/`, and dated findings outside current canon. A merged change alone does not supply a reason or prove that an unmerged alternative was rejected.

## Format

```markdown
# Short title of the decision

Date: YYYY-MM-DD

One to three sentences: context, decision, and why.
```

Add considered alternatives, consequences, or supersession only when they carry information a future reader cannot infer from that paragraph. Use the date of the accepted decision, not the date on which someone wrote its retrospective record. A public ADR must explain its decision without access to private research; cite first-party public evidence directly. If dated internal evidence is useful, identify it by date and title rather than a private file path or identifier.
