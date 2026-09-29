# Overview — what this is and how to move through it

A personal, offline, test-graded C# bootcamp. Everything runs with the .NET
SDK and nothing else. Companion to [`ROADMAP.md`](ROADMAP.md), which says
what is built and what is coming.

## The shelves

| shelf | shape | reach for it when |
| --- | --- | --- |
| `bootcamp/` | modules of graded exercises | you are about to write code |
| `cheatsheets/` | lookup tables, three-line examples | you need an answer in ten seconds |
| `quizzes/` | rapid-fire recall, answers hidden | you want to know what you actually retained |
| `docs/` | this file, the roadmap | you want the map |
| `FLIGHTPLAN.md` | a ten-hour schedule | you have a flight and a laptop |

## How an exercise works

One file. A prompt in comments, a stub that throws, tests underneath.

```
dotnet run bootcamp/01-language-core/exercises/01-count-vowels.cs
```

```
  ✔ counts lowercase vowels
  ✔ is case-insensitive
  ☐ handles the empty string todo

  2 passed · 1 todo
```

Write the code, run it again, repeat until `all green — next file!`. The
matching file in `solutions/` has the same tests, a working implementation,
and a `Walkthrough:` block that explains *why* — read it after a real
attempt, not during.

Nothing is installed and nothing is downloaded. .NET 10 runs a single `.cs`
file as a program, and the only outside reference is the local test harness
in `bootcamp/_lib/Check`. That is the entire build system.

## The tools

| command | what it does |
| --- | --- |
| `dotnet run bootcamp/progress.cs` | scoreboard for the whole track |
| `dotnet run bootcamp/progress.cs 03` | scoreboard for one module |
| `dotnet run bootcamp/verify.cs` | maintainer check: every solution green, every exercise a clean TODO |
| `dotnet run bootcamp/verify.cs 03` | the same, for one module |

Marks: `☐` not started · `◐` partly written · `✘` failing · `✔` done ·
`🐛` a hunt-module bug you have not caught yet.

Both tools run files in parallel. The first pass compiles everything and is
slow; later passes hit the build cache.

## The recommended path

**Phase 1 — the language (modules 01–02).** What a copy copies, where the
numeric types differ, how `null` is tracked, and how behaviour becomes a
value. The two stretch files in 01 (boxing, spans) are the ones that
separate "writes C#" from "knows what the runtime does".

*Exit test:* you can explain boxing, closure capture, and why `==` on
`object` is the wrong operator — out loud, without notes.

**Phase 2 — the daily work (modules 03–04).** LINQ is most of a working
day, and the module ends with deferred execution, which is the concept that
changes how you write signatures. Module 04 is about the two hidden inputs
that break code on somebody else's machine: the current culture and the
current time.

*Exit test:* given a query you can say when it runs, how many times it walks
its source, and what it throws when empty. And you take a clock as a
parameter without being reminded.

**Phase 3 — what comes next.** See [`ROADMAP.md`](ROADMAP.md): types and
OOP, errors, async, generics and iterators, then the hunt modules where the
code ships broken and you find the planted bug.

## The rules that make it work

- **Reconstruct, do not copy.** Stuck for ten minutes → read the
  walkthrough, close it, write it from memory.
- **Read the failing test first**, then the diff, then the code.
- **`★★★` is optional.** Skipping stretch is a strategy.
- **Say quiz answers out loud.** Recall beats recognition; speaking beats
  thinking.

## Extending it

`bootcamp/_lib/FORMAT.md` is the authoring contract — file template, test
rules, where types go in a top-level-statements file, the solution
walkthrough format, and the ships-red rule for hunt modules. `verify.cs`
enforces the mechanical half of it, so a module that passes verify is a
module that follows the format.
