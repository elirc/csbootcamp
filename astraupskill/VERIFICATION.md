# Run the lab and interpret its evidence

From the project root, execute:

```text
dotnet run --project guides/crud-web-lab --configuration Release -- --self-test
```

For manual local exploration, omit the --self-test arguments. The ordinary
host listens on loopback port 5088. Stop it when finished; its tasks live only
in memory. The self-test path chooses an available loopback port, starts the
same mapped application, sends requests with HttpClient, and shuts the host
down in a finally block. The command returns failure if any named check fails.

The project uses the installed .NET 10 shared web framework and has no external
package references. This does not mean compilation is unnecessary: the SDK
still builds the application and may create local bin and obj folders. Those
generated outputs are excluded from the reviewed source delivery. The original
bootcamp exercise files, solutions and scoreboard are not rewritten by this
companion lab.

Captured evidence is attached after the final run. It is the authority for
the executed case count, command and exit status. The checks cover task
creation and title validation, response locations and version tags, reads,
conditional replacement, stale-write preservation, conditional deletion and
one overlapping two-writer scenario. Several cases also read state after
rejection so that a correct status cannot hide an unwanted mutation.

These are actual local HTTP checks, but their storage is still the in-memory
TaskStore. They do not establish database durability, multi-instance safety,
authentication, authorization, browser behavior or complete HTTP conditional
request compliance. No deployment is performed. If you replace the store with
a database, keep these behavior checks and add tests that coordinate real
transactions before claiming equivalent concurrency protection.

Review includes source inspection, course-link checks, baseline comparison
and delivery hash verification. Compilation with warnings treated as errors
helps catch mistakes in this new lab; it does not validate all pre-existing
bootcamp exercises. Treat each evidence statement at its stated scope. A
useful engineering report says what executed, what changed and what remains
unproven, allowing the next learner to reproduce the work without guessing.

## Astra execution evidence

The captured focused production-source suite executed 13 tests: all passed, zero failed and zero skipped. Read the [captured output](evidence/crud-b02-http-lab.log) and [exact command and limits](evidence/results.json). Earlier check records remain preserved in the workspace.
