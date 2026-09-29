# 03 · Collections and LINQ

This is the module you will use every day. Nearly all business code is
"take these records, keep some, reshape them, group them, add them up" —
and in .NET that means picking a container and writing a query.

Two skills, and they fail in different ways. Choosing the wrong container
makes correct code slow. Misunderstanding LINQ's laziness makes fast code
wrong.

## The mental model

### 1 · Three containers cover almost everything

| container | good at | bad at | order |
| --- | --- | --- | --- |
| `List<T>` | indexing, appending, iterating | `Contains` (O(n)) | insertion |
| `Dictionary<K,V>` | key lookup (O(1)) | anything ordered | **none** |
| `HashSet<T>` | membership, dedupe (O(1)) | indexing | **none** |

`Dictionary` and `HashSet` need a sane `GetHashCode` on the key. Records,
value tuples, strings and primitives have one. A plain `class` does not —
it hashes by reference, so two "equal" keys are two entries.

`HashSet<T>.Add` returns `false` when the item was already there. That one
return value collapses "have I seen this?" and "remember it" into a single
probe, and it is the backbone of every dedupe you will write.

### 2 · A query is a recipe, not a result

```csharp
var q = items.Where(Expensive);   // nothing has run
var a = q.ToList();               // NOW it runs
var b = q.ToList();               // and it runs AGAIN
```

Deferred execution buys you two things — early exit (`First` over a
million rows stops at the first hit) and composition — and costs you two:
the query re-runs on every enumeration, and it reads whatever the source
holds *at that moment*, not when you wrote it.

The rule that follows: **materialise before you cross a boundary.** A
method that returns `List<T>` returns a value; one that returns a lazy
`IEnumerable<T>` returns a promise the caller can accidentally cash twice.

### 3 · Pick the operator whose shape matches the question

```csharp
customers.Join(orders, ...)       // one row per MATCHING PAIR   (inner join)
customers.GroupJoin(orders, ...)  // one row per LEFT ITEM       (left join)
sales.GroupBy(s => s.Region)      // one group per distinct key
```

If the answer must have one row per customer, `Join` is already wrong: a
customer with three orders appears three times and every total is
multiplied. That is fan-out, and it is the same bug in SQL.

### 4 · Every fold is three decisions

What the accumulator holds, what the seed is, and how one item updates it.
Choose the seed so the empty case falls out for free.

```csharp
results.Aggregate((Wins: 0, Losses: 0), (t, r) => r == "w" ? (t.Wins + 1, t.Losses) : (t.Wins, t.Losses + 1));
```

## The details that bite

1. **`Average`, `Max`, `Min`, `First`, `Single` and seedless `Aggregate`
   throw on an empty sequence.** "Sequence contains no elements" never
   tells you which query. Decide what empty means and say it.
2. **Multiple enumeration.** `source.Max() - source.Min()` walks the
   sequence twice. On a `List` that is waste; on a generator, a file, or
   a database query it is a bug. In review: an `IEnumerable<T>` parameter
   used twice in one method.
3. **`OrderBy` twice is not a tie-breaker.** The second call re-sorts from
   scratch. Use `ThenBy`.
4. **Sorting numeric strings sorts them as text** — `"10"` before `"9"`.
   Project to a number in the key selector.
5. **The `Dictionary` indexer throws on read and creates on write.**
   `TryGetValue` answers both questions in one hash probe;
   `ContainsKey` + indexer hashes twice and races.
6. **`Distinct` and `GroupBy` use the default equality comparer.** For a
   class that means reference equality, so "duplicates" survive. Use a
   record, a key selector (`DistinctBy`), or a comparer.
7. **`GroupBy` buffers.** It cannot yield a group before reading the whole
   source, so it is deferred but not streaming.
8. **`List.Sort` is unstable, LINQ's `OrderBy` is stable.** If equal items
   must keep their input order, that difference is your answer.
9. **A `HashSet` has no order.** It will look insertion-ordered in a small
   test and stop being so after a removal.

## Exercises

| # | file | ★ | what you build |
| --- | --- | --- | --- |
| 01 | `01-pick-the-collection.cs` | ★☆☆ | dedupe, tally and duplicate-detect — with the right container |
| 02 | `02-dictionary-patterns.cs` | ★☆☆ | the four ways to read a dictionary, and a hand-built index |
| 03 | `03-select-where-flatten.cs` | ★☆☆ | `Where` / `Select` / `SelectMany`, and the empty-sequence crash |
| 04 | `04-grouping.cs` | ★★☆ | group-then-aggregate: totals, best sellers, group sizes |
| 05 | `05-joins.cs` | ★★☆ | inner join vs left join, and the row that quietly disappears |
| 06 | `06-ordering.cs` | ★★☆ | primary + tie-breaker, descending, and "10" before "9" |
| 07 | `07-aggregate-and-fold.cs` | ★★☆ | running totals, a two-number accumulator, a longest streak |
| 08 | `08-deferred-execution.cs` | ★★★ | one query that must be eager, one that must be lazy, one that must read once |

Do them in order — 08 is where the first seven turn into a mental model,
and it is the file that will change how you write method signatures.

```
dotnet run exercises/04-grouping.cs
dotnet run solutions/04-grouping.cs
dotnet run ../verify.cs 03
dotnet run ../progress.cs 03
```

---

**Stuck?** `cheatsheets/linq.md` (every operator, what it costs, what it throws) · **Self-check:** `quizzes/03-collections-and-linq.md` · **Next:** `bootcamp/04-strings-and-time`
