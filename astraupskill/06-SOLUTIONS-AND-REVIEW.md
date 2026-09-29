# Worked answers and review rubric

The sequence begins with a created task at version 1. Both readers receive
the same representation and tag. The first conditional replacement succeeds
and returns version 2. The second reader's delete still carries version 1,
so it is rejected and leaves the updated task intact. The final GET returns
the first writer's fields at version 2. A new read does not itself increment
the version because it does not change the resource.

For the description extension, choose an explicit replacement contract. If
PUT describes the whole editable resource, omission can mean the declared
default or be rejected, but it should not accidentally inherit arbitrary
old values. A partial-update endpoint needs its own omitted-versus-supplied
rules. Whichever contract you choose, the stale request must change neither
field nor version. The fixture should inspect the complete resulting record.

The two-lock proposal is incorrect because both writers can read version 1
and pass their comparisons before either saves. Serializing only assignments
still lets the second assignment erase the first. In this lab, lookup,
comparison, construction and assignment share one protected region. In a
database implementation, the expected version belongs in the mutation predicate
rather than only in an earlier SELECT.

A malformed JSON check should send invalid JSON through HttpClient and inspect
the framework response, then confirm collection state is unchanged. A null
title is a valid JSON shape but fails the lab's title rule. These exercise
different boundaries. Do not infer real HTTP behavior from directly calling
TaskStore.Create, which expects already validated input in this design.

A database sketch can update title, done and version where id equals the
requested ID and version equals the expected value. One affected row means
the conditional mutation succeeded. Zero rows requires the chosen missing
versus conflict policy; any follow-up read has its own timing limitations.
Use actual database concurrency tests before claiming equivalence to the
single-process lock.

For the UI, preserve the user's unsaved draft and fetch the current record.
Let the user review differences before submitting a new intent. Automatically
replaying a full replacement with a fresh tag simply recreates the lost-update
problem under a different sequence of requests. Good recovery design keeps
the user's decision visible rather than hiding a conflict behind retries.
