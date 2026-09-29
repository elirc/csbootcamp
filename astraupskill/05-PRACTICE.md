# Exercises: predict, implement, review

First, predict a request sequence on paper. Create a task, read it twice,
replace it using the first reader's tag, attempt deletion using the second
reader's old tag, then read it again. Record status, title, done flag and
version after every step. Make your prediction before running any commands.
Explain which request changes state and which merely observes or rejects it.

Second, extend a scratch copy with a description field. Decide whether PUT
requires a full replacement and how omission should behave. Add success and
stale-write checks that inspect both title and description. Do not silently
turn PUT into partial update just because optional fields seem convenient.
Write the contract first, then compare the implementation against it.

Third, review a teammate's concurrency proposal: read the current version
under a lock, release it, compare outside the lock, and take a second lock
to save. Draw two writers passing the comparison before either assignment.
Explain why thread-safe dictionary access alone does not protect the larger
business operation. Identify the smallest region that must remain indivisible.

Fourth, add a malformed JSON HTTP check and a null-title check. Verify both
the response and absence of mutation. Distinguish framework binding rejection
from application title validation. Keep these as new observations rather than
assuming that the existing title-boundary test covers every malformed request.

Fifth, design the database version. Sketch a table with identifier, title,
done flag and version. Write a conditional update that changes only the row
whose identifier and version match. Describe how to distinguish no matching
resource from a stale version if the API needs both outcomes. Include a
two-client integration test that checks final stored state, not only responses.

For a final review exercise, propose a user interface response to 412. Consider
showing the current server record beside the unsaved draft. Explain why blindly
retrying with the latest version can defeat the very protection the API offers.
Score your answer on a clear invariant, explicit request semantics and useful
recovery behavior rather than on the number of frameworks added.
