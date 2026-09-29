# A version is part of the write contract

Suppose two clients read task 7 at version 1. Client A changes the title and
saves. Client B, still holding the original representation, changes the done
flag and sends a full replacement. An unconditional write would erase A's
title change. This lab requires each writer to include the version it read,
so the server can distinguish a current edit from an outdated one.

The API exposes the version as a quoted numeric ETag. A client sends that
value in If-Match on PUT or DELETE. In this deliberately narrow protocol,
missing preconditions return 428, unsupported or malformed tag syntax returns
400, and a valid but outdated version returns 412. Missing resource IDs have
their own not-found path. Learn these as this lab's explicit contract rather
than assuming it implements every conditional-request combination defined by
HTTP. Wildcards, weak tags and lists are intentionally unsupported here.

The crucial operation is compare-and-change. Reading a version, releasing the
lock, and later taking another lock to assign the item still permits two
writers to pass the comparison. TaskStore.Replace holds one lock while it
finds the current item, compares expected and actual versions, constructs the
replacement and stores it. Exactly one of two writers carrying the same
current version can advance that item in this process.

Validation is a different concern. A blank or oversized title is rejected
before the store mutates anything. A valid title does not prove a version is
current, and a current version does not make a blank title acceptable. Keep
those decisions separate so tests can isolate them. Trimming occurs before
the title length contract is evaluated and before storage.

Finally, a process-local lock is not a distributed database concurrency
mechanism. Two application instances would have separate locks and separate
dictionaries. Durable implementation requires the database operation itself
to express the expected version, plus an outcome for no matching row. This
lab supplies the observable behavior you can later preserve when replacing
the in-memory adapter with a real repository.
