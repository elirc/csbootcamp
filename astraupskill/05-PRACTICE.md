# Exercises: predict, implement, review

Do these in order. Exercises 1, 2 and 4 run code; 3, 5 and 6 are paper
designs you check against [06-SOLUTIONS-AND-REVIEW.md](06-SOLUTIONS-AND-REVIEW.md),
which answers them in the same order. For any exercise that edits code, work
in a scratch branch (`git switch -c lab-scratch`) so `git restore .` gets you
back to the reference lab.

## 1. Predict a request sequence, then run it

**Goal:** predict status, title, done flag and version after every step of:
create a task, read it twice (readers A and B), replace it using A's tag,
delete it using B's (now old) tag, read it again. Write the prediction down
before running anything, and mark which requests change state.

**Hints:** the trace table in [07-TRACE-LAB.md](07-TRACE-LAB.md) is the same
shape. A read never changes the version.

**Check:** start the lab with `dotnet run --project guides/crud-web-lab`, then
in a second Git Bash window:

```bash
curl -i -X POST http://127.0.0.1:5088/tasks -H 'Content-Type: application/json' -d '{"title":"Learn CRUD","isDone":false}'
curl -i http://127.0.0.1:5088/tasks/1
curl -i -X PUT http://127.0.0.1:5088/tasks/1 -H 'If-Match: "1"' -H 'Content-Type: application/json' -d '{"title":"Study HTTP","isDone":false}'
curl -i -X DELETE http://127.0.0.1:5088/tasks/1 -H 'If-Match: "1"'
curl -i http://127.0.0.1:5088/tasks/1
```

(A second GET would be identical to the first, so it is left out.) You should
see `201` with `ETag: "1"`, `200`, `200` with `ETag: "2"`, `412`,
and a final `200` whose body has `"title":"Study HTTP"` and `"version":2`.
Every mismatch with your prediction is the thing to study.

## 2. Add a description field to PUT

**Goal:** add `Description` to `TaskInput` and `TaskItem`, and decide what PUT
does when the client omits it. PUT in this lab is a full replacement — do not
quietly turn it into a partial update.

**Hints:** write the contract (one sentence) before the code. The records live
in `TaskStore.cs`; `Create` and `Replace` both construct `TaskItem`.
`HttpChecks.cs` compares whole records with `==`, so existing checks will
tell you if you forgot a field.

**Check:** add one success check and one stale-write check that both inspect
title *and* description. The self-test prints `# tests 15` and `# fail 0`.

## 3. Review a two-lock concurrency proposal (paper)

**Goal:** a teammate proposes: take the lock, read the current version,
release; compare outside the lock; take the lock again to save. Draw two
writers holding the same tag who both pass the comparison before either save.
Name the smallest region in `TaskStore.Replace` that must stay indivisible.

**Check:** your diagram ends with the second save overwriting the first, and
your named region matches the third paragraph of 06.

## 4. Add a malformed-JSON check and a null-title check

**Goal:** add two checks to `HttpChecks.cs`: one POSTs the body `{not json`,
one POSTs `{"title":null,"isDone":false}`. Each must assert a `400` *and*
that the collection length is unchanged, like the existing "blank title"
check does.

**Hints:** send raw text with
`new StringContent("{not json", System.Text.Encoding.UTF8, "application/json")`.
The malformed body is rejected by the framework's JSON binding before your
handler runs; the null title reaches `Valid` in `Program.cs`. Note which layer
answered each one.

**Check:** the self-test prints both new names as `ok - ...`, `# tests 15`
(or 17 if you kept exercise 2's checks) and `# fail 0`.

## 5. Design the database version (paper)

**Goal:** sketch a `tasks` table with id, title, done flag and version, and
write the conditional `UPDATE` for a replace. Decide how the API tells "no
such task" apart from "stale version" when the update touches zero rows.
Describe a two-client integration test that checks final stored state.

**Check:** both `id` and `version` appear in your `WHERE` clause, you state
what zero affected rows means, and your answer covers the points in the fifth
paragraph of 06.

## 6. Design the client's response to 412 (paper)

**Goal:** propose what a UI does when a save returns 412.

**Hints:** consider showing the current server record beside the unsaved
draft. Ask what happens if the client just re-sends with the newest tag.

**Check:** your design keeps the user's draft and never auto-retries the
full replacement; compare with the last paragraph of 06.
