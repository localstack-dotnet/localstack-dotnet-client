# Resolve a documentation or source conflict

Date: 2026-10-17

When two current statements disagree, **stop before implementing or copying either statement**. Identify the owner in [documentation ownership](documentation.md), pin the source revision and external dependency version if relevant, and classify the disagreement:

1. **Stale explanation:** Current source and a dated decision agree, but another current document does not. Correct the owner's explanation and shorten the other location to a pointer.
2. **Implementation defect:** A current accepted contract and the code disagree. Report the observable difference; do not rewrite the contract to make the defect disappear.
3. **Undecided product policy:** Sources show what happens but do not decide what should be promised (for example, v1 support after its announced window or service behavior on a particular emulator edition). Ask the owner for the decision before stating it as current policy.
4. **Historical evidence:** A plan, draft, or handoff describes a different branch or a past decision. Keep its date and status visible; do not promote it into the current canon without an accepted decision event.

After resolving the conflict, update the single current owner and any short relays in the same change. Keep unique, useful historical rationale in its decision record or dated evidence. Never silently restore a deleted agent guide, declare an unmerged branch implemented, suppress a failing check, or weaken a test to make the documents agree.
