# Work through a lost-update repair

To see why `TaskStore.Replace` is written the way it is, first imagine a
naive Replace implementation that looks up an item and assigns the submitted title and done flag unconditionally.

Create a task titled Learn CRUD. Its first representation carries version 1,
and the response exposes that version in an ETag. Give the same representation
to clients A and B. A submits title Study HTTP with the original tag. Inside
the store lock, the current version equals the expected version, so the store
constructs a new immutable record with version 2 and returns it. The route
returns that record and its new tag.

B then submits its old tag with title Study SQL. The store finds version 2,
compares it to B's expected version 1, and returns Conflict without assigning
anything. The route translates that outcome into 412. A subsequent GET still
returns Study HTTP at version 2. That final read matters: status alone would
not expose an implementation that writes the losing request before rejecting it.

The same version rule protects deletion. A stale delete must not remove a
task that another client has changed. A current delete returns no content;
a later read confirms the resource is gone. The lab does not automatically
retry a rejected write because a retry with the new version could overwrite
changes the user has not reviewed.

Compare this path with an invalid title. Validation returns before entering
the write operation, leaving both fields and version unchanged. Then compare
an unknown identifier with a well-formed tag: it reaches the store and produces
Missing. These neighboring paths teach why one generic failure branch is less
useful than explicit outcomes. Keep the data transition, HTTP response and
client recovery decision separate in your explanation of the example.
