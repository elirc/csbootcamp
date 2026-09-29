# Roadmap — what is built, what is next

This bootcamp is built module by module, and every module ships **green**:
`verify.cs` proves that every solution passes and every exercise is a clean
set of TODOs before a module is called done. That discipline is the reason
this file exists — it is more useful to say precisely what is finished than
to sketch nineteen half-modules.

Companion to [`OVERVIEW.md`](OVERVIEW.md). The authoring contract is
[`../bootcamp/_lib/FORMAT.md`](../bootcamp/_lib/FORMAT.md).

## Built and green

| module | files | what it covers |
| --- | --- | --- |
| `01-language-core` | 10 | value vs reference, numbers that lie, nullable, string comparison, patterns, tuples, `out`/`ref`, boxing, spans |
| `02-methods-and-delegates` | 8 | `Func`/`Action`, closures and capture, extension methods, composition, memoize, events, currying |
| `03-collections-and-linq` | 8 | containers, dictionary reads, filter/project/flatten, grouping, joins, ordering, folds, deferred execution |
| `04-strings-and-time` | 5 | building strings, culture-aware formatting, anchored regex, `DateOnly` arithmetic, an injected clock |

Plus the machinery: the `Check` harness, `verify.cs`, `progress.cs`, the
format contract, two cheat sheets, and the flight plan.

## Next, in build order

Each of these is an evening of work and follows the same contract. The
order is chosen so nothing depends on a module that does not exist yet.

1. **`05-types-and-oop`** — records vs classes vs structs, equality you
   write yourself, interfaces and polymorphism, composition over
   inheritance, static factories that validate, `IDisposable`.
2. **`06-errors-and-robustness`** — exception types and when to throw,
   the Try pattern versus exceptions, custom exceptions, `finally` and
   `using`, guard clauses, and a `Result<T>` that makes failure part of
   the signature.
3. **`07-async-and-tasks`** — `Task` and `async`/`await`, `WhenAll` and
   `WhenAny`, cancellation tokens, timeouts, `IAsyncEnumerable`, and the
   pitfalls that matter most in .NET: `async void`, sync-over-async
   (`.Result` / `.Wait()`), and the deadlock they cause.
4. **`08-generics-and-iterators`** — generic methods and constraints,
   `yield return`, custom `IEnumerable<T>`, `IEqualityComparer<T>`,
   variance, and why a lazy iterator's exceptions surface late.
5. **`09-dotnet-fundamentals`** — `Path` and `File`, streams, `System.Text.
   Json` (including the serialisation traps), environment and
   configuration, and writing a file atomically.
6. **`10-testing-and-quality`** — fakes vs mocks, injected clocks and
   randomness, property-style tests, testing the boundary rather than the
   middle. (The harness in `_lib/Check` is the worked example.)
7. **`11-design-patterns-and-di`** — constructor injection by hand, the
   options pattern, strategy, decorator, observer, and a small state
   machine. Written so the DI container is revealed as a convenience, not
   a mystery.
8. **`12-data-structures`** and **`13-algorithms`** — stack, queue, tree,
   heap, LRU cache, graph traversal; two pointers, sliding window, binary
   search bounds. The interview spine, in C#.

## The hunt modules (the highest-value part)

Built last because they draw on everything above, and worth the wait: in a
hunt module the code arrives **finished and broken**, the tests start red,
and your job is to find the planted bug and make the smallest fix that turns
them green. Reading code fast enough to find the one line that is lying is
the skill you get paid for and almost nobody drills on purpose.

- **`14-debug-hunts`** — one planted bug per file, each from a named class:
  off-by-one, shared reference, closure capture, `==` on boxed values,
  `IEnumerable` enumerated twice, `async void`, a `finally` that swallows,
  a comparer that is not a total order, `DateTime.Now` in the middle of a
  calculation.
- **`15-security-hunts`** — close the vulnerability: SQL string
  concatenation, path traversal, IDOR, mass assignment on a bound model,
  a predictable token from `Random`, secrets in logs, ReDoS.
- **`16-write-the-test`** — the inverse: given a known bug, write the test
  that catches it, graded by a meta-test that runs your test against both
  a correct and a broken implementation.

`verify.cs` and `progress.cs` already implement the ships-red rule for any
module whose directory name contains `-debug` or ends in `-hunts`, so those
modules need no tooling changes — only content.

## Also planned

- **Cheat sheets**: async, testing, .NET APIs (the current two cover the
  language and LINQ).
- **Quizzes**: rapid-fire recall per module, answers hidden, meant to be
  answered out loud.
- **Guides**: long-form essays on the two things that need more than a
  lesson — how `async`/`await` actually works (the state machine, the
  synchronisation context, why `.Result` deadlocks), and what the GC does
  with your allocations.

## What this deliberately will not have

Scope discipline is why it runs offline and alone: no NuGet packages, no
web framework, no Docker, no database engine, nothing that needs a network
to grade. If it cannot be graded by `dotnet run <file>.cs`, it does not
belong here.
