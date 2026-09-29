# C# Bootcamp — offline, test-graded, self-paced

A personal coding bootcamp for levelling up C# and .NET fundamentals. Built
to run **fully offline** — every exercise is a single self-contained file
graded by its own tests, runnable with nothing but the .NET SDK.

**Start here → [`FLIGHTPLAN.md`](FLIGHTPLAN.md)** — the ten-hour schedule.

## Quick start (30 seconds)

```bash
dotnet run bootcamp/01-language-core/exercises/01-count-vowels.cs   # any exercise
dotnet run bootcamp/progress.cs                                     # your scoreboard
```

An exercise file is a prompt in comments, a stub that throws, and tests
underneath. You write the code and re-run the file until:

```
  ✔ counts lowercase vowels
  ✔ is case-insensitive
  ✔ handles the empty string

  6 passed
  all green — next file!
```

No test runner to install, no `.csproj`, no NuGet restore, no watcher —
`dotnet run <file>.cs`, that's the whole workflow. (`☐ todo` = not written
yet · `✘` = failing · `✔` = done · `🐛` = a hunt-module bug you haven't
caught yet.)

### How it works with no project file

.NET 10 runs a **file-based app**: a `.cs` file with top-level statements is
a program. The only thing an exercise needs from outside is the test
harness, and it asks for that with a single directive at the top:

```csharp
#:project ../../_lib/Check/Check.csproj
```

That is the entire build system. Nothing is downloaded, ever — the harness
is local and depends on nothing but the BCL. First compile of a file takes a
few seconds; after that a run is under a second.

## What's here

```
FLIGHTPLAN.md        ← the ten-hour plan. Read it first.
bootcamp/            ← the main event: 4 modules, 31 exercises, 506 tests
  01-language-core/           value vs reference, numbers that lie,
                              nullable, string comparison, patterns,
                              tuples, out/ref, boxing, spans
  02-methods-and-delegates/   Func/Action, closures and capture,
                              extension methods, composition, memoize,
                              events, currying
  03-collections-and-linq/    containers, dictionary reads, filter/
                              project/flatten, grouping, joins,
                              ordering, folds, deferred execution
  04-strings-and-time/        building strings, culture-aware
                              formatting, anchored regex, DateOnly,
                              an injected clock
  _lib/Check/                 the test harness (zero dependencies)
  _lib/FORMAT.md              the authoring contract
  verify.cs                   maintainer check: every file, in parallel
  progress.cs                 the scoreboard
cheatsheets/         ← offline reference: the language, and LINQ
quizzes/             ← rapid-fire recall per module, answers hidden
docs/                ← OVERVIEW.md (the map) · ROADMAP.md (what's next)
```

Each module: `README.md` (the lesson — read first), `exercises/` (yours),
`solutions/` (reference implementations with teaching walkthroughs — peek
only after a real attempt).

## Tools

| command | what it does |
| --- | --- |
| `dotnet run bootcamp/progress.cs` | scoreboard for the whole track |
| `dotnet run bootcamp/progress.cs 03` | scoreboard for one module |
| `dotnet run bootcamp/verify.cs` | maintainer check: all solutions green, all exercises clean TODOs |
| `dotnet run bootcamp/verify.cs 03` | the same, one module |

Both run files in parallel across your cores. A full cold `verify.cs`
compiles every file and takes around 15 minutes; per-module runs after that
are a minute or two.

## Rules of engagement

- Stuck >10 min → read the solution's `Walkthrough:`, close it, rewrite from
  memory. Copy-pasting teaches nothing; reconstructing teaches plenty.
- ★★★ exercises are optional stretch. Skipping them is a strategy, not a
  failure.
- Read the failing test first, then the diff, then the code. In that order.
- The quizzes are for saying answers **out loud** — recall beats
  recognition.

## Status

Four modules are complete and verified: **62 files, 506 tests, 0 broken.**
Every solution passes and every exercise ships as clean TODOs — `verify.cs`
enforces both halves, so a module that passes it is a module that follows
the format.

[`docs/ROADMAP.md`](docs/ROADMAP.md) lists what is planned next and in what
order — types and OOP, errors, async and tasks, generics and iterators, the
.NET platform, testing, patterns and DI, data structures and algorithms, and
then the debug/security hunt modules where the code ships finished and
broken and your job is to find the planted bug. The tooling already
implements the ships-red rule for those, so they need content, not
plumbing.

To extend it yourself, [`bootcamp/_lib/FORMAT.md`](bootcamp/_lib/FORMAT.md)
is the contract.
