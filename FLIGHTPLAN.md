# Flight plan — C# / .NET, offline, ten hours

A schedule for a long flight, a quiet weekend, or ten evenings. Everything
here runs with the .NET SDK and nothing else: no NuGet restore, no test
runner to install, no internet.

**Before you take off**, run this once on a connected machine so the build
cache is warm:

```
dotnet run bootcamp/verify.cs 01
```

The first compile of a file takes a few seconds; after that a run is under
a second. Nothing is downloaded at any point — the only project reference
is the local `_lib/Check` harness.

## The workflow, in one line

```
dotnet run bootcamp/01-language-core/exercises/01-count-vowels.cs
```

Write code, re-run, repeat until `all green — next file!`. Then the next
file. `dotnet run bootcamp/progress.cs` is your scoreboard;
`dotnet run bootcamp/verify.cs NN` checks a whole module at once.

## Hour by hour

### Hours 1–2 · The language, and where it bites

`bootcamp/01-language-core`, in order. Ten files: value vs reference,
integer division and decimal money, nullable, string comparison, pattern
matching, tuples, `out`/`ref`, boxing, spans.

If you already write C# daily, do 03, 05, 09 and 10 and skip the rest — they
are the four that catch experienced people.

*Exit test:* you can say why `0.1 + 0.2 != 0.3`, why `==` on two boxed `1`s
is `false`, and what `a.ToLower() == b.ToLower()` does in Turkey.

### Hour 3 · Behaviour as a value

`bootcamp/02-methods-and-delegates`. Eight files: `Func`/`Action`, closures
and the loop-variable trap, extension methods, composition, memoize, events,
currying.

*Exit test:* you can explain, without looking, why three lambdas built in a
`for` loop all return the same number, and what `-=` on an event actually
compares.

### Hours 4–6 · The workhorse: collections and LINQ

`bootcamp/03-collections-and-linq`. Eight files, and the module you will use
every day: choosing a container, dictionary reads, filter/project/flatten,
grouping, inner vs left join, ordering, folds, and deferred execution.

Do not skip 08. It is where the previous seven turn into a mental model, and
it is the one that changes how you write method signatures.

*Exit test:* given a query, you can say when it runs, how many times it
walks its source, and what it throws when the source is empty.

### Hour 7 · The hidden parameters

`bootcamp/04-strings-and-time`. Five files: building strings, formatting for
machines vs humans, anchored regexes, calendar dates, and injecting a clock.

*Exit test:* you can name the two hidden inputs (culture and clock), and you
reach for `DateOnly` and an `IClock` parameter without being told.

### Hours 8–10 · Whatever the scoreboard says

```
dotnet run bootcamp/progress.cs
```

Anything still `☐` or `◐`, plus the `★★★` files you skipped. Then pick one:

- **Read a walkthrough properly.** Every solution carries one. They are the
  lesson, not the answer key — read the ones for the exercises you found
  easy, because that is where the surprises are.
- **Rebuild from memory.** Delete your version of `03-collections-and-linq/
  08-deferred-execution.cs` and write it again cold. Reconstruction is what
  makes it stick; re-reading is not.
- **Take the quizzes out loud.** `quizzes/` — say the answer before you
  expand it. Recall beats recognition, and speaking it beats thinking it.

## Rules of engagement

- Stuck more than ten minutes → read the solution's `Walkthrough:`, close
  the file, and write it again from memory. Copy-pasting teaches nothing.
- `★★★` is optional stretch. Skipping it is a strategy, not a failure.
- Every exercise is self-contained. You never need to open another file to
  understand one, and you never need the internet.
- When a test fails, read the test name first, then the diff, then the
  code. In that order.

## What is here, and what is not yet

Four modules are complete and green: language core, methods and delegates,
collections and LINQ, strings and time. `docs/ROADMAP.md` lists what is
planned next and why, in the order it will be built — async and tasks,
types and OOP, errors, generics and iterators, and the debug-hunt modules
where the code arrives finished and broken and you find the planted bug.

If you want to extend it yourself, `bootcamp/_lib/FORMAT.md` is the
authoring contract, and `verify.cs` enforces it.
