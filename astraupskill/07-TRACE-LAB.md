# Trace competing edits and rejected deletes

Use a single task so every state transition is visible. Its initial title
is Learn CRUD, done is false and version is 1. Two clients have read that
same record. Neither client can infer that another client has edited it until
the server compares their expected version with the current one.

| Step | Client request | Version before | Store action | Result |
| --- | --- | --- | --- | --- |
| 1 | A reads task | 1 | Return snapshot | ETag for 1 |
| 2 | B reads task | 1 | Return snapshot | ETag for 1 |
| 3 | A replaces using 1 | 1 | Compare, assign, increment | Success, version 2 |
| 4 | B replaces using 1 | 2 | Comparison fails | 412, no assignment |
| 5 | B deletes using 1 | 2 | Comparison fails | 412, item retained |
| 6 | B reads again | 2 | Return current snapshot | A's saved fields |

Trace step three through Program.cs and TaskStore.Replace. Mark where title
validation happens, where the header is parsed, and where the store lock is
acquired. The response's ETag is derived from the returned immutable record,
not from a separate reread that could observe a later writer. That detail
keeps the response tied to this successful mutation.

Now pause two replacement requests immediately before entering the lock.
Choose either as the first entrant. After it advances the version, the second
entrant sees the new value and rejects its old expectation. Reverse their
arrival order and repeat. The winning title changes, but the one-winner
property remains the same. The HTTP test therefore asserts the property
without requiring a particular scheduling winner.

Finally, trace an invalid title carrying the correct tag. It returns before
the mutation operation, so it does not consume a version. Then trace a missing
If-Match header with a valid title: the header check rejects it before store
mutation. Different early returns protect different rules. Write a short
explanation of why each rejected request leaves state unchanged, pointing to
the exact branch rather than merely quoting the response code from the table.
