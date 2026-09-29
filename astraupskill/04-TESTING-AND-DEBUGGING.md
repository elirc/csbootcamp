# Observe HTTP and stored state together

Run the companion with the --self-test argument shown in the verification
chapter. The checks execute through real HTTP against a local Kestrel host.
That includes request binding, route selection, headers, JSON serialization
and the in-memory store. Unlike the focused dependency-double tests in some
larger projects, these checks do not substitute the route or store. They still
do not exercise a durable database, browser interface or authenticated session.

Each named test should answer an observable question. Does creation return
the resource location and initial tag? Does an invalid title leave collection
size unchanged? Does a replacement advance the version and retain the ID?
Does a stale writer receive rejection while the winner's state survives?
These assertions connect transport behavior with state rather than treating
an expected status code as sufficient proof.

The concurrent-editor test issues two replacement requests with the same tag
and checks that exactly one succeeds and one receives 412. It accepts either
winner, avoiding an assumption about scheduling order. The final representation
must have version 2 and one submitted title. This is one executed overlapping
request scenario, not exhaustive stress testing of every possible interleaving.
The store lock and code review supply additional reasoning about the invariant.

When a check fails, identify its layer before editing. A 400 from malformed
JSON may happen before your handler is entered. A missing ETag points to the
response construction. Two successful concurrent writes point to comparison
and mutation being separated or the expected version being ignored. A 412
with changed state points to the rejection occurring too late.

For a mutation exercise, work in a scratch copy and remove the expected-version
comparison. The stale-writer and competing-writer checks should fail. Restore
it, then move comparison outside the lock and reason about an interleaving
even if one short run happens to pass. Tests can demonstrate a failure, but
a passing schedule cannot prove that an incorrectly synchronized implementation
is safe. Explain both the observed result and the code-level reason for it.
