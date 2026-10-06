# Run the lab and interpret its evidence

From the repository root, execute:

```text
dotnet run --project guides/crud-web-lab --configuration Release -- --self-test
```

For manual local exploration, omit the --self-test arguments. The ordinary
host listens on loopback port 5088. Stop it when finished; its tasks live only
in memory. The self-test path chooses an available loopback port, starts the
same mapped application, sends requests with HttpClient, and shuts the host
down in a finally block. The command returns failure if any named check fails.

The project uses the installed .NET 10 shared web framework and has no external
package references. The SDK still builds the application and creates local
`bin/` and `obj/` folders under `guides/crud-web-lab/`. `CrudWebLab.csproj`
sets `TreatWarningsAsErrors`, so a warning in code you add to the lab fails
the build. This lab does not touch the bootcamp exercises, solutions or
scoreboard; run `dotnet run bootcamp/verify.cs` separately for those.

## What a passing run looks like

`HttpChecks.Run` prints one `ok - <name>` or `not ok - <name>: <reason>` line
per check, then a summary:

```text
# tests 13
# pass 13
# fail 0
# skipped 0
```

The 13 checks in `HttpChecks.cs` cover: create (trimming, Location, initial
ETag), blank title, the 120/121 title-length boundary, read with ETag,
unknown ID, PUT without If-Match (428), malformed If-Match (400), a valid
replace, a stale replace (412), an invalid replacement that must not consume
a version, two concurrent editors, conditional delete, and replacing a
missing task (404). Several cases read state after a rejection so that a
correct status code cannot hide an unwanted mutation.

When you add a check (see the exercises), the `# tests` count goes up by one
and `# fail` must stay at 0.

## What it does not prove

These are real local HTTP checks, but storage is still the in-memory
TaskStore. They do not establish database durability, multi-instance safety,
authentication, authorization, browser behavior or full HTTP
conditional-request compliance. If you replace the store with a database,
keep these behavior checks and add tests that coordinate real transactions
before claiming equivalent concurrency protection.

## Recorded run

*Status as of 2026-09-22 (frozen record):* [`evidence/results.json`](evidence/results.json)
records one self-test run of the command above: exit code 0, 13 tests
executed, all passed. The raw console log it names (`crud-b02-http-lab.log`)
was not committed to this repository, so re-run the command to see the
output yourself.
