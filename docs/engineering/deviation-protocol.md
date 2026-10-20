# Resolve a documentation or source conflict

Date: 2026-10-20

When two current statements disagree, classify the disagreement before you implement or copy either statement. Identify the owner in [documentation ownership](documentation.md), pin the source revision and external dependency version if relevant, and classify it:

1. **Stale explanation:** Current source and the accepted decision agree, but another current document does not. Correct the owner's explanation and shorten the other location to a pointer.
2. **Implementation defect:** A current accepted contract and the code disagree. Fix the code when the task covers that behavior; otherwise report the observable difference. Do not rewrite the contract to make the defect disappear.
3. **Undecided product policy:** Sources show what happens but do not decide what should be promised (for example, service behavior on a particular emulator edition). Ask the owner for the decision before stating it as current policy.
4. **Historical evidence:** A plan, draft, or handoff describes a different branch or a past decision. Keep its date and status visible; do not promote it into the current canon without an accepted decision event.

A conflict blocks only the statement or behavior it affects. Finish the rest of the task, and end with each unresolved conflict, its evidence, and your proposed resolution.

After resolving the conflict, update the single current owner and any short relays in the same change. Keep rationale for the current decision in its evergreen ADR; retain historical context through Git history or separate dated evidence. Never silently restore a deleted agent guide, declare an unmerged branch implemented, suppress a failing check, or weaken a test to make the documents agree.
