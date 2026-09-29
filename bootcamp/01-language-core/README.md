# 01 · Language Core

C# is a big language with a small centre. This module is that centre: how a
value gets copied, what `null` really means now that the compiler has an
opinion about it, why money is `decimal` and never `double`, and how to say
a branching rule as an expression instead of a staircase of `if`s.

None of it is exotic. All of it shows up in the first hour of reading any
real .NET codebase, and every item here is a bug someone shipped this week.

## The mental model

### 1 · Values are copied, references are shared

Every type in .NET is one of two kinds, and the kind decides what happens
when you assign it or pass it to a method.

```csharp
struct Point { public int X; }      // a VALUE: the variable holds the data
class Box { public int X; }         // a REFERENCE: the variable holds an arrow

var a = new Point { X = 1 };
var b = a;  b.X = 99;               // a.X is still 1 — b got a copy

var c = new Box { X = 1 };
var d = c;  d.X = 99;               // c.X is now 99 — same object
```

`int`, `double`, `decimal`, `bool`, `char`, `DateTime`, `Guid` and every
`struct` are values. `string`, arrays, `List<T>`, and every `class` and
`record` are references. (`string` is a reference type that *behaves* like
a value, because it is immutable — you can never observe the sharing.)

### 2 · Nullable is a conversation with the compiler

In a modern project `string` means "never null" and `string?` means "might
be". The compiler tracks it and warns you; it does not enforce it at
runtime.

```csharp
string? maybe = GetName();
int length = maybe.Length;        // warning: may be null
int safe = maybe?.Length ?? 0;    // no warning, no crash
```

`?.` skips the member access and gives `null`; `??` supplies a fallback;
`??=` assigns only if the target is null. There is a fourth operator, `!`
("I know better"), and reaching for it is almost always how a
`NullReferenceException` gets born.

### 3 · A switch expression is a value, not a jump

```csharp
var band = grams switch
{
    < 0 => "invalid",
    0 => "free",
    < 1000 => "small",
    _ => "pallet",
};
```

Arms are tried top to bottom, first match wins, and the whole thing produces
a value — so the compiler can prove every path returns one. Patterns
compose: relational (`< 1000`), type (`is int n`), property
(`{ Weight: 0 }`), and the combinators `and`, `or`, `not`.

### 4 · There are three numeric worlds

| type | base | use it for | never use it for |
| --- | --- | --- | --- |
| `int` / `long` | 2 | counts, ids, indexes | anything divided |
| `double` | 2 | measurements, physics, ratios | money |
| `decimal` | 10 | money, tax, anything a human audits | tight loops, science |

`0.1 + 0.2 != 0.3` in `double` and *is* `0.3m` in `decimal`. That is not a
bug in .NET; it is base-2 fractions doing what base-2 fractions do.

## The details that bite

1. **`int / int` is integer division, whatever you assign it to.**
   `double avg = 7 / 2;` is `3.0`. The cast has to happen before the
   division: `(double)7 / 2`.
2. **`Math.Round` uses banker's rounding by default.**
   `Math.Round(0.5)` is `0`, not `1` — correct for statistics, wrong for
   an invoice. Say `MidpointRounding.AwayFromZero` when you mean money.
3. **`==` on `object` compares references.** Two boxed `1`s are two
   objects. Use `Equals(a, b)` — the static form, which is null-safe on
   both sides.
4. **`ToLower()` uses the current culture.** In Turkish, lowercasing `I`
   does not give `i`, so `a.ToLower() == b.ToLower()` compares different
   strings on a Turkish machine. For keys and ids, use
   `StringComparison.OrdinalIgnoreCase` or `ToLowerInvariant`.
5. **A blank string is not null and is still missing.** `name != null`
   passes for `"   "`. `string.IsNullOrWhiteSpace` is the check you meant.
6. **An `out` parameter must be assigned on every path**, including the
   failure path. A `Try` method that fails and leaves the old value in
   place hands the caller a stale number.
7. **Boxing allocates.** `object o = 42;` puts 42 on the heap. In a hot
   loop that is a garbage-collection problem, which is why the BCL has a
   generic overload of nearly everything.
8. **Mutable structs are a trap.** You mutate a copy and the change
   vanishes. Make structs `readonly` and return modified copies with
   `with`.

## Exercises

| # | file | ★ | what you build |
| --- | --- | --- | --- |
| 01 | `01-count-vowels.cs` | ★☆☆ | a string walk, and your first meeting with culture |
| 02 | `02-value-vs-reference.cs` | ★☆☆ | what a copy copies — struct vs class, and a "copy" that isn't |
| 03 | `03-numbers-that-lie.cs` | ★☆☆ | integer division, decimal money, rounding a half cent |
| 04 | `04-nullable-without-fear.cs` | ★★☆ | null, blank, present — and the operators that tell them apart |
| 05 | `05-strings-that-compare.cs` | ★★☆ | ordinal vs culture, and normalising a key you will look up |
| 06 | `06-pattern-matching.cs` | ★★☆ | shipping rules as switch expressions, not if-staircases |
| 07 | `07-tuples-and-deconstruction.cs` | ★☆☆ | two answers from one call, named and deconstructed |
| 08 | `08-out-and-ref.cs` | ★★☆ | the Try pattern, done properly, plus ref and discards |
| 09 | `09-boxing-and-equality.cs` | ★★★ | a settings bag of `object`, and what equals means in it |
| 10 | `10-spans-without-allocating.cs` | ★★★ | parsing with `ReadOnlySpan<char>` — no Split, no Substring |

Do the warm-ups and core in order; 01 through 08 is the spine. The two
stretch files are the ones an interviewer uses to separate "writes C#" from
"knows what the runtime is doing" — worth the time, skippable on a first
pass.

```
dotnet run exercises/01-count-vowels.cs   # yours
dotnet run solutions/01-count-vowels.cs   # the answer + walkthrough
dotnet run ../verify.cs 01                # the whole module
dotnet run ../progress.cs 01              # the scoreboard
```

---

**Stuck?** `cheatsheets/csharp-language.md` (syntax, operators, patterns) · **Self-check:** `quizzes/01-language-core.md` · **Next:** `bootcamp/02-methods-and-delegates`
